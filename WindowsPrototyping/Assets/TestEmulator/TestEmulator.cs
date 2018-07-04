using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEmulator : MonoBehaviour {

    [SerializeField]
    private GameObject placedTower;
    [SerializeField]
    private Tower tower;
    public GameObject towerPrefab;

    public PlaneEmulator planeEmulator;
	// Use this for initialization
	void Start () {
        StartCoroutine(PlaceTower());
	}

    private System.Collections.IEnumerator PlaceTower()
    {


            while (!planeEmulator.IsPlaneFound())
                yield return null;

                Debug.Log("drin");
                placedTower = Instantiate(towerPrefab, planeEmulator.GetPositionAndDestroy(), transform.rotation);
                tower = placedTower.GetComponent<Tower>();
                Debug.Log("drüber");

            Debug.Log("draussen");

        Debug.Log("fertig");
    }


            // Update is called once per frame
            void Update () {

	}
}
