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
        public GameObject Snow;
        public GameObject christmasTreePrefab;
        public GameObject[] decorationItems;
        public static int decorationItemIndex = 0;
        public static PlayMode currentPlayMode;

        // AR settings
        private ARSessionOrigin arSessionOrigin;
        private ARRaycastManager arRaycastManager;
        private List<ARRaycastHit> arRayHits;

        private void Start() 
        {
            // Scene settings
            Snow.SetActive(false);
            currentPlayMode = PlayMode.ChristmasTree;

            // AR settings
            arSessionOrigin = FindObjectOfType<ARSessionOrigin>();
            arRaycastManager = FindObjectOfType<ARRaycastManager>();
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
                else if (touchIN.phase == TouchPhase.Ended) 
                {

                }
            }
        }

        private void PlaceChristmasTree(Vector2 touchPosOnScreen) 
        {
            if (arRaycastManager.Raycast(touchPosOnScreen, arRayHits, TrackableType.Planes))
            {
                Pose placementPose = arRayHits[0].pose;
                Instantiate(christmasTreePrefab, placementPose.position, placementPose.rotation);
                currentPlayMode = PlayMode.Decoration;
            }
            Snow.SetActive(true);
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
                    Instantiate(decorationItems[decorationItemIndex], decoratePos, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f));
                }
            }
        }
    }
}