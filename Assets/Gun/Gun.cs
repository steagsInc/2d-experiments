using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : CustomPhysics2D
{
    private GameObject line;
    protected Transform spawningPoint;
    private Quaternion target;

    public bool isAutomatic = false;
    public float firingSpeed = 0.5f;
    private float lastFire = 0f;

    private SpriteRenderer sprite;
    protected Rigidbody2D rb2d;
    public GameObject bullet;
    public float bulletSpeed = 5;
    private List<GameObject> bulletPool = new List<GameObject>();
    private List<float> bulletLifeTime = new List<float>();
    public float bulletLife = 10f;

    private Vector2 aim;
    private Vector2 input;
    private bool joystick = false;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        line = transform.GetChild(0).gameObject;
        spawningPoint = transform.GetChild(1);

        getJoysticks();
    }

    private void getJoysticks()
    {
        string[] joysticks = Input.GetJoystickNames();

        joystick = !(joysticks.Length == 0);

    }

    void Update()
    {
        getInput();
        directionalRotation(transform, aim);
        //animate();
        lineRendering();
        poolUpdate();
    }

    private void getInput()
    {
        if (joystick)
        {
            aim = new Vector2(Input.GetAxis("AimHorizontal"), Input.GetAxis("AimVertical"));
        }
        else
        {
            Vector2 mouse = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            input = new Vector2(mouse.x - transform.position.x, mouse.y - transform.position.y);
            aim = new Vector2(mouse.y - transform.position.y, mouse.x - transform.position.x);
        }
        if (Input.GetButtonDown("Fire1") && Time.time - lastFire > firingSpeed)
        {
            shoot();
        }
        else if (isAutomatic && Input.GetButton("Fire1"))
        {
            automaticFire();
        }
    }

    protected virtual void shoot()
    {
        //transform.parent.GetComponent<SideCharacter>().knockBack(-input, 0.01f);
        pool();
        lastFire = Time.time;

    }

    private void automaticFire()
    {
        if(Time.time - lastFire > firingSpeed)
        {
            transform.parent.GetComponent<SideCharacter>().knockBack(-input, 20f);
            pool();
            lastFire = Time.time;

        }
    }

    protected void animate()
    {

        bool flipSprite = (sprite.flipX ? (transform.rotation.eulerAngles.z < 90 && transform.rotation.eulerAngles.z > -90) : (transform.rotation.eulerAngles.z > 90 || transform.rotation.eulerAngles.z < -90));
        if (flipSprite)
        {
            sprite.flipX = !sprite.flipX;
        }

    }

    private void lineRendering()
    {
        if((Input.GetAxis("AimHorizontal") != 0 || Input.GetAxis("AimVertical") != 0) && !line.activeSelf)
        {

            line.SetActive(true);

        }else if ((Input.GetAxis("AimHorizontal") == 0 & Input.GetAxis("AimVertical") == 0) && line.activeSelf)
        {
            line.SetActive(false);
        }
    }

    //Pool functions

    private GameObject pool()
    {

        for (int i = 0; i<bulletPool.Count; i++)
        {
            if (!bulletPool[i].activeSelf)
            {
                bulletPool[i].SetActive(true);
                bulletPool[i].transform.rotation = transform.rotation;
                bulletPool[i].transform.position = spawningPoint.position;
                bulletPool[i].GetComponent<Rigidbody2D>().AddForce(transform.right * bulletSpeed);

                bulletLifeTime[i] = Time.time;

                return bulletPool[i];
            }
        }

        GameObject b = Instantiate(bullet, spawningPoint.position, transform.rotation);

        bulletPool.Add(b);
        bulletLifeTime.Add(Time.time);

        b.GetComponent<Rigidbody2D>().AddForce(transform.right * bulletSpeed);

        return bulletPool[bulletPool.Count-1];
    }

    private void poolUpdate()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {

            if(bulletPool[i].activeSelf && Time.time - bulletLifeTime[i] > bulletLife)
            {
                bulletPool[i].SetActive(false);
            }

        }
    }
}
