using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerClickDetector : MonoBehaviour
{
    public BaseTravellerBehavior myTraveller;
    public bool isClickable = true;
    public void OnMouseDown()
    {
        if (!isClickable)
            return;

        myTraveller.OnClick();
    }
}
