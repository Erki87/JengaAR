using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;


public class TestCubeBehav : MonoBehaviour {

    public GameObject testObject;

    // Use this for initialization
    void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

    public void OnSetActive(bool newValue)
    {
        gameObject.SetActive(newValue);
    }

    public void OnTriggerToDebug()
    {
        Debug.Log("getriggert durch TestCubeBehav_OnTriggerToDebug()");
        GameObject.Instantiate(testObject);
    }
}
