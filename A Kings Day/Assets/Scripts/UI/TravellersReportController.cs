using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Kingdoms;
using Managers;
using Utilities;
using ResourceUI;

public class TravellersReportController : MonoBehaviour
{
    public TravellingSystem myController;
    public BasePanelBehavior myPanel;
    public BasePanelWindow myWindow;

    public RectTransform myRect;
    [Header("Report Mechanics")]
    public bool isShowing;
    public bool isTransitioning;
    public GameObject travellerClicked;
    public BaseTravellerBehavior currentTravellerReport;
    [Header("Showup Mechanics")]
    public Vector2 centerRectPos;
    public Vector2 unitRectPos;
    public float moveSpd = 1.75f;
    private float delayCounter = 0;
    private float delayMaxCount = 0.25f;
    private bool startMoving = false;
    private bool engagedToBattle = false;

    [Header("Visual Mechanics")]
    public GameObject noActionsTaken;
    public TextMeshProUGUI travellerUnitCount;
    public TravellerUnitsIcon unitsIconHandler;
    [Header("Relationship Visual Mechanics")]
    public TextMeshProUGUI travellerTypeAndRelationship;
    public Gradient relationshipColorGradient;
    [Header("Actions Visual Mechanics")]
    public Transform sentenceParent;
    public GameObject sentencePrefab;
    public List<TypeWriterEffectUI> actionList;

    public void Start()
    {
        centerRectPos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        myWindow.parentCloseCallback = () => CloseTravellerReport();
    }
    public void Update()
    {
        if (isTransitioning)
        {
            if (!startMoving && isShowing)
            {
                delayCounter += Time.deltaTime;
                if (delayCounter >= delayMaxCount)
                {
                    startMoving = true;
                    delayCounter = 0;
                }
            }
            else
            {
                float dist = Vector3.Distance(myRect.position, centerRectPos);

                if (isShowing)
                {
                    if (dist < 0.0015f)
                    {
                        isTransitioning = false;
                        startMoving = false;
                    }
                    float step = moveSpd;
                    myRect.position = Vector3.MoveTowards(myRect.position, centerRectPos, step);
                }
                else
                {
                    if (dist < 0.0015f && myRect.localScale == Vector3.zero)
                    {
                        isTransitioning = false;
                        startMoving = false;
                    }
                    float step = moveSpd;
                    myRect.position = Vector3.MoveTowards(myRect.position, unitRectPos, step);
                }

            }
        }
    }
    public void ShowTravellerReport(BaseTravellerBehavior thisTraveller)
    {
        EventBroadcaster.Instance.PostEvent(EventNames.HIDE_RESOURCES);
        EventBroadcaster.Instance.PostEvent(EventNames.ENABLE_TAB_COVER);
        ResourceInformationController.GetInstance.ShowResourcePanel(ResourcePanelType.side);
        StartCoroutine(myPanel.WaitAnimationForAction(myPanel.openAnimationName, SetupActionReport));
        travellerClicked = thisTraveller.gameObject;
        currentTravellerReport = thisTraveller;
        isShowing = true;

        unitRectPos = Camera.main.WorldToScreenPoint(thisTraveller.transform.position);

        myRect.position = unitRectPos;
        isTransitioning = true;

        InitializeTravellerReport();
    }

    public void CloseTravellerReport()
    {
        isShowing = false;
        isTransitioning = true;

        if(!engagedToBattle)
        {
            EventBroadcaster.Instance.PostEvent(EventNames.SHOW_RESOURCES);
        }

        EventBroadcaster.Instance.PostEvent(EventNames.DISABLE_TAB_COVER);
        ResourceInformationController.GetInstance.ShowResourcePanel(ResourcePanelType.overhead);
        if(!engagedToBattle)
        {
            StartCoroutine(myPanel.WaitAnimationForAction(myPanel.closeAnimationName, ResetTravellerReport));
        }

    }
    public void ResetTravellerReport()
    {
        if (actionList != null && actionList.Count > 0)
        {
            for (int i = 0; i < actionList.Count; i++)
            {
                DestroyImmediate(actionList[i].gameObject);
            }
            actionList.Clear();
        }
        noActionsTaken.SetActive(true);

        for (int i = 0; i < unitsIconHandler.unitsIcons.Count; i++)
        {
            unitsIconHandler.ShowIcon(i);
            unitsIconHandler.SetAsUnknownIcon(i);
        }

        currentTravellerReport = null;
    }
    public void InitializeTravellerReport()
    {
        // ACITONS TAKEN
        if (currentTravellerReport.myTravellerData.actionsDealt != null && currentTravellerReport.myTravellerData.actionsDealt.Count > 0)
        {
            noActionsTaken.SetActive(false);
        }
        else
        {
            noActionsTaken.SetActive(true);
        }


        // RELATIONSHIP COLOR
        UpdateRelationshipReport();

        // COUNT TEXT
        UpdateUnitCountReport();

        // ACTION REPORT
        SetupActionReport();

        // Icons
        SetupIconReport();
    }

