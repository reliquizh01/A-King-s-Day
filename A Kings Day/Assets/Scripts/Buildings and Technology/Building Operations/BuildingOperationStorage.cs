using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Buildings
{
    public class BuildingOperationStorage : MonoBehaviour
    {
        public List<BuildingInformationData> buildingOperationList;

        public BuildingInformationData ObtainBuildingOperation(string buildingName)
        {
            return buildingOperationList.Find(x => x.BuildingName == buildingName);
        }

        public BuildingInformationData ObtainBuildingOperation(BuildingType type)
        {
            return buildingOperationList.Find(x => x.buildingType == type);
        }
    }
}
