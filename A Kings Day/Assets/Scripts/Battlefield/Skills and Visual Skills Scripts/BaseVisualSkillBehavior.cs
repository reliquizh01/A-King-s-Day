using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using Battlefield;
using Characters;

public class BaseVisualSkillBehavior : MonoBehaviour
{
    public TileConversionHandler myParent;
    public Animation myAnim;
    
    public TeamType curSkillOwner;
    public TeamType targetTeam;

    public TargetType curTargetType;
    public TargetStats curTargetStat;

    public BaseSkillInformationData skillInformation;
    public List<BaseBuffInformationData> tileBuffList;

    public List<BaseCharacter> targetUnits;
    public TileConversionHandler targetTile;
    public float skillDuration;

    public void InitializeUnitOnlyTargetBehavior(List<BaseCharacter> newTargetUnits, BaseSkillInformationData newSkillInformation, TeamType ownerTeam, TeamType newTargetTeam)
    {
        skillInformation = new BaseSkillInformationData();
        skillInformation = newSkillInformation;

        curSkillOwner = ownerTeam;
        targetTeam = newTargetTeam;

        curTargetType = skillInformation.targetType;
        curTargetStat = skillInformation.targetStats;

        targetUnits = new List<BaseCharacter>();
        targetUnits.AddRange(newTargetUnits);

        myAnim.Play();
    }
    public void InitializeTileUnitsTargetBehavior(TileConversionHandler newTargetTile,BaseSkillInformationData newSkillInformation, TeamType ownerTeam, TeamType newTargetTeam)
    {
        skillInformation = new BaseSkillInformationData();
        skillInformation = newSkillInformation;

        curSkillOwner = ownerTeam;
        targetTeam = newTargetTeam;

        curTargetType = skillInformation.targetType;
        curTargetStat = skillInformation.targetStats;

        targetTile = newTargetTile;

        myAnim.Play();
    }

    public void InitializeTileBuffsTargetBehavior(TileConversionHandler newTargetTile, List<BaseBuffInformationData> buffToAddOnTile, TeamType ownerTeam, TeamType newTargetTeam)
    {
        curSkillOwner = ownerTeam;
        targetTeam = newTargetTeam;

        targetTile = newTargetTile;

        tileBuffList = new List<BaseBuffInformationData>();
        tileBuffList.AddRange(buffToAddOnTile);
    }

    public void DeliverSkillEffect()
    {
        float dmgCount = skillInformation.targetInflictedCount;
        switch (curTargetType)
        {
            case TargetType.UnitOnly:
                for (int i = 0; i < targetUnits.Count; i++)
                {
                    if (dmgCount < 0)
                    {
                        targetUnits[i].ReceiveDamage(dmgCount, UnitAttackType.SPELL, skillInformation.targetStats);
                    }
                    else
                    {
                        targetUnits[i].ReceiveHealing(dmgCount, UnitAttackType.SPELL, skillInformation.targetStats);
                    }
                }
                break;
            case TargetType.UnitOnTiles:

                if (targetTile.characterStepping != null && targetTile.characterStepping.Count > 0)
                {
                    for (int i = 0; i < targetTile.characterStepping.Count; i++)
                    {
                        if(targetTile.characterStepping[i].teamType == targetTeam)
                        {
                            if(dmgCount < 0)
                            {
                                targetTile.characterStepping[i].ReceiveDamage(dmgCount, UnitAttackType.SPELL, skillInformation.targetStats);
                            }
                            else
                            {
                                targetTile.characterStepping[i].ReceiveHealing(dmgCount, UnitAttackType.SPELL, skillInformation.targetStats);
                            }
                        }
                    }
                }
                break;
            case TargetType.TilesOnly:
                for (int i = 0; i < tileBuffList.Count; i++)
                {
                    BaseBuffInformationData newBuff = new BaseBuffInformationData();
                    switch (tileBuffList[i].targetStats)
                    {
                        case TargetStats.health:
                        case TargetStats.damage:
                        case TargetStats.speed:
                        case TargetStats.range:
                        case TargetStats.blockProjectile:
                        case TargetStats.blockMelee:

                            break;
                        default:
                            break;
                    }
                }
                break;
            default:
                break;
        }
    }

    public void CloseSkillEffect()
    {
        if(myParent != null)
        {
            myParent.skillsOnTile.Remove(this);
        }

        Destroy(this.gameObject);
    }
}
