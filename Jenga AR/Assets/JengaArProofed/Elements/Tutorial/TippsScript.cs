using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TippsScript : MonoBehaviour {

    private Text txt;
    private bool tippsActivated = false;

	public void SetStep(int step) {
		switch(step) {
			case 1:
				this.SetText("Find a plane surface and place the tower by touching the surface");
				break;
			case 2:
				this.SetText("Choose a block on the tower");
				break;
			case 3:
				this.SetText("Pull the block out carefully, so that the tower doesn't fall");
				break;
			default:
				this.SetText("Error. Step " + step + " doesn't exist");
				break;
		}
	}

	// Use this for initialization
	public void SetText (string text) {
		txt = gameObject.GetComponent<Text>();
	    txt.text = text;
	}
}
