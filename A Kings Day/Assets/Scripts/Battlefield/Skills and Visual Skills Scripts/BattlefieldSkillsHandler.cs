using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Characters;
using Kingdoms;
using Managers;

namespace Battlefield
{
    public class BattlefieldSkillsHandler : MonoBehaviour
    {
        public BattlefieldUnitSelectionController myController;
        public Sprite iconEmpty;
        [Header("Current Hero")]
        public BaseHeroInformationData currentHero;
        [Header("Skill Mechanics")]
        public List<BattlefieldSkillslotHandler> skillSlotList;
        public BattlefieldSkillslotHandler currentSkillSlot;
        public int curSkillIdx = 0;

        public void Start()
        {
            currentSkillSlot = skillSlotList[0];
            currentSkillSlot.SetAsCurrentSkillSlot();
        }
        public void Update()
        {
            SkillControl();
        }

        public void SkillControl()
        {
            if(myController.controlType == PlayerControlType.PlayerOne)
            {
                if(Input.GetKeyDown(KeyCode.Q))
                {
                    if(currentSkillSlot != null)
                    {
                        currentSkillSlot.SetAsUnselectedSkillSlot();
                    }

                    if(curSkillIdx <= 0)
                    {
                        curSkillIdx = skillSlotList.Count - 1;
                    }
                    else
                    {
                        curSkillIdx -= 1;
                    }
                    currentSkillSlot = skillSlotList[curSkillIdx];
                    skillSlotList[curSkillIdx].SetAsCurrentSkillSlot();
                }
                else if(Input.GetKeyDown(KeyCode.E))
                {
                    if (currentSkillSlot != null)
                    {
                        currentSkillSlot.SetAsUnselectedSkillSlot();
                    }

                    if (curSkillIdx >= skillSlotList.Count - 1)
                    {
                        curSkillIdx = 0;
                    }
                    else
                    {
                        curSkillIdx += 1;
                    }
                    currentSkillSlot = skillSlotList[curSkillIdx];
                    skillSlotList[curSkillIdx].SetAsCurrentSkillSlot();
                }

                if(Input.GetKeyDown(KeyCode.F))
                {
                    ActivateThisSkill(curSkillIdx);
                }
            }
            else if(myController.controlType == PlayerControlType.PlayerTwo)
            {
                if (Input.GetKeyDown(KeyCode.Keypad7))
                {
                    if (currentSkillSlot != null)
                    {
                        currentSkillSlot.SetAsUnselectedSkillSlot();
                    }

                    if (curSkillIdx <= 0)
                    {
                        curSkillIdx = skillSlotList.Count - 1;
                    }
                    else
                    {
                        curSkillIdx -= 1;
                    }
                    currentSkillSlot = skillSlotList[curSkillIdx];
                    skillSlotList[curSkillIdx].SetAsCurrentSkillSlot();
                }
                else if (Input.GetKeyDown(KeyCode.Keypad9))
                {
                    if (currentSkillSlot != null)
                    {
                        currentSkillSlot.SetAsUnselectedSkillSlot();
                    }

                    if (curSkillIdx >= skillSlotList.Count - 1)
                    {
                        curSkillIdx = 0;
                    }
                    else
                    {
                        curSkillIdx += 1;
                    }
                    currentSkillSlot = skillSlotList[curSkillIdx];
                    skillSlotList[curSkillIdx].SetAsCurrentSkillSlot();
                }

                if (Input.GetKeyDown(KeyCode.Keypad6))
                {
                    ActivateThisSkill(curSkillIdx);
                }
            }
        }
        public void SetupHeroSkillset(BaseHeroInformationData thisHero)
        {
            currentHero = new BaseHeroInformationData();    
            currentHero = thisHero;

            for (int i = 0; i < skillSlotList.Count; i++)
            {
                if(i <= currentHero.skillsList.Count-1)
                {
                    skillSlotList[i].SetAsSkill(currentHero.skillsList[i]);
                }
                else
                {
                    skillSlotList[i].SetAsEmpty();
                }
            }
        }