    public void UpdateRelationshipReport()
    {
        float curColorValue = (currentTravellerReport.myTravellerData.relationship + 100.0f) / 200.0f;
        travellerTypeAndRelationship.color = relationshipColorGradient.Evaluate(curColorValue);
        travellerTypeAndRelationship.text = currentTravellerReport.myTravellerData.travellerType.ToString();
    }
    public void SetupIconReport()
    {
        if(currentTravellerReport.myTravellerData.scoutSentSuccessfully)
        {
            RevealIconReport();
        }
        else
        {
            for (int i = 0; i < unitsIconHandler.unitsIcons.Count; i++)
            {
                if(i < currentTravellerReport.myTravellerData.troopsCarried.Count)
                {
                    unitsIconHandler.ShowIcon(i);
                }
                else
                {
                    unitsIconHandler.HideIcon(i);
                }
            }
        }
    }

    public void RevealIconReport()
    {
        if(!currentTravellerReport.myTravellerData.scoutSentSuccessfully)
        {
            return;
        }

        for (int i = 0; i < unitsIconHandler.unitsIcons.Count; i++)
        {
            if (i < currentTravellerReport.myTravellerData.troopsCarried.Count)
            {
                Sprite thisUnit = myController.unitStorage.GetUnitIcon(currentTravellerReport.myTravellerData.troopsCarried[i].unitInformation.unitGenericName);
                if(thisUnit != null)
                {
                    unitsIconHandler.SetAsNewicon(i, thisUnit);
                }
                else
                {
                    Debug.Log("WTF");
                }
            }
        }
    }
    public void UpdateUnitCountReport()
    {
        string unitCountText = "unknown";
        if (!currentTravellerReport.myTravellerData.scoutSentSuccessfully)
        {
            int min = currentTravellerReport.myTravellerData.ObtainVagueUnitCount(true);
            int max = currentTravellerReport.myTravellerData.ObtainVagueUnitCount(false);

            unitCountText = min.ToString() + " To " + max.ToString();
        }
        else
        {
            unitCountText = "around " + currentTravellerReport.myTravellerData.ObtainTotalUnitCount().ToString();
        }
        travellerUnitCount.text = unitCountText;
    }
    public void SetupActionReport()
    {
        if (currentTravellerReport.myTravellerData.actionsDealt == null || currentTravellerReport.myTravellerData.actionsDealt.Count <= 0)
        {
            return;
        }

        if(actionList == null)
        {
            actionList = new List<TypeWriterEffectUI>();
        }

        for (int i = 0; i < currentTravellerReport.myTravellerData.actionsDealt.Count; i++)
        {
            GameObject tmp = (GameObject)Instantiate(sentencePrefab);
            tmp.transform.SetParent(sentenceParent);

            TypeWriterEffectUI tmpWriter = tmp.GetComponent<TypeWriterEffectUI>();
            actionList.Add(tmpWriter);

            tmpWriter.SetTypeWriterMessage(currentTravellerReport.myTravellerData.actionsDealt[i], true);
        }
        if(actionList.Count > 0)
        {
            noActionsTaken.SetActive(false);
        }
        myWindow.OpenWindow();
    }

