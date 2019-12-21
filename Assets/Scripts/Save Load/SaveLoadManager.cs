using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ARChristmas.Save
{
    // ファイルのセーブ / ロードを行うスクリプト
    public static class SaveLoadManager <T>
    {
        /// <summary> 
        /// Save object to designated path.
        /// </summary>
        public static void SaveData(T saveObject, string pathToSave) 
        {
            var json = JsonUtility.ToJson(saveObject);
            var writer = new StreamWriter(pathToSave, false);
            writer.WriteLine(json);
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Load object from designated path
        /// </summary>
        public static T GetLoadData(string pathToLoad) 
        {
            var file = new FileInfo(pathToLoad);
            if (file.Exists)
            {
                var reader = new StreamReader(file.OpenRead());
                var json = reader.ReadToEnd();
                var loadedData = JsonUtility.FromJson<T>(json);
                return loadedData;
            }
            else 
            {
                Debug.Log("There is no file to load");
                return default(T);
            }
        }
    }
}