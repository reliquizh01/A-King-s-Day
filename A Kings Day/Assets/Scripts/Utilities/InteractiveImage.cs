using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public enum ImageInteractionType
    {
        Normal,
        fill,
        blinking,
        fadeIn,
        fadeOut,
    }
    public class InteractiveImage : MonoBehaviour
    {
        public Image myImage;
        public ImageInteractionType interactionType;


        [SerializeField]private float initialFillCount = 0.15f;
        [SerializeField]private float targetFillCount = 0;
        private float fillSpeed = 2.75f;
        [SerializeField]private bool startFilling = false;


        public void StartFilling(float targetFill)
        {
            targetFillCount = targetFill;
            startFilling = true;
        }

        public void Update()
        {
            if(startFilling)
            {
                if(myImage.fillAmount > targetFillCount)
                {

                    myImage.fillAmount -= fillSpeed * Time.deltaTime;

                    if (myImage.fillAmount <= initialFillCount || myImage.fillAmount <= targetFillCount)
                    {
                        startFilling = false;
                    }
                }
                else
                {
                    myImage.fillAmount += fillSpeed * Time.deltaTime;
                    if (myImage.fillAmount >= targetFillCount)
                    {
                        startFilling = false;
                    }
                }
            }
        }
    }
}
