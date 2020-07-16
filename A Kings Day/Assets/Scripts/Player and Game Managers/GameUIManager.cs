using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using TMPro;
using Kingdoms;
using ResourceUI;
using KingEvents;
using Utilities;
using UnityEngine.UI;

public class GameUIManager : BaseManager
{
    #region Singleton
    private static GameUIManager instance;
    public static GameUIManager GetInstance
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

    [Header("Kingdom Information")]
    public GameObject inGameView;
    public CardsEventController cardEventsController;
    [Header("Event Information")]
    public BasePanelBehavior eventBellBtn;
    private bool dontAllowTextSwitch = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        EventBroadcaster.Instance.AddObserver(EventNames.HIDE_RESOURCES, HideBellButton);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.HIDE_RESOURCES, HideBellButton);
    }
    public override void PreOpenManager()
    {
        base.PreOpenManager();
    }
    public override void StartManager()
    {
        base.StartManager();
        //Debug.Log("Starting Game UI Manager");
        inGameView.gameObject.SetActive(true);

        //KingdomManager.GetInstance.PreOpenManager();
    }

    public void InitializeData()
    {
        PlayerKingdomData data = PlayerGameManager.GetInstance.playerData;

    }
    public void ShowBellButton()
    {
        eventBellBtn.gameObject.SetActive(true);
        eventBellBtn.GetComponent<Button>().interactable = true;
        StartCoroutine(DelayBellButton());
    }

    public IEnumerator DelayBellButton()
    {
        yield return new WaitForSeconds(1);

        eventBellBtn.PlayOpenAnimation();
    }
    public void HideBellButton(Parameters p = null)
    {
        eventBellBtn.GetComponent<Button>().interactable = false;
        eventBellBtn.PlayCloseAnimation();
    }
    public void HideBellButton()
    {
        eventBellBtn.PlayCloseAnimation();
        StartCoroutine(DelayStartNextEvent());
    }

    IEnumerator DelayStartNextEvent()
    {
        yield return new WaitForSeconds(1);

        StartNextEvent();

    }
    public void StartNextEvent()
    {
        KingdomManager.GetInstance.StartEvent();
    }

}
