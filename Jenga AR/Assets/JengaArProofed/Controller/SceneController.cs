using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneController : MonoBehaviour
{

    public int sceneId;
    protected DeviceController deviceController;

    protected void Awake()
    {
        EverController.Service().RegisterScene(sceneId);
        EverController.Service().RegisterController(this);
        deviceController = DeviceController.Service();
        CustomAwake();
    }

    public abstract void OnUnLoad();

    public abstract void CustomAwake();

    public void ChangeScene(int id)
    {
        EverController.Service().LoadScene(id);
    }
}
