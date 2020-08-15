using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KingEvents;
using Kingdoms;

public class NotificationHandler : MonoBehaviour
{
    public List<BaseNotification> notifList;


    public void RevealResourceNotification(ResourceType thisType, int thisAmount, bool isReduce = false, string fromDesc = "")
    {
        BaseNotification selectedNotif = notifList.Find(x => !x.isShowing);

        selectedNotif.gameObject.SetActive(true);
        selectedNotif.SetIconTo(thisType);

        if(isReduce)
        {
            selectedNotif.SetAsReduce();
        }
        else
        {
            selectedNotif.SetAsAdd();
        }

        selectedNotif.SetShowing(true);

        if(!string.IsNullOrEmpty(fromDesc))
        {
            selectedNotif.text.postCountMesg = fromDesc;
        }
        else
        {
            selectedNotif.text.postCountMesg = "";
        }

        selectedNotif.text.SetTargetCount(thisAmount);

        StartCoroutine(selectedNotif.myPanel.WaitAnimationForAction(selectedNotif.myPanel.openAnimationName, selectedNotif.PreCloseAnim));
    }



}
