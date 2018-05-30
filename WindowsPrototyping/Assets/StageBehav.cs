using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBehav : MonoBehaviour {

    StageManager stageManager;
	// Use this for initialization
	void Start () {
        stageManager = FindObjectOfType<StageManager>();
        stageManager.SubscribeAsStage(this.transform);
	}

	// Update is called once per frame
	void Update () {

	}
}
