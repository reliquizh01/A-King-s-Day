using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Buildings;
using Managers;
using Kingdoms;
using Characters;
using ResourceUI;
using GameResource;

namespace Maps
{

    public class SendTroopsSubOptionPage : MonoBehaviour
    {
        public MapInformationBehavior myController;
        public BasePanelBehavior myPanel;
        public BasePanelWindow myWindow;

        public GameObject subOptionPrefab;
        public Transform panelListParent;
        public List<SubOptionPanel> subOptionsList;

        [Header("Troops Sent Mechanics")]
        public List<int> unitsToSend;
        private List<TroopsInformation> curPlayerUnits;

        [Header("Hero Sent Mechanics")]
        public ChooseLeadHeroHandler leaderHandler;
        public List<BaseHeroInformationData> heroesSent;

        public TextMeshProUGUI sendButtonText;
        public void Start()
        {
            myWindow.parentCloseCallback = myController.HideTroopsWindow;
        }
        public void OnDisable()
        {
            ResetSubOptionList();
        }
        public void SetUnitList(List<TroopsInformation> playerUnits)
        {
            if (myController.currentMapPoint.myPointInformation.ownedBy == TerritoryOwners.Player)
            {
                sendButtonText.text = "Reinforce";
            }
            else
            {
                sendButtonText.text = "Attack";
            }

            ResetSubOptionList();

            curPlayerUnits = new List<TroopsInformation>();
            curPlayerUnits.AddRange(playerUnits);

            unitsToSend = new List<int>();

            // UX PANELS
            if (subOptionsList == null)
            {
                subOptionsList = new List<SubOptionPanel>();
            }


            for (int i = 0; i < playerUnits.Count; i++)
            {
                GameObject tmp = (GameObject)Instantiate(subOptionPrefab, panelListParent);

                subOptionsList.Add(tmp.GetComponent<SubOptionPanel>());
                unitsToSend.Add(0);

                int totalCount = playerUnits[i].totalUnitCount;
                string unitName = playerUnits[i].unitInformation.unitName;
                subOptionsList[subOptionsList.Count - 1].SetupOptionPanel(totalCount, 0, unitName);
            }
        }
        // BUTTON
        public void SendUnits()
        {
            for (int i = 0; i < subOptionsList.Count; i++)
            {
                unitsToSend[i] = subOptionsList[i].amountToSell;
            }

            if (leaderHandler.IsHeroChosen())
            {
                Debug.Log("A Hero Is Chosen and is now Added!");
                heroesSent = new List<BaseHeroInformationData>();

                BaseHeroInformationData tmp = new BaseHeroInformationData();
                tmp = leaderHandler.availableHeroes[leaderHandler.idx];
                heroesSent.Add(tmp);
            }
            else
            {
                heroesSent.Clear();
            }

            List<TroopsInformation> troopsToSend = new List<TroopsInformation>();

            for (int i = 0; i < curPlayerUnits.Count; i++)
            {
                TroopsInformation tmp = new TroopsInformation();
                tmp.unitInformation = new UnitInformationData();
                tmp.unitInformation = curPlayerUnits[i].unitInformation;
                tmp.totalUnitCount = unitsToSend[i];

                troopsToSend.Add(tmp);
            }

            Debug.Log("Clicekd Send Units");

            if(myController.currentMapPoint.myPointInformation.ownedBy != TerritoryOwners.Player)
            {
                PlayerGameManager.GetInstance.SendThisUnits(troopsToSend, heroesSent, true);
                myController.SendTroopsToBattle();
            }
            else
            {
                Debug.Log("Sending Units As Reinforcement!");
                myController.currentMapPoint.myPointInformation.ReceiveReinforcementUnits(troopsToSend);

                for (int i = 0; i < troopsToSend.Count; i++)
                {
                    PlayerGameManager.GetInstance.RemoveTroops(troopsToSend[i].totalUnitCount, troopsToSend[i].unitInformation.unitName);
                }

                ResourceInformationController.GetInstance.UpdateCurrentPanel();
                myController.UpdateShownPointInformation(myController.currentMapPoint);
                StartCoroutine(myPanel.WaitAnimationForAction(myPanel.closeAnimationName, myWindow.CloseWindow));

                if (SaveData.SaveLoadManager.GetInstance != null)
                {
                    SaveData.SaveLoadManager.GetInstance.SaveCurrentCampaignData();
                }
            }
        }
        public void ResetSubOptionList()
        {
            if (subOptionsList == null || subOptionsList.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < subOptionsList.Count; i++)
            {
                DestroyImmediate(subOptionsList[i].gameObject);
            }

            subOptionsList.Clear();
        }
    }
}