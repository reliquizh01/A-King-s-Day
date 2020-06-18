using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using TMPro;
using Kingdoms;

namespace Technology
{
    public class TechnologyTabHandler : MonoBehaviour
    {
        public BasePanelBehavior myPanel;
        public TextMeshProUGUI title;
        public BaseTechnologyPageBehavior techPages;

        public void OpenTechnologyTab(ResourceType pageType)
        {
            this.gameObject.SetActive(true);
            StartCoroutine(myPanel.WaitAnimationForAction(myPanel.openAnimationName,()=> OpenTechPage(pageType)));
        }

        public void OpenTechPage(ResourceType type)
        {
            techPages.OpenPageTech(type);
        }

        public void HideTechnologyTab()
        {
            techPages.ClosePageTech(CloseTechnologyTab);
            EventBroadcaster.Instance.PostEvent(EventNames.DISABLE_TAB_COVER);
        }

        public void CloseTechnologyTab()
        {
            myPanel.PlayCloseAnimation();
        }
    }
}
