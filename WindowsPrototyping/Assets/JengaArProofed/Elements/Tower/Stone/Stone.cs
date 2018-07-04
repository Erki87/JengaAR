using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour {

    private void Awake()
    {
        GetComponent<Rigidbody>().Sleep();
    }
    // Use this for initialization
    void Start () {

	}

	// Update is called once per frame
	void Update () {

	}
}
