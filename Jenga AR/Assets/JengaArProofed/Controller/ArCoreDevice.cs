using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArCoreDevice : MonoBehaviour {

    public Vector3 startPosition;
    public Vector3 startEulers;
    public Camera firstPersonCam;

    private void Awake()
    {
        transform.position = startPosition;
        transform.eulerAngles = startEulers;
        StartCoroutine(EnableCamera());
    }

    private IEnumerator EnableCamera()
    {
        yield return new WaitForEndOfFrame();

        firstPersonCam.gameObject.SetActive(true);
    }
}
