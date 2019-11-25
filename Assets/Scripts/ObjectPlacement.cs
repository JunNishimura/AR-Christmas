using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARChristmasTree
{
    public enum PlayMode
    {
        ChristmasTree,
        Decoration,
    }
    public class ObjectPlacement : MonoBehaviour
    {
        public GameObject Snow;
        public GameObject christmasTreePrefab;
        public GameObject[] decorationItems;
        public static int decorationItemIndex = 0;

        public static PlayMode currentPlayMode;
        private ARSessionOrigin arSessionOrigin;
        private ARRaycastManager arRaycastManager;
        private List<ARRaycastHit> arRayHits;
        private Pose placementPose;
        private float maxRayDistance;

        private void Start() 
        {
            Snow.SetActive(false);
            currentPlayMode = PlayMode.ChristmasTree;
            arSessionOrigin = FindObjectOfType<ARSessionOrigin>();
            arRaycastManager = FindObjectOfType<ARRaycastManager>();
            arRayHits = new List<ARRaycastHit>();
            maxRayDistance = 100.0f;
        }

        private void Update() 
        {
            if (Input.touchCount > 0)
            {
                Touch touchIN = Input.GetTouch(0);
                UpdatePlacementPose(touchIN.position);

                // if the user touchces on UI, stop shooting a ray
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    return;
                }
                
                if (touchIN.phase == TouchPhase.Began) 
                {
                    // if no christmas tree in the scene, place it at first
                    if(currentPlayMode == PlayMode.ChristmasTree) 
                    {
                        PlaceChristmasTree();
                    } 
                    else if (currentPlayMode == PlayMode.Decoration)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(touchIN.position);
                        RaycastHit raycastHit;
                        
                        if (Physics.Raycast(ray, out raycastHit, maxRayDistance))
                        {
                            // if the ray hits ChristmasTree, let the user decorate ChristmasTree
                            if (raycastHit.transform.CompareTag("ChristmasTree"))
                            {
                                Decorate(raycastHit.point);
                            }
                        }
                    }
                }
                else if (touchIN.phase == TouchPhase.Ended) 
                {

                }
            }
        }

        private void UpdatePlacementPose(Vector2 touchPosOnScreen) 
        {
            if (arRaycastManager.Raycast(touchPosOnScreen, arRayHits, TrackableType.All))
            {   
                placementPose = arRayHits[0].pose;
            }
        }

        private void PlaceChristmasTree() 
        {
            Snow.SetActive(true);
            if (currentPlayMode == PlayMode.ChristmasTree)
            {
                Instantiate(christmasTreePrefab, placementPose.position, placementPose.rotation);
                currentPlayMode = PlayMode.Decoration;
            }
        }

        private void Decorate(Vector3 decoratePos) 
        {
            if (currentPlayMode == PlayMode.Decoration) 
            {
                Instantiate(decorationItems[decorationItemIndex], decoratePos, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f));
            }
        }
    }
}