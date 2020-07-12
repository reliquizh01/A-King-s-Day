using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlefield;
using UnityEngine.UI;


[System.Serializable]
public class KeyboardSpriteData
{
    public Sprite sprite;
    public KeyCode thisKey;
}
public class InteractiveControlInformation : MonoBehaviour
{
    public ControlsInformationHandler myController;
    public Image keyboardIcon;
    public Sprite unclickedKeyboard;
    public PlayerControlType playerControl;
    public bool showInteractiveControl;
    public int currentClickedIdx;

    public List<KeyboardSpriteData> keyboardSpriteList;
   
    public void Update()
    {
        if(showInteractiveControl)
        {
            if(keyboardSpriteList != null && keyboardSpriteList.Count > 0)
            {
                bool noClicked = true;
                for (int i = 0; i < keyboardSpriteList.Count; i++)
                {
                    if(Input.GetKey(keyboardSpriteList[i].thisKey) && noClicked)
                    {
                        AdjustKeyboardShow(i);
                        noClicked = false;
                    }
                }

                if(noClicked)
                {
                    keyboardIcon.sprite = unclickedKeyboard;
                }
            }
        }
    }

    public void AdjustKeyboardShow(int idx)
    {
        keyboardIcon.sprite = keyboardSpriteList[idx].sprite;
        currentClickedIdx = idx;

        myController.ShowThisVisuals(currentClickedIdx, playerControl);
    }

}
