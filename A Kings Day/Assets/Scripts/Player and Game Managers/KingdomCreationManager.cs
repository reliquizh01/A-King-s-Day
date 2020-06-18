using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Managers;
using Kingdoms;

public class KingdomCreationManager : BaseManager
{
    public GameObject creationView;

    public PlayerKingdomData CreateKingdom()
    {
        PlayerKingdomData tmpKingdom = new PlayerKingdomData();
            

        return tmpKingdom;
    }
    public override void PreOpenManager()
    {
        base.PreOpenManager();

    }
    public override void StartManager()
    {
        Debug.Log("Starting Kingdom Creation Manager");
        base.StartManager();
        creationView.gameObject.SetActive(true);
    }

    public override void CloseManager()
    {
        base.CloseManager();
        creationView.gameObject.SetActive(false);
    }
}
