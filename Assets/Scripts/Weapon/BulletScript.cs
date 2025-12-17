using System;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Start()
    {
        rb.AddForce(Vector3.forward*0.4f, ForceMode.Impulse);
    }
    
}
