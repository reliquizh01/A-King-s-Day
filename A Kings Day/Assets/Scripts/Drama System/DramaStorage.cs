using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Drama
{
    public enum DramaActorType
    {
        Generic,
        Unique,
        Tools,
        SFX,
    }
        

    [Serializable]
    public class ActorStorageBay
    {
        public string actorName;
        public string smallDescription;
        public DramaActorType actorType;
        public GameObject actorPrefab;
        public string prefabPath = "";
    }
    public class DramaStorage : MonoBehaviour
    {
        public List<DramaScenario> dramaSceneStorage;

        public List<ActorStorageBay> actorStorage;

        public DramaScenario ObtainDramaByTitle(string title)
        {
            DramaScenario tmp = new DramaScenario();
            tmp = dramaSceneStorage.Find(x => x.scenarioName == title);

            return tmp;
        }
    }
}