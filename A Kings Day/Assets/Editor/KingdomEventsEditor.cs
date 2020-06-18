using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kingdoms;

namespace KingEvents
{
    public class KingdomEventsEditor : EditorWindow
    {
        public GameObject eventStoragePrefab;
        public GameObject curEventStorage;
        public int curEventIndex;
        public int storyArcPopupIdx;

        // Storage List
        KingdomEventStorage curKingdomEventData;
        Vector2 eventScrollPos = Vector2.zero;
        Vector2 eventCreationScrollPos = Vector2.zero;
        public GUIStyle titleText;
        public GUIStyle notSelectedText;
        public GUIStyle selectedText;

        // Edit new Event Item
        EventDecisionData curEvent = new EventDecisionData();
        bool curEventLoaded = false;
        bool addNewDecision = false;
        bool removeLastDecision = false;
        // Selected Event from List
        Vector2 arcScrollPos = Vector2.zero;
        Vector2 arcEventsScrollPos = Vector2.zero;
        EventDecisionData selectedEvent;
        int selectedEventIdx;

        // Edit new Arc Item
        StoryArcEventsData curArc = new StoryArcEventsData();
        bool curArcLoaded = false;
        // Selected Story Arc From List
        StoryArcEventsData selectedArc;
        int selectedArcIdx;
        #region EDITOR_SETUP
        private static Vector2 itemStorShowOn = new Vector2(1000, 600);
        private static Vector2 itemStorShowOff = new Vector2(1000, 600);
        private static int leftPanelWidth = 800;

        [MenuItem("Game Storage/Kingdom Events Storage")]
        public static void ShowWindow()
        {
            GetWindow<KingdomEventsEditor>("Kingdom Events Storage").minSize = itemStorShowOff;
            GetWindow<KingdomEventsEditor>("Kingdom Events Storage").maxSize = itemStorShowOn;
        }
        #endregion

        private bool eventStorageLoaded = false;

        public void Awake()
        {
            titleText = new GUIStyle((GUIStyle)"toolbarTextField");
            notSelectedText = new GUIStyle((GUIStyle)"toolbarTextField");
            selectedText = new GUIStyle((GUIStyle)"toolbarTextField");
        }
        public void Save()
        {
            PrefabUtility.SaveAsPrefabAsset(curEventStorage, "Assets/Resources/Prefabs/Kingdom Events Storage.prefab");
            DestroyImmediate(curEventStorage);
            eventStorageLoaded = false;
            LoadEventStorage();
        }
        public void OnDestroy()
        {
            eventStorageLoaded = false;
            DestroyImmediate(curEventStorage);
        }
        public void OnEnable()
        {
            titleText.normal.textColor = Color.black;
            titleText.alignment = TextAnchor.UpperCenter;

            notSelectedText.normal.textColor = Color.black;
            selectedText.normal.background = MakeTex(700, 18, new Color(0,0,0,0));

            selectedText.normal.textColor = Color.white;
            selectedText.normal.background = MakeTex(700, 18, Color.grey);
        }
        public void OnGUI()
        {
            if(!eventStorageLoaded)
            {
                LoadEventStorage();
            }
            if(eventStorageLoaded)
            {

                ShowEventList();
                ShowEventStorage();
                ShowStoryArcList();
                ShowArcStorage();
            }
        }

