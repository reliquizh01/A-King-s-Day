using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Technology;
using UnityEngine;
using Kingdoms;
using Utilities;
using Managers;

namespace SaveData
{
    public class SaveLoadManager : MonoBehaviour
    {
        #region Singleton
        private static SaveLoadManager instance;
        public static SaveLoadManager GetInstance
        {
            get
            {
                return instance;
            }
        }
        public void Awake()
        {
            if (SaveLoadManager.GetInstance == null)
            {
                Debug.Log(gameObject.name);
                if(this.transform.parent == null)
                {
                    DontDestroyOnLoad(this.gameObject);
                }
                instance = this;
            }
            else
            {
                DestroyImmediate(this.gameObject);
            }
        }
        #endregion

        [SerializeField]private int curSaveSlotIdx;

        public List<PlayerKingdomData> saveDataList;
        public PlayerKingdomData currentData;


        [SerializeField]protected string savePath;


        public void Start()
        {
            ObtainSaves();
            EventBroadcaster.Instance.AddObserver(EventNames.SAVE_PLAYER_DATA, SaveCurrentData);
        }

        public void OnDestroy()
        {
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.SAVE_PLAYER_DATA, SaveCurrentData);
        }

        public void SetNewSaveData(PlayerKingdomData newData)
        {
            // Check last Data Index
            saveDataList.Add(newData);
            
        }

        public void SaveCurrentData(Parameters p = null)
        {
            saveDataList[curSaveSlotIdx] = PlayerGameManager.GetInstance.playerData;

            BinaryFormatter bf = new BinaryFormatter();

            bf = new BinaryFormatter();
            FileStream file = File.Create(savePath + "/Save" + curSaveSlotIdx + ".sav");
            bf.Serialize(file, saveDataList[curSaveSlotIdx]);
            file.Close();

            Debug.Log("------------------------------------ DATA WAS SUCCESSFULLY SAVED! ------------");
        }

        public void SetCurrentSlot(int newSlotIndex)
        {
            curSaveSlotIdx = newSlotIndex;
            SaveData();
        }

        public void SaveData()
        {
            Debug.Log("SAVING FILES!");

            BinaryFormatter bf = new BinaryFormatter();

            this.savePath = Application.persistentDataPath + "/Saves/";
            for (int i = 0; i < saveDataList.Count; i++)
            {
                if(saveDataList[i].fileData)
                {
                    bf = new BinaryFormatter();
                    FileStream file = File.Create(savePath + "/Save" + i + ".sav");
                    bf.Serialize(file, saveDataList[i]);
                    file.Close();

                    Debug.Log(savePath);
                }
            }
        }

        public void DeleteData()
        {
            savePath = Application.persistentDataPath + "/Saves";

            if (currentData != null)
            {
                File.Delete(savePath + "/Save" + curSaveSlotIdx + ".sav");
                saveDataList.RemoveAt(curSaveSlotIdx);

                #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
                #endif
                currentData = null;
            }
        }
        public void ObtainSaves()
        {
            savePath = Application.persistentDataPath + "/Saves";

            if(!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            DirectoryInfo tmp = new DirectoryInfo(savePath);

            List<FileInfo> saveFileList = GetSaveFiles(tmp);

            BinaryFormatter bf = new BinaryFormatter();

            if(saveFileList.Count > 0)
            {
                for (int i = 0; i < saveFileList.Count; i++)
                {
                    if(File.Exists(saveFileList[i].ToString()))
                    {
                        bf = new BinaryFormatter();
                        FileStream file = File.Open(saveFileList[i].ToString(), FileMode.Open);
                        saveDataList.Add((PlayerKingdomData)bf.Deserialize(file));
                        file.Close();
                    }
                }
            }
        }
        public void LoadSlot(int saveSlot)
        {

            currentData = saveDataList[saveSlot];
            SetCurrentSlot(saveSlot);
        }

        public List<FileInfo> GetSaveFiles(DirectoryInfo d)
        {
            List<FileInfo> saveFiles = new List<FileInfo>();

            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                if (fi.Extension.Contains(".sav"))
                {
                    saveFiles.Add(fi);
                }
            
            }

            return saveFiles;
        }

    }
}