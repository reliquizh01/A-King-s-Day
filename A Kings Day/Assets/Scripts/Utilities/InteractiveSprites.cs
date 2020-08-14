using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public enum SpriteInteractionType
{
    EaseInHover,
    TeleportToHover,
    EaseInClick,
    TeleportToClick,
    SwitchBetween,
    ChangeSprite,
}

public class InteractiveSprites : MonoBehaviour
{
    public SpriteRenderer sprite;
    public SpriteInteractionType startInteractionType;
    public SpriteInteractionType endInteractionType;

    public bool isHovered;
    public float easeSpeed;

    [Header("Click State")]
    public bool switchedOn = false;
    [Header("Local Positions")]
    public Vector2 origPoint;
    public Vector2 targetPoint;

    public bool isMoving = false;
    public bool isSwitching = false;
    private Vector2 curTargetPoint;
    private float interval = 0.25f;

    [SerializeField] private Sprite origSprite;
    [SerializeField] private Sprite pressSprite;
    public void Start()
    {

    }

    public void Update()
    {
        if(isMoving)
        {
            transform.localPosition = Vector2.MoveTowards(transform.localPosition, curTargetPoint, easeSpeed * Time.deltaTime);
            if((Vector2)transform.localPosition == curTargetPoint)
            {
                isMoving = false;
            }
        }
    }
    public void OnMouseEnter()
    {
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if(CheckMouseEnter())
        {
            StartInteraction();
        }
    }
    public void OnMouseExit()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (CheckMouseExit())
        {
            EndInteraction();
        }
    }

    public void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (startInteractionType == SpriteInteractionType.ChangeSprite)
        {
            sprite.sprite = pressSprite;
        }

        if(CheckMouseClick())
        {
            StartInteraction();
        }
    }

    public void OnMouseUp()
    {
        if (startInteractionType == SpriteInteractionType.ChangeSprite)
        {
            sprite.sprite = origSprite;
        }
    }

    public bool CheckMouseClick()
    {
        return startInteractionType == SpriteInteractionType.EaseInClick || startInteractionType == SpriteInteractionType.TeleportToClick;
    }
    public bool CheckMouseEnter()
    {
        return startInteractionType == SpriteInteractionType.ChangeSprite ||startInteractionType == SpriteInteractionType.SwitchBetween || startInteractionType == SpriteInteractionType.EaseInHover || startInteractionType == SpriteInteractionType.TeleportToHover;
    }
    public bool CheckMouseExit()
    {
        return startInteractionType == SpriteInteractionType.ChangeSprite  || startInteractionType == SpriteInteractionType.SwitchBetween || startInteractionType == SpriteInteractionType.EaseInHover || startInteractionType == SpriteInteractionType.TeleportToHover;
    }
    public void StartInteraction()
    {
        isMoving = true;
        switch (startInteractionType)
        {
            case SpriteInteractionType.EaseInHover:

                break;
            case SpriteInteractionType.TeleportToHover:
                sprite.transform.localPosition = targetPoint;
                break;
            case SpriteInteractionType.SwitchBetween:
                isSwitching = true;
                StartCoroutine(SwitchPosition());
                break;
            case SpriteInteractionType.ChangeSprite:
                
                break;
            case SpriteInteractionType.EaseInClick:
                MoveToPosition();
                break;
        }
    }

    private void MoveToPosition()
    {
        if((Vector2)transform.localPosition == origPoint)
        {
            curTargetPoint = targetPoint;
            switchedOn = true;
        }
        else
        {
            curTargetPoint = origPoint;
            switchedOn = false;
        }
    }
    private IEnumerator SwitchPosition()
    {
        yield return new WaitForSeconds(interval);

        sprite.transform.localPosition = curTargetPoint;

        if(curTargetPoint == targetPoint)
        {
            curTargetPoint = origPoint;
        }
        else
        {
            curTargetPoint = targetPoint;
        }

        if(isSwitching)
        {
            StartCoroutine(SwitchPosition());
        }
        else
        {
            sprite.transform.localPosition = origPoint;
            curTargetPoint = targetPoint;
        }
    }
    public void EndInteraction()
    {
        isMoving = true;
        switch (startInteractionType)
        {
            case SpriteInteractionType.EaseInHover:
                isMoving = false;
                break;
            case SpriteInteractionType.TeleportToHover:
                sprite.transform.localPosition = origPoint;
                break;
            case SpriteInteractionType.SwitchBetween:
                isSwitching = false;
                break;
            case SpriteInteractionType.ChangeSprite:
                sprite.sprite = origSprite;
                break;
        }
    }
}
