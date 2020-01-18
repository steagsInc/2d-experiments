using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SideCharacter : CustomPhysics2D
{
    //Component
    protected Rigidbody2D rb2d;
    private Transform spriteTransform;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Tilemap tilemap;
    private Transform footPos;
    private Collider2D collider2d;

    //public Variables
    public float gravityModifier = 1f;
    public float speed = 5f;
    public float maxSpeed = 7f;
    public float jumpForce = 7f;
    public float airFriction = 0.2f;
    
    public bool canDrill = false;
    public string team = "red";

    //Internal Variables
    private Vector2 velocity = Vector2.zero;
    private Vector2 targetVelocity = Vector2.zero;
    private List<MultipleFramedVelocity2d> delayedVelocities = new List<MultipleFramedVelocity2d>();
    private Vector2 move = Vector2.zero;
    private bool grounded = false;
    private ContactFilter2D playerFilter;
    private ContactFilter2D drillFilter;


    //Drilling
    private float drillPower;
    private bool drilling = false;
    private bool detectingGround = false;
    private bool inGround = false;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        collider2d = transform.GetChild(0).gameObject.GetComponent<Collider2D>();
        spriteTransform = transform.GetChild(0);
        spriteRenderer = spriteTransform.gameObject.GetComponent<SpriteRenderer>();
        animator = spriteTransform.gameObject.GetComponent<Animator>();

        //tilemap = GameObject.FindGameObjectWithTag("Ground").GetComponent<Tilemap>();
        footPos = transform.GetChild(1).transform;

        playerFilter.SetLayerMask(UnityEngine.Physics2D.GetLayerCollisionMask(gameObject.layer));
        drillFilter.SetLayerMask(UnityEngine.Physics2D.GetLayerCollisionMask(8));
    }

    private void FixedUpdate()
    {
        if (!drilling)
        {
            normalMovement();
        }
        else
        {
            Drilling();
        }

        setDrill();
    }

    //Physics
    private void normalMovement()
    {
        if (spriteTransform.rotation.eulerAngles.z != 0)
        {
            directionalRotation(spriteTransform, new Vector2(0, 0));
        }

        //Inputs
        targetVelocity.x += walkingSpeedtoVelocity(speed).x;

        wallJump(targetVelocity);

        if (grounded)
        {
            float j = jump(jumpForce);
            if (j != 0) grounded = false;
            targetVelocity.y += j;
            autoStep(targetVelocity,0.3f);
        }

        targetVelocity += addFramedVelocity(delayedVelocities);

        if (grounded)
        {
            velocity.x = targetVelocity.x;
        }
        else
        {
            velocity.x += targetVelocity.x;
            velocity.x *= 1 - airFriction;
        }

        velocity.y += targetVelocity.y;

        targetVelocity = Vector2.zero;

        velocity += gravityModifier * UnityEngine.Physics2D.gravity * Time.deltaTime;

        velocity = velocityClamp(velocity, maxSpeed);

        grounded = false;

        move = new Vector2(velocity.x,0) * Time.deltaTime;

        collision(rb2d, move, playerFilter);

        rb2d.position = rb2d.position + move;

        move = new Vector2(0,velocity.y) * Time.deltaTime;

        collision(rb2d, move, playerFilter);

        rb2d.position = rb2d.position + move;

    }

    protected override void onCollision(RaycastHit2D hit)
    {
        if (!drilling)
        {
            velocity = velocityCorrection(velocity, hit.normal);
            if (move.y != 0)
            {
                grounded = groundCollisionDetection(hit.normal);
            }
            move = wallCollision(move, hit.distance);
        }
        else if(!detectingGround)
        {
            velocity = velocityCorrection(velocity, hit.normal);
            move = wallCollision(move, hit.distance);
        }
    }

    public void knockBack(Vector2 direction, float force)
    {
        targetVelocity += directionalVelocity(direction, force);
    }

    // Can cause bug on knockback maybe change it later, will see
    private void autoStep(Vector2 velocity, float stepSize)
    {
    
        if (velocity.x == 0) return;

        RaycastHit2D[] foot = new RaycastHit2D[2], legs = new RaycastHit2D[2];

        Physics2D.Raycast(footPos.position, transform.right * velocity.normalized.x,playerFilter,foot,0.5f);
        //showRay(footPos.position, transform.right * velocity.normalized.x, Color.red);

        Vector2 uppertilePos = new Vector2(footPos.position.x, footPos.position.y + stepSize);

        Physics2D.Raycast(uppertilePos, transform.right * velocity.normalized.x, playerFilter, legs, 0.5f);
        //showRay(uppertilePos, transform.right * velocity.normalized.x, Color.red);

        if(foot[0].collider !=null && legs[0].collider == null && foot[0].collider.tag != "Player")
        {
            rb2d.position = rb2d.position + new Vector2(0, stepSize);
        }
    }

    private void wallJump(Vector2 velocity)
    {
        if (!grounded)
        {

            RaycastHit2D[] foot = new RaycastHit2D[2];

            int d = spriteRenderer.flipX ? -1 : 1;

            Physics2D.Raycast(footPos.position, transform.right * d, playerFilter, foot, 0.7f);
            //showRay(footPos.position, transform.right * d, Color.red);

            if (foot[0].collider != null && foot[0].collider.tag != "Player" && Input.GetButtonDown("Jump"))
            {
                delayedVelocities.Add(new MultipleFramedVelocity2d(new Vector2(speed * foot[0].normal.x * 2, 2f), 7));
            }
        }
    }

    //Drilling
    private void Drilling()
    {
        Vector2 direction;

        velocity += targetVelocity;

        if (inGround)
        {
            direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if(direction != Vector2.zero)  directionalRotation(spriteTransform, direction);
            velocity = directionalVelocity(-spriteTransform.up, speed);
        }
        else
        {
            velocity += gravityModifier * UnityEngine.Physics2D.gravity * Time.deltaTime;
            direction.x = -velocity.x;
            direction.y = velocity.y;
            directionalRotation(spriteTransform, -direction);
        }

        targetVelocity = Vector2.zero;

        velocity = velocityClamp(velocity, maxSpeed);

        move = velocity * Time.deltaTime;

        detectingGround = true;

        collision(rb2d, move, playerFilter);

        detectingGround = false;

        collision(rb2d, move, drillFilter);

        rb2d.position += move;

    }

    protected override void onCollisions(RaycastHit2D[] hits,float count)
    {
        if(drilling && detectingGround)
        {
            if(!inGround && count > 0)
            {
                inGround = true;
            }else if(inGround && count == 0)
            {
                inGround = false;
            }
        }
    }

    private void setDrill()
    {

        if (canDrill)
        {

            if (!drilling && Input.GetAxis("Drilling") > 0.4f)
            {
                knockBack(new Vector2(0, UnityEngine.Physics2D.gravity.y),gravityModifier*1000);
                drillPower = Input.GetAxis("Drilling");
                drilling = true;
            }
            else if (drilling && Input.GetAxis("Drilling") <= 0 && !inGround)
            {
                drillPower = Input.GetAxis("Drilling");
                drilling = false;
            }
        }
    }

    //Graphics
    private void Update()
    {
        animate(velocity);
    }

    protected void animate(Vector2 move)
    {

        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < -0.01f));
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / speed);

    }
}
