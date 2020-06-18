using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Technology;
using Kingdoms;

public class TechnologyStorageEditor : EditorWindow
{
    #region EDITOR_SETUP
    private static Vector2 itemStorShowOn = new Vector2(1000, 600);
    private static Vector2 itemStorShowOff = new Vector2(1000, 600);
    private static int leftPanelWidth = 800;

    [MenuItem("Game Storage/Technology Storage")]
    public static void ShowWindow()
    {
        GetWindow<TechnologyStorageEditor>("Technology Storage").minSize = itemStorShowOff;
        GetWindow<TechnologyStorageEditor>("Technology Storage").maxSize = itemStorShowOn;
    }
    #endregion
    public GUIStyle notSelectedText;
    public GUIStyle selectedText;

    public GameObject technologyStoragePrefab;
    public GameObject curTechnologyStorage;
    public bool techStorageLoaded = false;
    public int curTechIdx;
    public int selectedTechIdx;

    public KingdomTechnologyStorage curTechStorageData;
    public BaseTechnology currentTechnology;

    Vector2 coinRequirementList = Vector2.zero;
    Vector2 techStorageList = Vector2.zero;

    public void Awake()
    {
        notSelectedText = new GUIStyle((GUIStyle)"toolbarTextField");
        selectedText = new GUIStyle((GUIStyle)"toolbarTextField");
    }

    public void OnEnable()
    {

        notSelectedText.normal.textColor = Color.black;
        selectedText.normal.background = MakeTex(700, 18, new Color(0, 0, 0, 0));

        selectedText.normal.textColor = Color.white;
        selectedText.normal.background = MakeTex(700, 18, Color.grey);
    }
    public void OnGUI()
    {
        if(!techStorageLoaded)
        {
            LoadTechStorage();
        }
        else if(techStorageLoaded)
        {
            ShowTechStorage();
            ShowTechStorageList();
        }
    }

    public void Save()
    {
        PrefabUtility.SaveAsPrefabAsset(curTechnologyStorage, "Assets/Resources/Prefabs/Technology/Tech Storage.prefab");
        DestroyImmediate(curTechnologyStorage);
        techStorageLoaded = false;
        LoadTechStorage();
    }
    public void OnDestroy()
    {
        techStorageLoaded = false;
        DestroyImmediate(curTechnologyStorage);
    }


