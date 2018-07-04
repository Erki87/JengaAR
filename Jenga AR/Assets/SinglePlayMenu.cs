using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SinglePlayMenu: MonoBehaviour {

    [Space(5)]
    [Header("Menu-Items")]
    public Text tippText;
    public GameObject gameOverMenu;
    public GameObject moveFixedButton;
    public GameObject moveRelativeButton;
    public GameObject gesturesButton;
    public GameObject placingButton;
    Color blue;
	Color grey;

    bool wasPlacingAllowed = false;

	void Start () {
		// Get button references
		moveRelativeButton = GameObject.FindWithTag("RelativeButton");
        moveFixedButton = GameObject.FindWithTag("FixedButton");
		gesturesButton = GameObject.FindWithTag("GesturesButton");
		blue = new Color32( 0x00, 0xC0, 0xFF, 0xFF );
		grey = new Color32( 0x00, 0x00, 0x00, 0x38 );
	}

	public void activateRelativeMove () {
        moveRelativeButton.GetComponent<Image>().color  = blue;
        moveFixedButton.GetComponent<Image>().color 	= grey;
		gesturesButton.GetComponent<Image>().color  = grey;
	}

	public void activateFixedMove () {
        moveRelativeButton.GetComponent<Image>().color 	= grey;
        moveFixedButton.GetComponent<Image>().color 	= blue;
		gesturesButton.GetComponent<Image>().color 	= grey;
    }

	public void activateGesturesMove () {
        moveRelativeButton.GetComponent<Image>().color 	= grey;
        moveFixedButton.GetComponent<Image>().color 	= grey;
		gesturesButton.GetComponent<Image>().color 	= blue;
    }

    public void DisableAll()
    {
        moveRelativeButton.GetComponent<Image>().color = grey;
        moveFixedButton.GetComponent<Image>().color = grey;
        moveFixedButton.GetComponent<Image>().color = grey;
        gesturesButton.GetComponent<Image>().color = grey;
    }

    public void ShowTippText(string text)
    {
        tippText.text = text;
    }

    internal void gameOver()
    {
        gameOverMenu.gameObject.SetActive(true);
    }

    public void EnablePlacing(bool isPlacingAllowed)
    {
        if (isPlacingAllowed)
        {
            placingButton.SetActive(true);
            ShowTippText("Find a good place and tap release");
        }
        else
            placingButton.SetActive(false);
    }
}
