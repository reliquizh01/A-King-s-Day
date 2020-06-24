using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotPanel : MonoBehaviour
{
    public Image myBg;
    public Button myBtn;
    public Sprite highlightSprite;
    public Sprite normalSprite;

    public SaveSlotHandler myController;
    public GameObject emptyPanel;
    [Header("Kingdom information")]
    public TextMeshProUGUI kingdomName;
    public TextMeshProUGUI gameDifficulty;
    public TextMeshProUGUI weeksCount;
    [Header("Resource Information")]
    public TextMeshProUGUI coinsCount;
    public TextMeshProUGUI troopsCount;
    public TextMeshProUGUI populationCount;
    public TextMeshProUGUI foodCount;

    public void Start()
    {

        myBtn.onClick.AddListener(SetAsSelected);
    }
    public void SetAsSelected()
    {
        myController.SetAsSelectedPanel(this);
    }

}
