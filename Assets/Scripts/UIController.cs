using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using ARChristmas.Utilities.Save;
using ARChristmas.Utilities.ScreenShot.iOS;

namespace ARChristmas 
{
    public enum TabType
    {
        OrnamentColor,
        Lighting,
        Scaling,
        Camera,
        Save
    }

    public class TabState 
    {
        public TabType CurTabType;
        public Dictionary<TabType, GameObject> tabDicts;

        public TabState(TabType initTabType) 
        {
            tabDicts = new Dictionary<TabType, GameObject>();
            CurTabType = initTabType;
        }
    }
    public class UIController : MonoBehaviour
    {
        // ----- static -----
        public static Color SelectedColor;
        public static bool isOpenTabBar;

        // ----- public -----
        [Header("Tab types")]
        public Button ornamentColButton;
        public Button lightingButton;
        public Button scalingButton;
        public Button cameraButton;
        public Button saveButton;

        [Header("Tab contents")]
        public GameObject ornamentColTab;
        public GameObject lightingTab;
        public GameObject scalingTab;
        public GameObject cameraTab;
        public GameObject saveTab;

        [Header("Others")]
        public GameObject tabBar;
        public PostProcessVolume postProcessVol;
        public Image lightingProgressBar;
        public Image scalingProgressBar;
        public GameObject CamFlashEffectObj;
        public GameObject SaveCompleteObj;

        // ----- private -----
        [SerializeField] private Vector2 scaleRange;
        private static readonly Color highlightTabColor = new Color(174.0f/255.0f, 255.0f/255.0f, 217.0f/255.0f, 1.0f); // light blue
        private const float treeDefaultScale = 0.4f;
        private const float defaultLightIntensity = 5f;
        private TabState tabState;
        private GameObject childTree;
        private GameObject treeLightsObj;
        private List<Light> treeLights;
        private Image flashEffectImage;

        private void Awake() 
        {
            tabState = new TabState(TabType.OrnamentColor);
            InitTabState();
            LinkTabTypesWithContents();
            SetTabCallback();
            SetCallbackWithParams();
            
            // no post process by default
            postProcessVol.weight = 0f;

            CamFlashEffectObj.SetActive(false);
            SaveCompleteObj.SetActive(false);
        }

        private void Update() 
        {
            // execute camera flashing when taking screen shot
            if (CamFlashEffectObj.activeSelf)
            {
                CameraFlashing();
            }
        }

        /// <summary>
        /// ornamentColorTab is active by default
        /// </summary>
        private void InitTabState() 
        {
            ornamentColTab.SetActive(true);
            lightingTab.SetActive(false);
            scalingTab.SetActive(false);
            cameraTab.SetActive(false);
            saveTab.SetActive(false);

            // highlight active tab
            ChangeTabColor(TabType.OrnamentColor, highlightTabColor);
        }

        /// <summary>
        /// link tab types with tab contents as initial setting
        /// </summary>
        private void LinkTabTypesWithContents() 
        {
            tabState.tabDicts.Add(TabType.OrnamentColor, ornamentColTab);
            tabState.tabDicts.Add(TabType.Lighting, lightingTab);
            tabState.tabDicts.Add(TabType.Scaling, scalingTab);
            tabState.tabDicts.Add(TabType.Camera, cameraTab);
            tabState.tabDicts.Add(TabType.Save, saveTab);
        }

        /// <summary>
        /// set callbacks from script because each button needs to pass enum TabType as the parameter
        /// </summary>
        private void SetTabCallback() 
        {
            ornamentColButton.onClick.AddListener( () => OnPressedTab(TabType.OrnamentColor) );
            lightingButton.onClick.AddListener( () => OnPressedTab(TabType.Lighting) );
            scalingButton.onClick.AddListener( () => OnPressedTab(TabType.Scaling) );
            cameraButton.onClick.AddListener( () => OnPressedTab(TabType.Camera) );
            saveButton.onClick.AddListener( () => OnPressedTab(TabType.Save) );
        }

