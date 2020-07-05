using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Managers;
using Utilities;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public enum TeamType
{
    Neutral,
    Defender,
    Attacker,
}
public class TileControlledBehavior : MonoBehaviour
{
    public TeamType previousOwner;
    public TeamType currentOwner;
    public TeamType potentialOwner;

    public int currentControlLevel;
    public int maxControllevel = 7;

    [Header("Control Sprites")]
    public SpriteRenderer mySpriteRenderer;
    public Sprite neutral;
    public List<Sprite> neutralToRed;
    public List<Sprite> neutralToBlue;
    public List<Sprite> blueToRed;
    public List<Sprite> redToBlue;


    public bool CheckConversionIsComplete()
    {
        if(potentialOwner == TeamType.Neutral)
        {
            return true;
        }
        else if(potentialOwner == currentOwner && currentControlLevel != 0)
        {
            return false;
        }
        else
        {
            return (currentControlLevel >= maxControllevel);
        }
    }

    public void ProgressControlLevel(TeamType toThisType)
    {
        potentialOwner = toThisType;

        // Tries to change current Owner to what owner is right now.
        if (currentOwner == potentialOwner && currentControlLevel == 0) return;
        else if(currentOwner == potentialOwner && currentControlLevel >= 0)
        {
            currentControlLevel = 0;
            switch (currentOwner)
            {
                case TeamType.Neutral:
                    ReturnToNeutral();
                    break;
                case TeamType.Defender:
                    AttackerToDefender(7);
                    break;
                case TeamType.Attacker:
                    DefenderToAttacker(7);
                    break;
                default:
                    break;
            }
        }
        else // GOING FORWARD
        {
            potentialOwner = toThisType;
            currentControlLevel += 1;
            // Check Current Owner
            switch (currentOwner)
            {
                case TeamType.Neutral: // Is Neutral
                    switch (potentialOwner)
                    {
                        case TeamType.Attacker:
                            NeutralToAttacker(currentControlLevel);
                            break;
                        case TeamType.Defender:
                            NeutralToDefender(currentControlLevel);
                            break;
                        default:
                            break;
                    }
                    break;
                case TeamType.Attacker:
                    switch (potentialOwner)
                    {
                        case TeamType.Neutral:
                            ReturnToNeutral();
                            break;
                        case TeamType.Defender:
                            AttackerToDefender(currentControlLevel);
                            break;
                        default:
                            break;
                    }
                    break;
                case TeamType.Defender:
                    switch (potentialOwner)
                    {
                        case TeamType.Neutral:
                            ReturnToNeutral();
                            break;

                        case TeamType.Attacker:
                            DefenderToAttacker(currentControlLevel);
                            break;

                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
    public void NeutralToAttacker(int thisLevel)
    {
        mySpriteRenderer.sprite = neutralToRed[thisLevel];
    }
    public void NeutralToDefender(int thisLevel)
    {
        mySpriteRenderer.sprite = neutralToBlue[thisLevel];
    }

    public void DefenderToAttacker(int thisLevel)
    {
        mySpriteRenderer.sprite = blueToRed[thisLevel];
    }
    public void AttackerToDefender(int thisLevel)
    {
        mySpriteRenderer.sprite = redToBlue[thisLevel];
    }

    public void ReturnToNeutral()
    {
        mySpriteRenderer.sprite = neutral;
    }

    public void CompleteControlLevel()
    {
        if(currentOwner != potentialOwner)
        {
            previousOwner = currentOwner;
            currentOwner = potentialOwner;

            // Update total Victory
            if (previousOwner == TeamType.Neutral)
            {
                if(BattlefieldSystemsManager.GetInstance != null)
                {
                    BattlefieldSystemsManager.GetInstance.UpdateTileVictoryPoints();
                }
            }


            currentControlLevel = 0;
                
            if(potentialOwner == TeamType.Neutral)
            {
                ReturnToNeutral();
            }

            if(BattlefieldSystemsManager.GetInstance != null)
            {
                BattlefieldSystemsManager.GetInstance.PanelConquered(currentOwner, previousOwner);
            }
        }
    }
    public void ResetControlLevel()
    {
        currentControlLevel = 0;
        potentialOwner = currentOwner;

    }

}
