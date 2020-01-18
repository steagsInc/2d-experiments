using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClipperLib;

namespace DrawableCanvas
{
    public class ColliderBuilder : MonoBehaviour
    {

        private PolygonCollider2D collider;
        private List<List<IntPoint>> filter = new List<List<IntPoint>>();

        private const int PRECISION = 100;

        private List<int> emptyPath = new List<int>();

        private Vector2[] EMPTY = new Vector2[1];

        private void Awake()
        {
            collider = GetComponent<PolygonCollider2D>();
        }

        public void addShapeCollider(List<Vector2> points, Vector3 position)
        {

            points = new List<Vector2>(points);

            updatePointList(points, position);

            List<List<IntPoint>> pathclip = filterCollider(Vector2ArrayToListIntPoint(points.ToArray()));

            if (pathclip.Count == 0) return;

            if (collider.pathCount == 0)
            {

                for (int i = 0; i < pathclip.Count; i++)
                {
                    collider.pathCount = i + 1;
                    collider.SetPath(i, ListIntPointToVector2Array(pathclip[i]));
                }
                return;
            }
            else
            {
                Clipper c = new Clipper();

                foreach (List<IntPoint> p in pathclip)
                {
                    c.AddPath(p, PolyType.ptClip, true);
                }

                for (int i = 0; i < collider.pathCount; i++)
                {
                    c.AddPath(Vector2ArrayToListIntPoint(collider.GetPath(i)), PolyType.ptSubject, true);
                }

                List<List<IntPoint>> solution = new List<List<IntPoint>>();

                c.Execute(ClipType.ctUnion, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

                collider.pathCount = solution.Count;

                for (int i = 0; i < solution.Count; i++)
                {
                    collider.SetPath(i, ListIntPointToVector2Array(solution[i]));
                }

            }
        }

        public void removeShapeCollider(List<Vector2> points, Vector3 position)
        {

            points = new List<Vector2>(points);

            if (collider.pathCount == 0) return;

            updatePointList(points, position);

            List<List<IntPoint>> pathclip = filterCollider(Vector2ArrayToListIntPoint(points.ToArray()));

            if (pathclip.Count == 0) return;

            Clipper c = new Clipper();

            foreach (List<IntPoint> p in pathclip)
            {
                c.AddPath(p, PolyType.ptClip, true);
            }

            for (int i = 0; i < collider.pathCount; i++)
            {
                c.AddPath(Vector2ArrayToListIntPoint(collider.GetPath(i)), PolyType.ptSubject, true);
            }

            List<List<IntPoint>> solution = new List<List<IntPoint>>();

            c.Execute(ClipType.ctDifference, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

            collider.pathCount = solution.Count;

            for (int i = 0; i < solution.Count; i++)
            {
                collider.SetPath(i, ListIntPointToVector2Array(solution[i]));
            }
        }

        private List<List<IntPoint>> filterCollider(List<IntPoint> collider)
        {

            List<List<IntPoint>> solution = new List<List<IntPoint>>();
            if (filter.Count == 0)
            {
                solution.Add(collider);
                return solution;
            }
            Clipper c = new Clipper();

            c.AddPath(collider, PolyType.ptClip, true);

            foreach (List<IntPoint> f in filter)
            {

                c.AddPath(f, PolyType.ptSubject, true);

            }

            c.Execute(ClipType.ctIntersection, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

            return solution;
        }

        private void updatePointList(List<Vector2> points, Vector3 position)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = new Vector2(points[i].x + position.x, points[i].y + position.y);
            }

        }

        private void scalePointList(Vector2[] points, int n, Vector3 scale, Vector3 position)
        {
            for (int i = 0; i < n; i++)
            {
                points[i] = new Vector2(points[i].x * scale.x + position.x, points[i].y * scale.y + position.y);
            }

        }

        private void union(List<IntPoint> subject, List<IntPoint> clip, List<List<IntPoint>> solution)
        {

            Clipper c = new Clipper();
            c.AddPath(subject, PolyType.ptSubject, true);
            c.AddPath(clip, PolyType.ptClip, true);
            c.Execute(ClipType.ctUnion, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

        }

        private List<IntPoint> Vector2ArrayToListIntPoint(Vector2[] shape)
        {

            List<IntPoint> list = new List<IntPoint>();

            foreach (Vector2 v in shape)
            {
                list.Add(new IntPoint(v.x * PRECISION, v.y * PRECISION));
            }
            return list;
        }

        private Vector2[] ListIntPointToVector2Array(List<IntPoint> shape)
        {

            Vector2[] list = new Vector2[shape.Count];

            for (int i = 0; i < shape.Count; i++)
            {
                list[i] = new Vector2((float)shape[i].X / PRECISION, (float)shape[i].Y / PRECISION);
            }
            return list;
        }

        public void addFilter(Vector2[] points)
        {

            filter.Add(Vector2ArrayToListIntPoint(points));

        }

        public void addFilter(Vector2[] points, int n, Vector3 scale , Vector3 position)
        {

            scalePointList(points, n, scale, position);

            filter.Add(Vector2ArrayToListIntPoint(points));

        }

        public void removeFilter(Vector2[] points)
        {

            if (filter.IndexOf(Vector2ArrayToListIntPoint(points)) != -1) filter.Remove(Vector2ArrayToListIntPoint(points));

        }

        public void removeFilter(Vector2[] points, int n, Vector3 scale, Vector3 position)
        {

            scalePointList(points, n, scale, position);

            if (filter.IndexOf(Vector2ArrayToListIntPoint(points)) != -1) filter.Remove(Vector2ArrayToListIntPoint(points));

        }

    }
}
