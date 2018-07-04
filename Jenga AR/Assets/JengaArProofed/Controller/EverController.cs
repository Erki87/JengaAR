using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
// NOTE:
// - InstantPreviewInput does not support `deltaPosition`.
// - InstantPreviewInput does not support input from
//   multiple simultaneous screen touches.
// - InstantPreviewInput might miss frames. A steady stream
//   of touch events across frames while holding your finger
//   on the screen is not guaranteed.
// - InstantPreviewInput does not generate Unity UI event system
//   events from device touches. Use mouse/keyboard in the editor
//   instead.
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class EverController : MonoBehaviour {

    public static int awake = 0;

    public GameObject device;

    [SerializeField]
    private SceneController currentSceneController;

    [SerializeField]
    private static EverController instance;

    private bool loadInProcess = false;
    private bool scenesLoadable
    {
        get
        {
            if (!loadInProcess)
            {
                loadInProcess = true;
                return true;
            }
            return false;
        }
        set { loadInProcess = value; }
    }

    private bool _first = true;
    private bool first
    {
        get
        {
            if (!_first) return false;
            _first = false;
            return true;
        }
    }


    private void Awake()
    {
        if (first)
        {
            instance = this;
            currentSceneController = null;
            StartCoroutine(LaunchApp());
        }

        else
          DestroyImmediate(this);
    }

    private IEnumerator LaunchApp()
    {
        yield return new WaitForEndOfFrame();
        LoadScene(1);
    }

    public static EverController Service()
    {
        if (instance == null)
           return new EverController();
        return instance;
    }

    public void RegisterScene(int id)
    {
        Scene newScene = SceneManager.GetSceneByBuildIndex(id);
        if (currentSceneController != null)
        {
            currentSceneController.OnUnLoad();
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        }
        StartCoroutine(Activate(id));

        if (id == 1) device.SetActive(false);
        else device.SetActive(true);
        loadInProcess = false;
    }

    private IEnumerator Activate(int id)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(id));
    }

    public void RegisterController(SceneController controller)
    {
        currentSceneController = controller;

    }

    public void LoadScene(int id)
    {
        if (scenesLoadable)
            SceneManager.LoadSceneAsync(id, LoadSceneMode.Additive);
    }
}
