using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneEmulator : MonoBehaviour {

    private bool foundPlane;
    private Vector3 position;


	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            if (!foundPlane)
            {
                // Raycast against the location the player touched to search for planes.
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {

                    position = hit.point;
                    Debug.Log("Found emulated plane: " + position.ToString());
                    foundPlane = true;
                }
            }
        }
    }

    public bool IsPlaneFound()
    {
        return foundPlane;
    }

    public Vector3 GetPositionAndDestroy()
    {
        if (!foundPlane) return Vector3.zero;

        StartCoroutine(DestroyMe());
        return position;
    }

    private IEnumerator DestroyMe()
    {
        yield return new WaitForEndOfFrame();

        Destroy(this);
    }
}
