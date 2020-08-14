using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Maps
{
    public class CurrentMapBehavior : MonoBehaviour
    {
        public MapType mapType;
        public List<GameObject> pointPanels;

        [Header("Map Information Mechanics")]
        public MapInformationBehavior mapInformationHandler;
        [Header("Map Points")]
        public List<MapPointBehavior> myMapPoints;
        public MapPointBehavior selectedMapPoint;

        public void ShowPoints()
        {
            for (int i = 0; i < pointPanels.Count; i++)
            {
                pointPanels[i].SetActive(true);
            }
        }

        public void HidePoints()
        {
            for (int i = 0; i < pointPanels.Count; i++)
            {
                pointPanels[i].SetActive(false);
            }
        }

        public void SetAsSelectedPoint(MapPointBehavior thisPoint)
        {
            if(selectedMapPoint != null)
            {
                selectedMapPoint.RemoveClicked();
            }

            selectedMapPoint = thisPoint;
            mapInformationHandler.OnPointSelected();
        }

        public void ConquerMapPoint(List<MapPointInformationData> thisMapPoints)
        {
            List<int> removeConverted = new List<int>();
            for (int i = 0; i < thisMapPoints.Count; i++)
            {
                if(thisMapPoints[i].mapType == mapType)
                {
                    MapPointBehavior thisPoint = myMapPoints.Find(x => x.myPointInformation.pointName == thisMapPoints[i].pointName);
                    if(thisPoint != null)
                    {
                        thisPoint.ConqueredBy(thisMapPoints[i].ownedBy);
                        removeConverted.Add(i);
                    }
                }
            }

            if(removeConverted.Count > 0)
            {
                for (int i = 0; i < removeConverted.Count; i++)
                {
                    thisMapPoints.RemoveAt(removeConverted[i]);
                }
            }
        }
    }
}