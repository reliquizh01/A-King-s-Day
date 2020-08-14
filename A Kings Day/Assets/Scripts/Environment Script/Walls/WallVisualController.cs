using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balcony
{
    public class WallVisualController : MonoBehaviour
    {
        public WallDefenderSpawner wallSpawner;

        public bool defenderSpawned;
        [Header("Spawnable Units")]
        public GameObject archer;

        public void SpawnWallDefenders()
        {
            if (defenderSpawned)
                return;
            defenderSpawned = true;
            wallSpawner.StartSpawning(archer);
        }

        public void PlaceWallDefenders()
        {
            if (defenderSpawned)
                return;
            defenderSpawned = true;
            wallSpawner.StartSpawning(archer);

        }
        public void RetreatWallDefenders()
        {
            if (!defenderSpawned)
                return;

            defenderSpawned = false;
            wallSpawner.StartRetreating();
        }
    }
}
