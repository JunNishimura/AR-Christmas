using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARChristmas
{
    public enum PlayMode
    {
        ChristmasTree,
        Decoration,
    }
    public class ObjectPlacement : MonoBehaviour
    {
        // AR settings
        private ARSessionOrigin arSessionOrigin;
        private ARRaycastManager arRaycastManager;
        private ARPlaneManager arPlaneManager;
        private AREnvironmentProbeManager arEnvironmentProbeManager;
        private List<ARRaycastHit> arRayHits;

        // Game contents settings
        public static int decorationItemIndex = 0;
        public static PlayMode currentPlayMode;
        public GameObject Snow;
        public GameObject christmasTreePrefab;
        public GameObject[] decorationItems;

        private GameObject christmasTree;


        private void Start() 
        {
            // Scene settings
            Snow.SetActive(false);
            currentPlayMode = PlayMode.ChristmasTree;

            // AR settings
            arSessionOrigin = GetComponent<ARSessionOrigin>();
            arRaycastManager = GetComponent<ARRaycastManager>();
            arPlaneManager = GetComponent<ARPlaneManager>();
            arEnvironmentProbeManager = GetComponent<AREnvironmentProbeManager>();
            arRayHits = new List<ARRaycastHit>();
        }

        private void Update() 
        {
            if (Input.touchCount > 0)
            {
                Touch touchIN = Input.GetTouch(0);

                // if the user touchces on UI, stop shooting a ray
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    return;
                }
                
                if (touchIN.phase == TouchPhase.Began) 
                {
                    Vector2 touchPosOnScreen = touchIN.position;

                    // if no christmas tree in the scene, place it at first
                    if(currentPlayMode == PlayMode.ChristmasTree) 
                    {
                        PlaceChristmasTree(touchPosOnScreen);
                    } 
                    else if (currentPlayMode == PlayMode.Decoration)
                    {
                        Decorate(touchPosOnScreen);
                    }
                }
            }
        }

        private void PlaceChristmasTree(Vector2 touchPosOnScreen) 
        {
            if (arRaycastManager.Raycast(touchPosOnScreen, arRayHits, TrackableType.PlaneWithinPolygon))
            {
                Pose placementPose = arRayHits[0].pose;
                christmasTree = Instantiate(christmasTreePrefab, placementPose.position, placementPose.rotation) as GameObject;
                currentPlayMode = PlayMode.Decoration;
                Snow.SetActive(true);
                DisableARPlaneDetection();
            }
        }

        private void DisableARPlaneDetection() 
        {
            arPlaneManager.enabled = false;

            foreach(ARPlane plane in arPlaneManager.trackables) 
            {
                plane.gameObject.SetActive(false);
            }
        }

        private void Decorate(Vector2 touchPosOnScreen) 
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPosOnScreen);
            RaycastHit raycastHit;
                        
            if (Physics.Raycast(ray, out raycastHit))
            {
                // if the ray hits ChristmasTree, let the user decorate ChristmasTree
                if (raycastHit.transform.CompareTag("ChristmasTree"))
                {
                    Vector3 decoratePos = raycastHit.point;
                    Instantiate(decorationItems[decorationItemIndex], decoratePos, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f), christmasTree.transform);
                }
            }
        }

        public void ScaleChristmasTree(float sliderValue) 
        {
            Vector3 pivot = new Vector3(christmasTree.transform.localPosition.x, 0f, christmasTree.transform.localPosition.z);
            ScaleAroundPivot(pivot, Vector3.one * sliderValue);
        }

        private void ScaleAroundPivot(Vector3 pivot, Vector3 newScale) 
        {
            Vector3 pivotToCenter = christmasTree.transform.localPosition - pivot;
            float relScaleAmount = newScale.x / christmasTree.transform.localScale.x;

            // calculate final position
            Vector3 finalPosition = pivot + pivotToCenter * relScaleAmount;

            // execute scaling 
            christmasTree.transform.localScale = newScale;
            christmasTree.transform.localPosition = finalPosition;
        }
    }
}