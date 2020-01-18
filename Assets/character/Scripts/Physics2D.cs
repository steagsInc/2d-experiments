using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics2D : MonoBehaviour
{
    public struct MultipleFramedVelocity2d
    {
        public Vector2 velocity;
        public int frames;

        public MultipleFramedVelocity2d(Vector2 velocity,int frames)
        {
            this.velocity = velocity;
            this.frames = frames;
        }
    }

    //Constantes
    private const float minMoveDistance = 0.001f;
    private const float shellRadius = 0.01f;
    private const float minGroundNormalY = .65f;
    protected const float G = 6.67384e-11f;

    protected Vector2 velocityClamp(Vector2 velocity,float max)
    {

        velocity.x = Mathf.Clamp(velocity.x, -max, max);
        velocity.y = Mathf.Clamp(velocity.y, -max, max);

        return velocity;
    }

    protected Vector2 walkingSpeedtoVelocity(float speed)
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");

        return move * speed;
    }

    protected float jump(float jumpForce)
    {

        if (Input.GetButtonDown("Jump"))
        {
            return jumpForce;
        }

        return 0;
    }

    protected Vector2 addFramedVelocity(List<MultipleFramedVelocity2d> velocities)
    {
        Vector2 velocity = new Vector2();

        for(int i = 0; i<velocities.Count; i++)
        {
            if(velocities[i].frames == 0)
            {
                velocities.Remove(velocities[i]);
                continue;
            }
            velocity += velocities[i].velocity;
            velocities[i]= new MultipleFramedVelocity2d(velocities[i].velocity, velocities[i].frames-1);
        }

        return velocity;
    }

    protected Vector2 pointDirection(Vector2 point1,Vector2 point2)
    {
        return new Vector2(point2.x - point1.x, point2.y - point1.y);
    }

    protected Vector2 directionalVelocity(Vector2 direction,float speed)
    {

        return direction * speed;

    }

    public void directionalRotation(Transform t, Vector2 direction)
    {
        Quaternion target = Quaternion.Euler(0, 0, 0);

        float rad = t.rotation.eulerAngles.z * Mathf.Rad2Deg;

        Vector3 currentDirection = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);

        Vector3 aim = new Vector3(direction.x, direction.y, 0).normalized;

        float angle = Mathf.Atan2(aim.x, aim.y) * 180 / Mathf.PI;

        target = Quaternion.Euler(0, 0, angle);

        t.rotation = Quaternion.Slerp(t.rotation, target, Time.deltaTime* 5.0f);

        if(Mathf.Floor(t.rotation.eulerAngles.z) == target.eulerAngles.z)
        {
            t.rotation = target;
        }
        
    }

    protected void collision(Rigidbody2D rb2d, Vector2 direction, ContactFilter2D contactFilter)
    {
        float distance = direction.magnitude;

        if (distance > minMoveDistance)
        {
            RaycastHit2D[] hits = new RaycastHit2D[16];

            int count = rb2d.Cast(direction, contactFilter, hits, distance + shellRadius);

            onCollisions(hits,count);

            for(int i=0; i<count; i++)
            {
                onCollision(hits[i]);
            }

        }
    }

    protected bool groundCollisionDetection(Vector2 normal)
    {
        if(normal.y > minGroundNormalY)
        {
            groundCollsionEnter(normal);
            return true;
        }

        return false;
    }

    protected Vector2 wallCollision(Vector2 move, float hitDistance)
    {
        float distance = move.magnitude;

        float modifiedDistance = hitDistance - shellRadius;
        distance = modifiedDistance < distance ? modifiedDistance : distance;

        return move.normalized * distance;
    }

    protected Vector2 velocityCorrection(Vector2 velocity,Vector2 normal)
    {
        float projection = Vector2.Dot(velocity, normal);
        if (projection < 0)
        {
            velocity = velocity - projection * normal;
        }

        return velocity;
    }

    protected virtual Vector2 groundCollsionEnter(Vector2 normal)
    {
        return Vector2.zero;
    }

    protected virtual void onCollisions(RaycastHit2D[] hits,float count)
    {

    }

    protected virtual void onCollision(RaycastHit2D hit)
    {
        
    }

    protected void showVector(Vector2 v, Color c)
    {
        Debug.DrawLine(transform.position, new Vector3(v.x, v.y, 0) + transform.position, c, 0f, false);
    }

    protected void showRay(Vector2 pos, Vector2 dir, Color c)
    {
        Debug.DrawRay(pos, dir, c, 0f);
    }
}
