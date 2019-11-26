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
        public GameObject InventoryDisplayIcon;
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
        /// 現在のプレイモードを表示する
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
        /// Buttonから呼ばれる
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
        /// タッチされたdecoration itemのiconと同じオブジェクトが生成されるようにインデックスを変更する
        /// </summary>
        public void OnClick(int id) 
        {
            ObjectPlacement.decorationItemIndex = id;
        }
    }
}