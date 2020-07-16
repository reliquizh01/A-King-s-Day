using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Kingdoms;

public class ResourcePage : MonoBehaviour
{
    public KingdomCreationUiV2 myController;
    public TextMeshProUGUI countText;
    public ButtonChangeUI increaseBtn, decreaseBtn;
    public ResourceType resourceType;

    public void Start()
    {
        increaseBtn.AddActionCallback(ButtonActionChangeType.Down, IncreaseResource);
        decreaseBtn.AddActionCallback(ButtonActionChangeType.Down, DecreaseResource);
    }

    public void IncreaseResource()
    {
        if (myController == null)
            return;
        myController.IncreaseResource(resourceType);

        UpdateResourceCount();
    }

    public void UpdateResourceCount()
    {
        if (myController == null)
            return;

        int curCount = myController.ObtainResourceCount(resourceType);
        if(curCount < 10)
        {
            countText.text = "0" + curCount.ToString();
        }
        else
        {
            countText.text = curCount.ToString();
        }

    }
    public void DecreaseResource()
    {
        if (myController == null)
            return;


        myController.DecreaseResource(resourceType);

        UpdateResourceCount();
    }

  

}
