using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using ARChristmas.Utilities.Save;
using ARChristmas.Utilities.ScreenShot.iOS;

namespace ARChristmas
{
    public class UIController : MonoBehaviour
    {
        public GameObject CamFlashEffectObj;
        public Button settingButton;
        public Button backButton;
        public Button scaleButton;
        public Slider scaleSlider;
        public Button decorationButton;
        public GameObject decorationInventory;
        public Button cameraModeButton;
        public Button captureButton;
        public Button lightUpButton;
        public Button saveButton;
        public PostProcessVolume postProcessVolume;

        private GameObject childChristmasTree;
        private GameObject christmasTreeLights;
        [SerializeField] private float originalLightIntensity;
        private Image flashEffectImage;
        private bool isUIInitialized;

        private void Awake() 
        {
            ControlUIActivation(false, false, false, false, false, false, false, false, false, false);
            CamFlashEffectObj.SetActive(false);
            isUIInitialized = false;
            postProcessVolume.weight = 0f;
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
            if (CamFlashEffectObj.activeSelf) 
            {
                CameraFlashing();
            }
        }

        public void OnDecorationItemClicked(string hexadecimalColor) 
        {
            ObjectPlacement.PickedColor = hexadecimalColor;
        }

        public void OnScaleSliderChanged(float scaleVal) 
        {
            if (childChristmasTree == null)
            {
                // if this is the first time to change the slider value
                childChristmasTree  = ObjectPlacement.christmasTree.transform.Find("Christmas Tree").gameObject;
                christmasTreeLights = ObjectPlacement.christmasTree.transform.Find("Lights").gameObject;
            }
            childChristmasTree.transform.localScale = Vector3.one * scaleVal;
            var scaledRatio = scaleVal / scaleSlider.minValue;
            christmasTreeLights.transform.localScale = Vector3.one * scaledRatio;
            foreach (var light in christmasTreeLights.GetComponentsInChildren<Light>())
            {
                light.range = scaledRatio;
                if (light.name == "topLight")
                    light.range *= 1.5f;
                light.intensity = originalLightIntensity * scaledRatio;
            }
        }

        private void InitializeUI() 
        {
            // all buttons need to be true to set click event
            ControlUIActivation(true, true, true, true, true, false, true, true, true, true);

            // set click event for each button
            settingButton.onClick.AddListener(() => ControlUIActivation(false, true, true, false, true, false, true, false, true, true));
            backButton.onClick.AddListener(() => ControlUIActivation(true, false, false, false, false, false, false, false, false, false));
            scaleButton.onClick.AddListener(() => ControlUIActivation(false, true, false, true, false, false, false, false, false, false));
            scaleSlider.onValueChanged.AddListener( (value) => OnScaleSliderChanged(value) );
            decorationButton.onClick.AddListener(() => ControlUIActivation(false, true, false, false, false, true, false, false, false, false));
            cameraModeButton.onClick.AddListener(() => ControlUIActivation(false, true, false, false, false, false, false, true, false, false));
            captureButton.onClick.AddListener(OnPressedCaptureButton);
            UnityAction lightAction = LightUpController;
            lightAction += () => ControlUIActivation(true, false, false, false, false, false, false, false, false, false);
            lightUpButton.onClick.AddListener(lightAction);
            saveButton.onClick.AddListener(OnPressedSaveButton);

            // setting button can be only true as default
            ControlUIActivation(true, false, false, false, false, false, false, false, false, false);
        }

        public void ControlUIActivation(
            bool isSetting, 
            bool isBack, 
            bool isScale, 
            bool isScaleSlider, 
            bool isDecoration, 
            bool isDecorationInv, 
            bool isCameraMode, 
            bool isCapture, 
            bool isLightUp, 
            bool isSave) 
        {
            settingButton.gameObject.SetActive(isSetting);
            backButton.gameObject.SetActive(isBack);
            scaleButton.gameObject.SetActive(isScale);
            scaleSlider.gameObject.SetActive(isScaleSlider);
            decorationButton.gameObject.SetActive(isDecoration);
            decorationInventory.SetActive(isDecorationInv);
            cameraModeButton.gameObject.SetActive(isCameraMode);
            captureButton.gameObject.SetActive(isCapture);
            lightUpButton.gameObject.SetActive(isLightUp);
            saveButton.gameObject.SetActive(isSave);
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

        /// <summary>
        /// callback function for capture button. 
        /// Execute screenshot 
        /// </summary>
        public void OnPressedCaptureButton() 
        {
            PrepareScreenShot();

            // play sound for capturing
            captureButton.GetComponent<AudioSource>().Play(); 

            ScreenShotManager_iOS.CaptureAndSaveToAlbum("shot.png", PostScreenShotProcess);
        }

        /// <summary>
        /// proceed with the necessary process for screenshot such as disable UIs and AR planes which should not be seen in shot.
        /// </summary>
        private void PrepareScreenShot() 
        {
            // deactivate all UI except for captureButton
            // capture button can't be deactivated because it has ScreenShot script, so just make it transparent.
            ControlUIActivation(false, false, false, false, false, false, false, true, false, false); 
            captureButton.GetComponent<Image>().color = Color.clear; 
            FindObjectOfType<ObjectPlacement>().ToggleARPlaneDetection(false);
        }

        /// <summary>
        /// re-activate UI and AR planes which were out of screen while taking screen shots
        /// </suumary>
        private void PostScreenShotProcess() 
        {
            // re-activate UI and AR planes which were out of screen while taking screen shots
            captureButton.GetComponent<Image>().color = Color.white;
            FindObjectOfType<ObjectPlacement>().ToggleARPlaneDetection(true);

            // Execute camera flash effect after saving, not to disturb saving process.
            ActivateCamFlashEffect();
        }

        /// <summary>
        /// activate camera flash effect and set initial state
        /// </summary> 
        private void ActivateCamFlashEffect()
        {
            // disable all UIs before activating camera flash effect
            ControlUIActivation(false, false, false, false, false, false, false, false, false, false); 

            CamFlashEffectObj.SetActive(true);            
            flashEffectImage = CamFlashEffectObj.GetComponent<Image>();
            flashEffectImage.color = Color.white;
        }
        
        /// <summary>
        /// imitate camera flash by changing screen from opaque white to transparent.
        /// stop flashing if screen is back to transparent enough;
        /// </summary>
        private void CameraFlashing()
        {
            flashEffectImage.color = Color.Lerp(flashEffectImage.color, Color.clear, Time.deltaTime);

            // finish camera flash effect
            if (flashEffectImage.color.a <= 0.05f)
            {
                flashEffectImage.color = Color.clear;
                CamFlashEffectObj.SetActive(false);
                ControlUIActivation(true, false, false, false, false, false, false, false, false, false);
            }
        }

        /// <summary>
        /// callback function for save button
        /// </summary>
        private void OnPressedSaveButton() 
        {
            SaveLoadManager<TreeSaveData>.SaveData(new TreeSaveData(ObjectPlacement.christmasTree), Application.persistentDataPath + "/save.txt");
            ControlUIActivation(true, false, false, false, false, false, false, false, false, false);
        }
    }
}