        /// <summary>
        /// some UI passes parameters to callback functions (e.g. Slider)
        /// those callback functions need to be set from the script.
        /// </summary>
        private void SetCallbackWithParams() 
        {
            // initialize
            var lightingSlider = lightingTab.GetComponentInChildren<Slider>();
            lightingSlider.value = 0;
            lightingProgressBar.fillAmount = 0;
            var scalingSlider = scalingTab.GetComponentInChildren<Slider>();
            scalingSlider.minValue = scaleRange.x; // scaleRange.x is slider's minVal
            scalingSlider.maxValue = scaleRange.y; // scaleRange.y is slider's maxVal
            scalingSlider.value = treeDefaultScale;
            scalingProgressBar.fillAmount = (treeDefaultScale - scaleRange.x) / (scaleRange.y - scaleRange.x);

            // set callback
            lightingSlider.onValueChanged.AddListener( (value) => OnChangedLightSlider(value) );
            scalingSlider.onValueChanged.AddListener(  (value) => OnChangedScaleSlider(value) );
        }

        /// <summary>
        /// open or close tab bar with animation
        /// </summary>
        public void OnPressedSettingButton() 
        {
            var rect = tabBar.GetComponent<RectTransform>();
            Vector2 dest;
            if (isOpenTabBar) // if tab bar is open
            {
                // tab bar can disappear from the screen at (0, -600f)
                dest = new Vector2(0, -600f);
            }
            else // if tab bar is closed
            {
                dest = Vector2.zero;
            }
            rect.DOAnchorPos(dest, 1);
            isOpenTabBar = !isOpenTabBar;
        }

        /// <summary>
        /// change tab if pressed tab is different from current tab
        /// </summary>
        public void OnPressedTab(TabType tabType) 
        {
            if (tabState.CurTabType != tabType)
                ChangeTabType(tabType);
        }

        /// <summary>
        /// deactivate current tab and activate new tab
        /// </summary>
        private void ChangeTabType(TabType newTabType) 
        {
            // tab color
            ChangeTabColor(tabState.CurTabType, Color.white); // previouly selected tab -> white
            ChangeTabColor(newTabType, highlightTabColor);    // newly selected tab -> light blue

            // tab state
            tabState.tabDicts[tabState.CurTabType].SetActive(false);
            tabState.tabDicts[newTabType].SetActive(true);
            tabState.CurTabType = newTabType;
        }

        /// <summary>
        /// change tab color to the designated one
        /// </summary>
        private void ChangeTabColor(TabType tabType, Color color) 
        {
            switch(tabType)
            {
                case TabType.OrnamentColor:
                    ornamentColButton.GetComponent<Image>().color = color;
                    break;
                case TabType.Lighting:
                    lightingButton.GetComponent<Image>().color = color;
                    break;
                case TabType.Scaling:
                    scalingButton.GetComponent<Image>().color = color;
                    break;
                case TabType.Camera:
                    cameraButton.GetComponent<Image>().color = color;
                    break;
                case TabType.Save:
                    saveButton.GetComponent<Image>().color = color;
                    break;
            }
        }

        #region OrnamentColor Tab

        /// <summary>
        /// change ornament ball color to the selected one
        /// </summary>
        public void OnPressedOrnamentBallColor(string hexadecimalColor) 
        {
            SelectedColor = ConvertHexadecimalToColor(hexadecimalColor);
        }

        /// <summary>
        /// convert string hexadecimalcolor to UnityEngine.Color. 
        /// Return white color if it fails conversion.
        /// </summary>
        private Color ConvertHexadecimalToColor(string hexadecimalColor) 
        {
            if (ColorUtility.TryParseHtmlString(hexadecimalColor, out Color color))
            {
                return color;
            }
            else 
            {
                return Color.white;
            }
        }

