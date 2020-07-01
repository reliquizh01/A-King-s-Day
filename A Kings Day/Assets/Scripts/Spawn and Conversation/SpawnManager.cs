using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kingdoms;
using KingEvents;
using Managers;
using ConversationControls;
using Characters;


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
            instance = this;
        }
        #endregion

        public ScenePointBehavior spawnPoint;
        public ConversationController conversationController;

        public List<CourtGuestPrefabs> characterPrefabsList;

        public List<CourtGuest> queuedGuestList;
        public CourtGuest currentCourtGuest;

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
            switch(currentCourtGuest.reporterType)
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
