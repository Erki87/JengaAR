using GoogleARCore;

using UnityEngine;

public class DeviceController : MonoBehaviour {

    private static DeviceController instance;
    public Camera deviceCam;
    public GameObject planVisualising;

    public static DeviceController Service()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(gameObject);
    }

    public static Camera GetCam()
    {
        return instance.deviceCam;
    }

    public static Transform GetCamTransform()
    {
        return instance.deviceCam.transform;
    }

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;

    /// <summary>
    /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
    /// the application to avoid per-frame allocations.
    /// </summary>
    private System.Collections.Generic.List<DetectedPlane> m_AllPlanes = new System.Collections.Generic.List<DetectedPlane>();

    public bool IsATrackableFound()
    {
        Session.GetTrackables<DetectedPlane>(m_AllPlanes);

        for (int i = 0; i < m_AllPlanes.Count; i++)
        {
            if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
            {
                return true;
            }
        }
        return false;
    }

    // Update is called once per frame
    public void Update()
    {
        _UpdateApplicationLifecycle();
    }

    public Anchor AnchorOnTrackable(Vector2 pointOnScreen)
    {
        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(pointOnScreen.x, pointOnScreen.y, raycastFilter, out hit))
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if (!((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(Camera.main.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0))
            {
                return hit.Trackable.CreateAnchor(hit.Pose);
            }
        }
        return null;
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

    public void EnableVisualising(bool value)
    {
        planVisualising.SetActive(value);
    }

}
