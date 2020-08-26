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
        public List<ScenePointBehavior> pathsWithAttacker;
        public List<ScenePointBehavior> pathsWithDefender;

        public void AddPathWithAttacker(ScenePointBehavior thisPoint)
        {
            if(pathsWithAttacker == null)
            {
                pathsWithAttacker = new List<ScenePointBehavior>();
            }
            if (pathsWithAttacker.Find(x => x == thisPoint) != null)
            {
                return;
            }
            pathsWithAttacker.Add(thisPoint);
        }
        public void RemovePathWithAttacker(ScenePointBehavior thisPoint)
        {
            if(pathsWithAttacker == null)
            {
                pathsWithAttacker = new List<ScenePointBehavior>();
                return;
            }

            if (pathsWithAttacker.Count <= 0)
            {
                return;
            }
            int idx = -1;
                idx = pathsWithAttacker.FindIndex(x => x.gameObject.name == thisPoint.gameObject.name);
            if(idx != -1)
            {
                pathsWithAttacker.RemoveAt(idx);
            }
        }


        public void AddPathWithDefender(ScenePointBehavior thisPoint)
        {
            if (pathsWithDefender == null)
            {
                pathsWithDefender = new List<ScenePointBehavior>();
            }

            if (pathsWithDefender.Find(x => x == thisPoint) != null)
            {
                return;
            }

            pathsWithDefender.Add(thisPoint);
        }
        public void RemovePathWithDefender(ScenePointBehavior thisPoint)
        {
            if (pathsWithDefender == null)
            {
                pathsWithDefender = new List<ScenePointBehavior>();
                return;
            }

            if(pathsWithDefender.Count <= 0)
            {
                return;
            }

            int idx = -1;
                idx = pathsWithDefender.FindIndex(x => x.gameObject.name == thisPoint.gameObject.name);
            if(idx != -1)
            {
                pathsWithDefender.RemoveAt(idx);
            }
        }

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

        public List<ScenePointBehavior> ObtainWholeRowPath(int column)
        {
            return fieldPaths[column].scenePoints;
        }

        public List<ScenePointBehavior> ObtainWholeColumnPath(int row)
        {
            List<ScenePointBehavior> allInOneColumn = new List<ScenePointBehavior>();

            for (int i = 0; i < fieldPaths.Count; i++)
            {
                allInOneColumn.Add(fieldPaths[i].scenePoints[row]);
            }

            return allInOneColumn;
        }

        public List<ScenePointBehavior> ObtainTilesWithThisUnits(int tileCount, TeamType thisTeam)
        {
            List<ScenePointBehavior> thisPoints = new List<ScenePointBehavior>();
            switch (thisTeam)
            {
                case TeamType.Defender:
                    if (pathsWithDefender == null && pathsWithDefender.Count <= 0)
                    {
                        thisPoints.AddRange(ObtainWholeColumnPath(1));
                    }
                    else
                    {
                        for (int i = 0; i < tileCount; i++)
                        {
                            if(i < (pathsWithDefender.Count-1) && thisPoints.Count < tileCount)
                            {
                                thisPoints.Add(pathsWithDefender[i]);
                            }
                        }
                    }
                    break;
                case TeamType.Attacker:
                    if(pathsWithAttacker == null && pathsWithAttacker.Count < 0)
                    {
                        thisPoints.AddRange(ObtainWholeColumnPath((fieldPaths[0].scenePoints.Count - 1)));
                    }
                    else
                    {
                        for (int i = 0; i < tileCount; i++)
                        {
                            if (i < (pathsWithAttacker.Count - 1) && thisPoints.Count < tileCount)
                            {
                                thisPoints.Add(pathsWithAttacker[i]);
                            }
                        }
                    }
                    break;
                case TeamType.Neutral:
                default:
                    break;
            }

            return thisPoints;
        }
        public List<ScenePointBehavior> ObtainNearbyTiles(int column, int row, int range)
        {
            List<ScenePointBehavior> allNearbyTiles = new List<ScenePointBehavior>();

            int startingHeight = column - range;
            int lowestHeight = column + range;
            if(startingHeight < 0)
            {
                startingHeight = 0;
            }

            if (lowestHeight > (fieldPaths.Count-1))
            {
                lowestHeight = fieldPaths.Count - 1;
            }

            int startingLeft = row - range;
            int lowestRight = row + range;
            if (row < 0)
            {
                row = 0;
            }

            for (int i = 0; i < fieldPaths.Count; i++)
            {
                if(i >= startingHeight && i <= lowestHeight)
                {
                    for (int x = 0; x < fieldPaths[i].scenePoints.Count; x++)
                    {
                        int trueLowestRight = lowestRight;
                        if (lowestRight > (fieldPaths[i].scenePoints.Count - 1))
                        {
                            trueLowestRight = fieldPaths[i].scenePoints.Count - 1;
                        }

                        if(x >= startingLeft && x <= trueLowestRight)
                        {
                            allNearbyTiles.Add(fieldPaths[i].scenePoints[x]);
                        }
                    }
                }
            }
            
            return allNearbyTiles;
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
        
        public List<ScenePointBehavior> ObtainSpawnPoints()
        {
            List<ScenePointBehavior> tmp = new List<ScenePointBehavior>();

            for (int i = 0; i < fieldPaths.Count; i++)
            {
                tmp.Add(fieldPaths[i].attackerSpawnPoint);
                tmp.Add(fieldPaths[i].defenderSpawnPoint);
            }

            return tmp;
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
