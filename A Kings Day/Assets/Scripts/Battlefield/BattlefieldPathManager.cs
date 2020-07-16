using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

namespace Battlefield
{
    public class BattlefieldPathManager : MonoBehaviour
    {
        #region Singleton
        private static BattlefieldPathManager instance;
        public static BattlefieldPathManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            instance = this;
        }
        #endregion


        public List<BattlefieldPathHandler> fieldPaths;


        public int ObtainPathCount()
        {
            int tmp = 0;
            for (int i = 0; i < fieldPaths.Count; i++)
            {
                tmp += fieldPaths[i].scenePoints.Count;
            }

            return tmp;
        }
        public ScenePointBehavior ObtainPath(int column, int row)
        {
            return fieldPaths[column].scenePoints[row];
        }

        public ScenePointBehavior ObtainSpawnPoint(int column, bool isAttacker)
        {
            if(isAttacker)
            {
                return fieldPaths[column].attackerSpawnPoint;
            }
            else
            {
                return fieldPaths[column].defenderSpawnPoint;
            }
        }

        public ScenePointBehavior ObtainTargetPoint(int column, bool isAttacker)
        {
            if (isAttacker)
            {
                return fieldPaths[column].defenderSpawnPoint;
            }
            else
            {
                return fieldPaths[column].attackerSpawnPoint;
            }
        }

        public int ObtainColumnByPoint(ScenePointBehavior thisPoint)
        {
            int columnNumber = fieldPaths.FindIndex(x => x.attackerSpawnPoint == thisPoint || x.defenderSpawnPoint == thisPoint );

            return columnNumber;
        }
        
        public List<BaseCharacter> ObtainCharactersOnThisPath(int pathIdx)
        {
            List<BaseCharacter> tmp = new List<BaseCharacter>();

            for (int i = 0; i < fieldPaths[pathIdx].scenePoints.Count; i++)
            {
                if(fieldPaths[pathIdx].scenePoints[i].battleTile.characterStepping.Count > 0)
                {
                    tmp.AddRange(fieldPaths[pathIdx].scenePoints[i].battleTile.characterStepping);
                }
            }

            return tmp;
        }
        public void ResetAllPaths()
        {
            for (int i = 0; i < fieldPaths.Count; i++)
            {
                for (int x = 0; x < fieldPaths[i].scenePoints.Count; x++)
                {
                    fieldPaths[i].scenePoints[x].battleTile.ConvertTile(TeamType.Neutral);
                }
            }
        }
        public int ObtainConqueredTiles()
        {
            int totalCount = 0;
            for (int i = 0; i < fieldPaths.Count; i++)
            {
                for (int x = 0; x < fieldPaths[i].scenePoints.Count; x++)
                {
                    if(fieldPaths[i].scenePoints[x].battleTile != null)
                    {
                        if(fieldPaths[i].scenePoints[x].battleTile.convertedTile.currentOwner != TeamType.Neutral)
                        {
                            if(fieldPaths[i].scenePoints[x].battleTile.tileConquerable)
                            {
                                totalCount += 1;
                            }
                        }
                    }
                }
            }
            return totalCount;
        }

        public List<TileConversionHandler> ObtainConqueredTiles(TeamType thisTeam)
        {
            List<TileConversionHandler> tmp = new List<TileConversionHandler>();

            for (int i = 0; i < fieldPaths.Count; i++)
            {
                for (int x = 0; x < fieldPaths[i].scenePoints.Count; x++)
                {
                    if(fieldPaths[i].scenePoints[x].battleTile.convertedTile.currentOwner == thisTeam)
                    {
                        tmp.Add(fieldPaths[i].scenePoints[x].battleTile);
                        //Debug.Log("Adding :" + tmp.Count +" Cur Team : " + thisTeam);
                    }
                }
            }

            return tmp;
        }
        public void ConvertAllToOneTeam(TeamType thisTeam)
        {
            switch (thisTeam)
            {
                case TeamType.Neutral:
                    for (int i = 0; i < fieldPaths.Count; i++)
                    {
                        for (int x = 0; x < fieldPaths[i].scenePoints.Count; x++)
                        {
                            fieldPaths[i].scenePoints[x].battleTile.ConvertTile(TeamType.Neutral);
                        }
                    }
                    break;
                case TeamType.Defender:
                    for (int i = 0; i < fieldPaths.Count; i++)
                    {
                        for (int x = 0; x < fieldPaths[i].scenePoints.Count; x++)
                        {
                            fieldPaths[i].scenePoints[x].battleTile.ConvertTile(TeamType.Defender);
                        }
                    }
                    break;
                case TeamType.Attacker:
                    for (int i = 0; i < fieldPaths.Count; i++)
                    {
                        for (int x = 0; x < fieldPaths[i].scenePoints.Count; x++)
                        {
                            fieldPaths[i].scenePoints[x].battleTile.ConvertTile(TeamType.Attacker);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