        public void LoadEventStorage()
        {
            eventStoragePrefab = (GameObject)Resources.Load("Prefabs/Kingdom Events Storage");
            curEventStorage = (GameObject)Instantiate(eventStoragePrefab);

            if (curEventStorage == null)
            {
                Debug.LogWarning("Storage not found! Check Reference");
                return;
            }

            curKingdomEventData = curEventStorage.GetComponent<KingdomEventStorage>();

            // INTERVALS FOR ALL EVENTS.
            for (int i = 0; i < curKingdomEventData.storyArcEvents.Count; i++)
            {
                if(curKingdomEventData.storyArcEvents[selectedArcIdx].eventIntervals.Count <= 0)
                {
                    curKingdomEventData.storyArcEvents[selectedArcIdx].eventIntervals = new List<int>();
                    for (int x = 0; x < curKingdomEventData.storyArcEvents[selectedArcIdx].storyEvents.Count; x++)
                    {
                        curKingdomEventData.storyArcEvents[selectedArcIdx].eventIntervals.Add(1);
                    }

                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < curKingdomEventData.storyArcEvents.Count; i++)
            {
                if (curKingdomEventData.storyArcEvents[selectedArcIdx].eventBoosts.Count <= 0)
                {
                    curKingdomEventData.storyArcEvents[selectedArcIdx].eventBoosts = new List<float>();
                    for (int x = 0; x < curKingdomEventData.storyArcEvents[selectedArcIdx].storyEvents.Count; x++)
                    {
                        curKingdomEventData.storyArcEvents[selectedArcIdx].eventBoosts.Add(0.5f);
                    }

                }
                else
                {
                    break;
                }
            }


            eventStorageLoaded = true;
        }

        #region EVENTS AND DECISIONS
        public void ShowEventList()
        {
            bool addEvent = false;
            bool saveEvent = false;
            bool removeEvent = false;

            GUILayout.BeginArea(new Rect(680, 10, 400, position.height-375));
            GUILayout.BeginHorizontal();
            GUILayout.Box("Kingdom Events", titleText, GUILayout.Width(295), GUILayout.Height(20));
            GUILayout.EndHorizontal();

            eventScrollPos = GUILayout.BeginScrollView(eventScrollPos, new GUIStyle("RL Background"),GUILayout.Width(300), GUILayout.Height(position.height - 400));

            if(curKingdomEventData.kingdomEvents != null && curKingdomEventData.kingdomEvents.Count > 0)
            {
                for (int i = 0; i < curKingdomEventData.kingdomEvents.Count; i++)
                {
                    bool isClicked = false;
                    string listName = "";

                    GUILayout.BeginHorizontal();
                    isClicked = GUILayout.Button(curKingdomEventData.kingdomEvents[i].title, (selectedEvent != null && curKingdomEventData.kingdomEvents[i].title == selectedEvent.title) ? selectedText : notSelectedText);
                    if (!string.IsNullOrEmpty(curKingdomEventData.kingdomEvents[i].storyArc))
                    {
                        listName = "[" + curKingdomEventData.kingdomEvents[i].storyArc + "]";
                        GUILayout.Label(listName);
                    }
                    GUILayout.EndHorizontal();
                    if (isClicked)
                    {
                        if(curKingdomEventData.kingdomEvents[i] != null)
                        {
                            selectedEvent = curKingdomEventData.kingdomEvents[i];
                            selectedEventIdx = i;
                            curEvent = selectedEvent;
                        }
                        isClicked = false;
                    }
                }

            }

            GUILayout.EndArea();
            GUILayout.EndScrollView();

            GUILayout.BeginArea(new Rect(680, position.height-360, 400, 225));

            GUILayout.BeginHorizontal();
            saveEvent = GUILayout.Button((curEvent == selectedEvent) ? "Modify" : "Save", GUILayout.MaxWidth(100));
            if(curKingdomEventData.kingdomEvents.Find(x => x.title == curEvent.title) != null)
            {
                addEvent = GUILayout.Button("Create New", GUILayout.MaxWidth(100));
            }
            if (curKingdomEventData.kingdomEvents.Find(x => x.title == curEvent.title) != null)
            {
                removeEvent = GUILayout.Button("Remove", GUILayout.MaxWidth(100));
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            // ADD BUTTON
            if(addEvent)
            {
                curEvent = new EventDecisionData();
                selectedEvent = null;
            }
            // REMOVE BUTTON
            if(removeEvent)
            {
                GUI.FocusControl(null);
                removeEvent = false;
                if(selectedEvent != null)
                {
                    curKingdomEventData.kingdomEvents.RemoveAt(selectedEventIdx);
                    selectedEvent = null;
                    curEvent = new EventDecisionData();
                }
            }
            // SAVE BUTTON
            if (saveEvent && !string.IsNullOrEmpty(curEvent.title))
            {
                GUI.FocusControl(null);
                if (curKingdomEventData.kingdomEvents == null)
                {
                    curKingdomEventData.kingdomEvents = new List<EventDecisionData>();
                }
                if (curKingdomEventData.kingdomEvents.Find(x => x.title == curEvent.title) == null)
                {
                    curKingdomEventData.kingdomEvents.Add(curEvent);
                    SaveToStoryArcs(curEvent);
                    curEvent = new EventDecisionData();
                }
                else
                {
                    // MODIFY CURRENT EVENT
                    if((curEvent == selectedEvent))
                    {
                        if(selectedEvent.isStoryArc)
                        {
                            SaveToStoryArcs(selectedEvent);
                        }
                        curKingdomEventData.kingdomEvents[selectedEventIdx] = curEvent;
                        curEvent = new EventDecisionData();
                        selectedEvent = null;
                    }
                    else
                    {
                        Debug.LogError("MULTIPLE EVENTS WITH SAME TITLE OCCURRED, PLEACE CHECK LIST!");
                    }
                }
                saveEvent = false;
            }
        }
        public void ShowEventStorage()
        {
            bool SaveStorage = false;

            GUILayout.BeginArea(new Rect(5, 12, leftPanelWidth, 300));


            GUILayout.ExpandWidth(false);
            EditorStyles.textField.wordWrap = true;
            // Title
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 34;
            EditorGUILayout.LabelField("Title:", EditorStyles.boldLabel, GUILayout.Width(75));
            curEvent.title = EditorGUILayout.TextField(curEvent.title, GUILayout.MaxWidth(300));
            GUILayout.EndHorizontal();
            // Description
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Description:", GUILayout.MaxWidth(75));
            curEvent.description = EditorGUILayout.TextArea(curEvent.description, GUILayout.MaxHeight(100), GUILayout.MaxWidth(300));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            // ENUM TYPES
            GUILayout.BeginArea(new Rect(400, 10, 400, 300));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Event Card:", GUILayout.Width(100));
            curEvent.eventType = (CardType)EditorGUILayout.EnumPopup(curEvent.eventType, GUILayout.MaxWidth(150));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Reporter:", GUILayout.Width(100));
            curEvent.reporterType = (ReporterType)EditorGUILayout.EnumPopup(curEvent.reporterType, GUILayout.MaxWidth(150));
            GUILayout.EndHorizontal();
            EditorGUILayout.LabelField("[Level and Difficulty]",EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Kingdom Level:", GUILayout.Width(100));
            curEvent.levelRequired = (Territory.TerritoryLevel)EditorGUILayout.EnumPopup(curEvent.levelRequired, GUILayout.MaxWidth(150));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Difficulty :", GUILayout.Width(100));
            curEvent.difficultyType = (DifficultyType)EditorGUILayout.EnumPopup(curEvent.difficultyType, GUILayout.MaxWidth(150));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Story Arc :", GUILayout.Width(75));
            curEvent.isStoryArc = EditorGUILayout.Toggle(curEvent.isStoryArc, GUILayout.MaxWidth(20));
            if(curEvent.isStoryArc)
            {
                if (!string.IsNullOrEmpty(curEvent.storyArc))
                {
                    storyArcPopupIdx = curKingdomEventData.storyArcEvents.FindIndex(x => x.storyTitle == curEvent.storyArc);
                }
                else
                {
                    storyArcPopupIdx = 0;
                    curEvent.SetMyStoryArc(curKingdomEventData.storyArcEvents[storyArcPopupIdx]);
                }

                storyArcPopupIdx = EditorGUILayout.Popup(storyArcPopupIdx, ObtainStoryArc(), GUILayout.MaxWidth(150));

                if (string.IsNullOrEmpty(curEvent.storyArc))
                {
                    if (curKingdomEventData.storyArcEvents != null && curKingdomEventData.storyArcEvents.Count > 0)
                    {
                        // ADD THIS EVENT TO STORY ARC
                        Debug.Log("Entering :" + storyArcPopupIdx);
                        if(curKingdomEventData.kingdomEvents.Contains(curEvent))
                        {
                            SaveToStoryArcs(curEvent);
                        }
                    }
                }
                else if(curKingdomEventData.storyArcEvents[storyArcPopupIdx] != null && curEvent.storyArc != curKingdomEventData.storyArcEvents[storyArcPopupIdx].storyTitle)
                {
                    Debug.Log("Changes has been made!");
                    // Removing from previous Event
                    int prevIdx = curKingdomEventData.storyArcEvents.FindIndex(x => x.storyTitle == curEvent.storyArc);
                    Debug.Log("Index:" + prevIdx + " Story Title:" + curKingdomEventData.storyArcEvents[prevIdx].storyTitle);
                    curKingdomEventData.storyArcEvents[prevIdx].RemoveEventDecision(curEvent);

                    // Setting Changes made
                    curKingdomEventData.storyArcEvents[storyArcPopupIdx].AddEventDecision(curEvent);
                    curEvent.SetMyStoryArc(curKingdomEventData.storyArcEvents[storyArcPopupIdx]);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(curEvent.storyArc))
                {
                    RemoveFromStoryArc(curEvent);
                    storyArcPopupIdx = 0;
                }
            }
            GUILayout.EndHorizontal();


            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(300, position.height-50, leftPanelWidth, 300));
            SaveStorage = GUILayout.Button("SAVE STORAGE", GUILayout.MaxWidth(300));
            if(SaveStorage)
            {
                Save();
                SaveStorage = false;
            }
            GUILayout.EndArea();
            ShowDecisionOptions();

        }

        public void SaveToStoryArcs(EventDecisionData thisEvent)
        {
            int idx = curKingdomEventData.storyArcEvents.FindIndex(x => x.storyTitle == thisEvent.storyArc);

            if(curKingdomEventData.storyArcEvents[idx].storyEvents.Find(x => x.title == thisEvent.title) != null)
            {
                int storyIdx = curKingdomEventData.storyArcEvents[idx].storyEvents.FindIndex(x => x.title == thisEvent.title);
                curKingdomEventData.storyArcEvents[idx].storyEvents[storyIdx] = thisEvent;
            }
            else
            {
                curKingdomEventData.storyArcEvents[idx].AddEventDecision(thisEvent);
            }
            curKingdomEventData.storyArcEvents[idx].eventIntervals.Add(0);
            curKingdomEventData.storyArcEvents[idx].storyEventTitlesList.Add(thisEvent.title);
            thisEvent.SetMyStoryArc(curKingdomEventData.storyArcEvents[idx]);
        }
        public void RemoveFromStoryArc(EventDecisionData thisEvent)
        {
            int idx = curKingdomEventData.storyArcEvents.FindIndex(x => x.storyTitle == thisEvent.storyArc);
            curKingdomEventData.storyArcEvents[idx].RemoveEventDecision(thisEvent);

            if (curKingdomEventData.storyArcEvents[idx].eventIntervals.Count-1 >= idx)
            {
                curKingdomEventData.storyArcEvents[idx].eventIntervals.RemoveAt(idx);
            }
            if(curKingdomEventData.storyArcEvents[idx].storyEventTitlesList.Contains(thisEvent.title))
            {
                curKingdomEventData.storyArcEvents[idx].storyEventTitlesList.RemoveAt(idx);
            }
            thisEvent.storyArc = "";
        }
        public void ShowDecisionOptions()
        {
            GUILayout.BeginArea(new Rect(130, 130, leftPanelWidth, 300));
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Add Options:", GUILayout.Width(100));
            addNewDecision = GUILayout.Button("+", GUILayout.MaxWidth(50));
            removeLastDecision = GUILayout.Button("-", GUILayout.MaxWidth(50));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, 150, leftPanelWidth, 300));
            eventCreationScrollPos = EditorGUILayout.BeginScrollView(eventCreationScrollPos, new GUIStyle("RL Background"), GUILayout.Width(650), GUILayout.Height(position.height - 510));
            if (addNewDecision)
            {
                addNewDecision = false;
                if (curEvent.eventDecision == null)
                {
                    curEvent.eventDecision = new List<EventDecision>();
                }
                if (curEvent.eventDecision.Count < 4)
                {
                    curEvent.eventDecision.Add(new EventDecision());
                }
            }

            if (removeLastDecision && curEvent.eventDecision != null)
            {
                removeLastDecision = false;
                if (curEvent.eventDecision.Count > 0)
                {
                    curEvent.eventDecision.RemoveAt(curEvent.eventDecision.Count - 1);
                }
            }
            if (curEvent != null && curEvent.eventDecision != null && curEvent.eventDecision.Count > 0)
            {
                for (int i = 0; i < curEvent.eventDecision.Count; i++)
                {
                    DecisionOption(curEvent.eventDecision[i]);
                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public void DecisionOption(EventDecision thisDecision)
        {
            bool addReward = false;
            bool removeReward = false;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Decision Words:", GUILayout.MaxWidth(90));
            thisDecision.optionDescription = EditorGUILayout.TextArea(thisDecision.optionDescription, GUILayout.MaxHeight(15), GUILayout.MaxWidth(350));
            EditorGUILayout.LabelField("Add Rewards:", GUILayout.MaxWidth(80));
            addReward = GUILayout.Button("+", GUILayout.MaxWidth(50));
            removeReward = GUILayout.Button("-", GUILayout.MaxWidth(50));
            EditorGUILayout.EndHorizontal();

            if (addReward)
            {
                addReward = false;
                if (thisDecision.rewards == null)
                {
                    thisDecision.rewards = new List<ResourceReward>();
                }
                if (thisDecision.rewards.Count < 4)
                {
                    thisDecision.rewards.Add(new ResourceReward());
                }
            }
            else if (removeReward)
            {
                removeReward = false;
                if (thisDecision.rewards != null && thisDecision.rewards.Count > 0)
                {
                    thisDecision.rewards.RemoveAt(thisDecision.rewards.Count - 1);
                }
            }
            if (thisDecision.rewards != null)
            {
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < thisDecision.rewards.Count; i++)
                {
                    thisDecision.rewards[i].resourceType = (ResourceType)EditorGUILayout.EnumPopup(
                            "Type:",
                            thisDecision.rewards[i].resourceType, GUILayout.MaxWidth(75));
                    thisDecision.rewards[i].rewardAmount = EditorGUILayout.IntField("Amount:", thisDecision.rewards[i].rewardAmount, GUILayout.MaxWidth(75));
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        #endregion

        #region Story Arc
        public string[] ObtainStoryArc()
        {
            if(curKingdomEventData.storyArcEvents == null)
            {
                curKingdomEventData.storyArcEvents = new List<StoryArcEventsData>();
            }
            string[] tmp = new string[curKingdomEventData.storyArcEvents.Count];

            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = curKingdomEventData.storyArcEvents[i].storyTitle;
            }

            return tmp;
        }

        public void ShowStoryArcList()
        {
            bool addArc = false;
            bool saveArc = false;
            bool removeArc = false;

            GUILayout.BeginArea(new Rect(680, 275, 400, position.height - 375));
            GUILayout.BeginHorizontal();
            GUILayout.Box("Story Arcs", titleText, GUILayout.Width(295), GUILayout.Height(20));
            GUILayout.EndHorizontal();

            arcScrollPos = EditorGUILayout.BeginScrollView(arcScrollPos, new GUIStyle("RL Background"), GUILayout.Width(300), GUILayout.Height(position.height - 10));

            if (curKingdomEventData.storyArcEvents != null && curKingdomEventData.storyArcEvents.Count > 0)
            {
                for (int i = 0; i < curKingdomEventData.storyArcEvents.Count; i++)
                {
                    bool isClicked = false;
                    isClicked = GUILayout.Button(curKingdomEventData.storyArcEvents[i].storyTitle, (selectedArc != null && curKingdomEventData.storyArcEvents[i].storyTitle == selectedArc.storyTitle) ? selectedText : notSelectedText);
                    if (isClicked)
                    {
                        if (curKingdomEventData.storyArcEvents[i] != null)
                        {
                            selectedArc = curKingdomEventData.storyArcEvents[i];
                            selectedArcIdx = i;
                            curArc = selectedArc;
                        }
                        isClicked = false;
                    }
                }

            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(680, position.height - 95, 400, 225));

            GUILayout.BeginHorizontal();
            saveArc = GUILayout.Button((curArc == selectedArc) ? "Modify" : "Save", GUILayout.MaxWidth(100));
            if(curKingdomEventData.storyArcEvents == null)
            {
                curKingdomEventData.storyArcEvents = new List<StoryArcEventsData>();
            }
            if (curKingdomEventData.storyArcEvents.Find(x => x.storyTitle == curArc.storyTitle) != null)
            {
                addArc = GUILayout.Button("Create New", GUILayout.MaxWidth(100));
            }
            if (curKingdomEventData.storyArcEvents.Find(x => x.storyTitle == curArc.storyTitle) != null)
            {
                removeArc = GUILayout.Button("Remove", GUILayout.MaxWidth(100));
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            // ADD BUTTON
            if (addArc)
            {
                curEvent = new EventDecisionData();
                selectedEvent = null;
            }
            // REMOVE BUTTON
            if (removeArc)
            {
                GUI.FocusControl(null);
                removeArc = false;
                if (selectedEvent != null)
                {
                    curKingdomEventData.storyArcEvents.RemoveAt(selectedArcIdx);
                    selectedEvent = null;
                    curEvent = new EventDecisionData();
                }
            }
            // SAVE BUTTON
            if (saveArc && !string.IsNullOrEmpty(curArc.storyTitle))
            {
                GUI.FocusControl(null);
                if (curKingdomEventData.storyArcEvents == null)
                {
                    curKingdomEventData.storyArcEvents = new List<StoryArcEventsData>();
                }
                if (curKingdomEventData.storyArcEvents.Find(x => x.storyTitle == curArc.storyTitle) == null)
                {
                    curKingdomEventData.storyArcEvents.Add(curArc);
                    curArc = new StoryArcEventsData();
                }
                else
                {
                    // MODIFY CURRENT EVENT
                    if ((curArc == selectedArc))
                    {
                        curKingdomEventData.storyArcEvents[selectedArcIdx] = curArc;
                        curArc = new StoryArcEventsData();
                        selectedArc = null;
                    }
                    else
                    {
                        Debug.LogError("MULTIPLE ARCS WITH SAME TITLE OCCURRED, PLEACE CHECK LIST!");
                    }
                }
                saveArc = false;
            }
        }
        public void ShowArcEventsOptions()
        {
            GUILayout.BeginArea(new Rect(10, 375, 400, position.height - 375));
            GUILayout.BeginHorizontal();
            GUILayout.Box("Story Arc Events", titleText, GUILayout.Width(295), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(10, position.height - 215, leftPanelWidth, 315));

            arcEventsScrollPos = EditorGUILayout.BeginScrollView(arcEventsScrollPos, new GUIStyle("RL Background"), GUILayout.Width(650), GUILayout.Height(position.height - 470));



            if (curArc != null && curArc.storyEvents != null)
            {             
                string[] count = new string[curArc.storyEvents.Count];
                int[] choice = new int[curArc.storyEvents.Count];

                for (int i = 0; i < count.Length; i++)
                {
                    choice[i] = i;
                    count[i] = i.ToString();
                }
                for (int i = 0; i < curArc.storyEvents.Count; i++)
                {
                    int y = i;
                    EditorGUILayout.BeginHorizontal();
                    y = EditorGUILayout.IntPopup(y, count, choice, GUILayout.Width(40));
                    if(y != i)
                    {
                        SwapArcEvents(y, i);
                        GUI.FocusControl(null);
                        curArc = curKingdomEventData.storyArcEvents[selectedArcIdx];
                    }
                    EditorGUILayout.LabelField(curArc.storyEvents[i].title, GUILayout.Width(150));
                    EditorGUILayout.LabelField("[Difficulty: "+curArc.storyEvents[i].difficultyType + "]", GUILayout.Width(150), GUILayout.Height(20));

                    EditorGUILayout.LabelField(new GUIContent("Boost T:", "Current or Starting(if first Event) Week plus interval equals boost date."), GUILayout.Width(55));
                    curKingdomEventData.storyArcEvents[selectedArcIdx].eventIntervals[i] = EditorGUILayout.IntField(curKingdomEventData.storyArcEvents[selectedArcIdx].eventIntervals[i], GUILayout.Width(50), GUILayout.Height(20));

                    EditorGUILayout.LabelField(new GUIContent("Boost %:", "Amount of Chance Added to the Event being spawned."), GUILayout.Width(55));
                    curKingdomEventData.storyArcEvents[selectedArcIdx].eventBoosts[i] = EditorGUILayout.FloatField(curKingdomEventData.storyArcEvents[selectedArcIdx].eventBoosts[i], GUILayout.Width(50), GUILayout.Height(20));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    int eventIdx = curKingdomEventData.kingdomEvents.FindIndex(x => x.title == curArc.storyEvents[i].title);
                    if(curArc.storyEvents[i].eventDecision != null && curArc.storyEvents[i].eventDecision.Count > 0)
                    {
                        if(i == 0)
                        {
                            EditorGUILayout.Toggle(new GUIContent("Start:", "Toggle this if this is one of the end results."), true, GUILayout.Width(70));
                        }
                        else
                        {
                            curKingdomEventData.kingdomEvents[eventIdx].arcEnd = EditorGUILayout.Toggle(new GUIContent("End:", "Toggle this if this is one of the end results."), curKingdomEventData.kingdomEvents[eventIdx].arcEnd, GUILayout.Width(70));
                        }
                        EditorGUILayout.LabelField(new GUIContent("[Event Branches :", "Event Decisions that allows you to place what event should they go to after the said decision."), GUILayout.Width(100), GUILayout.Height(20));
                        for (int x = 0; x < curKingdomEventData.kingdomEvents[eventIdx].eventDecision.Count; x++)
                        {
                            int curIdx = curKingdomEventData.kingdomEvents[eventIdx].eventDecision[x].nextArcIdx;
                            EditorGUILayout.LabelField("Option [" + (x + 1) + "]", GUILayout.Width(60));
                            curKingdomEventData.kingdomEvents[eventIdx].eventDecision[x].nextArcIdx = (curIdx == 0 || curIdx >= curKingdomEventData.kingdomEvents.Count) ? i + 1 : curKingdomEventData.kingdomEvents[eventIdx].eventDecision[x].nextArcIdx;
                            curKingdomEventData.kingdomEvents[eventIdx].eventDecision[x].nextArcIdx = EditorGUILayout.IntPopup(curKingdomEventData.kingdomEvents[eventIdx].eventDecision[x].nextArcIdx, count, choice, GUILayout.Width(40));
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public void SwapArcEvents(int transferTo, int transferFrom)
        {
            EventDecisionData tmp = curKingdomEventData.storyArcEvents[selectedArcIdx].storyEvents[transferFrom];
            curKingdomEventData.storyArcEvents[selectedArcIdx].storyEvents[transferFrom] = curKingdomEventData.storyArcEvents[selectedArcIdx].storyEvents[transferTo];
            curKingdomEventData.storyArcEvents[selectedArcIdx].storyEvents[transferTo] = tmp;


            int temp = curKingdomEventData.storyArcEvents[selectedEventIdx].eventIntervals[transferFrom];
            curKingdomEventData.storyArcEvents[selectedEventIdx].eventIntervals[transferFrom] = curKingdomEventData.storyArcEvents[selectedEventIdx].eventIntervals[transferTo];
            curKingdomEventData.storyArcEvents[selectedEventIdx].eventIntervals[transferTo] = temp;


            float boost = curKingdomEventData.storyArcEvents[selectedEventIdx].eventBoosts[transferFrom];
            curKingdomEventData.storyArcEvents[selectedEventIdx].eventBoosts[transferFrom] = curKingdomEventData.storyArcEvents[selectedEventIdx].eventBoosts[transferTo];
            curKingdomEventData.storyArcEvents[selectedEventIdx].eventBoosts[transferTo] = boost;

            if(curKingdomEventData.storyArcEvents.Count-1 <= selectedArcIdx)
            {
                curKingdomEventData.storyArcEvents[selectedArcIdx].AdjustTitleList();
            }
        }
        public void ShowArcStorage()
        {
            bool SaveStorage = false;

            GUILayout.BeginArea(new Rect(5, 300, leftPanelWidth, 300));


            EditorStyles.textField.wordWrap = true;
            GUILayout.ExpandWidth(false);
            // Title
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 34;
            EditorGUILayout.LabelField("Title:", EditorStyles.boldLabel, GUILayout.Width(75));
            curArc.storyTitle = EditorGUILayout.TextArea(curArc.storyTitle, GUILayout.MaxWidth(300));
            GUILayout.EndHorizontal();
            // Description
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Description:", GUILayout.MaxWidth(75));
            curArc.description = EditorGUILayout.TextArea(curArc.description, GUILayout.MaxHeight(50), GUILayout.MaxWidth(300));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            // ENUM TYPES
            GUILayout.BeginArea(new Rect(400, 300, 400, 300));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Starting Week:", GUILayout.Width(100));
            curArc.startingWeek = EditorGUILayout.IntField(curArc.startingWeek, GUILayout.MaxWidth(150));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Repetition Type:", GUILayout.Width(100));
            curArc.repetitionType = (StoryRepetitionType)EditorGUILayout.EnumPopup(curArc.repetitionType, GUILayout.MaxWidth(150));
            GUILayout.EndHorizontal();


            GUILayout.EndArea();

            ShowArcEventsOptions();
        }

        #endregion
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}