        #endregion

        #region Lighting Tab
        
        /// <summary>
        /// apply lighting slider value to post process weight directly
        /// visualize lighting amount by progress bar
        /// </summary>
        private void OnChangedLightSlider(float sliderVal)
        {
            postProcessVol.weight = sliderVal;
            lightingProgressBar.fillAmount = sliderVal;
        }

        #endregion

        #region Scaling Tab

        /// <summary>
        /// There are 2 points to keep in mind.
        /// (1)
        /// Scale Christmas tree up and down around its bottom, not its center.
        /// To change scaling point from center to bottom, parent empty gameobject to the Christmas tree and use its offset.
        /// So we define childTree and we scale its localScale.
        /// (2)
        /// Christmas Tree has several lights around it for lighting functionality. 
        /// Those lights need to be scaled up and down at the same time. 
        /// So we define treeLights.
        /// </summary>
        private void OnChangedScaleSlider(float sliderVal) 
        {
            // if this is the first time to be called
            if (childTree == null || treeLights == null) 
                SetChildTreeAndLights();
            
            childTree.transform.localScale = Vector3.one * sliderVal;
            AdjustLightToScaling(sliderVal);
            scalingProgressBar.fillAmount = (sliderVal - scaleRange.x) / (scaleRange.y - scaleRange.x);
        }

        /// <summary>
        /// extract child tree and tree lights from child ObjectPlacement.christmasTree
        /// </summary>
        private void SetChildTreeAndLights() 
        {
            childTree = ObjectPlacement.christmasTree.transform.Find("Christmas Tree").gameObject;
            treeLightsObj = ObjectPlacement.christmasTree.transform.Find("Lights").gameObject;
            treeLights = new List<Light>(treeLightsObj.GetComponentsInChildren<Light>());
        }

        /// <summary>
        /// 2 things to adjust
        /// (1) scale of each light
        /// (2) light range and intensity
        /// </summary>
        private void AdjustLightToScaling(float sliderVal) 
        {  
            var scaledRatio = sliderVal / treeDefaultScale; 
            treeLightsObj.transform.localScale = Vector3.one * sliderVal;
            foreach( var light in treeLights )
            {
                light.range = scaledRatio;
                // top light(light above tree) needs to be a bit brighter than others
                if (light.name == "topLight") 
                    light.range *= 1.5f;
                light.intensity = defaultLightIntensity * scaledRatio;
            }
        }

        #endregion

        #region Camera Tab

        /// <summary>
        /// execute the whole process of taking a screen shot
        /// </summary>
        public void OnPressedScreenShot() 
        {
            // Deactivate UIs because All UIs should not be captured for the screen shot.
            tabBar.SetActive(false);

            // play shutter sound
            GetComponent<AudioSource>().Play();

            // passing the post process function 
            ScreenShotManager_iOS.CaptureAndSaveToAlbum("shot.png", PostScreenShotProcess);
        }

        /// <summary>
        /// activate camera flash
        /// </suumary>
        private void PostScreenShotProcess() 
        {
            // Not to disturb saving process, activate camera flash effect after saving
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
            Debug.Log("hello");
            flashEffectImage.color = Color.Lerp(flashEffectImage.color, Color.clear, Time.deltaTime);

            // finish camera flash effect
            if (flashEffectImage.color.a <= 0.05f)
            {
                flashEffectImage.color = Color.clear;
                CamFlashEffectObj.SetActive(false);
                
                tabBar.SetActive(true);
            }
        }

        #endregion 

        #region Save Tab
        /// <summary>
        /// callback function for save button
        /// </summary>
        public void OnPressedSave() 
        {
            SaveLoadManager<TreeSaveData>.SaveData(new TreeSaveData(ObjectPlacement.christmasTree), Application.persistentDataPath + "/save.txt");
            SaveCompleteObj.SetActive(true);
        }
        #endregion
    }
}