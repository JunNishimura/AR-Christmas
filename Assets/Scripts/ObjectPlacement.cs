﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARChristmas
{
    public class ObjectPlacement : MonoBehaviour
    {
        // AR settings
        private ARSessionOrigin arSessionOrigin;
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
            // Scene settings
            Snow.SetActive(false);
            GameSceneManager.isTreeInTheScene = false;

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
                    if(! GameSceneManager.isTreeInTheScene) 
                    {
                        PlaceChristmasTree(touchPosOnScreen);
                    } 
                    else
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
                christmasTree = Instantiate(christmasTreePrefab, placementPose.position, placementPose.rotation).GetComponent<ChristmasTree>();
                christmasTree.SetTreeLight(false);
                GameSceneManager.isTreeInTheScene = true;
                Snow.SetActive(true);
            }
        }

        public void ToggleARPlaneDetection(bool state) 
        {
            arPlaneManager.enabled = state;
            foreach(ARPlane plane in arPlaneManager.trackables) 
            {
                plane.gameObject.SetActive(state);
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
                    var item = Instantiate(decorationItemPrefab, decoratePos, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f), christmasTree.transform.Find("Christmas Tree")) as GameObject;

                    // set color
                    Color color;
                    if (! ColorUtility.TryParseHtmlString(PickedColor, out color))
                    {
                        // if it failed to convert hexadeciaml to Color, set Red just in case
                        color = Color.red;
                    }
                    // change base color and emission color to the selected color
                    item.GetComponent<Renderer>().material.SetColor("Color_A9AB75C1", color); // base color
                    item.GetComponent<Renderer>().material.SetColor("Color_B37F01A0", color); // emission color
                    christmasTree.decoItemPositions.Add(item.transform.position);
                    christmasTree.decoItemColors.Add(color);
                }
            }
        }

        // public void ScaleChristmasTree(float sliderValue) 
        // {
        //     Vector3 pivot = new Vector3(christmasTree.transform.position.x, 0f, christmasTree.transform.position.z);
        //     ScaleAroundPivot(pivot, Vector3.one * sliderValue);
        // }

        // private void ScaleAroundPivot(Vector3 pivot, Vector3 newScale) 
        // {
        //     Vector3 pivotToCenter = christmasTree.transform.position - pivot;
        //     float relScaleAmount = newScale.x / christmasTree.transform.localScale.x;

        //     // calculate final position
        //     Vector3 finalPosition = pivot + pivotToCenter * relScaleAmount;

        //     // execute scaling 
        //     christmasTree.transform.localScale = newScale;
        //     christmasTree.transform.localPosition = finalPosition;
        // }
    }
}