    public void LoadTechStorage()
    {

        technologyStoragePrefab = (GameObject)Resources.Load("Prefabs/Technology/Tech Storage");
        curTechnologyStorage = (GameObject)Instantiate(technologyStoragePrefab);

        if(curTechnologyStorage == null)
        {
            Debug.LogWarning("Storage not found! Check Reference");
            return;
        }

        curTechStorageData = curTechnologyStorage.GetComponent<KingdomTechnologyStorage>();
        currentTechnology = new BaseTechnology();
        techStorageLoaded = true;
    }
    public void ShowTechStorage()
    {
        bool SaveStorage = false;
        int levelLength = 0;
        Rect spriteRect = new Rect(355, 0, 75, 75);


        GUILayout.BeginArea(new Rect(5, 12, leftPanelWidth, 300));

        GUILayout.ExpandWidth(false);
        EditorStyles.textField.wordWrap = true;

        //Title
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name:", EditorStyles.boldLabel, GUILayout.Width(40));
        currentTechnology.technologyName = EditorGUILayout.TextField(currentTechnology.technologyName, GUILayout.MaxWidth(300));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        currentTechnology.improvedType = (ResourceType)EditorGUILayout.EnumPopup(currentTechnology.improvedType, GUILayout.MaxWidth(70));
        EditorGUILayout.LabelField("Increment:", GUILayout.MaxWidth(70));
        currentTechnology.bonusIncrement = EditorGUILayout.IntField(currentTechnology.bonusIncrement, GUILayout.MaxWidth(70));
        EditorGUILayout.LabelField("Levels:", EditorStyles.boldLabel, GUILayout.Width(40));
        EditorGUI.BeginChangeCheck();
        levelLength = EditorGUILayout.IntField((currentTechnology.goldLevelRequirements != null) ? currentTechnology.goldLevelRequirements.Count : levelLength, GUILayout.MaxWidth(30));
        if(EditorGUI.EndChangeCheck())
        {
            if(currentTechnology.goldLevelRequirements == null)
            {
                currentTechnology.goldLevelRequirements = new List<int>();
            }
            if(levelLength > currentTechnology.goldLevelRequirements.Count)
            {
                int addCount = levelLength - currentTechnology.goldLevelRequirements.Count;
                for (int i = 0; i < addCount; i++)
                {
                    currentTechnology.goldLevelRequirements.Add(0);
                }
            }
            else if(levelLength < currentTechnology.goldLevelRequirements.Count)
            {
                int removeCount = currentTechnology.goldLevelRequirements.Count - levelLength;
                Debug.Log("Length:" + levelLength + " Count: " + currentTechnology.goldLevelRequirements.Count + " Remove Start Index: " + removeCount);

                currentTechnology.goldLevelRequirements.RemoveRange(levelLength-1, removeCount);
            }
        }
        currentTechnology.techIcon = (Sprite)EditorGUI.ObjectField(spriteRect, currentTechnology.techIcon, typeof(Sprite), false);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        ShowSpecializedType();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Effect Mesg:", GUILayout.MaxWidth(75));
        currentTechnology.effectMesg = EditorGUILayout.TextField(currentTechnology.effectMesg, GUILayout.MaxWidth(250));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Witty Mesg:", GUILayout.MaxWidth(75));
        currentTechnology.wittyMesg = EditorGUILayout.TextField(currentTechnology.wittyMesg, GUILayout.MaxWidth(250));
        GUILayout.EndHorizontal();
        // Setup level requirements
        if (currentTechnology.goldLevelRequirements == null)
        {
            currentTechnology.goldLevelRequirements = new List<int>();
            for (int i = 0; i < levelLength; i++)
            {
                currentTechnology.goldLevelRequirements.Add(0);
            }
        }

        coinRequirementList = EditorGUILayout.BeginScrollView(coinRequirementList, new GUIStyle("RL Background"), GUILayout.Width(350), GUILayout.Height(position.height - 450));
        for (int i = 0; i < currentTechnology.goldLevelRequirements.Count; i++)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("[Level : "+ (i+1) +"]Gold Requirement:", EditorStyles.boldLabel, GUILayout.Width(140));
            currentTechnology.goldLevelRequirements[i] = EditorGUILayout.IntField(currentTechnology.goldLevelRequirements[i], GUILayout.MaxWidth(100));
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        bool clicked = GUILayout.Button("ADD", GUILayout.Width(150));
        if(clicked)
        {
            AddCurrentTech();
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    public void ShowSpecializedType()
    {
        switch (currentTechnology.improvedType)
        {
            case ResourceType.Food:
                currentTechnology.foodTechType = (FoodTechType)EditorGUILayout.EnumPopup(currentTechnology.foodTechType, GUILayout.MaxWidth(170));
                break;
            case ResourceType.Troops:
                currentTechnology.troopTechType = (TroopTechType)EditorGUILayout.EnumPopup(currentTechnology.troopTechType, GUILayout.MaxWidth(170));
                break;
            case ResourceType.Population:
                currentTechnology.popTechType = (PopulationTechType)EditorGUILayout.EnumPopup(currentTechnology.popTechType, GUILayout.MaxWidth(170));
                break;
            case ResourceType.Coin:
                currentTechnology.coinTechType = (CoinTechType)EditorGUILayout.EnumPopup(currentTechnology.coinTechType, GUILayout.MaxWidth(170));
                break;
            case ResourceType.Cows:
                break;
            default:
                break;
        }
    }
    public void ShowTechStorageList()
    {
        GUILayout.BeginArea(new Rect(450, 12, leftPanelWidth, 300));

        EditorGUILayout.BeginVertical();
        techStorageList = EditorGUILayout.BeginScrollView(techStorageList, new GUIStyle("RL Background"), GUILayout.Width(250), GUILayout.Height(position.height - 350));
        for (int i = 0; i < curTechStorageData.technologies.Count; i++)
        {
            GUILayout.BeginHorizontal();
            bool isclicked = false;
            isclicked = GUILayout.Button(curTechStorageData.technologies[i].technologyName, (currentTechnology != null && curTechStorageData.technologies[i].technologyName == currentTechnology.technologyName) ? selectedText : notSelectedText);
            GUILayout.EndHorizontal();
            if(isclicked)
            {
                GUI.FocusControl(null);
                if (curTechStorageData.technologies[i] != null)
                {
                    currentTechnology = curTechStorageData.technologies[i];
                    curTechIdx = i;
                }
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        bool remove = GUILayout.Button("Remove", GUILayout.Width(120));
        if(remove)
        {
            if(currentTechnology.technologyName == curTechStorageData.technologies[curTechIdx].technologyName)
            {
                curTechStorageData.technologies.RemoveAt(curTechIdx);
                currentTechnology = new BaseTechnology();
            }
        }

        bool save = GUILayout.Button("Save", GUILayout.Width(120));
        if(save)
        {
            Save();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        GUILayout.EndArea();
    }
    public void AddCurrentTech()
    {
        if(curTechStorageData.technologies.Find(x => x.technologyName == currentTechnology.technologyName) != null)
        {
            int idx = curTechStorageData.technologies.FindIndex(x => x.technologyName == currentTechnology.technologyName);
            curTechStorageData.technologies[idx] = currentTechnology;
        }
        else
        {
            curTechStorageData.technologies.Add(currentTechnology);
        }

        currentTechnology = null;
        GUI.FocusControl(null);
        Save();
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
