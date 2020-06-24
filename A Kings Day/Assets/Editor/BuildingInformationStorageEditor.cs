using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using KingEvents;
using Kingdoms;

namespace Buildings
{
    public class BuildingInformationStorageEditor : EditorWindow
    {
        #region EDITOR_SETUP
        private static Vector2 itemStorShowOn = new Vector2(1000, 800);
        private static Vector2 itemStorShowOff = new Vector2(1000, 800);
        private static int leftPanelWidth = 800;

        [MenuItem("Game Storage/Building Operation Storage")]
        public static void ShowWindow()
        {
            GetWindow<BuildingInformationStorageEditor>("Building Operation Storage").minSize = itemStorShowOff;
            GetWindow<BuildingInformationStorageEditor>("Building Operation Storage").maxSize = itemStorShowOn;
        }

        public GUIStyle titleText;
        public GUIStyle notSelectedText;
        public GUIStyle selectedText;
        public void Awake()
        {
            titleText = new GUIStyle((GUIStyle)"toolbarTextField");
            notSelectedText = new GUIStyle((GUIStyle)"toolbarTextField");
            selectedText = new GUIStyle((GUIStyle)"toolbarTextField");
        }
        #endregion

        public GameObject buildingStoragePrefab;
        public GameObject curBuildingStorage;

        public BuildingOperationStorage curBuildingStorageData;

        public BuildingInformationData selectedBuildingData;
        public BuildingInformationData curBuildingData;
        Vector2 buildingScrollPos = Vector2.zero;
        Vector2 cardScrollPos = Vector2.zero;
        Vector2 actionScrollPos = Vector2.zero;

        Vector2 buildingFlavorScrollPos = Vector2.zero;
        Vector2 cardFlavorScrollPos = Vector2.zero;
        Vector2 actionFlavorScrollPos = Vector2.zero;
        Vector2 actionRewardScrollPos = Vector2.zero;

        private bool buildingStorageLoaded = false;
        private int selectedBuildingIdx;
        private int selectedCardIdx;
        private int selectedActionIdx;
        private BuildingCardData curSelectedCard;
        private CardActiondata curSelectedAction;
        public void Save()
        {
            PrefabUtility.SaveAsPrefabAsset(curBuildingStorage, "Assets/Resources/Prefabs/Building Operations/Building Operation Storage.prefab");
            buildingStorageLoaded = false;
            LoadBuildingStorage();
        }

        public void LoadBuildingStorage()
        {
            if(curBuildingStorage == null)
            {
                buildingStoragePrefab = (GameObject)Resources.Load("Prefabs/Building Operations/Building Operation Storage");
                curBuildingStorage = (GameObject)Instantiate(buildingStoragePrefab);
            }

            if (curBuildingStorage == null)
            {
                Debug.LogWarning("Storage not found! Check Reference");
                return;
            }

            curBuildingStorageData = curBuildingStorage.GetComponent<BuildingOperationStorage>();

            if(curBuildingStorageData.buildingOperationList == null)
            {
                curBuildingStorageData.buildingOperationList = new List<BuildingInformationData>();
            }

            curBuildingData = new BuildingInformationData();
            buildingStorageLoaded = true;
        }
        public void OnDestroy()
        {
            buildingStorageLoaded = false;
            DestroyImmediate(curBuildingStorage.gameObject);
        }

        public void OnGUI()
        {
            if(!buildingStorageLoaded)
            {
                LoadBuildingStorage();
            }
            if(buildingStorageLoaded)
            {
                ShowBuildingList();
                ShowBuildingInformation();

                if(selectedBuildingData != null)
                {
                    ShowCardFlavorTexts();
                    ShowActionRewards();
                }
            }
        }

