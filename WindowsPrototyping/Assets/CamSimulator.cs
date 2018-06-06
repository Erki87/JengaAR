using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSimulator : MonoBehaviour {

    public Transform tower;
    public Transform fpCam;
    public Transform towerCam;
    private GameObject grabbedObject;

    // Use this for initialization
    void Start () {

	}

	// Update is called once per frame
	void Update () {

        HandleTowerCam();

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += (transform.forward * -1f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += (transform.forward * 1f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += (transform.right * -1f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += (transform.right * 1f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.PageUp))
        {
            transform.position += (transform.up * 1f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.PageDown))
        {
            transform.position += (transform.up * -1f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAround(tower.transform.position, Vector3.up, 10f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.RotateAround(tower.transform.position, Vector3.up, -10f * Time.deltaTime);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (grabbedObject == null)
            {
                // Raycast against the location the player touched to search for planes.
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.name.Contains("Stone"))
                    {
                        hit.transform.parent = transform;
                        grabbedObject = hit.transform.gameObject;
                        grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                    }

                }
            }

            else
            {
                grabbedObject.transform.parent = null;
                grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                towerCam.position =
                    new Vector3(towerCam.position.x, grabbedObject.transform.position.y + 0.2f, towerCam.position.z);
                grabbedObject = null;
            }


        }
    }

    private void HandleTowerCam()
    {
        towerCam.rotation = Quaternion.Euler(towerCam.eulerAngles.x, fpCam.eulerAngles.y, towerCam.eulerAngles.z);
    }
}
