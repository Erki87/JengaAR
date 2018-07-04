using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Class to control the single-play-mode...
/// </summary>
public class SingleEmulator : MonoBehaviour
{



    [Header("Cameras")]
    public Camera FirstPersonCamera;
    public SideCam sideCam;
    public PlaneEmulator planeEmulator;

    [Space(5)]
    [Header("GameObjects")]
    public GameObject ground;

    [Space(5)]
    [Header("Prefabs")]
    public GameObject towerPrefab;
    public GameObject imaginationalStonePrefab;

    [Space(5)]
    [Header("Menu-Items")]
    public Text debugText;
    public GameObject gameOverMenu;
    public GameObject freezeButton;

    private GameObject placedTower;
    private Tower tower;

    private GameObject imaginationalStone;
    private GameObject grabbedObject;
    private GameObject lastStone;

    private bool yourTurn = false;
    private bool freeze = false;



    public void Start()
    {
        placedTower = null;
        debugText.text = "LetsGo";
        StartCoroutine(PlaceTower());
    }

    private System.Collections.IEnumerator PlaceTower()
    {

        while (!planeEmulator.IsPlaneFound())
            yield return null;


        placedTower = Instantiate(towerPrefab, planeEmulator.GetPositionAndDestroy(), Quaternion.identity);
        placedTower.transform.Rotate(0, 180.0f, 0, Space.Self);

        placedTower.transform.parent = ground.transform;
        tower = placedTower.GetComponent<Tower>();

        debugText.text = "Tower Placed! Tower:" + placedTower.transform.position;

        yourTurn = true;
        sideCam.StartSideCam(placedTower.transform);
        yield return new WaitForSeconds(1);


    }


    public void Update()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        debugText.text = "CamPos: " + FirstPersonCamera.transform.position;
        if (yourTurn && (tower != null)) PerformTurn();
    }

    private void PerformTurn()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (grabbedObject == null) PerformGrabbing();
            else
            {
                PerformMoving();

                if (!freeze) PerformPlacing();
            }

        }
    }

    private void PerformGrabbing()
    {
        Touch touch;
        if (Input.touchCount > 0 && (touch = Input.GetTouch(0)).phase == TouchPhase.Began)
        {
            // Raycast against the location the player touched to search for planes.
            RaycastHit hit;
            Ray ray = FirstPersonCamera.ScreenPointToRay(touch.position);
            debugText.text = "touch: " + ray.ToString();
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name.ToLower().Contains("stone"))
                {
                    imaginationalStone = Instantiate(imaginationalStonePrefab, hit.transform.position, hit.transform.rotation);
                    imaginationalStone.transform.parent = FirstPersonCamera.transform;
                    grabbedObject = hit.transform.gameObject;
                    grabbedObject.GetComponent<Rigidbody>().useGravity = false;
                    debugText.text += " hit a stone";
                }
                //else

            }
            // else

        }
    }

    private void PerformMoving()
    {
        grabbedObject.transform.position =
               Vector3.Lerp(grabbedObject.transform.position, imaginationalStone.transform.position, 0.3f);
        grabbedObject.transform.rotation =
            Quaternion.Lerp(grabbedObject.transform.rotation, imaginationalStone.transform.rotation, 0.3f);
    }

    private void PerformPlacing()
    {
        if (Input.touchCount > 0 && (Input.GetTouch(0)).phase == TouchPhase.Began)
        {
            grabbedObject.transform.parent = ground.transform;
            grabbedObject.GetComponent<Rigidbody>().useGravity = true;
            grabbedObject = null;
            DestroyImmediate(imaginationalStone);
            freezeButton.SetActive(false);
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void OneMoreTime()
    {
        SceneManager.LoadScene("SinglePlayScene");
    }

    public void GameOver()
    {
        if (yourTurn)
        {
            debugText.text = "GAME-OVER \n You loose";
            yourTurn = false;
            gameOverMenu.gameObject.SetActive(true);
        }

    }

    public void Freeze()
    {
        if (imaginationalStone == null) return;

        if (imaginationalStone.transform.parent == ground.transform)
        {
            imaginationalStone.transform.parent = FirstPersonCamera.transform;
            debugText.text = "M O V E !";
            freeze = false;
        }

        else
        {
            imaginationalStone.transform.parent = ground.transform;
            debugText.text = "F R E E Z E !";
            freeze = true;
        }
    }
}

