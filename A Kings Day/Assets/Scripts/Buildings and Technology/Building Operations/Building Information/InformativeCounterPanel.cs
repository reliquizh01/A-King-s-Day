using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kingdoms;
using KingEvents;
using Utilities;
using TMPro;
using ResourceUI;

namespace Buildings
{
    public class InformativeCounterPanel : CardPanelHandler
    {
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI counterText;
        public TextMeshProUGUI countDescriptionText;
        public TextMeshProUGUI potentialGrowthCountText;

        private string currentTitle;
        private string currentDescription;
        private string currentCount;

        public override void InitializePanel(Parameters p = null)
        {
            base.InitializePanel(p);


            if (p.HasParameter("Description"))
            {
                currentDescription = p.GetWithKeyParameterValue("Description", "");

                if (!string.IsNullOrEmpty(currentDescription))
                {
                    countDescriptionText.text = currentDescription;
                }
            }
            if (p.HasParameter("Count"))
            {
                currentCount = p.GetWithKeyParameterValue("Count", "");

                if (!string.IsNullOrEmpty(currentCount))
                {
                    counterText.text = currentCount;
                }
            }

            if (p.HasParameter("Title"))
            {
                currentTitle = p.GetWithKeyParameterValue("Title", "");

                if (!string.IsNullOrEmpty(currentTitle))
                {
                    titleText.text = currentTitle;
                }
            }

            if (p.HasParameter("Growth"))
            {
                potentialGrowthCountText.text = p.GetWithKeyParameterValue("Growth", "");
            }
        }


        public override void ShowPanelPotential(Parameters p = null)
        {
            base.ShowPanelPotential(p);

            string newTitle = "";
            string newDescription = "";
            string newCount = "";
            
            if (p.HasParameter("Description"))
            {
                newDescription = p.GetWithKeyParameterValue("Description", "");

                if (!string.IsNullOrEmpty(newDescription))
                {
                    countDescriptionText.text = newDescription;
                }
            }
            if (p.HasParameter("Count"))
            {
                newCount = p.GetWithKeyParameterValue("Count", "");

                if (!string.IsNullOrEmpty(newCount))
                {
                    counterText.text = newCount;
                }
            }

            if (p.HasParameter("Title"))
            {
                newTitle = p.GetWithKeyParameterValue("Title", "");

                if (!string.IsNullOrEmpty(newTitle))
                {
                    titleText.text = newTitle;
                }
            }

        }
        public override void HidePanelPotential()
        {
            base.HidePanelPotential();

            if(countDescriptionText != null)
            {
                countDescriptionText.text = currentDescription;
            }
            if(counterText != null)
            {
                counterText.text = currentCount;
            }
            if(titleText != null)
            {
                titleText.text = currentTitle;
            }

        }
    }
}
