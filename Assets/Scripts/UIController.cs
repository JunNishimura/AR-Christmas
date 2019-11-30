using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ARChristmas
{
    public class UIController : MonoBehaviour
    {
        public GameObject CameraFlashEffect;
        public Button settingButton;
        public Button backButton;
        public Button scaleButton;
        public Slider scaleSlider;
        public Button decorationButton;
        public GameObject decorationInventory;
        public Button cameraModeButton;
        public Button captureButton;

        private PlayMode prevMode;
        private Image flashEffectImage;
        private bool isFlashEffect;

        private void Start() 
        {
            ControlUIActivation(false, false, false, false, false, false, false, false, false);
            prevMode = ObjectPlacement.currentPlayMode;
            isFlashEffect = false;
        }

        private void Update() 
        {
            // initialize uGUI after a christmas tree appears on the screen
            if (ObjectPlacement.currentPlayMode != prevMode)
            {
                InitializeUI();
                prevMode = ObjectPlacement.currentPlayMode;
            }

            // camera flash effect
            if (CameraFlashEffect.activeSelf) 
            {
                if(! isFlashEffect) 
                {
                    isFlashEffect = true;
                    flashEffectImage = CameraFlashEffect.GetComponent<Image>();
                    flashEffectImage.color = Color.white;
                }
                else 
                {
                    flashEffectImage.color = Color.Lerp(flashEffectImage.color, Color.clear, Time.deltaTime);

                    // finish camera flash effect
                    if (flashEffectImage.color.a <= 0.05f)
                    {
                        flashEffectImage.color = Color.clear;
                        isFlashEffect = false;
                        ControlUIActivation(false, true, false, false, false, false, false, false, false);
                    }
                }
            }
        }

        /// <summary>
        ///　生成するデコレーションアイテムをタップされたアイテムに切り替える
        /// </summary>
        public void TapDecorationItem(int id) 
        {
            ObjectPlacement.decorationItemIndex = id;
        }

        private void InitializeUI() 
        {
            // all buttons need to be true to set click event
            ControlUIActivation(false, true, true, true, true, true, false, true, true);

            // set click event for each button
            settingButton.onClick.AddListener(() => ControlUIActivation(false, false, true, true, false, true, false, true, false));
            backButton.onClick.AddListener(() => ControlUIActivation(false, true, false, false, false, false, false, false, false));
            scaleButton.onClick.AddListener(() => ControlUIActivation(false, false, true, false, true, false, false, false, false));
            scaleSlider.onValueChanged.AddListener(GameObject.FindObjectOfType<ObjectPlacement>().ScaleChristmasTree);
            decorationButton.onClick.AddListener(() => ControlUIActivation(false, false, true, false, false, false, true, false, false));
            cameraModeButton.onClick.AddListener(() => ControlUIActivation(false, false, true, false, false, false, false, false, true));
            captureButton.onClick.AddListener(captureButton.gameObject.GetComponent<ScreenShot>().ScreenShotPressed);
            
            // setting button can be only true as default
            ControlUIActivation(false, true, false, false, false, false, false, false, false);
        }

        public void ControlUIActivation(bool isFlashEffect, bool isSetting, bool isBack, bool isScale, bool isScaleSlider, bool isDecoration, bool isDecorationInv, bool isCameraMode, bool isCapture) 
        {
            CameraFlashEffect.SetActive(isFlashEffect);
            settingButton.gameObject.SetActive(isSetting);
            backButton.gameObject.SetActive(isBack);
            scaleButton.gameObject.SetActive(isScale);
            scaleSlider.gameObject.SetActive(isScaleSlider);
            decorationButton.gameObject.SetActive(isDecoration);
            decorationInventory.SetActive(isDecorationInv);
            cameraModeButton.gameObject.SetActive(isCameraMode);
            captureButton.gameObject.SetActive(isCapture);
        }
    }
}