using System;
using System.Collections;
using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public enum GameStatus
{
    TOWER_NOT_PLACED,
    NOT_PLAYERS_TURN,
    WATCHING,
    MOVE_FIXED,
    MOVE_RELATIVE,
    MOVE_GESTURES,
    PLACING,
    GAME_OVER_LOOSE,
    GAME_OVER_WIN,
}

public enum GestureStyle
{
    NO_FINGER,
    ONE_FINGER,
    TWO_FINGER
}
/// <summary>
/// Class to control the single-play-mode...
/// </summary>
public class SinglePlayController : SceneController {


    [Space(5)]
    [Header("GameObjects")]
    public GameObject ground;

    [Space(5)]
    [Header("Prefabs")]
    public GameObject towerPrefab;
    public GameObject ghostStonePrefab;

    [Space(5)]
    [Header("Menu")]
    public SinglePlayMenu menu;


    private GameObject placedTower;
    private Tower tower;

    private GameObject ghostStone;
    private GameObject grabbedObject;

    private Transform deviceCam;

    private GameStatus status;

    private Vector3 ghostDelta;


    #region LIFECYCLE

    public override void CustomAwake()
    {
        deviceCam = DeviceController.GetCamTransform();
        status = GameStatus.GAME_OVER_WIN;
        UpdateStatus(GameStatus.TOWER_NOT_PLACED);
    }

    public void Update()
    {
        menu.EnablePlacing(IsPlacingAllowed());

        if (IsUIInput()) return;

        switch (status)
        {
            case GameStatus.TOWER_NOT_PLACED:
            case GameStatus.NOT_PLAYERS_TURN:
            case GameStatus.GAME_OVER_LOOSE:
            case GameStatus.GAME_OVER_WIN:
                return;
            case GameStatus.WATCHING:
                PerformWatching();
                break;
            case GameStatus.PLACING:
                PerformPlacing();
                break;
            case GameStatus.MOVE_FIXED:
                break;
            case GameStatus.MOVE_RELATIVE:
                PerformRelative();
                break;
            case GameStatus.MOVE_GESTURES:
                PerformGestures();
                break;
        }
    }

    public void UpdateStatus(GameStatus newStatus)
    {
        if (status == newStatus) return;
        switch (newStatus)
        {
            case GameStatus.TOWER_NOT_PLACED: status = newStatus; StartTowerPlacing(); break;
            case GameStatus.WATCHING: status = newStatus; StartWatching(); break;
            case GameStatus.MOVE_FIXED: status = newStatus; StartMovingFixed(); break;
            case GameStatus.MOVE_RELATIVE: status = newStatus; StartMovingRelative(); break;
            case GameStatus.MOVE_GESTURES: status = newStatus; StartMovingGestures(); break;
            case GameStatus.GAME_OVER_LOOSE: status = newStatus; StartGameOver(); break;

            case GameStatus.PLACING: if (IsPlacingAllowed()) PerformPlacing(); else return; break;
        }
    }

    public void UpdateStatus(string newStatus)
    {
        switch (newStatus)
        {
            case "relative": UpdateStatus(GameStatus.MOVE_RELATIVE); break;
            case "release": UpdateStatus(GameStatus.PLACING); break;
            case "fixed": UpdateStatus(GameStatus.MOVE_FIXED); break;
            case "gestures": UpdateStatus(GameStatus.MOVE_GESTURES); break;
            case "u loose": UpdateStatus(GameStatus.GAME_OVER_LOOSE); break;

        }
    }

    public override void OnUnLoad()
    {
        if (ghostStone != null)
            Destroy(ghostStone);
    }

    #endregion

    #region PLACING THE TOWER

    private void StartTowerPlacing()
    {
        menu.DisableAll();
        StartCoroutine(PlaceTower());
    }

