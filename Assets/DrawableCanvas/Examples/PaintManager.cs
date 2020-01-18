using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrawableCanvas;

public class PaintManager : MonoBehaviour
{
    public Canvas2D red;
    public Canvas2D blue;

    public ColliderBuilder redCollider;
    public ColliderBuilder blueCollider;

    public PolygonCollider2D filter;
    public Sprite brush;

    private List<Vector2> Spoints = new List<Vector2>();

    void Start()
    {
        red.setColor(Color.red);
        blue.setColor(Color.blue);

        brush.GetPhysicsShape(0, Spoints);

        redCollider.addFilter(filter.points, filter.GetTotalPointCount(),filter.transform.lossyScale,filter.transform.position);
        blueCollider.addFilter(filter.points, filter.GetTotalPointCount(),filter.transform.lossyScale,filter.transform.position);

    }

    void Update()
    {
        paintTest();
        colliderTest();
    }

    private void paintTest()
    {
        if (Input.GetKey(KeyCode.A))
        {
            red.applyTextureOnPoint(brush.texture, Camera.main.ScreenToWorldPoint(Input.mousePosition), filter.bounds);
            blue.removeTextureOnPoint(brush.texture, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        else if (Input.GetKey(KeyCode.E))
        {
            blue.applyTextureOnPoint(brush.texture, Camera.main.ScreenToWorldPoint(Input.mousePosition), filter.bounds);
            red.removeTextureOnPoint(brush.texture, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    private void colliderTest()
    {
        if (Input.GetKey(KeyCode.A))
        {
            redCollider.addShapeCollider(Spoints, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            blueCollider.removeShapeCollider(Spoints, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        else if (Input.GetKey(KeyCode.E))
        {
            blueCollider.addShapeCollider(Spoints, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            redCollider.removeShapeCollider(Spoints, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

}
