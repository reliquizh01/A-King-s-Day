using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
