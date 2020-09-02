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
        [Tooltip("Player File To be used after tutorial.")]
        public PlayerKingdomData inheritanceData;
        

        public List<PlayerKingdomData> saveDataList;
        public PlayerKingdomData currentData;

        public List<PlayerCampaignData> saveCampaignDataList;
        public PlayerCampaignData currentCampaignData;

        [SerializeField]protected string savePath;


        public void Start()
        {
            ObtainSaves();
            ObtainCampaignSaves();
            EventBroadcaster.Instance.AddObserver(EventNames.SAVE_PLAYER_DATA, SaveCurrentData);
        }

        public void OnDestroy()
        {
            EventBroadcaster.Instance.RemoveActionAtObserver(EventNames.SAVE_PLAYER_DATA, SaveCurrentData);
        }

        // ONLY GETS CALLED AFTER PROLOGUE SCENE
        public void PostPrologueSave()
        {
            PlayerCampaignData temp = new PlayerCampaignData();

            SetNewSaveData(inheritanceData, temp);
        }

        public void SetNewSaveData(PlayerKingdomData newData, PlayerCampaignData campaignData)
        {
            if(saveDataList == null)
            {
                saveDataList = new List<PlayerKingdomData>();
            }
            // Check last Data Index
            saveDataList.Add(newData);
            
            if(saveCampaignDataList == null)
            {
                saveCampaignDataList = new List<PlayerCampaignData>();
            }
            saveCampaignDataList.Add(campaignData);

            SavePlayerCampaignData();
            SavePlayerData();

        }

        public void SaveCurrentData(Parameters p = null)
        {
            if(saveDataList == null || saveDataList.Count <= 0)
            {
                return;
            }
            int idx = saveDataList.FindIndex(x => x._fileName == PlayerGameManager.GetInstance.playerData._fileName);

            if(idx < 0)
            {
                saveDataList.Add(PlayerGameManager.GetInstance.playerData);
                SavePlayerData();
            }
            else
            {
                saveDataList[idx] = PlayerGameManager.GetInstance.playerData;

                BinaryFormatter bf = new BinaryFormatter();

                bf = new BinaryFormatter();
                FileStream file = File.Create(savePath + "/" + saveDataList[idx]._fileName);
                bf.Serialize(file, saveDataList[idx]);
                file.Close();

            }
            Debug.Log("------------------------------------ PLAYER DATA WAS SUCCESSFULLY SAVED! ------------");
        }

        public void SaveCurrentCampaignData(Parameters p = null)
        {
            if (saveCampaignDataList == null || saveCampaignDataList.Count <= 0 || TransitionManager.GetInstance.isNewGame)
            {
                return;
            }

            int idx = saveCampaignDataList.FindIndex(x => x._fileName == PlayerGameManager.GetInstance.campaignData._fileName);
            if(idx < 0)
            {
                saveCampaignDataList.Add(PlayerGameManager.GetInstance.campaignData);
                SavePlayerCampaignData();
            }
            else
            {
                saveCampaignDataList[idx] = PlayerGameManager.GetInstance.campaignData;

                BinaryFormatter bf = new BinaryFormatter();

                bf = new BinaryFormatter();
                FileStream file = File.Create(savePath + "/" + saveCampaignDataList[idx]._fileName);
                bf.Serialize(file, saveCampaignDataList[idx]);
                file.Close();

            }

            Debug.Log("------------------------------------ PLAYER CAMPAIGN DATA WAS SUCCESSFULLY SAVED! ------------");
        }

        public void SavePlayerData()
        {
            Debug.Log("SAVING FILES!");

            BinaryFormatter bf = new BinaryFormatter();

            this.savePath = Application.persistentDataPath + "/Saves/";

            for (int i = 0; i < saveDataList.Count; i++)
            {
                saveDataList[i]._fileName = "Save" + i + ".sav";
                saveDataList[i].fileData = true;

                if (saveDataList[i].fileData)
                {
                    bf = new BinaryFormatter();
                    FileStream file = File.Create(savePath + "/" + saveDataList[i]._fileName);
                    bf.Serialize(file, saveDataList[i]);
                    file.Close();

                    Debug.Log(savePath);
                }
            }
        }

        public void SavePlayerCampaignData()
        {
            Debug.Log("SAVING CAMPAIGN FILES!");

            BinaryFormatter bf = new BinaryFormatter();

            this.savePath = Application.persistentDataPath + "/Saves/";

            for (int i = 0; i < saveCampaignDataList.Count; i++)
            {
                saveCampaignDataList[i]._fileName = "CampaignSave" + i + ".dat";

                if (saveCampaignDataList[i].fileData)
                {
                    bf = new BinaryFormatter();
                    FileStream file = File.Create(savePath + "/" + saveCampaignDataList[i]._fileName);
                    bf.Serialize(file, saveCampaignDataList[i]);
                    file.Close();
                }
            }
        }

        public void DeleteCampaignData()
        {
            savePath = Application.persistentDataPath + "/Saves";

            if (currentCampaignData != null)
            {
                saveCampaignDataList.Remove(currentCampaignData);
                File.Delete(savePath + "/"+ currentCampaignData._fileName);

                #if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
                #endif
                currentCampaignData = null;
            }
        }
        public void DeleteData()
        {
            savePath = Application.persistentDataPath + "/Saves";

            if (currentData != null)
            {
                saveDataList.Remove(currentData);
                File.Delete(savePath + "/" + currentData._fileName);

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

            List<FileInfo> saveFileList = GetSavePlayerFiles(tmp);

            BinaryFormatter bf = new BinaryFormatter();

            if(saveFileList.Count > 0)
            {
                for (int i = 0; i < saveFileList.Count; i++)
                {
                    if(File.Exists(saveFileList[i].ToString()))
                    {
                        bf = new BinaryFormatter();
                        FileStream file = File.Open(saveFileList[i].ToString(), FileMode.Open);
                        PlayerKingdomData temp = new PlayerKingdomData();
                        temp = (PlayerKingdomData)bf.Deserialize(file);
                        saveDataList.Add(temp);
                        file.Close();
                    }
                }
            }
        }

        public void ObtainCampaignSaves()
        {
            savePath = Application.persistentDataPath + "/Saves";

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            DirectoryInfo tmp = new DirectoryInfo(savePath);

            List<FileInfo> savecampaignFileList = GetSaveCampaignFiles(tmp);

            BinaryFormatter bf2 = new BinaryFormatter();

            if (savecampaignFileList.Count > 0)
            {
                for (int i = 0; i < savecampaignFileList.Count; i++)
                {
                    if (File.Exists(savecampaignFileList[i].ToString()))
                    {
                        bf2 = new BinaryFormatter();
                        FileStream file = File.Open(savecampaignFileList[i].ToString(), FileMode.Open);
                        saveCampaignDataList.Add((PlayerCampaignData)bf2.Deserialize(file));
                        file.Close();
                    }
                }
            }
        }
        public void SetCurrentData(int saveSlot)
        {
            currentData = saveDataList[saveSlot];
            currentCampaignData = saveCampaignDataList[saveSlot];
        }

        public List<FileInfo> GetSavePlayerFiles(DirectoryInfo d)
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

        public List<FileInfo> GetSaveCampaignFiles(DirectoryInfo d)
        {
            List<FileInfo> saveFiles = new List<FileInfo>();

            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                if (fi.Extension.Contains(".dat"))
                {
                    saveFiles.Add(fi);
                }

            }

            return saveFiles;
        }
    }
}