using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour {

    private static Globals instance;

    private void Awake()
    {
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(this);
        }

        else
            DestroyImmediate(this);
    }

    public static Globals Service()
    {
        if (instance == null)
            return new Globals();
         return instance;
    }

    public Anchor anchorOnTrackable;

    public int menuScene;
    public int singlePlayScene;
    public int lobbyScene;
    public int multiPlayScene;

    public float heightToTop;

    public static bool TryPlaceHigher(float newHeight)
    {
        if (newHeight > instance.heightToTop)
        {
            instance.heightToTop = newHeight;
            return true;
        }
        return false;
    }

    public static bool IsHigherThanTop(float height)
    {
        return height > instance.heightToTop;
    }
}
