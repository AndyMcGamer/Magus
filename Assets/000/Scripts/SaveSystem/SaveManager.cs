using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet.Managing.Scened;

namespace Magus.SaveSystem
{
    [System.Serializable]
    public class PlayerInfo
    {
        public string username;

        public PlayerInfo(string username)
        {
            this.username = username;
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public PlayerInfo playerInfo;
        public bool shownControls;

        public SaveData()
        {
            playerInfo = new("AnonymousPlayer");
        }
    }

    public interface IDataPersistence
    {
        void LoadData(SaveData data);

        void SaveData(ref SaveData data);
    }

    public class FileDataHandler
    {
        private string fileDirectory = "";
        private string fileName = "";

        public FileDataHandler(string fileDirectory, string fileName)
        {
            this.fileDirectory = fileDirectory;
            this.fileName = fileName;
        }

        public SaveData Load()
        {
            string fullPath = Path.Combine(fileDirectory, fileName);

            SaveData loadedData = null;

            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad = "";
                    using (FileStream stream = new(fullPath, FileMode.Open))
                    {
                        using StreamReader reader = new(stream);
                        dataToLoad = reader.ReadToEnd();
                    }

                    loadedData = JsonUtility.FromJson<SaveData>(dataToLoad);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error occurred when trying to load data to file: " + fullPath + "\n" + e);
                }

            }
            return loadedData;

        }

        public void Save(SaveData data)
        {

            string fullPath = Path.Combine(fileDirectory, fileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                string dataToStore = JsonUtility.ToJson(data, true);

                using FileStream stream = new(fullPath, FileMode.Create);
                using StreamWriter writer = new(stream);
                writer.Write(dataToStore);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error occurred when trying to save data to file: " + fullPath + "\n" + e);
            }
        }
    }


    public class SaveManager : MonoBehaviour
    {
        public static SaveManager instance;

        private SaveData data;
        private List<IDataPersistence> persistenceObjects;

        [SerializeField] private string fileName;

        private FileDataHandler dataHandler;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            dataHandler = new(Application.persistentDataPath, fileName);
        }

        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            persistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }

        public void OnSceneUnloaded(Scene scene)
        {
            SaveGame();
        }

        public void NewGame()
        {
            data = new();
        }

        public void LoadGame()
        {
            data = dataHandler.Load();

            if (data == null)
            {
                NewGame();
            }

            foreach (IDataPersistence persistence in persistenceObjects)
            {
                persistence.LoadData(data);
            }
        }

        public void SaveGame()
        {
            foreach (IDataPersistence persistence in persistenceObjects)
            {
                persistence.SaveData(ref data);
            }

            dataHandler.Save(data);
        }

        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            IEnumerable<IDataPersistence> dataPersistanceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

            return new List<IDataPersistence>(dataPersistanceObjects);
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

    }
}
