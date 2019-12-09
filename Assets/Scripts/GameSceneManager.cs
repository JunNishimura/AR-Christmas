using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// すること
// saveとloadボタンのコールバック登録
// game managerを共通シーンで残らせる


namespace ARChristmas
{
    public class GameSceneManager : MonoBehaviour
    {
        public static bool isTreeInTheScene { get; set; }
        public static bool isNewTree { get; set; }

        // decide if the user creates new christmas tree or load saved tree.
        public void OnTreeTypeSelecct(bool isNewTree) 
        {
            if (isNewTree) GameSceneManager.isNewTree = true;
            else isNewTree = false;
        }
    }
}