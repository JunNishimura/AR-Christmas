// Reference http://edom18.hateblo.jp/entry/2019/08/11/223803
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARChristmas
{
    public class PeopleOcclusion : MonoBehaviour
    {
        public ARHumanBodyManager ARHumanBodyManager
        {
            get { return arHumanBodyManager;  }
            set { arHumanBodyManager = value; }
        }
        private ARHumanBodyManager arHumanBodyManager;
        private ARCameraBackground arCameraBackground;
        private RenderTexture captureTexture;

        [SerializeField] private Material occlusionMat;

        private void Awake() 
        {
            Camera camera = Camera.main.GetComponent<Camera>();
            camera.depthTextureMode |= DepthTextureMode.Depth;

            arHumanBodyManager = this.GetComponentInChildren<ARHumanBodyManager>();
            arCameraBackground = this.GetComponentInChildren<ARCameraBackground>();
            captureTexture = new RenderTexture(Screen.width, Screen.height, 0);
        }

        private void Update() 
        {
            SendTextureToShader();
        }

        /// <summary>
        /// send stencil, depth and capture texture to shader
        /// </summary>
        private void SendTextureToShader() 
        {
            Texture2D humanStencil = arHumanBodyManager.humanStencilTexture;
            Texture2D humanDepth = arHumanBodyManager.humanDepthTexture;
            occlusionMat.SetTexture("_StencilTex", humanStencil);
            occlusionMat.SetTexture("_DepthTex", humanDepth);
            occlusionMat.SetTexture("_BackgroundTex", captureTexture);
        }

        private void LateUpdate() 
        {
            if (arCameraBackground.material != null) 
            {
                Graphics.Blit(null, captureTexture, arCameraBackground.material);
            }
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest) 
        {
            Graphics.Blit(src, dest, occlusionMat);
        }
    }
}