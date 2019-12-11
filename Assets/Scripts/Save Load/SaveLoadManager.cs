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
        // save "Local(relative to the parent tree)" position, not "World" position
        public List<Vector3> decorationItemLocalPos;
        public List<Color> decorationItemColors;
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
            if (info.Exists) 
            {
                var reader = new StreamReader(info.OpenRead());
                var json = reader.ReadToEnd();
                var data = JsonUtility.FromJson<TreeSaveData>(json);

                // ロードしたデータを反映
                ObjectPlacement.christmasTree.decorationItemLocalPos = data.decorationItemLocalPos;
                ObjectPlacement.christmasTree.decorationItemColors = data.decorationItemColors;
            }
        }

        private TreeSaveData CreateSaveData() 
        {
            TreeSaveData save = new TreeSaveData();
            save.decorationItemLocalPos = ObjectPlacement.christmasTree.decorationItemLocalPos;
            save.decorationItemColors = ObjectPlacement.christmasTree.decorationItemColors;
            return save;
        }
    }
}