using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;
using Battlefield;

public class TileConversionHandler : MonoBehaviour
{
    public ScenePointBehavior myParent;
    public BattlefieldPathHandler myController;
    public TileControlledBehavior convertedTile;
    public GameObject blueHoverTile, redHoverTile;

    private Vector3 overlapSize = new Vector3(1.075f, 1.15f, 1);
    private Vector3 normalSize = new Vector3(1, 1, 1);

    [Header("Tile Conversaion Mechanics")]
    public TeamType convertingTo;
    public float currentChangeRate;
    public float tileChangeMaxRate = 1;
    public bool isConverting = false;
    public bool returnToCurrent = false;
    public bool tileConquerable = true;

    [Header("Characters In Tile")]
    public List<BaseCharacter> characterStepping;
    public BaseCharacter lastCharacterToStepIn;
    [Header("Skills on Tile")]
    public List<BaseVisualSkillBehavior> skillsOnTile;

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

    public void AddSkillOnTile(BaseVisualSkillBehavior thisSkill)
    {
        if(skillsOnTile == null)
        {
            skillsOnTile = new List<BaseVisualSkillBehavior>();
        }

        skillsOnTile.Add(thisSkill);
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
        if(BattlefieldSystemsManager.GetInstance.unitsInCamp)
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

        UpdatePathManager();
    }

    public void UpdatePathManager()
    {
        if(BattlefieldPathManager.GetInstance == null || myParent.isSpawnPoint)
        {
            return;
        }

        if(characterStepping == null || characterStepping.Count <= 0)
        {
            BattlefieldPathManager.GetInstance.RemovePathWithAttacker(myParent);
            BattlefieldPathManager.GetInstance.RemovePathWithDefender(myParent);
        }
        else
        {
            if(characterStepping.Find(x => x.teamType == TeamType.Defender))
            {
                BattlefieldPathManager.GetInstance.AddPathWithDefender(myParent);    
            }
            else
            {
                BattlefieldPathManager.GetInstance.RemovePathWithDefender(myParent);
            }

            if (characterStepping.Find(x => x.teamType == TeamType.Attacker))
            {
                BattlefieldPathManager.GetInstance.AddPathWithAttacker(myParent);
            }
            else
            {
                BattlefieldPathManager.GetInstance.RemovePathWithAttacker(myParent);
            }
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
        UpdatePathManager();
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
