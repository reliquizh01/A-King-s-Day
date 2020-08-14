using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Kingdoms;

namespace Maps
{

    public class MapPointBehavior : MonoBehaviour
    {
        public GameObject arrowClickedIcon;
        public bool isClicked;
        public BasePanelBehavior myAnim;

        [Header("Point Mechanics")]
        public CurrentMapBehavior myController;
        public MapPointInformationData myPointInformation;

        [Header("Icon Mechancis")]
        public Image flagIcon;
        public Sprite playerSprite, khanSprite, holySeeSprite;
        public Sprite neutralSprite;
        [Header("Path System")]
        public List<MapPointBehavior> neighborPoints;

        public void SetAsClicked()
        {
            if(myController.selectedMapPoint == this)
            return;

            arrowClickedIcon.SetActive(true);
            myController.SetAsSelectedPoint(this);
        }

        public void RemoveClicked()
        {
            arrowClickedIcon.SetActive(false);
        }

        public void OnDisable()
        {
            arrowClickedIcon.SetActive(false);
        }
        public void OnEnable()
        {
            myAnim.myAnim.Play("Show Map Animation");
        }
        public void LoadMapPointInformation(MapPointInformationData mapPointInformationData)
        {
            myPointInformation = new MapPointInformationData();
            myPointInformation = mapPointInformationData;
            myPointInformation.mapType = myController.mapType;
            UpdateFlagCrest();
        }
        public void RandomizeTravellerMovement()
        {
            if (myPointInformation.travellersOnPoint == null && myPointInformation.travellersOnPoint.Count <= 0)
            {
                return;
            }

            List<int> removedTravellers = new List<int>();
            for (int i = 0; i < myPointInformation.travellersOnPoint.Count; i++)
            {
                int moveChance = UnityEngine.Random.Range(0, 100);
                if (moveChance > 50)
                {
                    MoveThisTravellerToNeighbor(myPointInformation.travellersOnPoint[i]);
                    removedTravellers.Add(i);
                }
            }

            if (removedTravellers.Count > 0)
            {
                for (int i = 0; i < removedTravellers.Count; i++)
                {
                    myPointInformation.travellersOnPoint.RemoveAt(removedTravellers[i]);
                }
            }
        }

        public void MoveThisTravellerToNeighbor(BaseTravellerData thisTraveller)
        {
            int moveToIdx = 0;
            if (neighborPoints != null && neighborPoints.Count > 0)
            {
                moveToIdx = UnityEngine.Random.Range(0, neighborPoints.Count - 1);
            }

            neighborPoints[moveToIdx].ReceiveTraveller(thisTraveller);
        }

        public void ReceiveTraveller(BaseTravellerData thisTraveller)
        {
            BaseTravellerData temp = new BaseTravellerData();
            temp = thisTraveller;

            myPointInformation.travellersOnPoint.Add(temp);
        }

        public void ConqueredBy(TerritoryOwners newOwner)
        {
            Debug.Log("New Owner of : " + myPointInformation.pointName + " is : " + newOwner.ToString());
            myPointInformation.ownedBy = newOwner;
            SaveData.SaveLoadManager.GetInstance.SaveCurrentCampaignData();

            StartCoroutine(myAnim.WaitAnimationForAction("Map Point Conquered Animation", UpdateFlagCrest));

        }

        public void UpdateFlagCrest()
        {
            switch (myPointInformation.ownedBy)
            {
                case TerritoryOwners.Neutral:
                    flagIcon.sprite = neutralSprite;
                    break;
                case TerritoryOwners.HolySee:
                    flagIcon.sprite = holySeeSprite;
                    break;
                case TerritoryOwners.FurKhan:
                    flagIcon.sprite = khanSprite;
                    break;
                case TerritoryOwners.Gates:

                    break;
                case TerritoryOwners.Player:
                    flagIcon.sprite = playerSprite;
                    break;
                default:
                    break;
            }
        }
    }
}