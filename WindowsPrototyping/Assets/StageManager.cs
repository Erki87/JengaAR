using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour {

    public Transform activeStage;
    public List<Transform> registeredTransforms;
	// Use this for initialization

    public void SubscribeAsStage(Transform stage)
    {
        this.activeStage = stage;

        foreach(Transform transform in registeredTransforms)
        {
            transform.parent = stage;
        }
    }

    public void SubscribeAsTransform(Transform transform)
    {
        this.registeredTransforms.Add(transform);
        if (activeStage != null) transform.parent = activeStage;
    }


}
