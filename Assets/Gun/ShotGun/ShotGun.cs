using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : Gun
{

    private PolygonCollider2D cone;
    private ParticleSystem particles;
    private ContactFilter2D shootfilter;

    private Vector2 pointUp;
    private Vector2 pointDown;

    private void Awake()
    {
        shootfilter.SetLayerMask(UnityEngine.Physics2D.GetLayerCollisionMask(gameObject.layer));
        particles = transform.GetChild(3).GetComponent<ParticleSystem>();
        cone = transform.GetChild(2).GetComponent<PolygonCollider2D>();
    }

    protected override void shoot()
    {
        transform.parent.GetComponent<SideCharacter>().knockBack(-transform.right, 1f);
        particles.startColor = Color.red;
        particles.Play();
        collision(rb2d, transform.right, shootfilter);
    }

    protected override void onCollisions(RaycastHit2D[] hits, float count)
    {

        for(int i=0; i < count; i++)
        {
            //print(hits[i].collider.tag);
            if (hits[i].collider.tag == "Ground")
            {
                RaycastHit2D hit;
                pointUp = cone.transform.TransformPoint(cone.points[0]);
                pointDown = cone.transform.TransformPoint(cone.points[2]);
                hit = Physics2D.Raycast(spawningPoint.position, pointDirection(spawningPoint.position, pointUp), 2,shootfilter.layerMask);
                Debug.DrawRay(spawningPoint.position, pointDirection(spawningPoint.position, pointUp), Color.red);

                Vector2 point1 = hit.point;
                //print(point1);

                hit = Physics2D.Raycast(spawningPoint.position, pointDirection(spawningPoint.position, pointDown), 2, shootfilter.layerMask);
                Debug.DrawRay(spawningPoint.position, pointDirection(spawningPoint.position, pointDown), Color.red);

                Vector2 point2 = hit.point;
                //print(point2);



            }
        }
    }

}
