using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Territory;

public class TerritoryRequirementsEditor : EditorWindow
{
    public GameObject territoryHandlerPrefab;
    public GameObject curTerritoryhandler;
    public TerritoryHandler currentTerritoryHandlerData;
    public bool curHandlerLoaded = false;

    public bool addRequirement = false;

    public Vector2 townReqScrollPos = Vector2.zero;
    #region EDITOR_SETUP
    private static Vector2 itemStorShowOn = new Vector2(400, 600);
    private static Vector2 itemStorShowOff = new Vector2(400, 600);

    [MenuItem("Game Storage/Territory Requirements")]
    public static void ShowWindow()
    {
        GetWindow<TerritoryRequirementsEditor>("Territory Requirements").maxSize = itemStorShowOn;
        GetWindow<TerritoryRequirementsEditor>("Territory Requirements").minSize = itemStorShowOff;
    }
    #endregion

    public void OnGUI()
    {
        if(!curHandlerLoaded)
        {
            Initialize();
        }
        else if(curHandlerLoaded)
        {
            ShowCurrentRequirements();
        }
    }
    public void OnDestroy()
    {
        curHandlerLoaded = false;
        DestroyImmediate(curTerritoryhandler);
    }
    public void Initialize()
    {
        territoryHandlerPrefab = (GameObject)Resources.Load("Prefabs/Territory Events Handler");
        curTerritoryhandler = (GameObject)Instantiate(territoryHandlerPrefab);

        if(curTerritoryhandler == null)
        {
            Debug.Log("Check Reference");
            return;
        }

        currentTerritoryHandlerData = curTerritoryhandler.GetComponent<TerritoryHandler>();
        curHandlerLoaded = true;
    }
    public void ShowCurrentRequirements()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 400));

        GUILayout.BeginHorizontal();
        GUILayout.Label("Town Requirements", GUILayout.MaxWidth(120));
        bool add = GUILayout.Button("+", GUILayout.MaxWidth(50));

        if(add)
        {
            currentTerritoryHandlerData.requirements.Add(new TerritoryRequirements());
        }
        GUILayout.EndHorizontal();
        townReqScrollPos = EditorGUILayout.BeginScrollView(townReqScrollPos, new GUIStyle("RL Background"), GUILayout.Width(300), GUILayout.Height(300));

        if(currentTerritoryHandlerData.requirements != null && currentTerritoryHandlerData.requirements.Count > 0)
        {
            for (int i = 0; i < currentTerritoryHandlerData.requirements.Count; i++)
            {
                bool remove = false;
                GUILayout.BeginHorizontal();
                remove = GUILayout.Button("-", GUILayout.MaxWidth(50));
                GUILayout.Label("Rank:");
                currentTerritoryHandlerData.requirements[i].thisLevel = (TerritoryLevel)EditorGUILayout.EnumPopup(currentTerritoryHandlerData.requirements[i].thisLevel, GUILayout.MaxWidth(150));
                GUILayout.EndHorizontal();
                currentTerritoryHandlerData.requirements[i].foodReq = EditorGUILayout.IntField("Food:",currentTerritoryHandlerData.requirements[i].foodReq, GUILayout.MaxWidth(350));
                currentTerritoryHandlerData.requirements[i].troopReq = EditorGUILayout.IntField("Troops:", currentTerritoryHandlerData.requirements[i].troopReq, GUILayout.MaxWidth(350));
                currentTerritoryHandlerData.requirements[i].coinReq = EditorGUILayout.IntField("Coins:", currentTerritoryHandlerData.requirements[i].coinReq, GUILayout.MaxWidth(350));
                currentTerritoryHandlerData.requirements[i].populationReq= EditorGUILayout.IntField("Population:", currentTerritoryHandlerData.requirements[i].populationReq, GUILayout.MaxWidth(350));

                if(remove)
                {
                    currentTerritoryHandlerData.requirements.RemoveAt(i);
                    remove = false;
                }
            }
        }

        EditorGUILayout.EndScrollView();
        bool save = GUILayout.Button("SAVE", GUILayout.MaxWidth(70));

        if(save)
        {
            Save();
        }
        GUILayout.EndArea();
    }

    public void Save()
    {
        PrefabUtility.SaveAsPrefabAsset(curTerritoryhandler, "Assets/Resources/Prefabs/Territory Events Handler.prefab");
        DestroyImmediate(curTerritoryhandler);
        curHandlerLoaded= false;
        Initialize();
    }
}
