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
    public int visualLayoutOrder = 0;
    public TileConversionHandler battleTile;
    public List<ScenePointBehavior> neighborPoints;
    public bool isSpawnPoint = false;

    [Header("Scene Offsets")]
    public bool straightToOffset;
    public GameObject sceneOffset;

    [Header("Scene Loader")]
    public bool sceneLoader = false;
    public SceneType SceneToLoad;

    [Header("Scene Force Face Direction")]
    public bool forceFacing = false;
    public FacingDirection directionToFace;
    public void Awake()
    {
    }
    public override void Start()
    {
        base.Start();
        if (ScenePointPathfinder.GetInstance != null)
        {
            ScenePointPathfinder.GetInstance.AddCurrentScenePoints(this);
        }

        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        SwitchTileType();

        if(SpawnManager.GetInstance != null)
        {
            if(isSpawnPoint)
            {
                SpawnManager.GetInstance.AddSpawnPoint(this);
            }
        }
    }

    public void SwitchTileType()
    {
        switch (currentPointStatus)
        {
            case ScenePointStatus.Pathfinding:
                if (battleTile != null)
                {
                    battleTile.gameObject.SetActive(false);
                }
                break;
            case ScenePointStatus.BattlePathfinding:
                if (battleTile != null)
                {
                    battleTile.gameObject.SetActive(true);
                }
                break;
            default:
                break;
        }
    }
    public void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (!isClickable)
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
        if (!isClickable)
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
        if(!isClickable)
        {
            return;
        }
        if(currentPointStatus == ScenePointStatus.BattlePathfinding)
        {

        }
        else
        {
            BaseCharacter player = TransitionManager.GetInstance.currentSceneManager.player;
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if(TransitionManager.GetInstance != null)
            {
                EventBroadcaster.Instance.PostEvent(EventNames.HIDE_TOOLTIP_MESG);
                if(sceneLoader)
                {
                    Debug.Log("Is SceneLoader!");
                    EventBroadcaster.Instance.PostEvent(EventNames.HIDE_RESOURCES);
                    EventBroadcaster.Instance.PostEvent(EventNames.DISABLE_IN_GAME_INTERACTION);
                    player.OrderMovement(this, SceneLoader);
                }
                else
                {
                    if (forceFacing)
                    {
                        player.OrderMovement(this, ()=> player.OrderToFace(directionToFace));
                    }
                    else
                    {
                        player.OrderMovement(this);
                    }
                }

            }
        }
    }

    public void SceneLoader()
    {
        TransitionManager.GetInstance.LoadScene(SceneToLoad);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentPointStatus != ScenePointStatus.BattlePathfinding)
            return;

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
        if (currentPointStatus != ScenePointStatus.BattlePathfinding)
            return;


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
