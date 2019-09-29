using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Data
{
    public static class Progression
    {
        
        public static SaveData CurrentlyLoaded
        {
            get
            {
                if (!Load())
                {
                    NewSave();
                }
                return data;
            }
        }
        
        private static string filePath = "/saves";
        private static SaveData data;


        public static void NewSave()
        {
            Debug.Log("[NEW] New Save");
            data = new SaveData();
            Save();
        }
        
        public static void Delete()
        {
            Debug.Log("[DELETE] Deleted Save.");
            NewSave();
        }

        public static void Save()
        {
            if (data == null)
            {
                NewSave();
                return;
            }
            
            if (!Directory.Exists(Application.dataPath + filePath))
                Directory.CreateDirectory(Application.dataPath + filePath);
            
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Create(Application.dataPath + filePath + "/SaveData.dat");
            
            string json = JsonUtility.ToJson(data);
            
            bf.Serialize(file, json);
            
            file.Close();
            
            Debug.Log("[SAVE] Saved game.");
        }

        public static bool Load()
        {
            if (File.Exists(Application.dataPath + filePath + "/SaveData.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();

                FileStream file = File.Open(Application.dataPath + filePath + "/SaveData.dat", FileMode.Open);

                string json = bf.Deserialize(file).ToString();

                data = JsonUtility.FromJson<SaveData>(json);

                Debug.Log("[LOAD] Loaded Save");

                file.Close();

                return true;
            }
            else
            {
                NewSave();
                return false;
            }
        }
    }
}
