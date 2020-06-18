using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using Technology;
namespace Buildings
{
    public enum OptionType
    {
        Upgrade,
        Technology,
        Use,
    }

    public class BaseOptionIcon : MonoBehaviour
    {
        public SpriteRenderer iconBackground;
        public SpriteRenderer optionIcon;


        private Vector3 highlightSize = new Vector3(1.25f, 1.25f, 1.25f);
        private Color highlightColor = Color.green;
        public void SetOptionIcon(Sprite sprite)
        {
            optionIcon.sprite = sprite;
        }


        public virtual void OnMouseDown()
        {
            transform.localScale = highlightSize;
        }
        public virtual void OnMouseUp()
        {
            transform.localScale = new Vector3(1,1,1);
            iconBackground.color = Color.white;
        }
        public virtual void OnMouseEnter()
        {
            iconBackground.color = highlightColor;
            
        }
        public virtual void OnMouseExit()
        {
            iconBackground.color = Color.white;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
