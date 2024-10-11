using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HadpresentPhysic : MonoBehaviour
{
    private Rigidbody rb;
    public Transform target;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rb.velocity = (target.position - transform.position) /Time.fixedDeltaTime;

        //rotation
        Quaternion rotationDifference = target.rotation * Quaternion.Inverse(transform.rotation);
        rotationDifference.ToAngleAxis(out float angleInDegree, out Vector3 rotationAxis);

        Vector3 rotationDiffrenceInDegree = angleInDegree * rotationAxis;

        rb.angularVelocity = (rotationDiffrenceInDegree * Mathf.Rad2Deg / Time.fixedDeltaTime);
    }
}
