using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARChristmas
{
    [RequireComponent(typeof(ARCameraManager))]
    public class PeopleOcclusion : MonoBehaviour
    {
        [SerializeField] private Shader occlusionShader;
        private ARHumanBodyManager arHumanBodyManager;
        private ARCameraManager arCameraManager;

        private Texture2D camFeedTexture;
        private Material material;

        private void Awake() 
        {
            arHumanBodyManager = GetComponentInParent<ARHumanBodyManager>();
            arCameraManager = GetComponent<ARCameraManager>();

            material = new Material(occlusionShader);
            GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
        }

        private void OnEnable() 
        {
            arCameraManager.frameReceived += OnCameraFrameReceived;
        }

        private void OnDisable() 
        {
            arCameraManager.frameReceived -= OnCameraFrameReceived;
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest) 
        {
            if (PeopleOcclusionSupported())
            {
                if (camFeedTexture != null)
                {
                    material.SetFloat("_UVMultiplierLandScale", CalculateUVMultiplierLandScale(camFeedTexture));
                    material.SetFloat("_UVMultiplierPortrait", CalculateUVMultiplierPortrait(camFeedTexture));
                }

                if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
                {
                    material.SetFloat("_UVFlip", 0);
                    material.SetInt("_ONWIDE", 1);
                }
                else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
                {
                    material.SetFloat("_UVFLIP", 1);
                    material.SetInt("_ONWIDE", 1);
                }
                else 
                {
                    material.SetInt("_ONWIDE", 0);
                }

                material.SetTexture("_OcclusionDepth", arHumanBodyManager.humanDepthTexture);
                material.SetTexture("_OcclusionStencil", arHumanBodyManager.humanStencilTexture);

                Graphics.Blit(src, dest, material);
            }
            else 
            {
                Graphics.Blit(src, dest);
            }
        }

        private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
        {
            if (PeopleOcclusionSupported())
            {
                RefreshCameraFeedTexture();
            }
        }

        private bool PeopleOcclusionSupported() 
        {
            return arHumanBodyManager.subsystem != null && arHumanBodyManager.humanDepthTexture != null && arHumanBodyManager.humanStencilTexture != null;
        }

        private void RefreshCameraFeedTexture()
        {
            XRCameraImage cameraImage;
            arCameraManager.TryGetLatestImage(out cameraImage);
            if (camFeedTexture == null || camFeedTexture.width != cameraImage.width || camFeedTexture.height != cameraImage.height)
            {
                camFeedTexture = new Texture2D(cameraImage.width, cameraImage.height, TextureFormat.RGBA32, false);
            }

            CameraImageTransformation imageTransformation = Input.deviceOrientation == DeviceOrientation.LandscapeRight ? CameraImageTransformation.MirrorY : CameraImageTransformation.MirrorX;
            XRCameraImageConversionParams conversionParams = new XRCameraImageConversionParams(cameraImage, TextureFormat.RGBA32, imageTransformation);

            NativeArray<byte> rawTextureData = camFeedTexture.GetRawTextureData<byte>();

            try
            {
                unsafe
                {
                    cameraImage.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
                }
            }
            finally
            {
                cameraImage.Dispose();
            }

            camFeedTexture.Apply();
            material.SetTexture("_CameraFeed", camFeedTexture);
        }

        private float CalculateUVMultiplierLandScale(Texture2D camTexture)
        {
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float camTextureAspect = (float)camTexture.width / (float)camTexture.height;
            return screenAspect / camTextureAspect;
        }

        private float CalculateUVMultiplierPortrait(Texture2D camTexture)
        {
            float screenAspect = (float)Screen.height / (float)Screen.width;
            float camTextureAspect = (float)camTexture.width / (float) camTexture.height;
            return screenAspect / camTextureAspect;
        }
    }
}