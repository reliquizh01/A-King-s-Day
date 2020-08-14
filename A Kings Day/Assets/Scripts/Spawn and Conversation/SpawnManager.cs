using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using KingEvents;
using Managers;
using ConversationControls;
using Characters;
using Utilities;


namespace Managers
{

    [Serializable]
    public class CourtGuestPrefabs
    {
        public ReporterType reporterType;
        public GameObject prefab;
    }

    public class CourtGuest
    {
        public BaseCharacter characterSpawned;
        public ReporterType reporterType;

    }

    public class SpawnManager : BaseManager
    {
        #region Singleton
        private static SpawnManager instance;
        public static SpawnManager GetInstance
        {
            get
            {
                return instance;
            }
        }

        public void Awake()
        {
            if(SpawnManager.GetInstance != null && SpawnManager.GetInstance != this)
            {
                DestroyImmediate(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }
        #endregion

        public ConversationController conversationController;


        [Header("Spawn Manager Mechanics")]
        public List<BaseCharacter> spawnedCharacterUnits;
        public List<ScenePointBehavior> spawnPointList;

        [Header("Courtroom Mechanics")]
        public List<CourtGuestPrefabs> characterPrefabsList;
        public List<CourtGuest> queuedGuestList;
        public CourtGuest currentCourtGuest;

        public void OnEnable()
        {
            EventBroadcaster.Instance.AddObserver(EventNames.BEFORE_LOAD_SCENE, ResetSpawnManager);
        }
        public void OnDisable()
        {
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.BEFORE_LOAD_SCENE, ResetSpawnManager);
        }
        public override void Start()
        {
            base.Start();
            spawnedCharacterUnits = new List<BaseCharacter>();
        }

        public void ResetSpawnManager(Parameters p = null)
        {
            spawnedCharacterUnits.Clear();
            spawnPointList.Clear();
        }

        public void ClearSpawnedUnits(List<BaseCharacter> exceptThis = null)
        {
            if(exceptThis != null)
            {
                List<int> removeIdexes = new List<int>();
                for (int i = 0; i < spawnedCharacterUnits.Count; i++)
                {
                    if(exceptThis.Contains(spawnedCharacterUnits[i]))
                    {
                        continue;
                    }
                    removeIdexes.Add(i);
                }
            }
            else
            {
                for (int i = 0; i < spawnedCharacterUnits.Count; i++)
                {
                    DestroyImmediate(spawnedCharacterUnits[i]);
                }
                spawnedCharacterUnits.Clear();
            }
        }
        public void AddSpawnPoint(ScenePointBehavior spawnPoint)
        {
            if(spawnPointList == null)
            {
                spawnPointList = new List<ScenePointBehavior>();
            }

            spawnPointList.Add(spawnPoint);
        }
        public void SpawnCourtGuest(ReporterType guestType)
        {
            if (currentCourtGuest != null)
            {
                CourtGuest newGuest = CreateCourtGuest(guestType);

                if(queuedGuestList == null)
                {
                    queuedGuestList = new List<CourtGuest>();
                }
                queuedGuestList.Add(newGuest);
            }
            else
            {
                currentCourtGuest = CreateCourtGuest(guestType);
            }

            InstantiateCourtGuest(currentCourtGuest.reporterType);
        }

        public void InstantiateCourtGuest(ReporterType thisType)
        {
            GameObject tmp = null;
            ScenePointBehavior spawnPoint = spawnPointList.Find(x => x.gameObject.name == "Court Spawn");

            if (spawnPoint == null)
                return;

            switch (currentCourtGuest.reporterType)
            {
                case ReporterType.Soldier:
                    tmp = characterPrefabsList.Find(x => x.reporterType == ReporterType.Soldier).prefab;
                    tmp = GameObject.Instantiate(tmp, spawnPoint.transform.position, Quaternion.identity, null);
                    currentCourtGuest.characterSpawned = tmp.GetComponent<BaseCharacter>();
                    break;
                case ReporterType.Villager:
                     tmp = characterPrefabsList.Find(x => x.reporterType == ReporterType.Villager).prefab;
                    tmp = GameObject.Instantiate(tmp, spawnPoint.transform.position, Quaternion.identity, null);
                    currentCourtGuest.characterSpawned = tmp.GetComponent<BaseCharacter>();
                    break;

            }

            if (currentCourtGuest.characterSpawned != null)
            {
                currentCourtGuest.characterSpawned.SpawnInThisPosition(spawnPoint);
                currentCourtGuest.characterSpawned.OrderMovement(spawnPoint.neighborPoints[0], PreStartCourt);
            }
            else
            {
                Debug.LogError("Unable to obtain character for [" + currentCourtGuest.reporterType + "], unable to find prefab.");
            }
        }

        public CourtGuest CreateCourtGuest(ReporterType type)
        {
            CourtGuest tmp = new CourtGuest();
            tmp.reporterType = type;
            return tmp;
        }

        public void PreStartCourt()
        {
            // Call Conversation Controller to Converse with king First.

            conversationController.StartConversation();
        }
        public void StartCourt()
        {
            KingdomManager.GetInstance.StartCards();
        }
        public void PreLeaveCourt()
        {
            // Call Conversation Controller to Converse with king First
            conversationController.LeavingConversation();

        }
        public void LeaveCourt()
        {
            ScenePointBehavior spawnPoint = spawnPointList.Find(x => x.gameObject.name == "Court Spawn");

            if (spawnPoint == null)
                return;

            currentCourtGuest.characterSpawned.OrderMovement(spawnPoint, CheckCourtUse);
            currentCourtGuest.characterSpawned.isLeaving = true;
        }

        //Gets called when a unit leaves the court room, checks if there's another unit.
        public void CheckCourtUse()
        {
            KingdomManager.GetInstance.CheckNextEvent();
        }
    }
}
