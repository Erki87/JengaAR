using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : SceneController
{
	public void QuitGame()
	{
		Application.Quit ();
		Debug.Log ("Quit");
	}

    public override void OnUnLoad()
    {

    }

    public override void CustomAwake()
    {

    }
}
