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

    public SkillType skillType;
    public BaseSkillInformationData skillInformation;
    public List<BaseBuffInformationData> tileBuffList;

    public List<BaseCharacter> targetUnits;
    public TileConversionHandler targetTile;
    public float skillDuration;

    [Header("Projecitle Behavior")]
    public SpriteRenderer projectileVisualSprite;
    public bool startMoving = false;
    public float projectileSpeed = 0.025f;
    public Vector2 targetPos;

    public void Update()
    {
        if(startMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, projectileSpeed * Time.deltaTime);
            float dist = Vector2.Distance(transform.position, targetPos);
            if(dist < 0.015f)
            {
                Destroy(this.gameObject);
            }
        }
    }
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

    public void InitializeProjectileNormalBehavior(BaseCharacter spellCaster)
    {
        curTargetStat = TargetStats.health;
        curTargetType = TargetType.UnitOnly;
        curSkillOwner = spellCaster.teamType;

        skillInformation = new BaseSkillInformationData();
        skillInformation.targetInflictedCount = -UnityEngine.Random.Range(spellCaster.unitInformation.minDamage, spellCaster.unitInformation.maxDamage);
        skillInformation.targetStats = TargetStats.health;
        skillInformation.targetType = TargetType.UnitOnly;
        skillInformation.targetAlive = true;

        targetPos = spellCaster.myMovements.currentTargetPoint.transform.position;

        if(targetPos.x > transform.position.x)
        {
            projectileVisualSprite.flipX = true;
        }
        else
        {
            projectileVisualSprite.flipX = false;
        }

        startMoving = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        BaseCharacter tmp = collision.gameObject.GetComponent<BaseCharacter>();

        if (tmp == null)
            return;

        if (tmp.teamType == curSkillOwner)
            return;

        if (skillInformation.targetInflictedCount < 0)
            tmp.ReceiveDamage(skillInformation.targetInflictedCount, UnitAttackType.SPELL, skillInformation.targetStats);
        else
        {
            tmp.ReceiveHealing(skillInformation.targetInflictedCount, UnitAttackType.SPELL, skillInformation.targetStats);
        }

        Destroy(this.gameObject);
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
