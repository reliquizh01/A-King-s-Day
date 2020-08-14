using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kingdoms;
using Dialogue;

public class DialogueStorageEditor : EditorWindow
{
    #region EDITOR_SETUP
    private static Vector2 itemStorShowOn = new Vector2(1000, 600);
    private static Vector2 itemStorShowOff = new Vector2(1000, 600);
    private static int leftPanelWidth = 800;

    [MenuItem("Game Storage/Kingdom Dialogue Storage")]
    public static void ShowWindow()
    {
        GetWindow<DialogueStorageEditor>("Kingdom Dialogue Storage").minSize = itemStorShowOff;
        GetWindow<DialogueStorageEditor>("Kingdom Dialogue Storage").maxSize = itemStorShowOn;
    }
    #endregion

    public GameObject dialogueStoragePrefab;
    public GameObject curDialogueStorage;
    public bool dialogueStorageLoaded = false;
    public int curConversationIdx;
    public int selectedConversationIdx;
    public int selectedCharacterNameIdx;
    public int selectedPatternIdx;
    public int selectedName;

    public DialogueInformationStorage curDialogueStorageData;
    public ConversationInformationData selectedConversation;
    public ConversationInformationData curConversation;

    public Vector2 dialogueListScrollPos = Vector2.zero;
    public Vector2 characterListScrollPos = Vector2.zero;
    public Vector2 patternScrollPos = Vector2.zero;
    public Vector2 dialogueChoicesScrollPos = Vector2.zero;
    public GUIStyle titleText;
    public GUIStyle notSelectedText;
    public GUIStyle selectedText;

    public void Awake()
    {
        titleText = new GUIStyle((GUIStyle)"toolbarTextField");
        notSelectedText = new GUIStyle((GUIStyle)"toolbarTextField");
        selectedText = new GUIStyle((GUIStyle)"toolbarTextField");
    }