    private System.Collections.IEnumerator PlaceTower()
    {
        DeviceController.Service().EnableVisualising(true);
        while (status == GameStatus.TOWER_NOT_PLACED)
        {
            bool isTracking = deviceController.IsATrackableFound();

            if (isTracking)
                menu.ShowTippText("Tap on the plane to place the tower");
            else
                menu.ShowTippText("Lets find a surface to place the tower");

            if (Input.GetKeyDown(KeyCode.Return))
            {
                placedTower = Instantiate(towerPrefab, transform.position, transform.rotation, ground.transform);

            }
            Touch touch;
            if (isTracking && Input.touchCount > 0 && (touch = Input.GetTouch(0)).phase == TouchPhase.Ended)
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    try
                    {
                        Anchor anchor = deviceController.AnchorOnTrackable(touch.position);
                        placedTower = Instantiate(towerPrefab, anchor.transform.position, anchor.transform.rotation);
                        placedTower.transform.Rotate(0, 180.0f, 0, Space.Self);
                        placedTower.transform.parent = ground.transform;
                        tower = placedTower.GetComponent<Tower>();
                        DeviceController.Service().EnableVisualising(false);
                        UpdateStatus(GameStatus.WATCHING);
                    }
                    catch (Exception)
                    {
                        menu.ShowTippText("Please Restart, Sorry");
                    }
                }
            }
            yield return null;
        }


    }

    #endregion

    #region WATCHING

    private void StartWatching()
    {
        menu.DisableAll();
        menu.ShowTippText("Choose a stone by tapping");
    }

    private void PerformWatching()
    {
        Touch touch;
        if (Input.touchCount == 1 && (touch = Input.GetTouch(0)).phase == TouchPhase.Began)
        {
            RaycastHit hit;
            Ray ray = DeviceController.GetCam().ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.name.ToLower().Contains("stone"))
                {
                    ghostStone = Instantiate(ghostStonePrefab, hit.transform.position, hit.transform.rotation);

                    grabbedObject = hit.transform.gameObject;
                    grabbedObject.GetComponent<Stone>().ActivateGravity(ghostStone.transform);


                    UpdateStatus(GameStatus.MOVE_RELATIVE);
                }
            }

        }
    }

    #endregion

    #region MOVE FIXED RELATIVE OR WITH GESTURES

    private void StartMovingFixed()
    {
        ghostStone.transform.parent = deviceCam;
        menu.activateFixedMove();
        menu.ShowTippText("The stone will allways stay in front of your device.\n Try to reach the top.");
    }

    private void StartMovingRelative()
    {
        ghostStone.transform.parent = ground.transform;
        ghostDelta = ghostStone.transform.position - deviceCam.position;

        menu.activateRelativeMove();
        menu.ShowTippText("The stone moves like your device. Try to place it on top");
    }

    private void StartMovingGestures()
    {
        ghostStone.transform.parent = ground.transform;
        menu.activateGesturesMove();
    }

    private void PerformRelative()
    {
        ghostStone.transform.position = deviceCam.position + ghostDelta;
    }

    private GestureStyle style = GestureStyle.NO_FINGER;

    private void PerformGestures()
    {
        switch (style)
        {
            case GestureStyle.NO_FINGER:
                switch (Input.touchCount)
                {
                    case 1: StartOneFingerGesture(); break;
                    case 2: StartToTwoFingerGesture(); break;
                    default: menu.ShowTippText("Touch the screen with one or two fingers");break;
                }
                break;

            case GestureStyle.ONE_FINGER:
                switch (Input.touchCount)
                {
                    case 0: StopOneFingerGesture(); break;
                    case 1: PerformOneFingerGesture(); break;
                    case 2: SwitchToTwoFingerGesture(); break;
                }
                break;

            case GestureStyle.TWO_FINGER:
                switch (Input.touchCount)
                {
                    case 0: StopTwoFingerGesture(); break;
                    case 1: SwitchToOneFingerGesture(); break;
                    case 2: PerformTwoFingerGesture(); break;
                }
                break;
        }
    }

    private Vector2 lastPos;
    private void StartOneFingerGesture()
    {
        menu.ShowTippText("Rotate the stone by swiping left or right");
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            lastPos = touch.position;
            style = GestureStyle.ONE_FINGER;
        }
    }

    private void PerformOneFingerGesture()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Ended)
        {
            StopOneFingerGesture();
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            float deltaX = (touch.position.x - lastPos.x) * 0.2f;

            Vector3 ghostEulers = ghostStone.transform.eulerAngles;
            ghostStone.transform.eulerAngles = new Vector3(ghostEulers.x, ghostEulers.y + deltaX, ghostEulers.z);
            lastPos = touch.position;
        }
    }

    private void StopOneFingerGesture()
    {
        style = GestureStyle.NO_FINGER;
    }

    private void SwitchToOneFingerGesture()
    {
        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Ended)
            StopOneFingerGesture();
        else if (touch.phase == TouchPhase.Stationary)
        {
            lastPos = touch.position;
            style = GestureStyle.ONE_FINGER;
        }
    }

    private float maxDist;
    private void StartToTwoFingerGesture()
    {
        menu.ShowTippText("Pull the stone by zooming in \nPush the stone by zooming out");
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        if (!(
            touch1.phase == TouchPhase.Ended ||
            touch2.phase == TouchPhase.Ended ||
            touch1.phase == TouchPhase.Canceled ||
            touch2.phase == TouchPhase.Canceled
            ))
        {

            maxDist = Vector2.Distance(touch1.position, touch2.position) * 2f;

            style = GestureStyle.TWO_FINGER;
        }
    }

    private void PerformTwoFingerGesture()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        if (
            touch1.phase == TouchPhase.Ended ||
            touch2.phase == TouchPhase.Ended ||
            touch1.phase == TouchPhase.Canceled ||
            touch2.phase == TouchPhase.Canceled
            )
        {
            StopTwoFingerGesture();
        }
        else if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
        {
            float newDist = Vector2.Distance(touch1.position, touch2.position);

            Vector3 ghostPos = ghostStone.transform.position;
            Vector3 minPos = deviceCam.position +  (ghostPos - deviceCam.position) * 0.1f;
            Vector3 deltaPos = ghostPos - minPos;
            Vector3 maxPos = minPos + 2f * deltaPos;
            Vector3 newPos = ghostPos;

            if (newDist > maxDist) newPos = minPos;
            else
            {
                float learpFactor = newDist / maxDist;
                newPos = Vector3.Lerp(maxPos, minPos, learpFactor);
            }

            ghostStone.transform.position = newPos;
        }
    }

    private void StopTwoFingerGesture()
    {
        style = GestureStyle.NO_FINGER;
    }

    private void SwitchToTwoFingerGesture()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        if (
            touch1.phase == TouchPhase.Ended ||
            touch2.phase == TouchPhase.Ended ||
            touch1.phase == TouchPhase.Canceled ||
            touch2.phase == TouchPhase.Canceled
            )
        {
            StopOneFingerGesture();
        }
        else if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
        {
            StartToTwoFingerGesture();
        }
    }

    #endregion

    #region PLACING

    private void PerformPlacing()
    {
        if (Globals.TryPlaceHigher(grabbedObject.transform.position.y))
        {
            grabbedObject.GetComponent<Stone>().ActivateGravity(null);
            grabbedObject = null;
            Destroy(ghostStone);
            ghostStone = null;
            UpdateStatus(GameStatus.WATCHING);
        }
    }

    #endregion

    #region GAME OVER

    public void StartGameOver()
    {
        if (ghostStone != null)
        {
            Destroy(ghostStone);
            ghostStone = null;
        }
        if (grabbedObject != null) {
            grabbedObject.GetComponent<Stone>().ActivateGravity(null);
            grabbedObject = null;
        }
        menu.gameOver();
    }

    #endregion

    #region BOOL-CHECKS

    private bool IsUIInput()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (Input.touchCount == 0)
            {
                EventSystem.current.SetSelectedGameObject(null);
                return false;
            }
            return true;
        }
        return false;
    }

    private bool IsPlacingAllowed()
    {
        return ((grabbedObject != null) && (Globals.IsHigherThanTop(grabbedObject.transform.position.y)));
    }

    #endregion

    #region AWAKE- AND UNLOADHELPERS

    private void DestroyImagicial()
    {
        if (ghostStone != null)
        {
            Destroy(ghostStone);
            ghostStone = null;
        }
    }

    #endregion
}

