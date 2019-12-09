using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ARChristmas
{
    [Serializable]
    public class TreeSaveData
    {
        public List<Vector3> decoItemPositions;

    }   

    public class SaveLoadManager : MonoBehaviour
    {
        public void OnSaveTree() 
        {
            TreeSaveData save = CreateSaveData();
            var json = JsonUtility.ToJson(save);
            var path = Application.persistentDataPath + "/save.txt";
            var writer = new StreamWriter(path, false);
            writer.WriteLine(json);
            writer.Flush();
            writer.Close();
        }

        public void OnLoadTree()
        {
            var info = new FileInfo(Application.persistentDataPath + "/save.txt");
            if (info != null) 
            {
                var reader = new StreamReader(info.OpenRead());
                var json = reader.ReadToEnd();
                var data = JsonUtility.FromJson<TreeSaveData>(json);
                // ロードしたデータを反映
            }
        }

        private TreeSaveData CreateSaveData() 
        {
            TreeSaveData save = new TreeSaveData();
            save.decoItemPositions = ObjectPlacement.christmasTree.decoItemPositions;

            return save;
        }
    }
}