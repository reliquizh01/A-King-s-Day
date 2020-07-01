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
    Enemy,
    Player,
}
public class TileControlledBehavior : MonoBehaviour
{
    public TeamType previousOwner;
    public TeamType currentOwner;
    public TeamType potentialOwner;

    public int currentControlLevel;
    public int maxControllevel = 8;

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
        else
        {
            return (currentControlLevel >= maxControllevel);
        }
    }

    public void ProgressControlLevel(TeamType toThisType)
    {
        potentialOwner = toThisType;
        currentControlLevel += 1;

        if (currentOwner == toThisType) return;

        // Check Current Owner
        switch (currentOwner)
        {
            case TeamType.Neutral: // Is Neutral
                switch (toThisType)
                {
                    case TeamType.Enemy:
                        NeutralToEnemy(currentControlLevel);
                        break;
                    case TeamType.Player:
                        NeutralToPlayer(currentControlLevel);
                        break;
                    default:
                        break;
                }
                break;
            case TeamType.Enemy:
                switch (toThisType)
                {
                    case TeamType.Neutral:
                        ReturnToNeutral();
                        break;
                    case TeamType.Player:
                        EnemyToPlayer(currentControlLevel);
                        break;
                    default:
                        break;
                }
                break;
            case TeamType.Player:
                switch (toThisType)
                {
                    case TeamType.Neutral:
                        ReturnToNeutral();
                        break;

                    case TeamType.Enemy:
                        PlayerToEnemy(currentControlLevel);
                        break;

                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    public void NeutralToEnemy(int thisLevel)
    {
        mySpriteRenderer.sprite = neutralToRed[thisLevel];
    }
    public void NeutralToPlayer(int thisLevel)
    {
        mySpriteRenderer.sprite = neutralToBlue[thisLevel];
    }

    public void PlayerToEnemy(int thisLevel)
    {
        mySpriteRenderer.sprite = blueToRed[thisLevel];
    }
    public void EnemyToPlayer(int thisLevel)
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
            currentOwner = potentialOwner;
            currentControlLevel = 0;

            if(potentialOwner == TeamType.Neutral)
            {
                ReturnToNeutral();
            }
        }
    }
    public void ResetControlLevel()
    {
        currentControlLevel = 0;
        potentialOwner = currentOwner;

    }

}
