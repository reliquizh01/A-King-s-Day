using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

public class TileConversionHandler : MonoBehaviour
{
    public BattlefieldPathHandler myController;

    public TileControlledBehavior convertedTile;
    public GameObject hoverTitle;

    public List<BaseCharacter> characterStepping;
    public TeamType convertingTo;

    public float currentChangeRate;
    public float tileChangeMaxRate = 1;
    public bool isConverting = false;

    public void Update()
    {
        if(isConverting)
        {
            if(convertedTile.potentialOwner != convertingTo)
            {
                convertedTile.potentialOwner = convertingTo;
            }
            currentChangeRate += Time.deltaTime;
            if(currentChangeRate >= tileChangeMaxRate)
            {
                if (convertedTile.CheckConversionIsComplete())
                {
                    isConverting = false;
                    convertedTile.CompleteControlLevel();
                }
                else
                {
                    UpdateTileBehavior();
                }
                currentChangeRate = 0;
            }
        }
    }

    public void ConvertTile(TeamType thisNewOwner)
    {
        if(convertedTile.currentOwner == thisNewOwner)
        {
            return;
        }
        isConverting = true;

        convertingTo = thisNewOwner;
        convertedTile.potentialOwner = convertingTo;
    }

    public void UpdateTileBehavior()
    {
        convertedTile.ProgressControlLevel(convertingTo);

        if(convertedTile.currentControlLevel == convertedTile.maxControllevel)
        {
            isConverting = false;
            convertedTile.CompleteControlLevel();
        }

    }
    
    public void ShowHoverTite()
    {
        hoverTitle.gameObject.SetActive(true);
        myController.ShowSpawnArrow();
    }

    public void HideHoverTile()
    {
        hoverTitle.gameObject.SetActive(false);
        myController.HideSpawnArrow();
    }
}
