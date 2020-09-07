using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using TMPro;
public class PlayerGameGuide : MonoBehaviour
{
    #region Singleton
    private static PlayerGameGuide instance;
    public static PlayerGameGuide GetInstance
    {
        get
        {
            return instance;
        }
    }

    public void Awake()
    {
        if(PlayerGameGuide.GetInstance != null && PlayerGameGuide.GetInstance != this)
        {
            DestroyImmediate(this.gameObject);
        }

        instance = this;
    }
    #endregion

    public BasePanelBehavior myPanel;

    public GameObject guidePanel;
    public TextMeshProUGUI guideText;

    public void ShowGuideText(string message)
    {
        myPanel.PlayOpenAnimation();
        guideText.text = message;
    }

    public void HideGuideText()
    {
        guidePanel.SetActive(false);
    }
}
