using System;
using System.Collections;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// Class to control the single-play-mode...
/// </summary>
public class SinglePlayController : MonoBehaviour {


    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
    /// </summary>
    public Camera FirstPersonCamera;

    /// <summary>
    /// A model to place when a raycast from a user touch hits a plane.
    /// </summary>
    public GameObject tower;
    public GameObject ground;
    private GameObject placedTower;
    public GameObject imaginationalStonePrefab;
    [SerializeField]
    private GameObject imaginationalStone;

    [SerializeField]
    private GameObject grabbedObject;
    [SerializeField]
    private bool yourTurn = false;
    public Text debugText;
    public GameObject oneMoreTimeButton;
    /// <summary>
    /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
    /// </summary>
    public GameObject SearchingForPlaneUI;

    /// <summary>
    /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
    /// the application to avoid per-frame allocations.
    /// </summary>
    private System.Collections.Generic.List<DetectedPlane> m_AllPlanes = new System.Collections.Generic.List<DetectedPlane>();

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;

    public void Start()
    {
        placedTower = null;
        debugText.text = "LetsGo";
        StartCoroutine(PlaceTower());
    }

    private System.Collections.IEnumerator PlaceTower()
    {
        UsbManager
        int counter = 0;
        while (placedTower == null)
        {
            counter++;
            debugText.text = counter.ToString();
            // Hide snackbar when currently tracking at least one plane.
            Session.GetTrackables<DetectedPlane>(m_AllPlanes);
            bool showSearchingUI = true;
            for (int i = 0; i < m_AllPlanes.Count; i++)
            {
                if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
                {
                    showSearchingUI = false;
                    break;
                }
            }

            SearchingForPlaneUI.SetActive(showSearchingUI);

            // If the player has not touched the screen, we are done with this update.
            Touch touch;
            if (Input.touchCount > 0 && (touch = Input.GetTouch(0)).phase == TouchPhase.Began)
            {
                // Raycast against the location the player touched to search for planes.
                TrackableHit hit;
                TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                    TrackableHitFlags.FeaturePointWithSurfaceNormal;

                if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
                {
                    // Use hit pose and camera pose to check if hittest is from the
                    // back of the plane, if it is, no need to create the anchor.
                    if ((hit.Trackable is DetectedPlane) &&
                        Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                            hit.Pose.rotation * Vector3.up) < 0)
                    {
                        Debug.Log("Hit at back of the current DetectedPlane");
                    }
                    else
                    {
                        // Instantiate Andy model at the hit pose.
                        placedTower = Instantiate(tower, hit.Pose.position, hit.Pose.rotation);
                        placedTower.transform.Rotate(0, 180.0f, 0, Space.Self);

                       // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                        // world evolves.
                        var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                        // Make Andy model a child of the anchor.
                        placedTower.transform.parent = ground.transform;
                        yourTurn = true;
                        debugText.text = "Tower Placed! Tower:" + placedTower.transform.position + " \nCam: " + FirstPersonCamera.transform.position;
                        //yield return new WaitForSeconds(1);
                        FindObjectOfType<PointcloudVisualizer>().StopVisualising();
                    }
                }
            }
            yield return null;
        }
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update()
    {
        _UpdateApplicationLifecycle();
        if (yourTurn) PerformTurn();
    }

    private void PerformTurn()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            switch (grabbedObject == null)
            {
                case true: PerformGrabbing(); break;
                case false: PerformPlacing(); break;
            }
        }
        else debugText.text = "Touched eventSystem";

    }

    private void PerformGrabbing()
    {
        Touch touch;
        if (Input.touchCount > 0 && (touch = Input.GetTouch(0)).phase == TouchPhase.Began)
        {
            // Raycast against the location the player touched to search for planes.
            RaycastHit hit;
            Ray ray = FirstPersonCamera.ScreenPointToRay(touch.position);
            Debug.DrawRay(touch.position, new Vector3(touch.position.x, touch.position.y, 10));
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
                else debugText.text += " hit " + hit.collider.name;

            }
            else debugText.text += " hit nothing";

        }
    }

    private void PerformPlacing()
    {
        grabbedObject.transform.position = Vector3.Lerp(grabbedObject.transform.position, imaginationalStone.transform.position, 0.33f);
        grabbedObject.transform.rotation = Quaternion.Lerp(grabbedObject.transform.rotation, imaginationalStone.transform.rotation, 0.33f);

        Touch touch;
        if (Input.touchCount > 0 && (touch = Input.GetTouch(0)).phase == TouchPhase.Began)
        {
            grabbedObject.transform.parent = ground.transform;
            grabbedObject.GetComponent<Rigidbody>().useGravity = true;
            grabbedObject = null;
            DestroyImmediate(imaginationalStone);
        }
    }

    private void FixedUpdate()
    {
        if (grabbedObject!= null)
            grabbedObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }



    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    private void _UpdateApplicationLifecycle()
    {
        // Exit the app when the 'back' button is pressed.
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
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
            oneMoreTimeButton.gameObject.SetActive(true);
        }

    }
}

