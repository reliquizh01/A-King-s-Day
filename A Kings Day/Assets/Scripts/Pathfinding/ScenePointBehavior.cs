using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Managers;
using Utilities;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class ScenePointBehavior : BaseInteractableBehavior
{
    [Header("Scene Information")]
    public int sceneIndex = 0;
    public List<ScenePointBehavior> neighborPoints;

    [Header("Scene Offsets")]
    public bool straightToOffset;
    public GameObject sceneOffset;

    [Header("Scene Loader")]
    public bool sceneLoader = false;
    public SceneType SceneToLoad;
    public void Start()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if(ScenePointPathfinder.GetInstance != null)
        {
            ScenePointPathfinder.GetInstance.AddCurrentScenePoints(this);
        }
    }

    public void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (!string.IsNullOrEmpty(mesg) && !isInteractingWith)
        {
            Parameters p = new Parameters();
            p.AddParameter<string>("Mesg", mesg);
            EventBroadcaster.Instance.PostEvent(EventNames.SHOW_TOOLTIP_MESG, p);
        }
    }

    public void OnMouseExit()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (!string.IsNullOrEmpty(mesg))
        {
            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);
        }        
    }
    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (TransitionManager.GetInstance.currentSceneManager.king.myMovements.isMoving)
        {
            return;
        }

        if(TransitionManager.GetInstance != null)
        {
            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);
            TransitionManager.GetInstance.currentSceneManager.OrderCharacterToMove(this);
        }
    }

    public void SetAsActive(bool active)
    {
        if(active)
        {
            EnablePoint();
        }
        else
        {
            DisablePoint();
        }
    }

    private void EnablePoint()
    {
        if(isFurniture) // HIGHLIGHT
        {

        }
        else
        {

        }
    }
    private void DisablePoint()
    {
        if (isFurniture) // HIGHLIGHT
        {

        }
        else
        {

        }
    }
}
