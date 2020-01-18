using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClipperLib;

namespace DrawableCanvas{
    public class Canvas2D : MonoBehaviour
    {

        public int height;
        public int width;
        public int pixelPerUnit;

        private Sprite sprite;
        private Texture2D texture;
        private List<Vector2[]> filter = new List<Vector2[]>();
        private List<int> sizes = new List<int>();
        private List<float[]> constant = new List<float[]>();
        private List<float[]> multiple = new List<float[]>();
        private const int PRECISION = 100;

        private SpriteRenderer renderer;

        void Awake()
        {
            texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), pixelPerUnit);
            texture = sprite.texture;
            forAllPixel(texture, new Color(0, 0, 0, 0));
            renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
        }

        private void forAllPixel(Texture2D texture, Color c)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    texture.SetPixel(x, y, c);
                }
            }
            texture.Apply();
        }

        public void applyTextureOnPoint(Texture2D source, Vector2 point)
        {
            point = worldToTexture(point);

            Vector2 offset = new Vector2(point.x - source.width / 2, point.y - source.height / 2);

            for (int x = (int)offset.x, x2 = 0; x < (int)(offset.x + source.width) && x2 < (int)source.width; x++, x2++)
            {
                for (int y = (int)offset.y, y2 = 0; y < (int)(offset.y + source.height) && y2 < (int)source.height; y++, y2++)
                {
                    bool a = source.GetPixel(x2, y2).a > 0;
                    bool c = source.GetPixel(x2, y2) != texture.GetPixel(x, y);
                    if ((x >= 0 && y >= 0) && (x < texture.width && y < texture.height) && a && c) texture.SetPixel(x, y, source.GetPixel(x2, y2));
                }
            }
            texture.Apply();

        }

        public void applyTextureOnPoint(Texture2D source, Vector2 point, Bounds filter)
        {
            point = worldToTexture(point);

            Vector2 offset = new Vector2(point.x - source.width / 2, point.y - source.height / 2);

            for (int x = (int)offset.x, x2 = 0; x < (int)(offset.x + source.width) && x2 < (int)source.width; x++, x2++)
            {
                for (int y = (int)offset.y, y2 = 0; y < (int)(offset.y + source.height) && y2 < (int)source.height; y++, y2++)
                {
                    bool contains = filter != null ? filter.Contains(textureToWorld(new Vector2(x, y))) : true;
                    bool a = source.GetPixel(x2, y2).a > 0;
                    bool c = source.GetPixel(x2, y2) != texture.GetPixel(x, y);
                    if ((x >= 0 && y >= 0) && (x < texture.width && y < texture.height) && contains && a && c) texture.SetPixel(x, y, source.GetPixel(x2, y2));
                }
            }
            texture.Apply();

        }

        public void removeTextureOnPoint(Texture2D source, Vector2 point)
        {
            point = worldToTexture(point);

            Vector2 offset = new Vector2(point.x - source.width / 2, point.y - source.height / 2);

            for (int x = (int)offset.x, x2 = 0; x < (int)(offset.x + source.width) && x2 < (int)source.width; x++, x2++)
            {
                for (int y = (int)offset.y, y2 = 0; y < (int)(offset.y + source.height) && y2 < (int)source.height; y++, y2++)
                {
                    bool a = source.GetPixel(x2, y2).a > 0;
                    if ((x >= 0 && y >= 0) && (x < texture.width && y < texture.height) && a) texture.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }
            texture.Apply();

        }

        public void setColor(Color c)
        {
            renderer.color = c;
        }

        //Convertisseur

        public Vector2 worldToTexture(Vector2 point)
        {
            return (point - (Vector2)transform.position) * 16 + sprite.pivot;
        }

        public Vector2 textureToWorld(Vector2 point)
        {
            return (point - sprite.pivot) / 16 + (Vector2)transform.position;
        }
    }
}
