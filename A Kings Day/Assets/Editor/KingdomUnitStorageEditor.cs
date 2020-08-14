using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Technology;
using Kingdoms;
using Characters;
using GameItems;

public class KingdomUnitStorageEditor : EditorWindow
{

    #region EDITOR_SETUP
    private static Vector2 itemStorShowOn = new Vector2(1400, 600);
    private static Vector2 itemStorShowOff = new Vector2(1400, 600);
    private static int leftPanelWidth = 800;

    [MenuItem("Game Storage/Kingdom Unit Storage")]
    public static void ShowWindow()
    {
        GetWindow<KingdomUnitStorageEditor>("Unit Storage").minSize = itemStorShowOff;
        GetWindow<KingdomUnitStorageEditor>("Unit Storage").maxSize = itemStorShowOn;
    }
    #endregion

    public GUIStyle titleText;
    public GUIStyle notSelectedText;
    public GUIStyle selectedText;


    public GameObject unitStoragePrefab;
    public GameObject curUnitStorage;
    public bool unitStorageLoaded;

    public KingdomUnitStorage curUnitStorageData;
    public int curUnitIdx;

    // HERO
    public BaseHeroInformationData selectedHeroData;
    public BaseHeroInformationData currentHeroData;
    public int selectedHeroIdx;
    Vector2 heroListScrollPos = Vector2.zero;

    // MERCHANT
    public BaseMerchantInformationData selectedMerchantData;
    public BaseMerchantInformationData currentMerchantData;
    public int selectedMerchantIdx;
    Vector2 merchantListScrollPos = Vector2.zero;
    // SKILLS
    public BaseSkillInformationData selectedSkillData;
    public BaseSkillInformationData currentSkillData;
    public int selectedSkillIdx;
    Vector2 skillListScrollPos = Vector2.zero;

    // BUFF
    public BaseBuffInformationData selectedBuffData;
    public BaseBuffInformationData currentBuffData;
    public int selectedBuffIdx;
    Vector2 buffListScrollPos = Vector2.zero;

    // Unit Storage
    public UnitInformationData selectedUnitData;
    public UnitInformationData currentUnitData;
    public Object currentGameObjectPrefab;
    public int selectedUnitIdx;
    Vector2 unitListScrollPos = Vector2.zero;

    public void Awake()
    {
        titleText = new GUIStyle((GUIStyle)"toolbarTextField");
        titleText.alignment = TextAnchor.UpperCenter;
        notSelectedText = new GUIStyle((GUIStyle)"toolbarTextField");
        selectedText = new GUIStyle((GUIStyle)"toolbarTextField");
    }

    public void OnEnable()
    {
        currentMerchantData = new BaseMerchantInformationData();
        currentHeroData = new BaseHeroInformationData();

        notSelectedText.normal.textColor = Color.black;
        selectedText.normal.background = MakeTex(700, 18, new Color(0, 0, 0, 0));

        selectedText.normal.textColor = Color.white;
        selectedText.normal.background = MakeTex(700, 18, Color.grey);
    }
    public void OnDestroy()
    {
        unitStorageLoaded = false;
        DestroyImmediate(curUnitStorage);
    }

    public void Save()
    {
        PrefabUtility.SaveAsPrefabAsset(curUnitStorage, "Assets/Resources/Prefabs/Unit and Items/Kingdom Unit Storage.prefab");
        DestroyImmediate(curUnitStorage);
        unitStorageLoaded = false;
        LoadUnitStorage();
    }

    public void OnGUI()
    {
        if(!unitStorageLoaded)
        {
            LoadUnitStorage();
        }
        else
        {
            ShowMerchantStorageList();
            ShowCurrentMerchantData();

            ShowHeroStorageList();
            ShowCurrentHeroData();

            ShowSkillData();
            ShowCurrentSkill();

            ShowBuffStorageList();
            ShowCurrentBuff();

            ShowUnitStorageList();
            ShowCurrentUnit();
        }
    }

    public void LoadUnitStorage()
    {
        unitStoragePrefab = (GameObject)Resources.Load("Prefabs/Unit and Items/Kingdom Unit Storage");
        curUnitStorage = (GameObject)Instantiate(unitStoragePrefab);

        if (curUnitStorage == null)
        {
            Debug.LogWarning("Storage not found! Check Reference");
            return;
        }

        curUnitStorageData = curUnitStorage.GetComponent<KingdomUnitStorage>();

        currentHeroData = new BaseHeroInformationData();
        currentMerchantData = new BaseMerchantInformationData();
        currentSkillData = new BaseSkillInformationData();

        unitStorageLoaded = true;
    }

