﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ARChristmas.Utilities.Save;

namespace ARChristmas
{
    public class ObjectPlacement : MonoBehaviour
    {
        // AR settings
        private ARRaycastManager arRaycastManager;
        private ARPlaneManager arPlaneManager;
        private AREnvironmentProbeManager arEnvironmentProbeManager;
        private List<ARRaycastHit> arRayHits;
        
        // Game contents settings
        public static ChristmasTree christmasTree;
        public static string PickedColor;
        public GameObject Snow;
        public GameObject christmasTreePrefab;
        public GameObject decorationItemPrefab;

        private void Start() 
        {
            // AR settings
            arRaycastManager = GetComponent<ARRaycastManager>();
            arPlaneManager = GetComponent<ARPlaneManager>();
            arEnvironmentProbeManager = GetComponent<AREnvironmentProbeManager>();
            arRayHits = new List<ARRaycastHit>();

            // Scene settings
            Snow.SetActive(false);
            GameSceneManager.isTreeInTheScene = false;
        }

        private void Update() 
        {
            if (isAbleToPlace(out Vector2 screenTouchPos))
            {
                // decorate if christmas tree is not in the scene, otherwise create a new tree.
                if(GameSceneManager.isTreeInTheScene) 
                {
                    if (isAbleToDecorate(screenTouchPos, out Vector3 hitPoint))
                    {
                        Decorate(hitPoint);
                    }
                } 
                else
                {
                    if (arRaycastManager.Raycast(screenTouchPos, arRayHits, TrackableType.PlaneWithinPolygon))
                    {
                        PlaceChristmasTree(screenTouchPos);
                    }
                }
            }
        }

        /// <summary>
        /// return true if the screen touch from user is detected
        /// </summary>
        private bool isAbleToPlace(out Vector2 screenTouchPos) 
        {
            screenTouchPos = Vector2.zero;
            if (Input.touchCount > 0)
            {
                Touch touchIN = Input.GetTouch(0);

                // if the user touchces UI, stop shooting a ray
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    return false;
                }
                
                if (touchIN.phase == TouchPhase.Began) 
                {
                    screenTouchPos = touchIN.position;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// return true if the ray from camera hits Christmas tree.
        /// return ray hit point at the same time.
        /// </summary>
        private bool isAbleToDecorate(Vector2 screenTouchPos, out Vector3 hitPoint) 
        {
            Ray ray = Camera.main.ScreenPointToRay(screenTouchPos);
            RaycastHit raycastHit;
            hitPoint = Vector3.zero;

            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.transform.CompareTag("ChristmasTree"))
                {
                    hitPoint = raycastHit.point;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// create and register new decoration item
        /// </summary>
        private void Decorate(Vector3 hitPoint) 
        {
            // creation
            (Vector3, Color) newItem = CreateNewDecorateItem(hitPoint);

            // registration
            christmasTree.decorationItemLocalPos.Add(newItem.Item1); //item1: item's local position relative to the parent christmas tree
            christmasTree.decorationItemColors.Add(newItem.Item2);   //item2: item's color
        }

        /// <summary>
        /// create new christmas tree and return info which is registered to christmasTree
        /// </summary>
        private (Vector3, Color) CreateNewDecorateItem(Vector3 hitPoint)
        {
            var newItem = Instantiate(decorationItemPrefab, hitPoint, Quaternion.identity, christmasTree.transform.Find("Christmas Tree")) as GameObject;

            // set color
            Color color;
            if (! ColorUtility.TryParseHtmlString(PickedColor, out color))
            {
                // if it failed to convert hexadeciaml to Color, set red just in case
                color = Color.red;
            }
            newItem.GetComponent<Renderer>().material.SetColor("Color_A9AB75C1", color); // base color
            newItem.GetComponent<Renderer>().material.SetColor("Color_B37F01A0", color); // emission color

            return (newItem.transform.localPosition, color);
        }
        
        private void PlaceChristmasTree(Vector2 screenTouchPos) 
        {
            Pose placementPose = arRayHits[0].pose;
            christmasTree = Instantiate(christmasTreePrefab, placementPose.position, placementPose.rotation).GetComponent<ChristmasTree>();

            // execute loading if user pressed "Load" button.
            if (! GameSceneManager.isNewTree) 
            {
                var loadObj = SaveLoadManager<TreeSaveData>.GetLoadData(Application.persistentDataPath + "/save.txt");
                christmasTree.DecorateWithLoadData(loadObj);
            }

            christmasTree.SetTreeLight(false);
            GameSceneManager.isTreeInTheScene = true;
            Snow.SetActive(true);
        }

        public void ToggleARPlaneDetection(bool state) 
        {
            arPlaneManager.enabled = state;
            foreach(ARPlane plane in arPlaneManager.trackables) 
            {
                plane.gameObject.SetActive(state);
            }
        }
    }
}