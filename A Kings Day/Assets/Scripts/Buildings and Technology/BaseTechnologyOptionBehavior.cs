using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Technology
{
    public class BaseTechnologyOptionBehavior : MonoBehaviour
    {
        public Image techIcon;
        public Image bgTechIcon;
        public Image bgTechFill;
        public Image highlight;

        public bool isSelected = false;
        private RectTransform highlightRect;

        public void Start()
        {
            highlightRect = highlight.rectTransform;
        }
        public void Update()
        {
            if(isSelected)
            {
                highlightRect.Rotate(0, 0, 2.5f * Time.deltaTime);
            }
        }

        public void Select()
        {
            isSelected = true;
            highlight.gameObject.SetActive(true);
        }
        public void DeSelect()
        {
            isSelected = false;
            highlight.gameObject.SetActive(false);
            highlightRect.rotation = new Quaternion(0, 0, 0,0);
        }
    }

}