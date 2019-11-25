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
        public GameObject decorationItemListObj;
        private PlayMode prevPlayMode;

        private void Start() 
        {
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

        public void UpdatePlayModeText() 
        {
            if (ObjectPlacement.currentPlayMode == PlayMode.ChristmasTree) 
            {
                playModeText.text = "Mode: Christmas Tree";
                decorationItemListObj.SetActive(false);
            }
            else if (ObjectPlacement.currentPlayMode == PlayMode.Decoration)
            {
                playModeText.text = "Mode: Decoration";
                decorationItemListObj.SetActive(true);
            }
        }

        // ここのコードもっと綺麗にしたい
        public void DecorationItem1() 
        {
            ObjectPlacement.decorationItemIndex = 0;
        }

        public void DecorationItem2() 
        {
            ObjectPlacement.decorationItemIndex = 1;
        }

        public void DecorationItem3() 
        {
            ObjectPlacement.decorationItemIndex = 2;
        }

        public void DecorationItem4() 
        {
            ObjectPlacement.decorationItemIndex = 3;
        }
    }
}