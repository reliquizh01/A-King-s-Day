using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kingdoms;
using Characters;
using Dialogue;
using Managers;

namespace Drama
{

    public class DramaStorageEditor : EditorWindow
    {
        #region EDITOR_SETUP
        private static Vector2 itemStorShowOn = new Vector2(1000, 600);
        private static Vector2 itemStorShowOff = new Vector2(1000, 600);
        private static int leftPanelWidth = 800;

        [MenuItem("Game Storage/Drama Scenario Storage")]
        public static void ShowWindow()
        {
            GetWindow<DramaStorageEditor>("Drama Scenario Storage").minSize = itemStorShowOff;
            GetWindow<DramaStorageEditor>("Drama Scenario Storage").maxSize = itemStorShowOn;
        }
        #endregion

        public GameObject dramaStoragePrefab;
        public GameObject curDramaGameObject;
        public DramaStorage curDramaStorage;

        public int selectedDramaIdx;
        public DramaScenario selectedDrama;
        public DramaScenario curDrama;

        public int selectedActorIdx;
        public ActorStorageBay selectedActor;
        public ActorStorageBay curActor;
        public Object currentActorPrefab;

        public int selectedFrameIdx;
        public DramaFrame selectedFrame;
        public DramaFrame curFrame;

        public int selectedActionIdx;
        public int selectedActorActionIdx;
        public DramaAction selectedAction;
        public DramaAction curAction;

        // ACTION EASY PLACE MECHANICS
        public GameObject easyPlacePosition;

        public bool isDramaStorageLoaded = false;

        public Vector2 dramaListScrollPos = Vector2.zero;
        public Vector2 actorListScrollPos = Vector2.zero;
        public Vector2 currentActorListScrollPos = Vector2.zero;
        public Vector2 frameListScrollPos = Vector2.zero;
        public Vector2 actionListScrollPos = Vector2.zero;

        public GUIStyle titleText;
        public GUIStyle notSelectedText;
        public GUIStyle selectedText;

        public float actorListPosY = 300;
        public void Awake()
        {
            titleText = new GUIStyle((GUIStyle)"toolbarTextField");
            notSelectedText = new GUIStyle((GUIStyle)"toolbarTextField");
            selectedText = new GUIStyle((GUIStyle)"toolbarTextField");
        }
        public void OnEnable()
        {
            titleText.normal.textColor = Color.black;
            titleText.alignment = TextAnchor.UpperCenter;

            notSelectedText.normal.textColor = Color.black;
            selectedText.normal.background = MakeTex(700, 18, new Color(0, 0, 0, 0));

            selectedText.normal.textColor = Color.white;
            selectedText.normal.background = MakeTex(700, 18, Color.grey);
        }

        public void Save()
        {
            PrefabUtility.SaveAsPrefabAsset(curDramaGameObject, "Assets/Resources/Prefabs/Drama System/Drama Storage.prefab");
            DestroyImmediate(curDramaGameObject);
            isDramaStorageLoaded = false;
            LoadDramaStorage();
        }

        public void OnDestroy()
        {
            isDramaStorageLoaded = false;
            DestroyImmediate(curDramaGameObject);
        }

        public void OnGUI()
        {
            if(!isDramaStorageLoaded)
            {
                LoadDramaStorage();
            }
            else
            {
                ShowDramaList();
                ShowActorList();
                ShowCurrentDrama();
                ShowActionList();
                ShowCurrentAction();
            }
        }


        public void LoadDramaStorage()
        {
            dramaStoragePrefab = (GameObject)Resources.Load("Prefabs/Drama System/Drama Storage");
            curDramaGameObject = (GameObject)Instantiate(dramaStoragePrefab);
            curDramaStorage = curDramaGameObject.GetComponent<DramaStorage>();
            if (curDramaGameObject == null)
            {
                Debug.LogWarning("Storage not found! Check Reference");
                return;
            }

            isDramaStorageLoaded = true;
        }

