using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maps
{

    public class SelectedVisualMapBehavior : MonoBehaviour
    {
        public BasePanelBehavior myPanel;
        public List<CurrentMapBehavior> mapList;
        public CurrentMapBehavior currentMap;
        public void ShowCurrentMap(MapType thisType, bool initialShow = false)
        {

            for (int i = 0; i < mapList.Count; i++)
            {
                if (thisType != mapList[i].mapType)
                {
                    mapList[i].HidePoints();
                    mapList[i].gameObject.SetActive(false);
                }
                else
                {
                    mapList[i].gameObject.SetActive(true);
                    if (initialShow)
                    {
                        StartCoroutine(myPanel.WaitAnimationForAction(myPanel.openAnimationName, mapList[i].ShowPoints));
                    }
                    else
                    {
                        mapList[i].ShowPoints();
                    }
                }
            }
        }
    }
}