    public void AddAction(int actionIdx)
    {
        string tmp = "[Week 1]";
        string prePurpose = "Purpose";

        if(PlayerGameManager.GetInstance != null)
        {
            tmp = "[Week " + PlayerGameManager.GetInstance.playerData.weekCount +"]";
        }

        switch (actionIdx)
        {
            case 0: // SCOUTING
                if (currentTravellerReport.myTravellerData.scoutSentSuccessfully)
                    return;
                prePurpose = "When We Tried to scout them.||..";
                if (PlayerGameManager.GetInstance != null)
                {
                    if (PlayerGameManager.GetInstance.playerData.GetTotalTroops <= 0)
                    {
                        tmp += "We dont have troops to scout them!";
                    }
                    else
                    {
                        CheckScoutSuccess();

                        if (currentTravellerReport.myTravellerData.scoutSentSuccessfully)
                        {
                            tmp += "Scout mission success! revealing info!";
                        }
                        else
                        {
                            tmp += "Scout Mission failed! " + PlayerGameManager.GetInstance.playerData.RemoveScoutTroops() + " sent died.";
                        }
                    }
                }
                else
                {
                    CheckScoutSuccess();

                    if(currentTravellerReport.myTravellerData.scoutSentSuccessfully)
                    {
                        tmp += "Scout mission success! revealing info!";
                    }
                    else
                    {
                        if(PlayerGameManager.GetInstance != null)
                        {
                            tmp += "Scout Mission failed! "+ PlayerGameManager.GetInstance.playerData.RemoveScoutTroops() + " sent died.";
                        }
                        else
                        {
                            tmp += "Scout Mission failed! a Troop send died.";
                        }
                    }

                }
                break;
            case 1: // Send Gift
                if (PlayerGameManager.GetInstance != null && PlayerGameManager.GetInstance.playerData.coins < 100)
                {
                    tmp += "Plentiful Food, but we need coins!";
                }
                else
                {
                    List<TravellerFlavourPhrase> travellerTexts = currentTravellerReport.myTravellerData.GetRelationshipFlavorText();
                    if(travellerTexts.Count > 0)
                    {
                        prePurpose = "We approach them with gifts.||..";
                        int rand = UnityEngine.Random.Range(0, travellerTexts.Count);
                        tmp += travellerTexts[rand].flavourText;
                        currentTravellerReport.myTravellerData.UpdateRelationship(10);
                        myController.CheckRelationship(currentTravellerReport.myTravellerData.relationship);

                        if(PlayerGameManager.GetInstance != null)
                        {
                            PlayerGameManager.GetInstance.playerData.coins -= 100;
                        }
                    }
                }
                break;
            case 2:
                if(TransitionManager.GetInstance == null || PlayerGameManager.GetInstance == null)
                {
                    return;
                }
                EventBroadcaster.Instance.PostEvent(EventNames.HIDE_RESOURCES);
                prePurpose = "Their intentions were bad! so we decided to.||..";
                tmp += "Engaged In battle!";

                PlayerGameManager.GetInstance.unitsToSend = PlayerGameManager.GetInstance.ConvertWholeGarrisonAsTraveller();
                StartCoroutine(myPanel.WaitAnimationForAction(myPanel.closeAnimationName, () => TransitionManager.GetInstance.FaceTravellerInBattle(currentTravellerReport.myTravellerData, false)));

                engagedToBattle = true;
                break;

            default:
                break;
        }

        if(currentTravellerReport.myTravellerData.actionsDealt == null)
        {
            currentTravellerReport.myTravellerData.actionsDealt = new List<string>();
        }
        currentTravellerReport.myTravellerData.actionsDealt.Add(tmp);

        GameObject preTemp = (GameObject)Instantiate(sentencePrefab);
        preTemp.transform.SetParent(sentenceParent);

        TypeWriterEffectUI preTempWriter = preTemp.GetComponent<TypeWriterEffectUI>();
        actionList.Add(preTempWriter);
        preTempWriter.SetTypeWriterMessage(prePurpose, true, () => CallNextWriter(currentTravellerReport.myTravellerData.actionsDealt[currentTravellerReport.myTravellerData.actionsDealt.Count - 1], preTempWriter));

        noActionsTaken.SetActive(false);
    }
    public void CallNextWriter(string mesg, TypeWriterEffectUI destroyThis)
    {
        actionList.Remove(destroyThis);
        DestroyImmediate(destroyThis.gameObject);

        GameObject temp = (GameObject)Instantiate(sentencePrefab);
        temp.transform.SetParent(sentenceParent);

        TypeWriterEffectUI tempWriter = temp.GetComponent<TypeWriterEffectUI>();
        actionList.Add(tempWriter);

        tempWriter.SetTypeWriterMessage(mesg, true);

        UpdateUnitCountReport();
        RevealIconReport();
        UpdateRelationshipReport();

    }

    public void CheckScoutSuccess()
    {
        float successChance = 50;
        bool success = (UnityEngine.Random.Range(0, 100) > successChance) ? true : false;

        currentTravellerReport.myTravellerData.scoutSentSuccessfully = success;
    }
    public string ObtainActionResult(int idx)
    {
        string tmp = "";

        return tmp;
    }
}
