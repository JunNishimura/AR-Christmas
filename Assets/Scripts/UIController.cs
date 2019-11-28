using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ARChristmas
{
    public class UIController : MonoBehaviour
    {
        public TextMeshProUGUI playModeText;
        public GameObject Inventory;
        public GameObject InventoryDisplayIcon; // もう少し良い変数名前を考えよう。
        private PlayMode prevPlayMode;
        private bool isInventoryON;

        private void Start() 
        {
            Inventory.SetActive(false);
            InventoryDisplayIcon.SetActive(false);
            isInventoryON = false;

            prevPlayMode = ObjectPlacement.currentPlayMode;
            UpdatePlayModeText();
        }

        private void Update() 
        {
            if (prevPlayMode != ObjectPlacement.currentPlayMode) 
            {
                prevPlayMode = ObjectPlacement.currentPlayMode;
                UpdatePlayModeText();
            }
        }

        /// <summary>
        /// Display the current play mode
        /// </summary>
        public void UpdatePlayModeText() 
        {
            if (ObjectPlacement.currentPlayMode == PlayMode.ChristmasTree) 
            {
                playModeText.text = "Mode: Christmas Tree";
            }
            else if (ObjectPlacement.currentPlayMode == PlayMode.Decoration)
            {
                playModeText.text = "Mode: Decoration";
                DisplayToggle();
            }
        }

        /// <summary>
        /// InventoryUIの表示/非表示を切り替える
        /// </summary>
        public void DisplayToggle() 
        {
            if (isInventoryON) 
            {
                Inventory.SetActive(false);
                InventoryDisplayIcon.SetActive(true);
            }
            else 
            {
                Inventory.SetActive(true);
                InventoryDisplayIcon.SetActive(false);
            }
            isInventoryON = !isInventoryON;
        }

        /// <summary>
        ///　生成するデコレーションアイテムをタップされたアイテムに切り替える
        /// </summary>
        public void TapDecorationItem(int id) 
        {
            ObjectPlacement.decorationItemIndex = id;
        }
    }
}