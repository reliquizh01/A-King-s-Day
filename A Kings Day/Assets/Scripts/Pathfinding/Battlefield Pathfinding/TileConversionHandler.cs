using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;

public class TileConversionHandler : MonoBehaviour
{
    public BattlefieldPathHandler myController;

    public TileControlledBehavior convertedTile;
    public GameObject blueHoverTile, redHoverTile;

    private Vector3 overlapSize = new Vector3(1.075f, 1.15f, 1);
    private Vector3 normalSize = new Vector3(1, 1, 1);
    public List<BaseCharacter> characterStepping;
    public BaseCharacter lastCharacterToStepIn;

    public TeamType convertingTo;

    public float currentChangeRate;
    public float tileChangeMaxRate = 1;
    public bool isConverting = false;
    public bool returnToCurrent = false;
    public bool tileConquerable = true;
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
                currentChangeRate = 0.0f;
                if (convertedTile.CheckConversionIsComplete())
                {
                    if(isConverting)
                    {
                        convertedTile.CompleteControlLevel();
                    }
                    isConverting = false;
                }
                else
                {
                    UpdateTileBehavior();
                }
            }
        }

        if(characterStepping != null && characterStepping.Count > 0)
        {
            List<int> idxToRemove = new List<int>();
            for (int i = 0; i < characterStepping.Count; i++)
            {
                if(characterStepping[i].unitInformation.curhealth <= 0)
                {
                    idxToRemove.Add(i);
                }
            }
            if(idxToRemove.Count > 0)
            {
                for (int i = 0; i < idxToRemove.Count; i++)
                {
                    if(idxToRemove[i] < characterStepping.Count -1)
                    {
                        characterStepping.RemoveAt(idxToRemove[i]);
                    }
                }
            }

        }
    }

    public void ConvertTile(TeamType thisNewOwner)
    {
        if(convertedTile.currentOwner == thisNewOwner && convertedTile.currentControlLevel == 0)
        {
            return;
        }
        if(!tileConquerable)
        {
            return;
        }

        isConverting = true;

        convertingTo = thisNewOwner;
        convertedTile.potentialOwner = convertingTo;
    }

    public void AddCharacterSteppingIn(BaseCharacter thisCharacter)
    {
        if(characterStepping == null)
        {
            characterStepping = new List<BaseCharacter>();
        }

        characterStepping.Add(thisCharacter);
        lastCharacterToStepIn = thisCharacter;

        if (characterStepping.Count > 0)
        {
            CheckCharactersSteppedIn();
        }
    }

    public void CheckCharactersSteppedIn()
    {
        BaseCharacter attacker = null;
        BaseCharacter defender = null;

        if(characterStepping.Count > 0)
        {
            for (int i = 0; i < characterStepping.Count; i++)
            {
                // Check if its alive
                if(characterStepping[i].unitInformation.curhealth > 0)
                {
                    if(characterStepping[i].teamType == TeamType.Attacker)
                    {
                        attacker = characterStepping[i];
                    }
                    else if(characterStepping[i].teamType == TeamType.Defender)
                    {
                        defender = characterStepping[i];
                    }
                }
            }
        }

        if(attacker != null && defender != null)
        {
            isConverting = false;
        }
        else if(attacker != null && defender == null)
        {
            ConvertTile(attacker.teamType);
        }
        else if (attacker == null && defender != null)
        {
            ConvertTile(defender.teamType);
        }
        else if(attacker == null && defender == null)
        {
            BaseCharacter lastCharacter = null;
            if(lastCharacterToStepIn.unitInformation.curhealth > 0)
            {
                lastCharacter = lastCharacterToStepIn;
            }

            if(lastCharacter != null)
            {
                ConvertTile(lastCharacter.teamType);
            }
            else
            {
                ConvertTile(convertedTile.currentOwner);
                isConverting = false;
            }
        }
    }

    public void RemoveChacracterSteppingIn(BaseCharacter thisCharacter)
    {
        if (characterStepping.Contains(thisCharacter))
        {
            characterStepping.Remove(thisCharacter);
        }
        CheckCharactersSteppedIn();
    }
    public void UpdateTileBehavior()
    {
        convertedTile.ProgressControlLevel(convertingTo);

        if(convertedTile.currentControlLevel == convertedTile.maxControllevel)
        {
            if(isConverting)
            {
                convertedTile.CompleteControlLevel();
            }

            isConverting = false;
        }

    }
    
    public void ShowHoverTile()
    {
        blueHoverTile.gameObject.SetActive(true);
        myController.ShowSpawnArrow();
        CheckOverlapping();
    }

    public void HideHoverTile()
    {
        blueHoverTile.gameObject.SetActive(false);
        myController.HideSpawnArrow();
        CheckOverlapping();
    }

    public void ShowDefenderTile()
    {
        redHoverTile.gameObject.SetActive(true);
        myController.ShowDefenderArrow();
        CheckOverlapping();
    }

    public void HideDefenderTile()
    {
        redHoverTile.gameObject.SetActive(false);
        myController.HideDefenderArrow();
        CheckOverlapping();
    }

    public void CheckOverlapping()
    {
        if(BattlefieldSceneManager.GetInstance == null)
        {
            return;
        }

        if(BattlefieldSceneManager.GetInstance.battleUIInformation.CheckOverLapping())
        {
            redHoverTile.transform.localScale = overlapSize;
        }
        else
        {
            redHoverTile.transform.localScale = normalSize;
        }
    }
}
