using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehav : MonoBehaviour {

    Rigidbody rb;
    public float addedForce;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update () {

	}

    public void MoveLeft()
    {
        rb.AddForce(transform.right * -addedForce);
    }

    public void MoveRight()
    {
        rb.AddForce(transform.right * addedForce);
    }

    public void MoveForward()
    {
        rb.AddForce(transform.forward * addedForce);
    }

    public void MoveBackward()
    {
        rb.AddForce(transform.forward * -addedForce);
    }

    public void MoveUp()
    {
        rb.AddForce(transform.up * addedForce);
    }

    public void MoveDown()
    {
        rb.AddForce(transform.up * -addedForce);
    }
}

