using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Managers;
using Utilities;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Characters;

public enum ScenePointStatus
{
    Pathfinding,
    BattlePathfinding,
}

public class ScenePointBehavior : BaseInteractableBehavior
{
    [Header("Scene Information")]
    public ScenePointStatus currentPointStatus = ScenePointStatus.Pathfinding;
    public int sceneIndex = 0;
    public TileConversionHandler battleTile;
    public List<ScenePointBehavior> neighborPoints;
    [Header("Scene Offsets")]
    public bool straightToOffset;
    public GameObject sceneOffset;

    [Header("Scene Loader")]
    public bool sceneLoader = false;
    public SceneType SceneToLoad;

    public override void Start()
    {
        base.Start();
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
        if(currentPointStatus == ScenePointStatus.BattlePathfinding)
        {
            if (BattlefieldSceneManager.GetInstance.isCampaignMode)
            {
                battleTile.ShowHoverTile();
            }
        }
        else if (!string.IsNullOrEmpty(mesg) && !isInteractingWith)
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
        if (currentPointStatus == ScenePointStatus.BattlePathfinding)
        {
            if (BattlefieldSceneManager.GetInstance.isCampaignMode)
            {
                battleTile.HideHoverTile();
            }
        }
        else if (!string.IsNullOrEmpty(mesg))
        {
            EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);
        }        
    }
    public void OnMouseDown()
    {
        if(currentPointStatus == ScenePointStatus.BattlePathfinding)
        {

        }
        else
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
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        BaseCharacter colCharacter = (collision.gameObject.GetComponent<BaseCharacter>() != null) ? collision.gameObject.GetComponent<BaseCharacter>() : null;

        if(colCharacter == null)
        {
            return;
        }

        if(battleTile != null)
        {
            battleTile.AddCharacterSteppingIn(colCharacter);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        BaseCharacter colCharacter = (collision.gameObject.GetComponent<BaseCharacter>() != null) ? collision.gameObject.GetComponent<BaseCharacter>() : null;

        if (colCharacter == null)
        {
            return;
        }

        if(battleTile != null)
        {
            battleTile.RemoveChacracterSteppingIn(colCharacter);
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