        public void ActivateThisSkill(int idx)
        {
            if(currentHero.skillsList == null || currentHero.skillsList.Count <= 0)
            {
                return;
            }

            if (idx > (currentHero.skillsList.Count-1))
            {
                return;
            }

            BaseSkillInformationData thisSkill = new BaseSkillInformationData();
            thisSkill = currentHero.skillsList[idx];

            if(thisSkill.isOnCooldown)
            {
                return;
            }
            Debug.Log("-------------------[ ACTIVATING INDEX SKILL #" + idx + " RIGHT NOW! ]---------------------------------");

            // Place Where The Skill was Activated
            int columnPoint = myController.curColumnIdx;
            int rowPoint = myController.curRowIdx;


            // GET AFFECTED UNITS
            List<ScenePointBehavior> targetTiles = new List<ScenePointBehavior>();
            List<BaseCharacter> targetUnits = new List<BaseCharacter>();
            TeamType targetTeam = TeamType.Neutral;

            switch (thisSkill.skillType)
            {
                case SkillType.Offensive:
                case SkillType.OffensiveBuff: // All Enemies on the boar
                    if (myController.teamType == TeamType.Attacker)
                    {
                        Debug.Log("Target Position is Defender!");
                        targetTeam = TeamType.Defender;
                    }
                    else
                    {
                        Debug.Log("Target Position is Attacker!");
                        targetTeam = TeamType.Attacker;
                    }
                    break;

                case SkillType.Defensive:
                case SkillType.DefensiveBuff: // All Allies on the board
                    targetTeam = myController.teamType;

                    break;
                default:
                    break;
            }

            if(thisSkill.targetType == TargetType.UnitOnTiles)
            {
                // GET AFFECTED AREA
                switch (thisSkill.affectedArea)
                {
                    case AreaAffected.Single:
                        ScenePointBehavior tileSkillUsedOn = BattlefieldPathManager.GetInstance.ObtainPath(columnPoint, rowPoint);
                        targetTiles.Add(tileSkillUsedOn);
                        break;
                    case AreaAffected.Row:
                        List<ScenePointBehavior> rowPath = BattlefieldPathManager.GetInstance.ObtainWholeRowPath(rowPoint);
                        targetTiles.AddRange(rowPath);
                        break;
                    case AreaAffected.Column:
                        List<ScenePointBehavior> columnPath = BattlefieldPathManager.GetInstance.ObtainWholeColumnPath(columnPoint);
                    
                        targetTiles.AddRange(columnPath);
                        break;
                    case AreaAffected.Nearby:
                        List<ScenePointBehavior> nearestPath = BattlefieldPathManager.GetInstance.ObtainNearbyTiles(columnPoint, rowPoint, thisSkill.maxRange);
                        targetTiles.AddRange(nearestPath);
                        break;
                    case AreaAffected.All:
                        for (int i = 0; i < BattlefieldPathManager.GetInstance.fieldPaths.Count; i++)
                        {
                            targetTiles.AddRange(BattlefieldPathManager.GetInstance.fieldPaths[i].scenePoints);
                        }
                        break;
                    case AreaAffected.Aimed:
                        targetTiles.AddRange(BattlefieldPathManager.GetInstance.ObtainTilesWithThisUnits(thisSkill.maxRange, targetTeam));
                         break;
                    default:
                        break;
                }

                // GET ALL TARGETED UNITS
                for (int i = 0; i < targetTiles.Count; i++)
                {
                    targetUnits.AddRange(targetTiles[i].battleTile.characterStepping.FindAll(x => x.teamType == targetTeam));
                }
            }
            else if(thisSkill.targetType == TargetType.UnitOnly)
            {
                switch (targetTeam)
                {
                    case TeamType.Defender:
                        if(thisSkill.targetAlive)
                        {
                            targetUnits.AddRange(BattlefieldSpawnManager.GetInstance.defenderSpawnedUnits.FindAll(x => x.unitInformation.curhealth > 0));
                        }
                        else
                        {
                            targetUnits.AddRange(BattlefieldSpawnManager.GetInstance.defenderSpawnedUnits.FindAll(x => x.unitInformation.curhealth <= 0));
                        }
                        break;

                    case TeamType.Attacker:
                        if (thisSkill.targetAlive)
                        {
                            targetUnits.AddRange(BattlefieldSpawnManager.GetInstance.attackerSpawnedUnits.FindAll(x => x.unitInformation.curhealth > 0));
                        }
                        else
                        {
                            targetUnits.AddRange(BattlefieldSpawnManager.GetInstance.attackerSpawnedUnits.FindAll(x => x.unitInformation.curhealth <= 0));
                        }
                        break;

                    case TeamType.Neutral:
                    default:
                        break;
                }

                Debug.Log("Target Unit Count: " + targetUnits.Count);
            }

            if (thisSkill.spawnPrefab)
            {
                GameObject tmp = null;
                BaseVisualSkillBehavior skillBehavior;

                string skillPath = thisSkill.prefabStringPath.Split('.')[0];
                skillPath = skillPath.Replace("Assets/Resources/", "");


                switch (thisSkill.affectedArea)
                {
                    case AreaAffected.Single:
                        tmp = GameObject.Instantiate((GameObject)Resources.Load(skillPath, typeof(GameObject)), targetTiles[0].transform.position, Quaternion.identity, null);

                        skillBehavior = tmp.GetComponent<BaseVisualSkillBehavior>();
                        skillBehavior.myParent = targetTiles[0].battleTile;

                        switch (thisSkill.targetType)
                        {
                            case TargetType.UnitOnly:
                                skillBehavior.InitializeUnitOnlyTargetBehavior(targetUnits, thisSkill, myController.teamType, targetTeam);
                                break;
                            case TargetType.UnitOnTiles:
                                skillBehavior.InitializeTileUnitsTargetBehavior(targetTiles[0].battleTile, thisSkill, myController.teamType, targetTeam);
                                break;
                            case TargetType.TilesOnly:
                                skillBehavior.InitializeTileBuffsTargetBehavior(targetTiles[0].battleTile, thisSkill.buffList, myController.teamType, targetTeam);
                                break;
                            default:
                                break;
                        }

                        targetTiles[0].battleTile.AddSkillOnTile(skillBehavior);
                        skillSlotList[idx].SetOnCooldown();
                        break;

                    case AreaAffected.Row:
                    case AreaAffected.Column:
                    case AreaAffected.Nearby:
                    case AreaAffected.All:
                    case AreaAffected.Aimed:
                        Debug.Log("Skill is Aimed!");
                        switch (thisSkill.targetType)
                        {
                            case TargetType.UnitOnly:
                                for (int i = 0; i < thisSkill.maxRange; i++)
                                {
                                    if(i <= (targetUnits.Count-1))
                                    {
                                        tmp = GameObject.Instantiate((GameObject)Resources.Load(skillPath, typeof(GameObject)), targetUnits[i].transform.position, Quaternion.identity, null);

                                        skillBehavior = tmp.GetComponent<BaseVisualSkillBehavior>();

                                        List<BaseCharacter> temp = new List<BaseCharacter>();
                                        temp.Add(targetUnits[i]);

                                        skillBehavior.InitializeUnitOnlyTargetBehavior(temp, thisSkill, myController.teamType, targetTeam);
                                    }
                                }
                                break;
                            case TargetType.UnitOnTiles:
                                for (int i = 0; i < targetTiles.Count; i++)
                                {
                                    tmp = GameObject.Instantiate((GameObject)Resources.Load(skillPath, typeof(GameObject)), targetTiles[i].transform.position, Quaternion.identity, null);

                                    skillBehavior = tmp.GetComponent<BaseVisualSkillBehavior>();
                                    skillBehavior.myParent = targetTiles[0].battleTile;
                                    skillBehavior.InitializeTileUnitsTargetBehavior(targetTiles[i].battleTile, thisSkill, myController.teamType, targetTeam);

                                    targetTiles[i].battleTile.AddSkillOnTile(skillBehavior);
                                }
                                break;
                            case TargetType.TilesOnly:
                                for (int i = 0; i < targetTiles.Count; i++)
                                {
                                    tmp = GameObject.Instantiate((GameObject)Resources.Load(skillPath, typeof(GameObject)), targetTiles[i].transform.position, Quaternion.identity, null);

                                    skillBehavior = tmp.GetComponent<BaseVisualSkillBehavior>();
                                    skillBehavior.myParent = targetTiles[0].battleTile;
                                    skillBehavior.InitializeTileBuffsTargetBehavior(targetTiles[i].battleTile, thisSkill.buffList, myController.teamType, targetTeam);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (thisSkill.skillType)
                {
                    case SkillType.Offensive: // Direct Damage
                        for (int i = 0; i < thisSkill.maxRange; i++)
                        {
                            if (i <= (targetUnits.Count - 1))
                            {
                                targetUnits[i].ReceiveDamage(thisSkill.targetInflictedCount, UnitAttackType.SPELL, thisSkill.targetStats);
                            }
                        }
                        break;
                    case SkillType.Defensive: // Direct Attack
                        for (int i = 0; i < thisSkill.maxRange; i++)
                        {
                            if (i <= (targetUnits.Count - 1))
                            {
                                targetUnits[i].ReceiveHealing(thisSkill.targetInflictedCount, UnitAttackType.SPELL, thisSkill.targetStats);
                            }
                        }
                        break;
                    case SkillType.OffensiveBuff:
                    case SkillType.DefensiveBuff:
                        foreach (BaseCharacter item in targetUnits)
                        {
                            Debug.Log("Adding Buff For Unit: " + item.unitInformation.unitName + " Amount of Buff: " + thisSkill.buffList.Count);
                            for (int i = 0; i < thisSkill.buffList.Count; i++)
                            {
                                Debug.Log("[Added This Buff:" + thisSkill.buffList[i].buffName + "]");
                                item.unitInformation.AddBuff(thisSkill.buffList[i]);
                                item.UpdateStats();
                                Debug.Log("[Checking BuffList Count:" + item.unitInformation.buffList.Count);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            skillSlotList[idx].SetOnCooldown();
        }
    }
}