using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


namespace Buildings
{

    public class CloseOption : MonoBehaviour
    {
        public BuildingOptionHandler myController;
        public bool isClickable = true;

        public void OnMouseDown()
        {
            if(myController.isOpen && isClickable)
            {
                isClickable = false;
                myController.CloseIcons();
            }
        }
    }
}
