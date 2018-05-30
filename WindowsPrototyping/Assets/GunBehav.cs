using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBehav : MonoBehaviour {

    public GameObject gunSphere;
    public GameObject projectile;
    public Transform start;
    public Transform end;
    public float projectileForce;

    StageManager stageManager;
	// Use this for initialization
	void Start () {
        stageManager = FindObjectOfType<StageManager>();
	}

	// Update is called once per frame
	void Update () {

	}

    public void Fire()
    {
        Transform newProjectile = Instantiate(projectile, start.position, start.rotation).transform;
        stageManager.SubscribeAsTransform(newProjectile);
        newProjectile.LookAt(end);
        newProjectile.GetComponent<Rigidbody>().AddForce(newProjectile.transform.forward * projectileForce);
    }

    public void GunUp()
    {
        gunSphere.transform.Rotate(Vector3.left * 10);
    }

    public void GunDown()
    {
        gunSphere.transform.Rotate(Vector3.left * -10);
    }
}