        public void ShowBuildingList()
        {
            bool addBuilding = false;
            bool saveBuilding = false;
            bool removeBuilding = false;

            EditorStyles.textField.wordWrap = true;

            GUILayout.BeginArea(new Rect(680, 10, 400, position.height - 375));
            GUILayout.BeginHorizontal();
            GUILayout.Box("Building List", titleText, GUILayout.Width(295), GUILayout.Height(20));
            GUILayout.EndHorizontal();

            buildingScrollPos = GUILayout.BeginScrollView(buildingScrollPos, new GUIStyle("RL Background"), GUILayout.Width(300), GUILayout.Height(position.height - 400));

            if(curBuildingStorageData != null && curBuildingStorageData.buildingOperationList != null && curBuildingStorageData.buildingOperationList.Count > 0)
            {
                for (int i = 0; i < curBuildingStorageData.buildingOperationList.Count; i++)
                {
                    bool isClicked = false;
                    string listName = "";

                    GUILayout.BeginHorizontal();
                    isClicked = GUILayout.Button(curBuildingStorageData.buildingOperationList[i].BuildingName, (selectedBuildingData != null && curBuildingStorageData.buildingOperationList[i].BuildingName == selectedBuildingData.BuildingName) ? selectedText : notSelectedText, GUILayout.Width(300));
                    if (!string.IsNullOrEmpty(curBuildingStorageData.buildingOperationList[i].BuildingName))
                    {
                        listName = "[" + curBuildingStorageData.buildingOperationList[i].BuildingName + "]";
                    }
                    GUILayout.EndHorizontal();

                    if(isClicked)
                    {
                        if (curBuildingStorageData.buildingOperationList[i] != null)
                        {
                            selectedBuildingData = curBuildingStorageData.buildingOperationList[i];
                            selectedBuildingIdx = i;
                            curBuildingData = selectedBuildingData;
                            if (selectedBuildingData.buildingCard != null && selectedBuildingData.buildingCard.Count > 0)
                            {
                                curSelectedCard = selectedBuildingData.buildingCard[0];
                            }
                            else
                            {
                                curSelectedCard = null;
                            }

                            selectedCardIdx = 0;

                            GUI.FocusControl(null);
                            isClicked = false;
                        }
                    }
                }
            }
            GUILayout.EndScrollView();

            GUILayout.EndArea();


            GUILayout.BeginArea(new Rect(680, position.height - 360, 400, 225));

            GUILayout.BeginHorizontal();
            saveBuilding = GUILayout.Button((curBuildingData == selectedBuildingData) ? "Modify" : "Save", GUILayout.MaxWidth(100));
            addBuilding = GUILayout.Button("Create New", GUILayout.MaxWidth(100));
            if (selectedBuildingData != null && curBuildingStorageData.buildingOperationList.Find(x => x.BuildingName == selectedBuildingData.BuildingName) != null)
            {
                removeBuilding = GUILayout.Button("Remove", GUILayout.MaxWidth(100));
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if(saveBuilding)
            {
                if(curBuildingStorageData.buildingOperationList.Contains(curBuildingData))
                {
                    selectedBuildingData = curBuildingData;
                }
                else
                {
                    curBuildingStorageData.buildingOperationList.Add(curBuildingData);
                    curBuildingData = new BuildingInformationData();
                }
                Save();
                selectedBuildingData = null;
                curSelectedCard = null;
                curBuildingData = new BuildingInformationData();
                curSelectedAction = null;
                selectedActionIdx = 0;
            }
            if(addBuilding)
            {
                selectedBuildingData = null;
                curBuildingData = new BuildingInformationData();
                curSelectedCard = null;
                GUI.FocusControl(null);
            }
            if(removeBuilding)
            {
                removeBuilding = false;
                if(selectedBuildingData != null)
                {
                    curBuildingStorageData.buildingOperationList.RemoveAt(selectedBuildingIdx);
                    selectedBuildingData = null;
                }
                Save();
                GUI.FocusControl(null);
            }
        }
        
        public void ShowBuildingInformation()
        {
            bool SaveStorage = false;

            if(curBuildingData == null)
            {
                curBuildingData = new BuildingInformationData();
            }

            GUILayout.BeginArea(new Rect(5, 12, leftPanelWidth, 300));


            GUILayout.ExpandWidth(false);
            EditorStyles.textField.wordWrap = true;

            // Building Name
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 34;
            EditorGUILayout.LabelField("Building Name:", EditorStyles.boldLabel, GUILayout.Width(100));
            curBuildingData.BuildingName = EditorGUILayout.TextField(curBuildingData.BuildingName, GUILayout.MaxWidth(300));
            EditorGUILayout.LabelField("Start Condition: ", EditorStyles.boldLabel, GUILayout.Width(100));
            curBuildingData.buildingCondition = (BuildingCondition)EditorGUILayout.EnumPopup(curBuildingData.buildingCondition, GUILayout.MaxWidth(100));
            GUILayout.EndHorizontal();

            // Building Information
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Type: ", EditorStyles.boldLabel, GUILayout.Width(100));
            curBuildingData.buildingType = (BuildingType)EditorGUILayout.EnumPopup(curBuildingData.buildingType, GUILayout.MaxWidth(100));
            EditorGUILayout.LabelField("Repair Price: ", EditorStyles.boldLabel, GUILayout.Width(100));
            curBuildingData.repairPrice = EditorGUILayout.IntField(curBuildingData.repairPrice, GUILayout.MaxWidth(100));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(5, 50, 300, 300));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Cards (Max 3): ", EditorStyles.boldLabel, GUILayout.Width(100));
            bool addCard = false;
            bool removeCard = false;
            addCard = GUILayout.Button("Add Card", GUILayout.MaxWidth(100));
            if(curBuildingData.buildingCard == null)
            {
                curBuildingData.buildingCard = new List<BuildingCardData>();
            }

            if(addCard)
            {
                if(curBuildingData.buildingCard.Count >= 3)
                {
                    return;
                }

                addCard = false;
                BuildingCardData tmp = new BuildingCardData();
                tmp.cardName = "New Card";

                curBuildingData.buildingCard.Add(tmp);
                selectedCardIdx = curBuildingData.buildingCard.Count - 1;
                curSelectedCard = curBuildingData.buildingCard[selectedCardIdx];
            }
            removeCard = GUILayout.Button("Remove Card", GUILayout.Width(120));
            if(removeCard)
            {
                if(curBuildingData.buildingCard.Count > 0)
                {
                    curBuildingData.buildingCard.RemoveAt(curBuildingData.buildingCard.Count - 1);
                }
            }
            GUILayout.EndHorizontal();

            float widthX = 300;

            // Cards
            Rect cardSpriteRect = new Rect(60, 40, 180, 240);
            List<string> cardList = new List<string>();
            if(curBuildingData.buildingCard == null)
            {
                curBuildingData.buildingCard = new List<BuildingCardData>();
            }
            else
            {
                if(curBuildingData.buildingCard.Count > 0)
                {
                    for (int i = 0; i < curBuildingData.buildingCard.Count; i++)
                    {
                        cardList.Add(curBuildingData.buildingCard[i].cardName);
                    }
                    GUILayout.BeginHorizontal();
                    selectedCardIdx = EditorGUILayout.Popup(selectedCardIdx, cardList.ToArray(), GUILayout.Width(150));
                    if (curSelectedCard != curBuildingData.buildingCard[selectedCardIdx])
                    {
                        curSelectedCard = curBuildingData.buildingCard[selectedCardIdx];
                    }
                    if(curSelectedCard != null)
                    {
                        curSelectedCard.cardName = GUILayout.TextField(curSelectedCard.cardName, GUILayout.Width(150));
                    }
                    GUILayout.EndHorizontal();

                    curSelectedCard.cardIcon = (Sprite)EditorGUI.ObjectField(cardSpriteRect, curSelectedCard.cardIcon, typeof(Sprite), false);
                }
                else
                {
                    GUILayout.Label("[No Card Available]", GUILayout.Width(200));
                }

            }
            GUILayout.EndArea();
            // ACTION
            GUILayout.BeginArea(new Rect(310, 50, 350, 300));
            bool addAction = false;
            bool removeButton = false;
            float spritePosY = 0;
            Rect spriteRect = new Rect(160, spritePosY, 150, 15);
            if (curSelectedCard != null)
            {
                GUILayout.BeginHorizontal();
                addAction = GUILayout.Button("Add Action", GUILayout.MaxWidth(100));
                if (curSelectedCard.actionTypes == null)
                {
                    curSelectedCard.actionTypes = new List<CardActiondata>();
                }

                if (addAction)
                {
                    if(curSelectedCard == null)
                    {
                        return;
                    }
                    if (curSelectedCard.actionTypes.Count >= 3)
                    {
                        return;
                    }

                    addAction = false;
                    curSelectedCard.actionTypes.Add(new CardActiondata());
                }
                removeButton = GUILayout.Button("Remove Button", GUILayout.MaxWidth(100));
                if(removeButton)
                {
                    if(curSelectedCard.actionTypes.Count > 0)
                    {
                        curSelectedCard.actionTypes.RemoveAt(curSelectedCard.actionTypes.Count - 1);
                    }
                }
                GUILayout.EndHorizontal();
                actionScrollPos = GUILayout.BeginScrollView(actionScrollPos, new GUIStyle("RL Background"), GUILayout.Width(500), GUILayout.Height(position.height - 400));
                for (int i = 0; i < curSelectedCard.actionTypes.Count; i++)
                {
                    Texture2D texture;
                    GUILayout.BeginHorizontal();
                    curSelectedCard.actionTypes[i].actionType = (CardActionType)EditorGUILayout.EnumPopup(curSelectedCard.actionTypes[i].actionType, GUILayout.MaxWidth(140));
                    GUILayout.EndHorizontal();

                    if(i == 0)
                    {
                        spriteRect = new Rect(2, 20, 150, 15);
                    }
                    else
                    {
                        spriteRect = new Rect(2, (i * 70), 150, 15);
                    }
                    Texture2D tmp;
                    float posY = 0;
                    if(i == 0)
                    {
                        posY = 10;
                    }
                    else
                    {
                        posY = i * 75;
                    }
                    switch (curSelectedCard.actionTypes[i].actionType)
                    {
                        case CardActionType.MessageOnly:
                            GUILayout.BeginHorizontal();
                            curSelectedCard.actionTypes[i].message = GUILayout.TextField(curSelectedCard.actionTypes[i].message, GUILayout.Width(150));
                            GUILayout.EndHorizontal();
                            break;
                        case CardActionType.LogoWithMessage:
                            GUILayout.BeginHorizontal();
                            curSelectedCard.actionTypes[i].logoIcon = (Sprite)EditorGUILayout.ObjectField(curSelectedCard.actionTypes[i].logoIcon, typeof(Sprite), false, GUILayout.Width(100));
                           
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();

                            curSelectedCard.actionTypes[i].message = GUILayout.TextField(curSelectedCard.actionTypes[i].message, GUILayout.Width(150));
                            GUILayout.EndHorizontal();
                            break;
                        case CardActionType.CostMessageOnly:
                            break;
                        case CardActionType.LogoOnly:
                            GUILayout.BeginHorizontal();
                            curSelectedCard.actionTypes[i].logoIcon = (Sprite)EditorGUILayout.ObjectField(curSelectedCard.actionTypes[i].logoIcon, typeof(Sprite), false, GUILayout.Width(100));

                            GUILayout.EndHorizontal();
                            break;
                        default:
                            break;
                    }
                    GUILayout.Space(10);
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }

        public void ShowCardFlavorTexts()
        {
            // PER BUILDING BASIS

            GUILayout.BeginArea(new Rect(5, 350, 600, 300));
            // -------------------------------------        Building
            buildingFlavorScrollPos = GUILayout.BeginScrollView(buildingScrollPos, GUILayout.Width(600), GUILayout.Height(100));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Intro Flavor : " +curBuildingData.BuildingName, EditorStyles.boldLabel, GUILayout.Width(130));
            bool addBldgFlavor = GUILayout.Button("Add Text", GUILayout.MaxWidth(100));
            if (addBldgFlavor)
            {
                string tmp = "New Introduction";
                curBuildingData.introductionMessages.Add(tmp);
            }
            GUILayout.EndHorizontal();

            for (int i = 0; i < curBuildingData.introductionMessages.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Introduction Flavor[" + (i + 1) + "]:", EditorStyles.boldLabel, GUILayout.MaxWidth(140));
                curBuildingData.introductionMessages[i] = GUILayout.TextField(curBuildingData.introductionMessages[i], GUILayout.MaxWidth(350));
                bool removeFlavor = GUILayout.Button("-", GUILayout.MaxWidth(50));
                if (removeFlavor)
                {
                    curBuildingData.introductionMessages.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            // -------------------------------        Card
            cardFlavorScrollPos = GUILayout.BeginScrollView(cardFlavorScrollPos, GUILayout.Width(600), GUILayout.Height(100));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Card Flavor : " + curSelectedCard.cardName, EditorStyles.boldLabel, GUILayout.Width(150));
 
            bool addPositiveCardFlavor = GUILayout.Button("Positive Text", GUILayout.MaxWidth(100));
            bool addNegativeCardFlavor = GUILayout.Button("Negative Text", GUILayout.MaxWidth(100));
            if(addPositiveCardFlavor)
            {
                string tmp = "New Positivity";
                curSelectedCard.cardPosMesg.Add(tmp);
            }
            if(addNegativeCardFlavor)
            {
                string tmp = "New Negativity";
                curSelectedCard.cardNegMesg.Add(tmp);
            }
            GUILayout.EndHorizontal();
            if (curSelectedCard.cardPosMesg == null)
            {
                curSelectedCard.cardPosMesg = new List<string>();
            }
            for (int i = 0; i < curSelectedCard.cardPosMesg.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Positive Flavor[" + (i + 1) + "]:",EditorStyles.boldLabel, GUILayout.MaxWidth(140));
                curSelectedCard.cardPosMesg[i] = GUILayout.TextField(curSelectedCard.cardPosMesg[i], GUILayout.MaxWidth(350));
                bool removeFlavor = GUILayout.Button("-", GUILayout.MaxWidth(50));
                if (removeFlavor)
                {
                    curSelectedCard.cardPosMesg.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }
            if(curSelectedCard.cardNegMesg == null)
            {
                curSelectedCard.cardNegMesg = new List<string>();
            }
            for (int i = 0; i < curSelectedCard.cardNegMesg.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Negative Flavor[" + (i + 1) + "]:", EditorStyles.boldLabel, GUILayout.MaxWidth(140));
                curSelectedCard.cardNegMesg[i] = GUILayout.TextField(curSelectedCard.cardNegMesg[i], GUILayout.MaxWidth(350));
                bool removeFlavor = GUILayout.Button("-", GUILayout.MaxWidth(50));
                if (removeFlavor)
                {
                    curSelectedCard.cardNegMesg.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            // -----------------------------------          Actions
            actionFlavorScrollPos = GUILayout.BeginScrollView(actionFlavorScrollPos, GUILayout.Width(600), GUILayout.Height(100));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Action Flavor", EditorStyles.boldLabel, GUILayout.Width(80));
            List<string> actionList = new List<string>();
            for (int i = 0; i < curSelectedCard.actionTypes.Count; i++)
            {
                string tmp = "Action " + (i + 1).ToString();
                actionList.Add(tmp);
            }
            
            selectedActionIdx = EditorGUILayout.Popup(selectedActionIdx, actionList.ToArray(), GUILayout.Width(70));
            if(curSelectedCard != null)
            {
                curSelectedAction = curSelectedCard.actionTypes[selectedActionIdx];

                if(curSelectedAction.AcceptMesg == null)
                {
                    curSelectedAction.AcceptMesg = new List<string>();
                }
                if (curSelectedAction.DenyMesg == null)
                {
                    curSelectedAction.DenyMesg = new List<string>();
                }
            }
            bool addPositiveActionFlavor = GUILayout.Button("Positive Text", GUILayout.MaxWidth(100));
            bool addNegativeActionFlavor = GUILayout.Button("Negative Text", GUILayout.MaxWidth(100));
            if(addPositiveActionFlavor)
            {
                string tmp = "New Positivity";
                curSelectedAction.AcceptMesg.Add(tmp);
            }
            if(addNegativeActionFlavor)
            {
                string tmp = "New Negativity";
                curSelectedAction.DenyMesg.Add(tmp);
            }
            GUILayout.EndHorizontal();

            if(curSelectedAction.AcceptMesg == null)
            {
                curSelectedAction.AcceptMesg = new List<string>();
            }
            for (int i = 0; i < curSelectedAction.AcceptMesg.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Positive Flavor[" + (i + 1) + "]:", EditorStyles.boldLabel, GUILayout.MaxWidth(140));
                curSelectedAction.AcceptMesg[i] = GUILayout.TextField(curSelectedAction.AcceptMesg[i], GUILayout.MaxWidth(350));
                bool removeFlavor = GUILayout.Button("-", GUILayout.MaxWidth(50));
                if (removeFlavor)
                {
                    curSelectedAction.AcceptMesg.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }

            if (curSelectedAction.DenyMesg == null)
            {
                curSelectedAction.DenyMesg = new List<string>();
            }
            for (int i = 0; i < curSelectedAction.DenyMesg.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Negative Flavor[" + (i + 1) + "]:", EditorStyles.boldLabel, GUILayout.MaxWidth(140));
                curSelectedAction.DenyMesg[i] = GUILayout.TextField(curSelectedAction.DenyMesg[i], GUILayout.MaxWidth(350));
                bool removeFlavor = GUILayout.Button("-", GUILayout.MaxWidth(50));
                if(removeFlavor)
                {
                    curSelectedAction.DenyMesg.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }

        public void ShowActionRewards()
        {
            if (curSelectedAction == null) return;

            GUILayout.BeginArea(new Rect(5, 675, 600, 300));

            bool addReward = false;
            bool removeReward = false;

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("[COST AND REWARD] [ACTION " + (selectedActionIdx+1) + "]" ,EditorStyles.boldLabel, GUILayout.MaxWidth(225));
            addReward = GUILayout.Button("+", GUILayout.MaxWidth(50));
            GUILayout.EndHorizontal();

            if (curSelectedAction.rewardList != null)
            {
                if(addReward)
                {
                    curSelectedAction.rewardList.Add(new ResourceReward());
                }
                if(curSelectedAction.rewardList.Count > 0)
                {
                    actionRewardScrollPos = EditorGUILayout.BeginScrollView(actionRewardScrollPos, new GUIStyle("RL Background"), GUILayout.Width(600), GUILayout.Height(100));
                    for (int i = 0; i < curSelectedAction.rewardList.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        curSelectedAction.rewardList[i].resourceType = (ResourceType)EditorGUILayout.EnumPopup("Type:", curSelectedAction.rewardList[i].resourceType, GUILayout.MaxWidth(150));
                        GUILayout.Label("Amount:", GUILayout.MaxWidth(100));
                        curSelectedAction.rewardList[i].rewardAmount = EditorGUILayout.IntField(curSelectedAction.rewardList[i].rewardAmount, GUILayout.MaxWidth(150));
                        removeReward = GUILayout.Button("-", GUILayout.MaxWidth(50));

                        if(removeReward)
                        {
                            curSelectedAction.rewardList.RemoveAt(i);
                            removeReward = false;
                        }
                        GUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();
                }
            }
            else
            {
                curSelectedAction.rewardList = new List<ResourceReward>();
            }
            GUILayout.EndArea();
        }

        private Texture2D ObtainTexture(Sprite thisSprite)
        {
            Sprite sprite = thisSprite;
            Texture2D tmp = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            var pixel = sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height);
            tmp.SetPixels(pixel);
            tmp.Apply();

            return tmp;
        }
    }

}