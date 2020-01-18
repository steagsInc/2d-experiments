using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ground : MonoBehaviour
{
    private Tilemap ground;

    private void Start()
    {
        ground = GetComponent<Tilemap>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.collider.GetComponentInParent<Rigidbody2D>().gameObject.SetActive(false);

        //print(collision.contactCount);

        for(int i = 0; i<collision.contactCount; i++)
        {

            gunContact(collision.GetContact(i).point);

        }
    }

    public void gunContact(Vector2 point)
    {

        Vector3Int groundPoint = ground.layoutGrid.WorldToCell(point);

            if (ground.GetTile(groundPoint) != null)
            {

                
            }

    }
}
