using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using Utilities;
using Managers;

public class BaseInteractableBehavior : MonoBehaviour
{
    [Header("Tooltip Mesg")]
    public string mesg;
    public bool isInteractingWith;
    public bool isClickable = true;
    public bool hasInteractionOptions = true;
    //MAKE AN INTERACTABLE FURNITURES.
    public bool isFurniture = false;
    public BoxCollider2D myCol;
    public virtual void Start()
    {

    }

    public virtual void SetupInteractableInformation()
    {

    }
}