    public void Save()
    {
        PrefabUtility.SaveAsPrefabAsset(curDialogueStorage, "Assets/Resources/Prefabs/UI/Dialogue System/Dialogue Storage.prefab");
        DestroyImmediate(curDialogueStorage);
        dialogueStorageLoaded = false;
    }
    public void OnDestroy()
    {
        dialogueStorageLoaded = false;
        DestroyImmediate(curDialogueStorage);
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

    public void LoadDialogueStorage()
    {
        dialogueStoragePrefab = (GameObject)Resources.Load("Prefabs/UI/Dialogue System/Dialogue Storage");
        curDialogueStorage = (GameObject)Instantiate(dialogueStoragePrefab);


        if (curDialogueStorage == null)
        {
            Debug.LogWarning("Storage not found! Check Reference");
            return;
        }

        curDialogueStorageData = curDialogueStorage.GetComponent<DialogueInformationStorage>();
        dialogueStorageLoaded = true;
    }
    public void OnGUI()
    {
        if(!dialogueStorageLoaded)
        {
            LoadDialogueStorage();
        }
        else
        {
            ShowDialogueList();
            ShowCurrentConversation();
            ShowCurrentDialoguePattern();
        }
    }

    public void ShowDialogueList()
    {
        bool addEvent = false;
        bool saveEvent = false;
        bool removeEvent = false;

        GUILayout.BeginArea(new Rect(680, 10, 400, position.height - 375));
        GUILayout.BeginHorizontal();
        GUILayout.Box("Conversations", titleText, GUILayout.Width(295), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        dialogueListScrollPos = GUILayout.BeginScrollView(dialogueListScrollPos, new GUIStyle("RL Background"), GUILayout.Width(300), GUILayout.Height(position.height - 400));

        if (curDialogueStorageData.conversationList != null && curDialogueStorageData.conversationList.Count > 0)
        {
            for (int i = 0; i < curDialogueStorageData.conversationList.Count; i++)
            {
                bool isClicked = false;
                string listName = "";

                GUILayout.BeginHorizontal();
                isClicked = GUILayout.Button(curDialogueStorageData.conversationList[i].conversationTitle, (selectedConversation != null && curDialogueStorageData.conversationList[i].conversationTitle == selectedConversation.conversationTitle) ? selectedText : notSelectedText);
                if (!string.IsNullOrEmpty(curDialogueStorageData.conversationList[i].conversationTitle))
                {
                    if(curDialogueStorageData.conversationList[i].dialoguePattern != null &&
                        curDialogueStorageData.conversationList[i].dialoguePattern.Count > 0)
                    {
                        listName = "[Length: " + curDialogueStorageData.conversationList[i].dialoguePattern.Count + "]";
                    }
                    else
                    {
                        listName = "[Length: 0]";
                    }
                    GUILayout.Label(listName);
                }
                GUILayout.EndHorizontal();
                if (isClicked)
                {
                    if (curDialogueStorageData.conversationList[i] != null)
                    {
                        selectedConversation = curDialogueStorageData.conversationList[i];
                        selectedConversationIdx = i;
                        curConversation = selectedConversation;
                    }
                    isClicked = false;
                }
            }

        }

        GUILayout.EndArea();
        GUILayout.EndScrollView();

        GUILayout.BeginArea(new Rect(680, position.height - 360, 400, 225));

        GUILayout.BeginHorizontal();
        if(curConversation == null)
        {
            curConversation = new ConversationInformationData();
        }
        saveEvent = GUILayout.Button((curConversation == selectedConversation) ? "Modify" : "Save", GUILayout.MaxWidth(100));
        if (curDialogueStorageData.conversationList.Find(x => x.conversationTitle == curConversation.conversationTitle) != null)
        {
            addEvent = GUILayout.Button("Create New", GUILayout.MaxWidth(100));
        }
        if (curDialogueStorageData.conversationList.Find(x => x.conversationTitle == curConversation.conversationTitle) != null)
        {
            removeEvent = GUILayout.Button("Remove", GUILayout.MaxWidth(100));
        }
        GUILayout.EndHorizontal();

        GUILayout.EndArea();

        // ADD BUTTON
        if (addEvent)
        {
            curConversation = new ConversationInformationData();
            selectedConversation = null;
        }
        // REMOVE BUTTON
        if (removeEvent)
        {
            GUI.FocusControl(null);
            removeEvent = false;
            if (selectedConversation != null)
            {
                curDialogueStorageData.conversationList.RemoveAt(selectedConversationIdx);
                selectedConversation = null;
                curConversation = new ConversationInformationData();
            }
        }
        // SAVE BUTTON
        if (saveEvent && !string.IsNullOrEmpty(curConversation.conversationTitle))
        {
            if (curDialogueStorageData.conversationList == null)
            {
                curDialogueStorageData.conversationList = new List<ConversationInformationData>();
            }
            if (curDialogueStorageData.conversationList.Find(x => x.conversationTitle == curConversation.conversationTitle) == null)
            {
                curDialogueStorageData.conversationList.Add(curConversation);
                curConversation = new ConversationInformationData();
            }
            else
            {
                // MODIFY CURRENT EVENT
                if ((curConversation == selectedConversation))
                {
                    curDialogueStorageData.conversationList[selectedConversationIdx] = curConversation;
                    curConversation = new ConversationInformationData();
                    selectedConversation = null;
                }
                else
                {
                    Debug.LogError("MULTIPLE EVENTS WITH SAME TITLE OCCURRED, PLEACE CHECK LIST!");
                }
            }
            GUI.FocusControl(null);
            saveEvent = false;
            Save();
        }
    }
    public void ShowCurrentConversation()
    {
        bool SaveStorage = false;
        if (curConversation == null)
        {
            curConversation = new ConversationInformationData();
        }
        GUILayout.BeginArea(new Rect(5, 12, leftPanelWidth, 300));

        // Conversation Title
        GUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 34;
        EditorGUILayout.LabelField("Title:", EditorStyles.boldLabel, GUILayout.Width(75));
        curConversation.conversationTitle = EditorGUILayout.TextField(curConversation.conversationTitle, GUILayout.MaxWidth(300));
        int maxIdx = (curConversation.dialoguePattern != null && curConversation.dialoguePattern.Count > 0) ? curConversation.dialoguePattern.Count : 0;
        EditorGUILayout.LabelField("Index:" + selectedPatternIdx + "/" + maxIdx, GUILayout.MaxWidth(100));
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(5, 32, 150, 300));
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Characters:", EditorStyles.boldLabel, GUILayout.Width(75));
        if(curConversation.characterNames != null && selectedCharacterNameIdx < curConversation.characterNames.Count)
        {
            EditorGUI.BeginChangeCheck();
            string tmp = curConversation.characterNames[selectedCharacterNameIdx];
            tmp = EditorGUILayout.TextField(curConversation.characterNames[selectedCharacterNameIdx], GUILayout.MaxWidth(120));

            if(EditorGUI.EndChangeCheck())
            {
                if(tmp != curConversation.characterNames[selectedCharacterNameIdx])
                {
                    UpdateNamesOnPatterns(tmp);
                    curConversation.characterNames[selectedCharacterNameIdx] = tmp;
                }
            }
        }
        GUILayout.EndHorizontal();

        characterListScrollPos = GUILayout.BeginScrollView(characterListScrollPos, new GUIStyle("RL Background"), GUILayout.Width(300), GUILayout.Height(position.height - 400));
        if (curConversation.characterNames != null && curConversation.characterNames.Count > 0)
        {
            int removedNames = -1;
            for (int i = 0; i < curConversation.characterNames.Count; i++)
            {
                bool isClicked = false;
                bool isRemoved = false;
                GUILayout.BeginHorizontal();
                isClicked = GUILayout.Button(curConversation.characterNames[i], (selectedCharacterNameIdx == i) ? selectedText : notSelectedText, GUILayout.MinWidth(90));
                isRemoved = GUILayout.Button("-", GUILayout.MaxWidth(50));
                GUILayout.EndHorizontal();

                if(isClicked)
                {
                    selectedCharacterNameIdx = i;
                    GUI.FocusControl(null);
                    isClicked = false;
                }

                if(isRemoved)
                {
                    removedNames = i;
                    isRemoved = false;
                }
            }

            if(removedNames != -1)
            {
                curConversation.characterNames.RemoveAt(removedNames);
            }
        }
        GUILayout.EndScrollView();

        bool addName = GUILayout.Button("+", GUILayout.MaxWidth(50));
        if (addName)
        {
            if(curConversation.characterNames == null)
            {
                curConversation.characterNames = new List<string>();
            }
            string newName = "New Name";
            curConversation.characterNames.Add(newName);
            GUI.FocusControl(null);
        }
        GUILayout.EndArea();

        // Dialogue List
        GUILayout.BeginArea(new Rect(510, 32, 150, 300));
        GUILayout.Box("Dialogue Patterns", titleText, GUILayout.Width(150), GUILayout.Height(20));
        patternScrollPos = GUILayout.BeginScrollView(patternScrollPos, new GUIStyle("RL Background"), GUILayout.Width(300), GUILayout.Height(position.height - 400));
        if(curConversation.dialoguePattern != null && curConversation.dialoguePattern.Count > 0)
        {
            int removeDialogue = -1;
            for (int i = 0; i < curConversation.dialoguePattern.Count; i++)
            {
                {
                    bool isClick = false;
                    bool isRemove = false;
                    GUILayout.BeginHorizontal();
                    isClick = GUILayout.Button(curConversation.dialoguePattern[i].charName, (selectedPatternIdx == i) ? selectedText : notSelectedText, GUILayout.MinWidth(90));
                    isRemove = GUILayout.Button("-", GUILayout.MaxWidth(50));
                    GUILayout.EndHorizontal();

                    if (isClick)
                    {
                        selectedPatternIdx = i;

                        isClick = false;
                        GUI.FocusControl(null);
                    }

                    if (isRemove)
                    {
                        removeDialogue = i;
                        isRemove = false;
                    }
                }
            }
            if(removeDialogue != -1)
            {
                curConversation.dialoguePattern.RemoveAt(removeDialogue);
            }
        }
        GUILayout.EndScrollView();
        if(curConversation.characterNames != null && curConversation.characterNames.Count > 0)
        {
            bool addDialogue = GUILayout.Button("+", GUILayout.MaxWidth(50));

            if(addDialogue)
            {
                if(curConversation.dialoguePattern == null)
                {
                    curConversation.dialoguePattern = new List<DialogueBy>();
                }

                DialogueBy tmp = new DialogueBy();
                if(curConversation.characterNames != null && curConversation.characterNames.Count > 0)
                {   
                    tmp.charName = curConversation.characterNames[0];
                }
                else
                {
                    tmp.charName = "[New]";
                }

                curConversation.dialoguePattern.Add(tmp);
                GUI.FocusControl(null);
            }
        }
        GUILayout.EndArea();
    }
    public void ShowCurrentDialoguePattern()
    {
        if(curConversation.dialoguePattern == null)
        {
            curConversation.dialoguePattern = new List<DialogueBy>();
        }

        if(selectedPatternIdx >= curConversation.dialoguePattern.Count)
        {
            return;
        }

        GUILayout.BeginArea(new Rect(160, 32, 400, 300));
        if(curConversation.characterNames != null && curConversation.characterNames.Count > 0)
        {
            List<string> charNames = curConversation.characterNames;

            selectedName = curConversation.characterNames.FindIndex(x => x == curConversation.dialoguePattern[selectedPatternIdx].charName);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Character Name: ", GUILayout.MaxWidth(100));
            selectedName = EditorGUILayout.Popup(selectedName, charNames.ToArray(), GUILayout.MaxWidth(180));
            if(charNames[selectedName] != curConversation.dialoguePattern[selectedPatternIdx].charName)
            {
                curConversation.dialoguePattern[selectedPatternIdx].charName = charNames[selectedName];
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Dialogue Type: ", GUILayout.MaxWidth(100));
            curConversation.dialoguePattern[selectedPatternIdx].type = (DialogueType)EditorGUILayout.EnumPopup(curConversation.dialoguePattern[selectedPatternIdx].type, GUILayout.MaxWidth(80));
            GUILayout.Label("Reaction: ", GUILayout.MaxWidth(60));
            curConversation.dialoguePattern[selectedPatternIdx].reaction = (DialogueReaction)EditorGUILayout.EnumPopup(curConversation.dialoguePattern[selectedPatternIdx].reaction, GUILayout.MaxWidth(80));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Box(curConversation.dialoguePattern[selectedPatternIdx].type.ToString(), titleText, GUILayout.Width(295), GUILayout.Height(20));
            if(curConversation.dialoguePattern[selectedPatternIdx].type == DialogueType.Choices)
            {
                bool newChoice = GUILayout.Button("+", GUILayout.MaxWidth(50));
                GUILayout.EndHorizontal();

                if(newChoice)
                {
                    if (curConversation.dialoguePattern[selectedPatternIdx].choiceDialogues == null)
                    {
                        curConversation.dialoguePattern[selectedPatternIdx].choiceDialogues = new List<string>();
                    }
                    string tmp = " ";

                    curConversation.dialoguePattern[selectedPatternIdx].choiceDialogues.Add(tmp);
                }

                dialogueChoicesScrollPos = GUILayout.BeginScrollView(dialogueListScrollPos, new GUIStyle("RL Background"), GUILayout.Width(340), GUILayout.Height(position.height - 442));
                int removeChoiceIdx = -1;

                if(curConversation.dialoguePattern[selectedPatternIdx].choiceDialogues == null)
                {
                    curConversation.dialoguePattern[selectedPatternIdx].choiceDialogues = new List<string>();
                }
                for (int i = 0; i < curConversation.dialoguePattern[selectedPatternIdx].choiceDialogues.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    curConversation.dialoguePattern[selectedPatternIdx].choiceDialogues[i] =
                        EditorGUILayout.TextField(curConversation.dialoguePattern[selectedPatternIdx].choiceDialogues[i], GUILayout.MaxWidth(290));

                    bool removeChoice = GUILayout.Button("-", GUILayout.MaxWidth(50));

                    if(removeChoice)
                    {
                        removeChoiceIdx = i;
                    }
                    GUILayout.EndHorizontal();
                }
                if(removeChoiceIdx != -1)
                {
                    curConversation.dialoguePattern[selectedPatternIdx].choiceDialogues.RemoveAt(removeChoiceIdx);
                }

                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                curConversation.dialoguePattern[selectedPatternIdx].single = EditorGUILayout.TextField(curConversation.dialoguePattern[selectedPatternIdx].single, GUILayout.MaxWidth(290));
                GUILayout.EndHorizontal();
            }

        }
        GUILayout.EndArea();
    }


    public void UpdateNamesOnPatterns(string newName)
    {
        if(curConversation.dialoguePattern == null || curConversation.dialoguePattern.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < curConversation.dialoguePattern.Count; i++)
        {
            if (curConversation.dialoguePattern[i].charName == curConversation.characterNames[selectedCharacterNameIdx])
            {
                curConversation.dialoguePattern[i].charName = newName;
            }
        }
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
