using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Dialogue;

public enum DialogueDeliveryType
{
    Continuous,
    EnterAfterEachSpeech,
}

public class DialogueController : MonoBehaviour
{
    [Header("Dialogue Controlled Information")]
    public float maxDialogueSpeed = 1.0f;
    public float curDialogueSpeed = 0.8f;
    public DialogueDeliveryType deliveryType;

    [Header("User Interface Controls")]
    public Slider speedSlider;
    public TMP_Dropdown optionDropdown;
    [Header("Sample Messages")]
    public TypeWriterEffectUI mesgText;
    public List<string> messageList;

    public void Start()
    {
        InitialSlide();
        StartMessageSample();
    }

    public void InitialSlide()
    {
        speedSlider.maxValue = maxDialogueSpeed;
        speedSlider.value = curDialogueSpeed;
    }
    public void UpdateInitialSlides()
    {
        deliveryType = (DialogueDeliveryType)optionDropdown.value;
        if(deliveryType == DialogueDeliveryType.EnterAfterEachSpeech)
        {
            mesgText.allowMesgControl = true;
        }
        else
        {
            mesgText.allowMesgControl = false;
        }
        speedSlider.maxValue = maxDialogueSpeed;
        curDialogueSpeed = speedSlider.value;
        mesgText.intervalPerLetter = curDialogueSpeed;
        mesgText.deliveryType = deliveryType;

        if(DialogueManager.GetInstance != null)
        {
            DialogueManager.GetInstance.UpdateDialogueMechanics(deliveryType, curDialogueSpeed);
        }
    }

    public void StartMessageSample()
    {
        int rand = UnityEngine.Random.Range(0, messageList.Count);

        mesgText.SetTypeWriterMessage(messageList[rand], true, StartMessageSample);
    }

}