        public void ShowActorList()
        {
            if(curDramaStorage.actorStorage == null)
            {
                curDramaStorage.actorStorage = new List<ActorStorageBay>();
            }
            bool addEvent = false;
            bool saveEvent = false;
            bool removeEvent = false;


            GUILayout.BeginArea(new Rect(680, actorListPosY-20, 400, position.height - 350));
            ShowCurrentActor();
            GUILayout.BeginHorizontal();
            GUILayout.Box("Available Actors List", titleText, GUILayout.Width(295), GUILayout.Height(20));
            GUILayout.EndHorizontal();

            actorListScrollPos = GUILayout.BeginScrollView(actorListScrollPos, new GUIStyle("RL Background"), GUILayout.Width(300), GUILayout.Height(position.height - 400));

            if(curDramaStorage.actorStorage != null && curDramaStorage.actorStorage.Count > 0)
            {
                for (int i = 0; i < curDramaStorage.actorStorage.Count; i++)
                {
                    bool isClicked = false;
                    string label = "";
                    switch (curDramaStorage.actorStorage[i].actorType)
                    {
                        case DramaActorType.Generic:
                            label = "[G}";
                            break;
                        case DramaActorType.Unique:
                            label = "[U}";
                            break;
                        case DramaActorType.Tools:
                            label = "[TOOLS}";
                            break;
                        case DramaActorType.SFX:
                            label = "[SFX}";
                            break;
                        default:
                            break;
                    }
                    label += " -" + curDramaStorage.actorStorage[i].smallDescription + ".";
                    GUILayout.BeginHorizontal();
                    isClicked = GUILayout.Button(curDramaStorage.actorStorage[i].actorName,(selectedActor != null && curDramaStorage.actorStorage[i].actorName == selectedActor.actorName) ? selectedText : notSelectedText, GUILayout.MinWidth(80));
                    GUILayout.Label(label);
                    GUILayout.EndHorizontal();

                    if (isClicked)
                    {
                        GUI.FocusControl(null);
                        if (curDramaStorage.actorStorage[i] != null)
                        {
                            selectedActor = curDramaStorage.actorStorage[i];
                            selectedActorIdx = i;
                            curActor = selectedActor;
                        }
                        isClicked = false;
                    }
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(680, actorListPosY + 240, 400, 225));

            GUILayout.BeginHorizontal();
            saveEvent = GUILayout.Button((curActor == selectedActor && selectedActor != null) ? "Modify" : "Save", GUILayout.MaxWidth(100));

            if(curDramaStorage.actorStorage == null)
            {
                curDramaStorage.actorStorage = new List<ActorStorageBay>();
            }

            if(curActor == null)
            {
                curActor = new ActorStorageBay();
            }
            if (curDramaStorage.actorStorage.Count <= 0 || curActor != null && curDramaStorage.actorStorage.Find(x => x.actorName == curActor.actorName) != null)
            {
                addEvent = GUILayout.Button("Create New", GUILayout.MaxWidth(100));
            }
            if (curDramaStorage.actorStorage.Find(x => x.actorName == curActor.actorName) != null)
            {
                removeEvent = GUILayout.Button("Remove", GUILayout.MaxWidth(100));
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            // ADD BUTTON
            if (addEvent)
            {
                curActor = new ActorStorageBay();
                selectedActor = null;
            }
            // REMOVE BUTTON
            if (removeEvent)
            {
                GUI.FocusControl(null);
                removeEvent = false;
                if (selectedActor != null)
                {
                    curDramaStorage.actorStorage.RemoveAt(selectedActorIdx);
                    selectedActor = null;
                    curActor = new ActorStorageBay();
                }
            }
            // SAVE BUTTON
            if (saveEvent && !string.IsNullOrEmpty(curActor.actorName))
            {
                GUI.FocusControl(null);
                if (curDramaStorage.actorStorage == null)
                {
                    curDramaStorage.actorStorage = new List<ActorStorageBay>();
                }
                if (curDramaStorage.actorStorage.Find(x => x.actorName == curActor.actorName) == null)
                {
                    curDramaStorage.actorStorage.Add(curActor);
                    curActor = new ActorStorageBay();
                }
                else
                {
                    // MODIFY CURRENT EVENT
                    if ((curActor == selectedActor))
                    {
                        curDramaStorage.actorStorage[selectedActorIdx] = curActor;
                        curActor = new ActorStorageBay();
                        selectedActor = null;
                    }
                    else
                    {
                        Debug.LogError("MULTIPLE EVENTS WITH SAME TITLE OCCURRED, PLEACE CHECK LIST!");
                    }
                }
                saveEvent = false;
                Save();
            }
        }
        public void ShowCurrentActor()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box("Actor Information", titleText, GUILayout.Width(295), GUILayout.Height(20));
            GUILayout.EndHorizontal();

            if (curActor == null)
                return;

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name:", EditorStyles.boldLabel, GUILayout.Width(50));
            curActor.actorName = EditorGUILayout.TextField(curActor.actorName, GUILayout.MaxWidth(120));
            EditorGUILayout.LabelField("Type:", EditorStyles.boldLabel, GUILayout.Width(35));
            curActor.actorType = (DramaActorType)EditorGUILayout.EnumPopup(curActor.actorType, GUILayout.MaxWidth(80));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Description:", EditorStyles.boldLabel, GUILayout.Width(80));
            curActor.smallDescription = EditorGUILayout.TextField(curActor.smallDescription, GUILayout.MaxWidth(210));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            curActor.prefabPath = GUILayout.TextField(curActor.prefabPath, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(curActor.prefabPath))
            {
                currentActorPrefab = (Object)AssetDatabase.LoadAssetAtPath(curActor.prefabPath, typeof(Object));
            }
            else
            {
                currentActorPrefab = null;
            }
            GUILayout.Label("Prefab:", EditorStyles.boldLabel, GUILayout.Width(50));
            EditorGUI.BeginChangeCheck();
            currentActorPrefab = (Object)EditorGUILayout.ObjectField(currentActorPrefab, typeof(Object), true, GUILayout.MaxWidth(240));
            if (EditorGUI.EndChangeCheck())
            {
                if (currentActorPrefab != null)
                {
                    curActor.prefabPath = AssetDatabase.GetAssetPath(currentActorPrefab);
                    Debug.Log("Changes has been made : " + curActor.prefabPath);
                }
            }
            UpdateAllDramaWithActor(curActor.actorName);
            GUILayout.EndHorizontal();
        }
        public void ShowDramaList()
        {
            if(curDramaStorage.dramaSceneStorage == null)
            {
                curDramaStorage.dramaSceneStorage = new List<DramaScenario>();
            }
            bool addEvent = false;
            bool saveEvent = false;
            bool removeEvent = false;

            GUILayout.BeginArea(new Rect(680, 10, 400, position.height - 375));
            GUILayout.BeginHorizontal();
            GUILayout.Box("Drama Scene List", titleText, GUILayout.Width(295), GUILayout.Height(20));
            GUILayout.EndHorizontal();

            dramaListScrollPos = GUILayout.BeginScrollView(dramaListScrollPos, new GUIStyle("RL Background"), GUILayout.Width(300), GUILayout.Height(position.height - 400));

            if (curDramaStorage.dramaSceneStorage != null && curDramaStorage.dramaSceneStorage.Count > 0)
            {
                for (int i = 0; i < curDramaStorage.dramaSceneStorage.Count; i++)
                {
                    bool isClicked = false;
                    string listName = "";

                    GUILayout.BeginHorizontal();
                    isClicked = GUILayout.Button(curDramaStorage.dramaSceneStorage[i].scenarioName, (selectedDrama != null && curDramaStorage.dramaSceneStorage[i].scenarioName == selectedDrama.scenarioName) ? selectedText : notSelectedText);
                    if (curDramaStorage.dramaSceneStorage[i].actionsPerFrame != null && curDramaStorage.dramaSceneStorage[i].actionsPerFrame.Count > 0)
                    {
                        listName = "[Frames: " + curDramaStorage.dramaSceneStorage[i].actionsPerFrame.Count + "]";
                        GUILayout.Label(listName);
                    }
                    GUILayout.EndHorizontal();
                    if (isClicked)
                    {
                        GUI.FocusControl(null);
                        if (curDramaStorage.dramaSceneStorage[i] != null)
                        {
                            selectedDrama = curDramaStorage.dramaSceneStorage[i];
                            selectedDramaIdx = i;
                            curDrama = selectedDrama;
                        }
                        curAction = null;
                        selectedFrame = null;
                        isClicked = false;
                    }
                }
            }

            GUILayout.EndArea();
            GUILayout.EndScrollView();

            GUILayout.BeginArea(new Rect(680, position.height - 360, 400, 225));

            GUILayout.BeginHorizontal();
            saveEvent = GUILayout.Button((curDrama == selectedDrama) ? "Modify" : "Save", GUILayout.MaxWidth(100));

            if(curDramaStorage.dramaSceneStorage == null)
            {
                curDramaStorage.dramaSceneStorage = new List<DramaScenario>();
            }

            if(curDrama == null)
            {
                curDrama = new DramaScenario();
            }
            if (curDramaStorage.dramaSceneStorage.Count <= 0 || curDramaStorage.dramaSceneStorage.Find(x => x.scenarioName == curDrama.scenarioName) != null)
            {
                addEvent = GUILayout.Button("Create New", GUILayout.MaxWidth(100));
            }
            if (curDramaStorage.dramaSceneStorage.Find(x => x.scenarioName == curDrama.scenarioName) != null)
            {
                removeEvent = GUILayout.Button("Remove", GUILayout.MaxWidth(100));
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            // ADD BUTTON
            if (addEvent)
            {
                curDrama = new DramaScenario();
                selectedDrama = null;
            }
            // REMOVE BUTTON
            if (removeEvent)
            {
                GUI.FocusControl(null);
                removeEvent = false;
                if (selectedDrama != null)
                {
                    curDramaStorage.dramaSceneStorage.RemoveAt(selectedDramaIdx);
                    selectedDrama = null;
                    curDrama = new DramaScenario();
                }
            }
            // SAVE BUTTON
            if (saveEvent && !string.IsNullOrEmpty(curDrama.scenarioName))
            {
                GUI.FocusControl(null);
                if (curDramaStorage.dramaSceneStorage == null)
                {
                    curDramaStorage.dramaSceneStorage = new List<DramaScenario>();
                }
                if (curDramaStorage.dramaSceneStorage.Find(x => x.scenarioName == curDrama.scenarioName) == null)
                {
                    curDramaStorage.dramaSceneStorage.Add(curDrama);
                    curDrama = new DramaScenario();
                }
                else
                {
                    // MODIFY CURRENT EVENT
                    if ((curDrama == selectedDrama))
                    {
                        curDramaStorage.dramaSceneStorage[selectedDramaIdx] = curDrama;
                        curDrama = new DramaScenario();
                        selectedDrama = null;
                    }
                    else
                    {
                        Debug.LogError("MULTIPLE EVENTS WITH SAME TITLE OCCURRED, PLEACE CHECK LIST!");
                    }
                }
                saveEvent = false;
                Save();
            }
        }

        public void UpdateAllDramaWithActor(string actorName)
        {
            for (int i = 0; i < curDramaStorage.dramaSceneStorage.Count; i++)
            {
                int idx = -1;
                    idx = curDramaStorage.dramaSceneStorage[i].actors.FindIndex(x => x.characterName == actorName);
                if(idx != -1)
                {
                    curDramaStorage.dramaSceneStorage[i].actors[idx].characterPrefabPath = curActor.prefabPath;
                    curDramaStorage.dramaSceneStorage[i].actors[idx].characterName = curActor.actorName;
                    curDramaStorage.dramaSceneStorage[i].actors[idx].actorType = curActor.actorType;
                }
            }
        }
        public void ShowCurrentDrama()
        {
            #region Current Scene Actors
            GUILayout.BeginArea(new Rect(480, 15, leftPanelWidth, 300));
            GUILayout.BeginHorizontal();
            GUILayout.Box("Current Scene Actors", titleText, GUILayout.Width(175), GUILayout.Height(20));
            GUILayout.EndHorizontal();

            bool addActor = false;
            if(curDrama.actors == null)
            {
                curDrama.actors = new List<DramaActor>();
            }

            currentActorListScrollPos = GUILayout.BeginScrollView(actorListScrollPos, new GUIStyle("RL Background"), GUILayout.Width(180), GUILayout.Height(position.height - 450));
            if(curDrama.actors != null && curDrama.actors.Count > 0)
            {
                List<string> actorNames = new List<string>();

                for (int i = 0; i < curDramaStorage.actorStorage.Count; i++)
                {
                    actorNames.Add(curDramaStorage.actorStorage[i].actorName);
                }

                int removeThisIdx = -1;
                for (int i = 0; i < curDrama.actors.Count; i++)
                {
                    bool isRemoved = false;
                    int selectedActor = (curDramaStorage.actorStorage != null && curDramaStorage.actorStorage.Count > 0) ? curDramaStorage.actorStorage.Count - 1 : -1;
                    EditorGUILayout.BeginHorizontal();
                    if(curDramaStorage.actorStorage.Find(x => x.actorName == curDrama.actors[i].characterName) != null)
                    {
                        selectedActor = curDramaStorage.actorStorage.FindIndex(x => x.actorName == curDrama.actors[i].characterName);
                    }
                    if(selectedActor >= 0)
                    {
                        selectedActor = EditorGUILayout.Popup(selectedActor, actorNames.ToArray(), GUILayout.MaxWidth(130));
                        isRemoved = GUILayout.Button("-", GUILayout.Width(30));
                        if(isRemoved)
                        {
                            removeThisIdx = i;
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("No Actor Available", GUILayout.MaxWidth(200));
                    }
                    if(curDrama.actors[i].characterName != curDramaStorage.actorStorage[selectedActor].actorName)
                    {
                        curDrama.actors[i].characterName = curDramaStorage.actorStorage[selectedActor].actorName;
                        curDrama.actors[i].characterPrefabPath = curDramaStorage.actorStorage[selectedActor].prefabPath;
                        curDrama.actors[i].actorType = curDramaStorage.actorStorage[selectedActor].actorType;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if(removeThisIdx != -1)
                {
                    curDrama.actors.RemoveAt(removeThisIdx);
                }
            }

            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            addActor= GUILayout.Button("+" , GUILayout.MaxWidth(50));
            GUILayout.EndHorizontal();
            if(addActor)
            {
                DramaActor newActor = new DramaActor();
                newActor.characterName = curDramaStorage.actorStorage[0].actorName;
                newActor.characterPrefabPath = curDramaStorage.actorStorage[0].prefabPath;
                newActor.actorType = curDramaStorage.actorStorage[0].actorType;

                curDrama.actors.Add(newActor);
            }
            GUILayout.EndArea();
            #endregion

            GUILayout.BeginArea(new Rect(15, 15, leftPanelWidth, 300));

            if(curFrame == null)
            {
                curFrame = new DramaFrame();
            }
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scenario Act Name:", EditorStyles.boldLabel, GUILayout.Width(80));
            curDrama.scenarioName = EditorGUILayout.TextField(curDrama.scenarioName, GUILayout.MaxWidth(380));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Drama Type:", EditorStyles.boldLabel, GUILayout.Width(80));
            curDrama.sceneType = (DramaSceneType)EditorGUILayout.EnumPopup(curDrama.sceneType, GUILayout.MaxWidth(200));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Box("Current Frame", titleText, GUILayout.Width(450), GUILayout.Height(20));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Description:", EditorStyles.boldLabel, GUILayout.Width(80));
            curFrame.description = EditorGUILayout.TextField(curFrame.description, GUILayout.MaxWidth(367));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (curDrama != null && curDrama.actionsPerFrame != null)
            {
                if (selectedFrameIdx >= curDrama.actionsPerFrame.Count - 1)
                {
                    curFrame.callNextStory = EditorGUILayout.Toggle(curFrame.callNextStory, GUILayout.MaxWidth(20));
                    GUILayout.Label("Call Next Story:", EditorStyles.boldLabel, GUILayout.Width(100));
                    if(curFrame.callNextStory)
                    {
                        curFrame.nextDramaTitle = EditorGUILayout.TextField(curFrame.nextDramaTitle, GUILayout.MaxWidth(325));
                    }
                }
                else
                {
                    curFrame.callNextStory = false;
                    curFrame.nextDramaTitle = "";
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Box("Frame List", titleText, GUILayout.Width(450), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            frameListScrollPos = GUILayout.BeginScrollView(frameListScrollPos, new GUIStyle("RL Background"), GUILayout.Width(450), GUILayout.Height(position.height - 400));
            if(curDrama.actionsPerFrame == null)
            {
                curDrama.actionsPerFrame = new List<DramaFrame>();
            }
            if(curDrama.actionsPerFrame != null && curDrama.actionsPerFrame.Count> 0)
            {
                for (int i = 0; i < curDrama.actionsPerFrame.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    bool isClicked = false;
                    isClicked = GUILayout.Button(curDrama.actionsPerFrame[i].frameName, (selectedFrame != null && curDrama.actionsPerFrame[i].frameName == selectedFrame.frameName) ? selectedText : notSelectedText);
                    GUILayout.Label("- " +curDrama.actionsPerFrame[i].description, GUILayout.MaxWidth(350));
                    GUILayout.EndHorizontal();

                    if(isClicked)
                    {
                        GUI.FocusControl(null);
                        selectedFrameIdx = i;
                        selectedFrame = curDrama.actionsPerFrame[selectedFrameIdx];
                        curFrame = selectedFrame;
                        curAction = null;
                        selectedAction = null;
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(15, 320, leftPanelWidth, 300));
            GUILayout.BeginHorizontal();
            bool saveCurrentFrame = GUILayout.Button((curFrame != null && curFrame == selectedFrame) ? "Modify" : "Save", GUILayout.MaxWidth(100));
            bool addNewFrame = GUILayout.Button("Create New", GUILayout.MaxWidth(100));
            if(saveCurrentFrame)
            {
                GUI.FocusControl(null);
                if(string.IsNullOrEmpty(curFrame.description))
                {
                    return;
                }
                if (curFrame == selectedFrame)
                {
                    int idx = curDrama.actionsPerFrame.FindIndex(x => x.frameName == curFrame.frameName);
                    curDrama.actionsPerFrame[idx] = curFrame;
                }
                else
                {
                    curFrame.frameName = "Frame#" + (curDrama.actionsPerFrame.Count + 1);
                    curDrama.actionsPerFrame.Add(curFrame);
                    curFrame = new DramaFrame();
                    selectedFrame = null;
                }
                Save();
            }
            if(addNewFrame)
            {
                GUI.FocusControl(null);
                curFrame = new DramaFrame();
                selectedFrame = null;
            }
            if(selectedFrame != null)
            {
                bool removeSelectedFrame = GUILayout.Button("-", GUILayout.MaxWidth(100));
                if(removeSelectedFrame)
                {
                    GUI.FocusControl(null);
                    if(curDrama.actionsPerFrame.Find(x => x.frameName == selectedFrame.frameName) != null)
                    {
                        curDrama.actionsPerFrame.Remove(selectedFrame);
                        selectedFrame = null;
                        curFrame = new DramaFrame();
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public void ShowActionList()
        {
            GUILayout.BeginArea(new Rect(480, actorListPosY + 50, 400, position.height - 350));

            GUILayout.BeginHorizontal();
            GUILayout.Box("Current Action List", titleText, GUILayout.Width(175), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            if(curFrame.actionsOnFrameList == null)
            {
                curFrame.actionsOnFrameList = new List<DramaAction>();
            }
            actionListScrollPos = GUILayout.BeginScrollView(actionListScrollPos, new GUIStyle("RL Background"), GUILayout.Width(180), GUILayout.Height(position.height - 450));
            if(selectedFrame != null && curFrame.actionsOnFrameList != null && curFrame.actionsOnFrameList.Count > 0)
            {
                int removeThisIdx = -1;
                for (int i = 0; i < curFrame.actionsOnFrameList.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    bool isClicked = false;
                    bool isRemoved = false;

                    if(curFrame.actionsOnFrameList[i].thisActor == null)
                    {
                        curFrame.actionsOnFrameList[i].thisActor = new DramaActor();
                        curFrame.actionsOnFrameList[i].thisActor.characterName = curDrama.actors[0].characterName;
                        curFrame.actionsOnFrameList[i].thisActor.characterPrefabPath = curDrama.actors[0].characterPrefabPath;
                        curFrame.actionsOnFrameList[i].thisActor.actorType = curDrama.actors[0].actorType;

                    }
                    string btnTitle = "";
                    switch (curFrame.actionsOnFrameList[i].actionType)
                    {
                        case DramaActionType.MakeActorMove:
                            btnTitle = "[" + curFrame.actionsOnFrameList[i].thisActor.characterName + "] Moving";
                            break;
                        case DramaActionType.ShowConversation:
                            btnTitle = "Start Conversation";
                            break;
                        case DramaActionType.BanishActor:
                            btnTitle = "[" + curFrame.actionsOnFrameList[i].thisActor.characterName + "] Banish Actor";
                            break;
                        case DramaActionType.ShowTabCover:
                            btnTitle = "Show Tab Cover";
                            break;
                        case DramaActionType.HideTabCover:
                            btnTitle = "Hide Tab Cover";
                            break;
                        case DramaActionType.ShowActor:
                            btnTitle = "[" + curFrame.actionsOnFrameList[i].thisActor.characterName + "] Show Actor";
                            break;
                        case DramaActionType.FadeToDark:
                            btnTitle = "Fade To Dark";
                            break;
                        case DramaActionType.FadeToClear:
                            btnTitle = "Fade To Clear";
                            break;
                        case DramaActionType.LoadThisScene:
                            btnTitle = "[Load]" + curFrame.actionsOnFrameList[i].loadThisScene.ToString();
                            break;
                        default:
                            break;
                    }

                    isClicked = GUILayout.Button(btnTitle, (selectedAction != null && curFrame.actionsOnFrameList[i] == selectedAction) ? selectedText : notSelectedText);
                    isRemoved = GUILayout.Button("-", GUILayout.MaxWidth(50), GUILayout.MaxHeight(15));
                    GUILayout.EndHorizontal();
                    if(isClicked)
                    {
                        easyPlacePosition = null;
                        selectedActionIdx = i;
                        selectedAction = selectedFrame.actionsOnFrameList[selectedActionIdx];
                        curAction = selectedAction;
                        isClicked = false;
                    }
                    if(isRemoved)
                    {
                        removeThisIdx = i;
                        isRemoved = false;
                    }
                }

                if(removeThisIdx != -1)
                {
                    curFrame.actionsOnFrameList.RemoveAt(removeThisIdx);
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(480, actorListPosY + 230, 400, position.height - 350));
            bool addNewAction = GUILayout.Button("+", GUILayout.MaxWidth(50));

            if(addNewAction)
            {
                DramaAction tmp = new DramaAction();
                curFrame.actionsOnFrameList.Add(tmp);
            }

            GUILayout.EndArea();
        }

        public void ShowCurrentAction()
        {
            GUILayout.BeginArea(new Rect(15, actorListPosY + 50, 650, position.height - 350));

                GUILayout.BeginHorizontal();
                GUILayout.Box("Action Information", titleText, GUILayout.Width(450), GUILayout.Height(20));
                GUILayout.EndHorizontal();

            if(curAction == null)
            {
                GUILayout.Label("Pick Action First", GUILayout.Width(300));
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Action Type:", GUILayout.Width(75));
                curAction.actionType = (DramaActionType)EditorGUILayout.EnumPopup(curAction.actionType, GUILayout.MaxWidth(120));
                GUILayout.Label("Start After(Seconds):", GUILayout.Width(120));
                curAction.delayBeforeStart = EditorGUILayout.FloatField(curAction.delayBeforeStart, GUILayout.MaxWidth(30));
                GUILayout.Label("Stay On Act:", GUILayout.Width(73));
                curAction.stayOnLastState = EditorGUILayout.Toggle(curAction.stayOnLastState, GUILayout.MaxWidth(18));

                GUILayout.EndHorizontal();
                if (curAction.actionType == DramaActionType.MakeActorMove)
                {
                    List<string> actorNames = new List<string>();
                    if (curDrama.actors != null && curDrama.actors.Count > 0)
                    {

                        for (int i = 0; i < curDrama.actors.Count; i++)
                        {
                            actorNames.Add(curDrama.actors[i].characterName);
                        }
                    }

                    if (curAction.thisActor == null)
                    {
                        curAction.thisActor = new DramaActor();
                        curAction.thisActor = curDrama.actors[0];
                    }

                    int selectedActorActionIdx = 0;
                    if (curDrama.actors != null && curDrama.actors.Count > 0)
                    {
                        selectedActorActionIdx = (curAction.thisActor != null) ? curDrama.actors.FindIndex(x => x.characterName == curAction.thisActor.characterName) : 0;
                    }
                    else
                    {
                        selectedActorActionIdx = -1;
                    }

                    GUILayout.BeginHorizontal();
                    if (selectedActorActionIdx == -1)
                    {
                        GUILayout.Label("No Actors Available", GUILayout.MaxWidth(300));
                    }
                    else
                    {
                        GUILayout.Label("Actor:", GUILayout.MaxWidth(40));
                        selectedActorActionIdx = EditorGUILayout.Popup(selectedActorActionIdx, actorNames.ToArray(), GUILayout.MaxWidth(160));
                        GUILayout.Label("Read Position:", GUILayout.MaxWidth(85));
                        easyPlacePosition = (GameObject)EditorGUILayout.ObjectField(easyPlacePosition, typeof(Object), true, GUILayout.MaxWidth(112));
                        if (curAction.thisActor.characterName != curDrama.actors[selectedActorActionIdx].characterName)
                        {
                            curAction.thisActor.characterName = curDrama.actors[selectedActorActionIdx].characterName;
                            curAction.thisActor.characterPrefabPath = curDrama.actors[selectedActorActionIdx].characterPrefabPath;
                            curAction.thisActor.actorType = curDrama.actors[selectedActorActionIdx].actorType;

                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Box("Actor Positions FACING GUIDE: UP[0] DOWN[1] RIGHT[2] LEFT[3]", titleText, GUILayout.Width(400), GUILayout.Height(20));
                    bool addNewPosition = GUILayout.Button("+", GUILayout.MaxWidth(50), GUILayout.MaxHeight(17));
                    if (addNewPosition)
                    {
                        if (curAction.actorsPosition == null)
                        {
                            curAction.actorsPosition = new List<Vector3>();
                        }
                        if (curAction.characterStates == null)
                        {
                            curAction.characterStates = new List<CharacterStates>();
                        }
                        curAction.actorsPosition.Add(new Vector2());
                        curAction.characterStates.Add(new CharacterStates());
                        curAction.facingDirection.Add(0);
                    }
                    GUILayout.EndHorizontal();

                    if(curAction.facingDirection == null)
                    {
                        curAction.facingDirection = new List<float>();
                        for (int i = 0; i < curAction.actorsPosition.Count; i++)
                        {
                            curAction.facingDirection.Add(0);
                        }
                    }
                    if (curAction.actorsPosition != null && curAction.actorsPosition.Count > 0)
                    {
                        int actionToRemove = -1;
                        for (int i = 0; i < curAction.actorsPosition.Count; i++)
                        {
                            curAction.facingDirection.Add(0.0f);
                            GUILayout.BeginHorizontal();

                            float x = curAction.actorsPosition[i].x;
                            float y = curAction.actorsPosition[i].y;
                            float z = curAction.actorsPosition[i].z;

                            GUILayout.Label("x:", GUILayout.MaxWidth(15));
                            x = EditorGUILayout.FloatField(x, GUILayout.MaxWidth(45));
                            GUILayout.Label("y:", GUILayout.MaxWidth(15));
                            y = EditorGUILayout.FloatField(y, GUILayout.MaxWidth(45));
                            GUILayout.Label("z:", GUILayout.MaxWidth(15));
                            z = EditorGUILayout.FloatField(z, GUILayout.MaxWidth(45));

                            // CHARACTER STATE
                            curAction.actorsPosition[i] = new Vector3(x, y, z);

                            curAction.characterStates[i] = (CharacterStates)EditorGUILayout.EnumPopup(curAction.characterStates[i], GUILayout.MaxWidth(70));

                            // FACING DIRECTION
                            if (curAction.facingDirection == null)
                            {
                                curAction.facingDirection = new List<float>();
                            }
                            if(curAction.facingDirection.Count-1 < i)
                            {
                                curAction.facingDirection.Add(0);
                            }
                            else
                            {
                                GUILayout.Label("Face:", GUILayout.MaxWidth(40));
                                curAction.facingDirection[i] = EditorGUILayout.FloatField(curAction.facingDirection[i], GUILayout.MaxWidth(20));
                            }

                            // QUICKLY PLACE POSITION OF ACTOR
                            bool readQuickCharacter = GUILayout.Button((easyPlacePosition == null) ? "[No Guide]" : "Read Guide", GUILayout.Width(70));
                            if (readQuickCharacter)
                            {
                                curAction.actorsPosition[i] = easyPlacePosition.transform.position;
                            }
                            bool removeAction = GUILayout.Button("-", GUILayout.Width(30));
                            if (removeAction)
                            {
                                actionToRemove = i;
                            }
                            GUILayout.EndHorizontal();
                        }
                        if (actionToRemove != -1)
                        {
                            curAction.actorsPosition.RemoveAt(actionToRemove);
                        }

                    }
                }
                else if (curAction.actionType == DramaActionType.ShowConversation)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Conversation Title:", GUILayout.MaxWidth(120));
                    curAction.conversationTitle = EditorGUILayout.TextField(curAction.conversationTitle, GUILayout.MaxWidth(220));
                    GUILayout.Label("Line Actions:", GUILayout.MaxWidth(80));
                    curAction.hasLineActions = EditorGUILayout.Toggle(curAction.hasLineActions, GUILayout.MaxWidth(20));
                    GUILayout.EndHorizontal();
                }

                else if(curAction.actionType == DramaActionType.BanishActor)
                {
                    List<string> actorNames = new List<string>();
                    if (curDrama.actors != null && curDrama.actors.Count > 0)
                    {

                        for (int i = 0; i < curDrama.actors.Count; i++)
                        {
                            if(curDrama.actors[i].actorType == DramaActorType.Generic || curDrama.actors[i].actorType == DramaActorType.Unique)
                            {
                                actorNames.Add(curDrama.actors[i].characterName);
                            }
                        }
                    }


                    GUILayout.BeginHorizontal();
                    if (selectedActorActionIdx == -1)
                    {
                        GUILayout.Label("No Actors Available", GUILayout.MaxWidth(300));
                    }
                    else
                    {
                        GUILayout.Label("Actor:", GUILayout.MaxWidth(40));
                        selectedActorActionIdx = EditorGUILayout.Popup(selectedActorActionIdx, actorNames.ToArray(), GUILayout.MaxWidth(160));
                    }
                    GUILayout.EndHorizontal();

                    if (curAction.thisActor.characterName != curDrama.actors[selectedActorActionIdx].characterName)
                    {
                        curAction.thisActor.characterName = curDrama.actors[selectedActorActionIdx].characterName;
                        curAction.thisActor.characterPrefabPath = curDrama.actors[selectedActorActionIdx].characterPrefabPath;
                        curAction.thisActor.actorType = curDrama.actors[selectedActorActionIdx].actorType;

                    }
                }
                else if (curAction.actionType == DramaActionType.ShowActor)
                {
                    List<string> actorNames = new List<string>();
                    if (curDrama.actors != null && curDrama.actors.Count > 0)
                    {

                        for (int i = 0; i < curDrama.actors.Count; i++)
                        {
                            if (curDrama.actors[i].actorType == DramaActorType.Generic || curDrama.actors[i].actorType == DramaActorType.Unique)
                            {
                                actorNames.Add(curDrama.actors[i].characterName);
                            }
                        }
                    }


                    GUILayout.BeginHorizontal();
                    if (selectedActorActionIdx == -1)
                    {
                        GUILayout.Label("No Actors Available", GUILayout.MaxWidth(300));
                    }
                    else
                    {
                        GUILayout.Label("Actor:", GUILayout.MaxWidth(40));
                        selectedActorActionIdx = EditorGUILayout.Popup(selectedActorActionIdx, actorNames.ToArray(), GUILayout.MaxWidth(160));
                    }
                    GUILayout.EndHorizontal();

                    if (curAction.thisActor.characterName != curDrama.actors[selectedActorActionIdx].characterName)
                    {
                        curAction.thisActor.characterName = curDrama.actors[selectedActorActionIdx].characterName;
                        curAction.thisActor.characterPrefabPath = curDrama.actors[selectedActorActionIdx].characterPrefabPath;
                        curAction.thisActor.actorType = curDrama.actors[selectedActorActionIdx].actorType;

                    }
                }
                else if(curAction.actionType == DramaActionType.FadeToDark || curAction.actionType == DramaActionType.FadeToClear)
                {

                }
                else if(curAction.actionType == DramaActionType.LoadThisScene)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Scene:", GUILayout.Width(50));
                    curAction.loadThisScene = (SceneType)EditorGUILayout.EnumPopup(curAction.loadThisScene, GUILayout.MaxWidth(180));
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndArea();
        }
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