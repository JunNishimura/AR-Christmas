using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Rendering.PostProcessing;

namespace ARChristmas
{
    /// <summary>
    /// A component that can be used to access the most
    /// recently received light estimation information
    /// for the physical environment as observed by an
    /// AR device.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class LightEstimation : MonoBehaviour
    {
        private Light m_Light;
        [SerializeField] PostProcessVolume postProcessVolume;
        private Bloom bloom;

        [SerializeField] 
        [Tooltip("The ARCameraManager which will produce frame events containing light estimation information.")]
        ARCameraManager m_arCameraManager;

        private bool isChristmasTreeLightON;


        /// <summary>
        /// Get or set the <c>ARCameraManager</c>.
        /// </summary>
        public ARCameraManager arCameraManager
        {
            get { return m_arCameraManager; }
            set 
            {
                if (m_arCameraManager == value) 
                    return;

                if (m_arCameraManager != null) 
                    m_arCameraManager.frameReceived -= FrameChanged;

                m_arCameraManager = value;

                if (m_arCameraManager != null && enabled) 
                    m_arCameraManager.frameReceived += FrameChanged;
            }
        }

        /// <summary>
        /// The estimated brightness of the physical environment, if available.
        /// </summary>
        public float? brightness { get; private set; }

        /// <summary>
        /// The estimated color temperature of the physical environment, if available.
        /// </summary>
        public float? colorTemperature { get; private set; }

        /// <summary>
        /// The estimated color correction value of the physical environment, if available.
        /// </summary>
        public Color? colorCorrection { get; private set; }

        private void Awake() 
        {
            m_Light = GetComponent<Light>();
            postProcessVolume.profile.TryGetSettings(out bloom);
            isChristmasTreeLightON = false;
        }

        private void OnEnable() 
        {
            if (m_arCameraManager != null)
                m_arCameraManager.frameReceived += FrameChanged;
        }

        private void OnDisable() 
        {
            if (m_arCameraManager != null) 
                m_arCameraManager.frameReceived -= FrameChanged;
        }

        private void FrameChanged(ARCameraFrameEventArgs args) 
        {
            if (args.lightEstimation.averageBrightness.HasValue)
            {
                brightness = args.lightEstimation.averageBrightness.Value;
                m_Light.intensity = brightness.Value;

                // maximum brightness is about 0.5
                // darker the environment is, the stronger the bloom is
                if (bloom != null)
                    bloom.intensity.value = 25 / (1 + brightness.Value);

                // if the environment is dark enough, let's light up christmas tree with lights which tree has
                // set 1 for threshold as an arbitrary value
                float threshold = 0.1f;
                if (m_Light.intensity < threshold && isChristmasTreeLightON == false) 
                {
                    Debug.Log("dark: light on");
                    ObjectPlacement.christmasTree.SetTreeLight(true);
                    isChristmasTreeLightON = true;
                } 
                else if (m_Light.intensity >= threshold && isChristmasTreeLightON == true)
                {
                    Debug.Log("not dark: light off");
                    ObjectPlacement.christmasTree.SetTreeLight(false);
                    isChristmasTreeLightON = false;
                }
            }

            if (args.lightEstimation.averageColorTemperature.HasValue)
            {
                colorTemperature = args.lightEstimation.averageColorTemperature.Value;
                m_Light.colorTemperature = colorTemperature.Value;
            }

            if (args.lightEstimation.colorCorrection.HasValue) 
            {
                colorCorrection = args.lightEstimation.colorCorrection.Value;
                m_Light.color = colorCorrection.Value;
            }
        }
    }
}