    #region UNIT STORAGE
    public void ShowUnitStorageList()
    {
        GUILayout.BeginArea(new Rect(1240, 12, leftPanelWidth, 300));

        GUILayout.BeginHorizontal();
        GUILayout.Box("Basic Units", titleText, GUILayout.Width(145), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        unitListScrollPos = EditorGUILayout.BeginScrollView(unitListScrollPos, new GUIStyle("RL Background"), GUILayout.Width(150), GUILayout.Height(position.height - 350));

        if (curUnitStorageData.basicUnitStorage != null && curUnitStorageData.basicUnitStorage.Count > 0)
        {
            for (int i = 0; i < curUnitStorageData.basicUnitStorage.Count; i++)
            {
                GUILayout.BeginHorizontal();
                bool isClicked = false;
                isClicked = GUILayout.Button(curUnitStorageData.basicUnitStorage[i].unitName, (currentUnitData != null
                  && !string.IsNullOrEmpty(currentUnitData.unitName) && curUnitStorageData.basicUnitStorage[i].unitName == currentUnitData.unitName) ? selectedText : notSelectedText);
                GUILayout.Label("[" + curUnitStorageData.basicUnitStorage[i].attackType + "]", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();
                if (isClicked)
                {
                    GUI.FocusControl(null);
                    if (curUnitStorageData.basicUnitStorage[i] != null)
                    {
                        selectedUnitData = curUnitStorageData.basicUnitStorage[i];
                        currentUnitData = selectedUnitData;
                        selectedUnitIdx = i;
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.BeginHorizontal();
        bool saveUnit = GUILayout.Button((currentUnitData == selectedUnitData) ? "Modify" : "Save", GUILayout.MaxWidth(50));
        bool addUnit = GUILayout.Button("New", GUILayout.MaxWidth(40));
        if (selectedUnitData != null)
        {
            bool removeUnit = GUILayout.Button("-", GUILayout.MaxWidth(40));
            if (removeUnit)
            {
                curUnitStorageData.basicUnitStorage.RemoveAt(selectedUnitIdx);
                selectedUnitData = null;
                currentUnitData = new UnitInformationData();
                currentUnitData = new UnitInformationData();
                Save();
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (addUnit)
        {
            currentUnitData = new UnitInformationData();
            currentHeroData.unitInformation = new UnitInformationData();
            selectedHeroData = null;
        }

        if (saveUnit && !string.IsNullOrEmpty(currentUnitData.unitName))
        {
            GUI.FocusControl(null);
            if (curUnitStorageData.basicUnitStorage == null)
            {
                curUnitStorageData.basicUnitStorage = new List<UnitInformationData>();
            }
            if (curUnitStorageData.basicUnitStorage.Find(x => x.unitName == currentUnitData.unitName) == null)
            {
                curUnitStorageData.basicUnitStorage.Add(currentUnitData);
                currentUnitData = new UnitInformationData();
                currentUnitData = new UnitInformationData();
            }
            else
            {
                // Modify Current event
                if (currentUnitData == selectedUnitData)
                {
                    SaveUnitData(currentUnitData);
                }
            }
            Save();
        }
    }

    public void ShowCurrentUnit()
    {
        if (currentUnitData == null)
        {
            currentUnitData = new UnitInformationData();
            currentUnitData.unitName = "";
        }

        if (currentUnitData == null)
        {
            currentUnitData = new UnitInformationData();
            currentUnitData.buffList = new List<BaseBuffInformationData>();
        }

        GUILayout.BeginArea(new Rect(950, 12, leftPanelWidth, 300));
        GUILayout.BeginHorizontal();
        GUILayout.Label("Unit Name:", EditorStyles.boldLabel, GUILayout.Width(65));
        currentUnitData.unitName = EditorGUILayout.TextField(currentUnitData.unitName, GUILayout.MaxWidth(80));
        GUILayout.Label("Atk Type:", EditorStyles.boldLabel, GUILayout.Width(60));
        currentUnitData.attackType = (UnitAttackType)EditorGUILayout.EnumPopup(currentUnitData.attackType, GUILayout.MaxWidth(60));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("[HP]", EditorStyles.boldLabel, GUILayout.Width(40));
        currentUnitData.maxHealth = EditorGUILayout.FloatField(currentUnitData.maxHealth, GUILayout.MaxWidth(50));
        GUILayout.Label("[SPD]", EditorStyles.boldLabel, GUILayout.Width(40));
        currentUnitData.origSpeed = EditorGUILayout.FloatField(currentUnitData.origSpeed, GUILayout.MaxWidth(50));
        GUILayout.Label("[DTH]", EditorStyles.boldLabel, GUILayout.Width(40));
        currentUnitData.deathThreshold = EditorGUILayout.FloatField(currentUnitData.deathThreshold, GUILayout.MaxWidth(45));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Cooldown:", EditorStyles.boldLabel, GUILayout.Width(65));
        currentUnitData.unitCooldown = EditorGUILayout.IntField(currentUnitData.unitCooldown, GUILayout.MaxWidth(45));
        GUILayout.Label("Heal Cost:", EditorStyles.boldLabel, GUILayout.Width(65));
        currentUnitData.healcost = EditorGUILayout.IntField(currentUnitData.healcost, GUILayout.MaxWidth(45));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("[Damage List]", titleText, GUILayout.Width(280));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("[MIN]", EditorStyles.boldLabel, GUILayout.Width(40));
        currentUnitData.minDamage = EditorGUILayout.IntField((int)currentUnitData.minDamage, GUILayout.MaxWidth(50));
        GUILayout.Label("[MAX]", EditorStyles.boldLabel, GUILayout.Width(40));
        currentUnitData.maxDamage = EditorGUILayout.IntField((int)currentUnitData.maxDamage, GUILayout.MaxWidth(50));
        GUILayout.Label("RANGE:", EditorStyles.boldLabel, GUILayout.Width(50));
        currentUnitData.range = EditorGUILayout.IntField(currentUnitData.range, GUILayout.MaxWidth(35));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("[Object Prefab]", titleText, GUILayout.Width(280));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (!string.IsNullOrEmpty(currentUnitData.prefabDataPath))
        {
            currentGameObjectPrefab = (Object)AssetDatabase.LoadAssetAtPath(currentUnitData.prefabDataPath, typeof(Object));
        }
        else
        {
            currentGameObjectPrefab = null;
        }
        GUILayout.Label("Prefab:", EditorStyles.boldLabel, GUILayout.Width(50));
        EditorGUI.BeginChangeCheck();
        currentGameObjectPrefab = (Object)EditorGUILayout.ObjectField(currentGameObjectPrefab, typeof(Object), true, GUILayout.MaxWidth(225));
        if (EditorGUI.EndChangeCheck())
        {
            if(currentGameObjectPrefab != null)
            {
                currentUnitData.prefabDataPath = AssetDatabase.GetAssetPath(currentGameObjectPrefab);
                Debug.Log("Changes has been made : " + currentUnitData.prefabDataPath);
            }
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("[Buff List]", titleText, GUILayout.Width(200));
        bool addBuff = GUILayout.Button("+", GUILayout.Width(76));
        GUILayout.EndHorizontal();

        if(currentUnitData.buffList == null)
        {
            currentUnitData.buffList = new List<BaseBuffInformationData>();
        }
        if (addBuff)
        {

            BaseBuffInformationData tmp = new BaseBuffInformationData();
            tmp = curUnitStorageData.buffStorage[0];
            currentUnitData.buffList.Add(tmp);
        }
        if(currentUnitData.buffList != null && currentUnitData.buffList.Count > 0)
        {
            List<string> buffNameList = new List<string>();
            for (int i = 0; i < curUnitStorageData.buffStorage.Count; i++)
            {
                buffNameList.Add(curUnitStorageData.buffStorage[i].buffName + " [" + curUnitStorageData.buffStorage[i].targetStats + "]");
            }

            for (int i = 0; i < currentUnitData.buffList.Count; i++)
            {
                GUILayout.BeginHorizontal();

                int selectedItem = curUnitStorageData.buffStorage.FindIndex(x => x.buffName == currentUnitData.buffList[i].buffName);
                selectedItem = EditorGUILayout.Popup(selectedItem, buffNameList.ToArray(), GUILayout.MaxWidth(200));

                if (currentUnitData.buffList[i] != curUnitStorageData.buffStorage[selectedItem])
                {
                    currentUnitData.buffList[i] = curUnitStorageData.buffStorage[selectedItem];
                }

                bool removeBuff = GUILayout.Button("-", GUILayout.Width(72));

                if (removeBuff)
                {
                    currentUnitData.buffList.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndArea();
    }


    public void RemoveThisBuffOnUnits(BaseBuffInformationData thisBuff)
    {
        List<UnitInformationData> infoData = curUnitStorageData.basicUnitStorage.FindAll(x => x.buffList.Contains(thisBuff));

        if(infoData.Count > 0)
        {
            for (int i = 0; i < infoData.Count; i++)
            {
                infoData[i].RemoveBuff(thisBuff);
            }
        }
    }

    public void SaveUnitData(UnitInformationData thisUnit)
    {
        int idx = curUnitStorageData.basicUnitStorage.FindIndex(x => x.unitName == thisUnit.unitName);
        
        if(curUnitStorageData.basicUnitStorage[idx].unitName == thisUnit.unitName)
        {
            curUnitStorageData.basicUnitStorage[idx] = thisUnit;
            curUnitStorageData.basicUnitStorage[idx].prefabDataPath = thisUnit.prefabDataPath;
        }
        GUI.FocusControl(null);

    }
    #endregion

    #region HERO STORAGE
    public void ShowHeroStorageList()
    {
        GUILayout.BeginArea(new Rect(775, 12, leftPanelWidth, 300));

        GUILayout.BeginHorizontal();
        GUILayout.Box("Hero Units", titleText, GUILayout.Width(145), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        heroListScrollPos = EditorGUILayout.BeginScrollView(heroListScrollPos, new GUIStyle("RL Background"), GUILayout.Width(150), GUILayout.Height(position.height - 350));
        
        if(curUnitStorageData.heroStorage != null && curUnitStorageData.heroStorage.Count > 0)
        {
            for (int i = 0; i < curUnitStorageData.heroStorage.Count; i++)
            {
                GUILayout.BeginHorizontal();
                bool isClicked = false;
                isClicked = GUILayout.Button(curUnitStorageData.heroStorage[i].unitInformation.unitName, (currentHeroData != null
                  && currentHeroData.unitInformation != null && curUnitStorageData.heroStorage[i].unitInformation.unitName == currentHeroData.unitInformation.unitName) ? selectedText : notSelectedText);
                GUILayout.Label("[" + curUnitStorageData.heroStorage[i].unitInformation.attackType + "]", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();
                if(isClicked)
                {
                    GUI.FocusControl(null);
                    if(curUnitStorageData.heroStorage[i] != null)
                    {
                        selectedHeroData = curUnitStorageData.heroStorage[i];
                        currentHeroData = selectedHeroData;
                        selectedHeroIdx = i;
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        bool saveHeroUnit = GUILayout.Button((currentHeroData == selectedHeroData) ? "Modify" : "Save", GUILayout.MaxWidth(50));
        bool addUnit = GUILayout.Button("New", GUILayout.MaxWidth(40));
        if(selectedHeroData != null)
        {
            bool removeUnit = GUILayout.Button("-", GUILayout.MaxWidth(40));
            if(removeUnit)
            {
                curUnitStorageData.heroStorage.RemoveAt(selectedHeroIdx);
                selectedHeroData = null;
                currentHeroData = new BaseHeroInformationData();
                currentHeroData.unitInformation = new UnitInformationData();
                Save();
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();

        if(addUnit)
        {
            currentHeroData = new BaseHeroInformationData();
            currentHeroData.unitInformation = new UnitInformationData();
            selectedHeroData = null;
        }

        if(saveHeroUnit && !string.IsNullOrEmpty(currentHeroData.unitInformation.unitName))
        {
            GUI.FocusControl(null);
            if(curUnitStorageData.heroStorage == null)
            {
                curUnitStorageData.heroStorage = new List<BaseHeroInformationData>();
            }
            if (curUnitStorageData.heroStorage.Find(x => x.unitInformation.unitName == currentHeroData.unitInformation.unitName) == null)
            {
                curUnitStorageData.heroStorage.Add(currentHeroData);
                currentHeroData = new BaseHeroInformationData();
                currentHeroData.unitInformation = new UnitInformationData();
            }
            else
            {
                // Modify Current event
                if(currentHeroData == selectedHeroData)
                {
                    SaveHeroUnit(currentHeroData);
                }
            }
            Save();
        }
    }
    public void ShowCurrentHeroData()
    {
        if(currentHeroData == null)
        {
            currentHeroData = new BaseHeroInformationData();
            currentHeroData.unitInformation.unitName = "";
        }

        if (currentHeroData.unitInformation == null)
        {
            currentHeroData.unitInformation = new UnitInformationData();
        }

        GUILayout.BeginArea(new Rect(460, 12, leftPanelWidth, 300));

        GUILayout.BeginHorizontal();
        GUILayout.Label("Hero Name:", EditorStyles.boldLabel, GUILayout.Width(70));
        currentHeroData.unitInformation.unitName = EditorGUILayout.TextField(currentHeroData.unitInformation.unitName, GUILayout.MaxWidth(100));
        GUILayout.Label("Price:", EditorStyles.boldLabel, GUILayout.Width(40));
        currentHeroData.baseHeroCoinPrice = EditorGUILayout.IntField(currentHeroData.baseHeroCoinPrice, GUILayout.MaxWidth(55));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Atk Type:", EditorStyles.boldLabel, GUILayout.Width(60));
        currentHeroData.unitInformation.attackType = (UnitAttackType)EditorGUILayout.EnumPopup(currentHeroData.unitInformation.attackType, GUILayout.MaxWidth(80));
        GUILayout.Label("Rarity:", EditorStyles.boldLabel, GUILayout.Width(45));
        currentHeroData.heroRarity = (HeroRarity)EditorGUILayout.EnumPopup(currentHeroData.heroRarity, GUILayout.MaxWidth(80));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();    
        GUILayout.Label("[HP]", EditorStyles.boldLabel, GUILayout.Width(40));
        GUILayout.Label("HP:", EditorStyles.boldLabel, GUILayout.Width(40));
        currentHeroData.unitInformation.maxHealth= EditorGUILayout.IntField((int)currentHeroData.unitInformation.maxHealth, GUILayout.MaxWidth(75));
        GUILayout.Label("Random Stats:", EditorStyles.boldLabel, GUILayout.Width(90));
        currentHeroData.isRandomGenerated = EditorGUILayout.Toggle(currentHeroData.isRandomGenerated, GUILayout.MaxWidth(20));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("[DMG]", EditorStyles.boldLabel, GUILayout.Width(40));
        GUILayout.Label("Min:", EditorStyles.boldLabel, GUILayout.Width(40));
        currentHeroData.unitInformation.minDamage = EditorGUILayout.IntField((int)currentHeroData.unitInformation.minDamage, GUILayout.MaxWidth(75));
        GUILayout.Label("Max:", EditorStyles.boldLabel, GUILayout.Width(32));
        currentHeroData.unitInformation.maxDamage = EditorGUILayout.IntField((int)currentHeroData.unitInformation.maxDamage, GUILayout.MaxWidth(75));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("[SPD]", EditorStyles.boldLabel, GUILayout.Width(40));
        GUILayout.Label("Speed:", EditorStyles.boldLabel, GUILayout.Width(40));
        currentHeroData.unitInformation.origSpeed = EditorGUILayout.IntField((int)currentHeroData.unitInformation.origSpeed, GUILayout.MaxWidth(75));
        GUILayout.Label("Range:", EditorStyles.boldLabel, GUILayout.Width(42));
        currentHeroData.unitInformation.range = EditorGUILayout.IntField((int)currentHeroData.unitInformation.range, GUILayout.MaxWidth(65));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("[Growth Rate Per Level]", titleText, GUILayout.Width(275));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("HP:",GUILayout.Width(30));
        currentHeroData.healthGrowthRate = EditorGUILayout.IntField((int)currentHeroData.healthGrowthRate, GUILayout.MaxWidth(40));
        GUILayout.Label("SPEED:", GUILayout.Width(45));
        currentHeroData.speedGrowthRate = EditorGUILayout.IntField((int)currentHeroData.speedGrowthRate, GUILayout.MaxWidth(40));
        GUILayout.Label("DAMAGE:", GUILayout.Width(60));
        currentHeroData.damageGrowthRate = EditorGUILayout.IntField((int)currentHeroData.damageGrowthRate, GUILayout.MaxWidth(40));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("[Starting Skills]", titleText, GUILayout.Width(200));
        bool addSkill = GUILayout.Button("+", GUILayout.Width(72));
        GUILayout.EndHorizontal();

        if (addSkill)
        {
            if (curUnitStorageData.skillStorage == null && curUnitStorageData.skillStorage.Count <= 0)
            {
                return;
            }

            if (currentHeroData.skillsList == null)
            {
                currentHeroData.skillsList = new List<BaseSkillInformationData>();
            }

            if (curUnitStorageData.skillStorage == null || curUnitStorageData.skillStorage.Count <= 0)
            {
                GUILayout.Label("No Skill Items to Add", GUILayout.Width(272));
            }
            else
            {
                Debug.Log("ADDING STUFF");
                currentHeroData.skillsList.Add(curUnitStorageData.skillStorage[0]);
            }
        }

        if (currentHeroData.skillsList != null && currentHeroData.skillsList.Count > 0)
        {
            List<string> skillNameList = new List<string>();
            for (int i = 0; i < curUnitStorageData.skillStorage.Count; i++)
            {
                skillNameList.Add(curUnitStorageData.skillStorage[i].skillName);
            }

            for (int i = 0; i < currentHeroData.skillsList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                BaseSkillInformationData tmp = currentHeroData.skillsList[i];
                int selectedItem = curUnitStorageData.skillStorage.FindIndex(x => x.skillName == tmp.skillName);
                selectedItem = EditorGUILayout.Popup(selectedItem, skillNameList.ToArray(), GUILayout.MaxWidth(200));

                if (currentHeroData.skillsList[i] != curUnitStorageData.skillStorage[selectedItem])
                {
                    currentHeroData.skillsList[i] = curUnitStorageData.skillStorage[selectedItem];
                }

                bool removeBuff = GUILayout.Button("-", GUILayout.Width(72));

                if (removeBuff)
                {
                    currentHeroData.skillsList.RemoveAt(i);
                }

                GUILayout.EndHorizontal();

            }
        }
        GUILayout.EndArea();

    }
    public void SaveHeroUnit(BaseHeroInformationData thisHero)
    {
        int idx = curUnitStorageData.heroStorage.FindIndex(x => x.unitInformation.unitName == thisHero.unitInformation.unitName);

        if(curUnitStorageData.heroStorage[idx].unitInformation.unitName == thisHero.unitInformation.unitName)
        {
            curUnitStorageData.heroStorage[idx].healthGrowthRate = thisHero.healthGrowthRate;
            curUnitStorageData.heroStorage[idx].baseHeroCoinPrice = thisHero.baseHeroCoinPrice;
            curUnitStorageData.heroStorage[idx].damageGrowthRate = thisHero.damageGrowthRate;
            curUnitStorageData.heroStorage[idx].speedGrowthRate = thisHero.speedGrowthRate;

            curUnitStorageData.heroStorage[idx].equipments = thisHero.equipments;
            curUnitStorageData.heroStorage[idx].unitInformation = thisHero.unitInformation;
        }
        else
        {
            curUnitStorageData.heroStorage.Add(thisHero);
        }
    }
    #endregion

    #region MERCHANT STORAGE
    public void ShowMerchantStorageList()
    {
        GUILayout.BeginArea(new Rect(305, 12, leftPanelWidth, 300));

        GUILayout.BeginHorizontal();
        GUILayout.Box("Merchant Units", titleText, GUILayout.Width(145), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        merchantListScrollPos = EditorGUILayout.BeginScrollView(merchantListScrollPos, new GUIStyle("RL Background"), GUILayout.Width(150), GUILayout.Height(position.height - 350));

        if(curUnitStorageData.merchantStorage == null)
        {
            curUnitStorageData.merchantStorage = new List<BaseMerchantInformationData>();
        }

        if(currentMerchantData == null)
        {
            currentMerchantData = new BaseMerchantInformationData();
            currentMerchantData.unitInformation = new UnitInformationData();
        }

        if (curUnitStorageData.merchantStorage != null && curUnitStorageData.merchantStorage.Count > 0)
        {
            for (int i = 0; i < curUnitStorageData.merchantStorage.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if(curUnitStorageData.merchantStorage[i].unitInformation == null)
                {
                    curUnitStorageData.merchantStorage[i].unitInformation = new UnitInformationData();
                }

                bool isClicked = false;
                isClicked = GUILayout.Button(curUnitStorageData.merchantStorage[i].unitInformation.unitName, notSelectedText);
                GUILayout.Label("[" + curUnitStorageData.merchantStorage[i].unitInformation.attackType + "]", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();
                if (isClicked)
                {
                    GUI.FocusControl(null);
                    if (curUnitStorageData.merchantStorage[i] != null)
                    {
                        selectedMerchantData = curUnitStorageData.merchantStorage[i];
                        currentMerchantData = selectedMerchantData;
                        selectedMerchantIdx = i;
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        bool saveHeroUnit = GUILayout.Button((currentMerchantData == selectedMerchantData) ? "Modify" : "Save", GUILayout.MaxWidth(50));
        bool addUnit = GUILayout.Button("New", GUILayout.MaxWidth(40));
        if (selectedMerchantData != null)
        {
            bool removeUnit = GUILayout.Button("-", GUILayout.MaxWidth(40));
            if (removeUnit)
            {
                curUnitStorageData.merchantStorage.RemoveAt(selectedHeroIdx);
                selectedMerchantData = null;
                currentMerchantData = new BaseMerchantInformationData();
                currentMerchantData.unitInformation = new UnitInformationData();
                Save();
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (addUnit)
        {
            currentMerchantData = new BaseMerchantInformationData();
            currentMerchantData.unitInformation = new UnitInformationData();
            selectedMerchantData = null;
        }

        if (saveHeroUnit && !string.IsNullOrEmpty(currentMerchantData.unitInformation.unitName))
        {
            GUI.FocusControl(null);
            if (curUnitStorageData.merchantStorage == null)
            {
                curUnitStorageData.merchantStorage = new List<BaseMerchantInformationData>();
            }
            if (curUnitStorageData.merchantStorage.Find(x => x.unitInformation.unitName == currentMerchantData.unitInformation.unitName) == null)
            {
                curUnitStorageData.merchantStorage.Add(currentMerchantData);
                currentMerchantData = new BaseMerchantInformationData();
                currentMerchantData.unitInformation = new UnitInformationData();
            }
            else
            {
                // Modify Current event
                if (currentMerchantData == selectedMerchantData)
                {
                    SaveMerchantUnit(currentMerchantData);
                }
            }
            Save();
        }
    }

    public void ShowCurrentMerchantData()
    {
        if (currentMerchantData == null)
        {
            currentMerchantData = new BaseMerchantInformationData();
            currentMerchantData.unitInformation.unitName = "";
        }

        if (currentMerchantData.unitInformation == null)
        {
            currentMerchantData.unitInformation = new UnitInformationData();
        }

        GUILayout.BeginArea(new Rect(25, 12, leftPanelWidth, 300));

        GUILayout.BeginHorizontal();
        GUILayout.Label("Merchant Name:", EditorStyles.boldLabel, GUILayout.Width(100));
        currentMerchantData.unitInformation.unitName = EditorGUILayout.TextField(currentMerchantData.unitInformation.unitName, GUILayout.MaxWidth(170));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Atk Type:", EditorStyles.boldLabel, GUILayout.Width(60));
        currentMerchantData.unitInformation.attackType = (UnitAttackType)EditorGUILayout.EnumPopup(currentMerchantData.unitInformation.attackType, GUILayout.MaxWidth(80));
        GUILayout.Label("Type:", EditorStyles.boldLabel, GUILayout.Width(45));
        currentMerchantData.merchantType = (MerchantType)EditorGUILayout.EnumPopup(currentMerchantData.merchantType, GUILayout.MaxWidth(80));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("[HP]", EditorStyles.boldLabel, GUILayout.Width(40));
        GUILayout.Label("HP:", EditorStyles.boldLabel, GUILayout.Width(40));
        currentMerchantData.unitInformation.maxHealth = EditorGUILayout.IntField((int)currentMerchantData.unitInformation.maxHealth, GUILayout.MaxWidth(75));
        GUILayout.Label("Random Stats:", EditorStyles.boldLabel, GUILayout.Width(90));
        currentMerchantData.isRandomGenerated = EditorGUILayout.Toggle(currentMerchantData.isRandomGenerated, GUILayout.MaxWidth(10));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("[DMG]", EditorStyles.boldLabel, GUILayout.Width(40));
        GUILayout.Label("Min:", EditorStyles.boldLabel, GUILayout.Width(40));
        currentMerchantData.unitInformation.minDamage = EditorGUILayout.IntField((int)currentMerchantData.unitInformation.minDamage, GUILayout.MaxWidth(75));
        GUILayout.Label("Max:", EditorStyles.boldLabel, GUILayout.Width(32));
        currentMerchantData.unitInformation.maxDamage = EditorGUILayout.IntField((int)currentMerchantData.unitInformation.maxDamage, GUILayout.MaxWidth(75));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("[Starting Skills]", titleText, GUILayout.Width(275));
        GUILayout.EndHorizontal();

        GUILayout.EndArea();

    }

    public void SaveMerchantUnit(BaseMerchantInformationData thisMerchant)
    {
        int idx = curUnitStorageData.merchantStorage.FindIndex(x => x.unitInformation.unitName == thisMerchant.unitInformation.unitName);

        if (curUnitStorageData.merchantStorage[idx].unitInformation.unitName == thisMerchant.unitInformation.unitName)
        {
            curUnitStorageData.merchantStorage[idx].unitInformation = thisMerchant.unitInformation;
            curUnitStorageData.merchantStorage[idx].itemsSold = thisMerchant.itemsSold;
        }
        else
        {
            curUnitStorageData.merchantStorage.Add(thisMerchant);
        }
    }
    #endregion

    #region SKILL STORAGE
    public void ShowSkillData()
    {
        GUILayout.BeginArea(new Rect(25, 325, 950, 300));

        GUILayout.BeginHorizontal();
        GUILayout.Box("[SKILLS INFORMATION]", titleText, GUILayout.Width(1000), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUILayout.EndArea();

        ShowSkillStorageList();
    }
    public void ShowSkillStorageList()
    {
        GUILayout.BeginArea(new Rect(825, 350, leftPanelWidth, 300));

        GUILayout.BeginHorizontal();
        GUILayout.Box("Skill List", titleText, GUILayout.Width(145), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        skillListScrollPos = EditorGUILayout.BeginScrollView(skillListScrollPos, new GUIStyle("RL Background"), GUILayout.Width(150), GUILayout.Height(position.height - 400));

        if (curUnitStorageData.skillStorage != null && curUnitStorageData.skillStorage.Count > 0)
        {
            for (int i = 0; i < curUnitStorageData.skillStorage.Count; i++)
            {
                GUILayout.BeginHorizontal();
                bool isClicked = false;
                isClicked = GUILayout.Button(curUnitStorageData.skillStorage[i].skillName, (currentSkillData != null
                  && !string.IsNullOrEmpty(currentSkillData.skillName) && curUnitStorageData.skillStorage[i].skillName == currentSkillData.skillName) ? selectedText : notSelectedText);

                GUILayout.Label("[" + curUnitStorageData.skillStorage[i].skillType + "]", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();

                if (isClicked)
                {
                    GUI.FocusControl(null);
                    if (curUnitStorageData.skillStorage[i] != null)
                    {
                        selectedSkillData = curUnitStorageData.skillStorage[i];
                        currentSkillData = selectedSkillData;
                        selectedSkillIdx = i;
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();

        bool saveSkill = GUILayout.Button((currentSkillData == selectedSkillData) ? "Modify" : "Save", GUILayout.MaxWidth(50));
        bool addSKill = GUILayout.Button("New", GUILayout.MaxWidth(40));
        if (selectedHeroData != null)
        {
            bool removeSkill = GUILayout.Button("-", GUILayout.MaxWidth(40));
            if (removeSkill)
            {
                curUnitStorageData.skillStorage.RemoveAt(selectedSkillIdx);
                selectedSkillData = null;
                currentSkillData = new BaseSkillInformationData();
                Save();
            }
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (addSKill)
        {
            currentSkillData = new BaseSkillInformationData();
            selectedSkillData = null;
        }

        if (saveSkill && !string.IsNullOrEmpty(currentSkillData.skillName))
        {
            GUI.FocusControl(null);
            if (curUnitStorageData.skillStorage == null)
            {
                curUnitStorageData.skillStorage = new List<BaseSkillInformationData>();
            }
            if (curUnitStorageData.skillStorage.Find(x => x.skillName == currentSkillData.skillName) == null)
            {
                curUnitStorageData.skillStorage.Add(currentSkillData);
                currentSkillData = new BaseSkillInformationData();
            }
            else
            {
                // Modify Current event
                if (selectedSkillData != null && currentSkillData == selectedSkillData)
                {
                    SaveSkill(currentSkillData);
                }
            }
            Save();
        }
    }

    public void ShowCurrentSkill()
    {
        if(currentSkillData == null)
        {
            currentSkillData = new BaseSkillInformationData();
        }

        GUILayout.BeginArea(new Rect(525, 350, leftPanelWidth, 300));

        GUILayout.BeginHorizontal();
        GUILayout.Label("Skill Name:", EditorStyles.boldLabel, GUILayout.Width(70));
        currentSkillData.skillName = EditorGUILayout.TextField(currentSkillData.skillName, GUILayout.MaxWidth(110));
        GUILayout.Label("Level:", EditorStyles.boldLabel, GUILayout.Width(40));
        currentSkillData.skillLevel = EditorGUILayout.IntField((int)currentSkillData.skillLevel, GUILayout.MaxWidth(45));

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Type:", EditorStyles.boldLabel, GUILayout.Width(35));
        currentSkillData.skillType = (SkillType)EditorGUILayout.EnumPopup(currentSkillData.skillType, GUILayout.MaxWidth(110));
        if (currentSkillData.buffList != null && currentSkillData.buffList.Count > 0)
        {
            GUILayout.Label("[Buff Based Skill]", GUILayout.MaxWidth(140));
        }
        else
        {
            GUILayout.Label("Hit Stat:", EditorStyles.boldLabel, GUILayout.Width(50));
            currentSkillData.targetStats = (TargetStats)EditorGUILayout.EnumPopup(currentSkillData.targetStats, GUILayout.MaxWidth(70));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Type:", EditorStyles.boldLabel, GUILayout.Width(40));
        currentSkillData.targetType = (TargetType)EditorGUILayout.EnumPopup(currentSkillData.targetType, GUILayout.MaxWidth(85));
        if(currentSkillData.buffList != null && currentSkillData.buffList.Count > 0)
        {
            GUILayout.Label("[Buff Based Skill]", GUILayout.MaxWidth(140));
        }
        else
        {
            GUILayout.Label("Direct Dmg:", EditorStyles.boldLabel, GUILayout.Width(75));
            currentSkillData.targetInflictedCount = EditorGUILayout.IntField((int)currentSkillData.targetInflictedCount, GUILayout.MaxWidth(65));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Area Effect:", EditorStyles.boldLabel, GUILayout.Width(70));
        currentSkillData.affectedArea = (AreaAffected)EditorGUILayout.EnumPopup(currentSkillData.affectedArea, GUILayout.MaxWidth(85));
        GUILayout.Label("Range:", EditorStyles.boldLabel, GUILayout.Width(45));
        currentSkillData.maxRange = EditorGUILayout.IntField(currentSkillData.maxRange, GUILayout.MaxWidth(65));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("[Buff List]", titleText, GUILayout.Width(200));
        bool addBuff = GUILayout.Button("+", GUILayout.Width(72));
        GUILayout.EndHorizontal();

        if(addBuff)
        {
            if(curUnitStorageData.buffStorage == null && curUnitStorageData.buffStorage.Count <= 0)
            {
                return;
            }

            if (currentSkillData.buffList == null)
            {
                currentSkillData.buffList = new List<BaseBuffInformationData>();
            }
            if(curUnitStorageData.buffStorage == null || curUnitStorageData.buffStorage.Count <=0)
            {
                GUILayout.Label("No Buff Items to Add", GUILayout.Width(272));
            }
            else
            {
                currentSkillData.buffList.Add(curUnitStorageData.buffStorage[0]);
            }
        }

        if(currentSkillData.buffList != null && currentSkillData.buffList.Count > 0)
        {
            List<string> buffNameList = new List<string>();
            for (int i = 0; i < curUnitStorageData.buffStorage.Count; i++)
            {
                buffNameList.Add(curUnitStorageData.buffStorage[i].buffName);
            }
            for (int i = 0; i < currentSkillData.buffList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                BaseBuffInformationData tmp = currentSkillData.buffList[i];
                int selectedItem = curUnitStorageData.buffStorage.FindIndex(x => x.buffName == tmp.buffName);
                selectedItem = EditorGUILayout.Popup(selectedItem, buffNameList.ToArray(), GUILayout.MaxWidth(200));

                if (currentSkillData.buffList[i] != curUnitStorageData.buffStorage[selectedItem])
                {
                    currentSkillData.buffList[i] = curUnitStorageData.buffStorage[selectedItem];
                }

                bool removeBuff = GUILayout.Button("-", GUILayout.Width(72));

                if (removeBuff)
                {
                    currentSkillData.buffList.RemoveAt(i);
                }
                GUILayout.EndHorizontal();

            }
        }
        GUILayout.EndArea();
    }

    public void SaveSkill(BaseSkillInformationData thisSkill)
    {
        int idx = curUnitStorageData.skillStorage.FindIndex(x => x.skillName == thisSkill.skillName);

        if (curUnitStorageData.skillStorage[idx].skillName == thisSkill.skillName)
        {
            curUnitStorageData.skillStorage[idx].skillName = thisSkill.skillName;
            curUnitStorageData.skillStorage[idx].skillName = thisSkill.skillName;
        }
        else
        {
            curUnitStorageData.skillStorage.Add(thisSkill);
        }
    }
    #endregion

    #region BUFF STORAGE
    public void ShowBuffStorageList()
    {
        GUILayout.BeginArea(new Rect(310, 350, leftPanelWidth, 300));

        GUILayout.BeginHorizontal();
        GUILayout.Box("Buff List", titleText, GUILayout.Width(175), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        buffListScrollPos = EditorGUILayout.BeginScrollView(buffListScrollPos, new GUIStyle("RL Background"), GUILayout.Width(180), GUILayout.Height(position.height - 400));

        if(curUnitStorageData.buffStorage == null)
        {
            curUnitStorageData.buffStorage = new List<BaseBuffInformationData>();
        }

        if (curUnitStorageData.buffStorage != null && curUnitStorageData.buffStorage.Count > 0)
        {
            for (int i = 0; i < curUnitStorageData.buffStorage.Count; i++)
            {
                GUILayout.BeginHorizontal();
                bool isClicked = false;
                isClicked = GUILayout.Button(curUnitStorageData.buffStorage[i].buffName, (currentBuffData != null
                  && !string.IsNullOrEmpty(currentBuffData.buffName) && curUnitStorageData.buffStorage[i].buffName == currentBuffData.buffName) ? selectedText : notSelectedText);

                GUILayout.Label("[" + curUnitStorageData.buffStorage[i].targetStats + "]", GUILayout.MaxWidth(100));
                GUILayout.Label("[" + curUnitStorageData.buffStorage[i].effectAmount + "]", GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();

                if (isClicked)
                {
                    GUI.FocusControl(null);
                    if (curUnitStorageData.buffStorage[i] != null)
                    {
                        selectedBuffData = curUnitStorageData.buffStorage[i];
                        currentBuffData = selectedBuffData;
                        selectedBuffIdx = i;
                    }
                }
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();

        bool saveBuff = GUILayout.Button((currentBuffData == selectedBuffData) ? "Modify" : "Save", GUILayout.MaxWidth(60));
        bool addBuff = GUILayout.Button("New", GUILayout.MaxWidth(60));
        if (selectedBuffData != null)
        {
            bool removeBuff = GUILayout.Button("-", GUILayout.MaxWidth(50));
            if (removeBuff)
            {
                RemoveThisBuffOnUnits(curUnitStorageData.buffStorage[selectedBuffIdx]);
                curUnitStorageData.buffStorage.RemoveAt(selectedBuffIdx);
                selectedBuffData = null;
                currentBuffData = new BaseBuffInformationData();
                Save();
            }
        }

        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (addBuff)
        {
            GUI.FocusControl(null);
            currentBuffData = new BaseBuffInformationData();
            selectedBuffData = null;
        }

        if (saveBuff && !string.IsNullOrEmpty(currentBuffData.buffName))
        {
            GUI.FocusControl(null);
            if (curUnitStorageData.buffStorage == null)
            {
                curUnitStorageData.buffStorage = new List<BaseBuffInformationData>();
            }
            if (curUnitStorageData.buffStorage.Find(x => x.buffName == currentBuffData.buffName) == null)
            {
                curUnitStorageData.buffStorage.Add(currentBuffData);
                currentBuffData = new BaseBuffInformationData();
            }
            else
            {
                // Modify Current event
                if (currentBuffData == selectedBuffData)
                {
                    SaveBuff(currentBuffData);
                }
            }
            Save();
        }
    }

    public void ShowCurrentBuff()
    {
        if (currentBuffData == null)
        {
            currentBuffData = new BaseBuffInformationData();
        }

        GUILayout.BeginArea(new Rect(30, 350, leftPanelWidth, 300));

        GUILayout.BeginHorizontal();
        GUILayout.Label("Buff Name:", EditorStyles.boldLabel, GUILayout.Width(70));
        currentBuffData.buffName = EditorGUILayout.TextField(currentBuffData.buffName, GUILayout.MaxWidth(180));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Aim Stat:", EditorStyles.boldLabel, GUILayout.Width(60));
        currentBuffData.targetStats = (TargetStats)EditorGUILayout.EnumPopup(currentBuffData.targetStats, GUILayout.MaxWidth(110));
        GUILayout.Label("Effect:", EditorStyles.boldLabel, GUILayout.Width(40));
        currentBuffData.effectAmount = EditorGUILayout.FloatField(currentBuffData.effectAmount, GUILayout.MaxWidth(35));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Permanent:", EditorStyles.boldLabel, GUILayout.Width(70));
        currentBuffData.permanentBuff = EditorGUILayout.Toggle(currentBuffData.permanentBuff, GUILayout.Width(15));
        if(!currentBuffData.permanentBuff)
        {
            GUILayout.Label("Duration:", EditorStyles.boldLabel, GUILayout.Width(60));
            currentBuffData.duration = EditorGUILayout.FloatField(currentBuffData.duration, GUILayout.MaxWidth(100));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Triggered By:", EditorStyles.boldLabel, GUILayout.Width(90));
        currentBuffData.buffTrigger = (TriggeredBy)EditorGUILayout.EnumPopup(currentBuffData.buffTrigger, GUILayout.MaxWidth(160));
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    public void SaveBuff(BaseBuffInformationData thisBuff)
    {
        int idx = curUnitStorageData.buffStorage.FindIndex(x => x.buffName == thisBuff.buffName);

        if (curUnitStorageData.buffStorage[idx].buffName == thisBuff.buffName)
        {
            curUnitStorageData.buffStorage[idx].buffName = thisBuff.buffName;
            curUnitStorageData.buffStorage[idx].buffName = thisBuff.buffName;
        }
        else
        {
            curUnitStorageData.buffStorage.Add(thisBuff);
        }
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
