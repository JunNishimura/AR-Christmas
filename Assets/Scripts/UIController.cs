﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

namespace ARChristmas
{
    public class UIController : MonoBehaviour
    {
        public GameObject CameraFlashEffect;
        public Button settingButton;
        public Button backButton;
        // public Button scaleButton;
        // public Slider scaleSlider;
        public Button decorationButton;
        public GameObject decorationInventory;
        public Button cameraModeButton;
        public Button captureButton;
        public Button lightUpButton;
        public PostProcessVolume postProcessVolume;
        private Image flashEffectImage;
        private bool isUIInitialized;
        private bool isFlashEffect;

        private void Awake() 
        {
            ControlUIActivation(false, false, false, false, false, false, false, false, false, false);
            isUIInitialized = false;
            postProcessVolume.weight = 0f;
            isFlashEffect = false;
        }

        private void Update() 
        {
            // initialize uGUI after a christmas tree appears on the screen
            if (GameSceneManager.isTreeInTheScene && !isUIInitialized)
            {
                InitializeUI();
                isUIInitialized = true;
            }

            // camera flash effect
            if (CameraFlashEffect.activeSelf) 
            {
                if(! isFlashEffect) 
                {
                    // called at the first frame after cameraFlashEffect gets activated
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
                        ControlUIActivation(false, true, false, false, false, false, false, false, false, false);
                    }
                }
            }
        }

        public void OnDecorationItemClicked(string hexadecimalColor) 
        {
            ObjectPlacement.PickedColor = hexadecimalColor;
        }

        private void InitializeUI() 
        {
            // all buttons need to be true to set click event
            ControlUIActivation(false, true, true, true, true, true, false, true, true, true);

            // set click event for each button
            settingButton.onClick.AddListener(() => ControlUIActivation(false, false, true, true, false, true, false, true, false, true));
            backButton.onClick.AddListener(() => ControlUIActivation(false, true, false, false, false, false, false, false, false, false));
            // scaleButton.onClick.AddListener(() => ControlUIActivation(false, false, true, false, true, false, false, false, false));
            // scaleSlider.onValueChanged.AddListener(GameObject.FindObjectOfType<ObjectPlacement>().ScaleChristmasTree);
            decorationButton.onClick.AddListener(() => ControlUIActivation(false, false, true, false, false, false, true, false, false, false));
            cameraModeButton.onClick.AddListener(() => ControlUIActivation(false, false, true, false, false, false, false, false, true, false));
            captureButton.onClick.AddListener(captureButton.gameObject.GetComponent<ScreenShot>().ScreenShotPressed);
            UnityEngine.Events.UnityAction lightAction = LightUpController;
            lightAction += () => ControlUIActivation(false, true, false, false, false, false, false, false, false, false);
            lightUpButton.onClick.AddListener(lightAction);
            
            // setting button can be only true as default
            ControlUIActivation(false, true, false, false, false, false, false, false, false, false);
        }

        public void ControlUIActivation(bool isFlashEffect, bool isSetting, bool isBack, bool isScale, bool isScaleSlider, bool isDecoration, bool isDecorationInv, bool isCameraMode, bool isCapture, bool isLightUp) 
        {
            CameraFlashEffect.SetActive(isFlashEffect);
            settingButton.gameObject.SetActive(isSetting);
            backButton.gameObject.SetActive(isBack);
            // scaleButton.gameObject.SetActive(isScale);
            // scaleSlider.gameObject.SetActive(isScaleSlider);
            decorationButton.gameObject.SetActive(isDecoration);
            decorationInventory.SetActive(isDecorationInv);
            cameraModeButton.gameObject.SetActive(isCameraMode);
            captureButton.gameObject.SetActive(isCapture);
            lightUpButton.gameObject.SetActive(isLightUp);
        }

        private void LightUpController() 
        {
            if (postProcessVolume.weight == 0) // if light is off, turn on
            {
                postProcessVolume.weight = 1;
                lightUpButton.GetComponentInChildren<Text>().text = "Light\nOff";
            }
            else // if light is on, turn off
            {
                postProcessVolume.weight = 0;
                lightUpButton.GetComponentInChildren<Text>().text = "Light\nOn";
            }
        }
    }
}