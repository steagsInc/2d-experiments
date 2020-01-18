using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{

    public Transform target;
    public bool sameRotation = false;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 nextpos = new Vector3(target.position.x, target.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, nextpos, 0.5f);

        if (sameRotation) transform.rotation = target.GetChild(0).rotation;
    }
}
