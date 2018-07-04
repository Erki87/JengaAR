using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideCam : MonoBehaviour {

    public Transform firstPersonCam;
    private Transform tower;
    private void Start()
    {
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update () {
        if (tower == null) return;

        transform.position = firstPersonCam.position;
        transform.RotateAround(tower.position, Vector3.up, 90f);
	}

    public void StartSideCam(Transform tower)
    {
        this.tower = tower;
        gameObject.SetActive(true);
    }
}
