using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using TMPro;
using Kingdoms;
using Buildings;
using ResourceUI;

namespace Technology
{
    public class TechnologyTabHandler : MonoBehaviour
    {
        public BasePanelBehavior myPanel;
        public TextMeshProUGUI title;
        public BaseTechnologyPageBehavior techPages;

        public void OpenTechnologyTab(BuildingType bldgType)
        {
            this.gameObject.SetActive(true);
            ResourceType resType = ResourceType.Population;

            switch (bldgType)
            {
                    
                case BuildingType.Barracks:
                    resType = ResourceType.Troops;
                    break;
                case BuildingType.Houses:
                    resType = ResourceType.Population;
                    break;
                case BuildingType.Farm:
                    resType = ResourceType.Food;
                    break;
                case BuildingType.Market:
                    resType = ResourceType.Coin;
                    break;

                case BuildingType.Tavern:
                case BuildingType.Smithery:
                case BuildingType.Shop:
                default:
                    break;
            }
            if (ResourceInformationController.GetInstance != null)
            {
                ResourceInformationController.GetInstance.ShowResourcePanel(ResourcePanelType.side);
            }

            StartCoroutine(myPanel.WaitAnimationForAction(myPanel.openAnimationName,()=> OpenTechPage(resType)));
        }

        public void OpenTechPage(ResourceType type)
        {
            techPages.OpenPageTech(type);
        }

        public void HideTechnologyTab()
        {
            techPages.ClosePageTech(CloseTechnologyTab);
            EventBroadcaster.Instance.PostEvent(EventNames.DISABLE_TAB_COVER);

            if (ResourceInformationController.GetInstance != null)
            {
                ResourceInformationController.GetInstance.ShowResourcePanel(ResourcePanelType.overhead);
            }
        }

        public void CloseTechnologyTab()
        {
            myPanel.PlayCloseAnimation();
        }
    }
}
