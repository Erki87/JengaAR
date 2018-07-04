using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour {

    public Transform podest;
    public Transform towerGround;
    public Transform smallStone;
    public Transform mediumStone;
    public Transform bigStone;

    public Vector3 placePosition;
    public Vector3 placeRotation;
    public Vector3 currentHeight;
    public int layers;


	// Use this for initialization
	void Start() {
        StartCoroutine(CreateTower());
	}

    private IEnumerator CreateTower()
    {
        int currentLayers = 0;
        bool frontalLayer = true;

        placePosition = transform.position;
        PlacePodest();
        PlaceGround();

        while (currentLayers < this.layers)
        {
            yield return new WaitForFixedUpdate();

            if (frontalLayer) CreateFrontalLayer();
            else CreateQuerLayer();

            currentLayers++;
            frontalLayer = !frontalLayer;

        }
        Globals.Service().heightToTop = currentHeight.y;
    }

    private void PlacePodest()
    {
        Vector3 position = placePosition + Vector3.up * podest.lossyScale.y * 0.5f;
        Instantiate(podest, position, transform.rotation);
        placePosition += Vector3.up * podest.lossyScale.y;
    }

    private void CreateQuerLayer()
    {
        Quaternion rotation = Quaternion.Euler(placeRotation);
        Transform stone = RandomStone();
        Vector3 position = new Vector3(currentHeight.x, currentHeight.y + stone.lossyScale.y * 0.5f, currentHeight.z - 0.06f);
        Instantiate(stone, position, rotation);

        stone = RandomStone();
        position = new Vector3(currentHeight.x, currentHeight.y + stone.lossyScale.y * 0.5f, currentHeight.z);
        Instantiate(stone, position, rotation);

        stone = RandomStone();
        position = new Vector3(currentHeight.x, currentHeight.y + stone.lossyScale.y * 0.5f, currentHeight.z + 0.06f);
        Instantiate(stone, position, rotation);
        currentHeight = new Vector3(currentHeight.x, currentHeight.y + 0.03f, currentHeight.z);
    }

    private void CreateFrontalLayer()
    {
        Quaternion rotation = Quaternion.Euler(new Vector3(placeRotation.x, placeRotation.y + 90f, placeRotation.z));
        Transform stone = RandomStone();
        Vector3 position = new Vector3(currentHeight.x - 0.06f, currentHeight.y + stone.lossyScale.y * 0.5f, currentHeight.z);
        Instantiate(stone, position, rotation);

        stone = RandomStone();
        position = new Vector3(currentHeight.x, currentHeight.y + stone.lossyScale.y * 0.5f, currentHeight.z);
        Instantiate(stone, position, rotation);

        stone = RandomStone();
        position = new Vector3(currentHeight.x + 0.06f, currentHeight.y + stone.lossyScale.y * 0.5f, currentHeight.z);
        Instantiate(stone, position, rotation);
        currentHeight = new Vector3(currentHeight.x, currentHeight.y + 0.03f, currentHeight.z);

    }

    private Transform RandomStone()
    {
        switch ((int) UnityEngine.Random.Range(0, 3))
        {
            case 0: return smallStone;
            case 1: return bigStone;
            default: return mediumStone;
        }
    }

    private void PlaceGround()
    {
        placePosition += Vector3.up * towerGround.lossyScale.y*0.5f;
        Instantiate(towerGround, placePosition, Quaternion.Euler(placeRotation));
        currentHeight = new Vector3(placePosition.x, placePosition.y + towerGround.lossyScale.y*0.5f, placePosition.z);
    }

    // Update is called once per frame
    void Update () {

	}
}
