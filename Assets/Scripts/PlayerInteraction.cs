using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ARChristmas.Utilities.Save;

namespace ARChristmas
{
    public class PlayerInteraction : MonoBehaviour
    {
        // AR settings
        private ARRaycastManager arRaycastManager;
        private ARPlaneManager arPlaneManager;
        private List<ARRaycastHit> arRayHits;
        
        // Game contents settings
        public static ChristmasTree christmasTree;
        public GameObject Snow;
        public GameObject christmasTreePrefab;
        public GameObject decorationItemPrefab;

        private void Start() 
        {
            // AR settings
            arRaycastManager = GetComponent<ARRaycastManager>();
            arPlaneManager = GetComponent<ARPlaneManager>();
            arRayHits = new List<ARRaycastHit>();

            // Scene settings
            Snow.SetActive(false);
        }

        private void Update() 
        {
            if (! isScreenTouched(out Vector2 screenTouchPos))
                return;
            
            if(christmasTree == null) 
            {   
                if (arRaycastManager.Raycast(screenTouchPos, arRayHits, TrackableType.PlaneWithinPolygon))
                {
                    PlaceChristmasTree(screenTouchPos);
                }
            }
            else
            {
                if (isHitTree(screenTouchPos, out Vector3 hitPoint) && TabState.CurTabType == TabType.OrnamentColor)
                {
                    Decorate(hitPoint);
                }
            }
        }

        /// <summary>
        /// detect user touch on the screen
        /// </summary>
        private bool isScreenTouched(out Vector2 screenTouchPos) 
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
        private bool isHitTree(Vector2 screenTouchPos, out Vector3 hitPoint) 
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
        /// create and register new decoration item to ChristmasTree class
        /// </summary>
        private void Decorate(Vector3 hitPoint) 
        {
            // creation
            (Vector3 localPos, Color color) newItem = CreateNewOrnamentBall(hitPoint);

            // registration
            christmasTree.decorationItemLocalPos.Add(newItem.localPos);
            christmasTree.decorationItemColors.Add(newItem.color);
        }

        /// <summary>
        /// create new ornament ball and return info which is registered to christmasTree
        /// </summary>
        private (Vector3, Color) CreateNewOrnamentBall(Vector3 hitPoint)
        {
            var newItem = Instantiate(decorationItemPrefab, hitPoint, Quaternion.identity, christmasTree.transform.Find("Christmas Tree")) as GameObject;

            Color color = UIController.SelectedColor;
            newItem.GetComponent<Renderer>().material.SetColor("Color_A9AB75C1", color); // base color
            newItem.GetComponent<Renderer>().material.SetColor("Color_B37F01A0", color); // emission color

            return (newItem.transform.localPosition, color);
        }

        private void PlaceChristmasTree(Vector2 screenTouchPos) 
        {
            Pose placementPose = arRayHits[0].pose;
            christmasTree = Instantiate(christmasTreePrefab, placementPose.position, placementPose.rotation).GetComponent<ChristmasTree>();

            // execute loading if user pressed "Load" button.
            if (ChristmasTree.isLoadingON) 
            {
                var loadObj = SaveLoadManager<TreeSaveData>.GetLoadData(Application.persistentDataPath + "/save.txt");
                christmasTree.DecorateWithLoadData(loadObj);
            }

            christmasTree.SetTreeLight(false);
            Snow.SetActive(true);
            FindObjectOfType<UIController>().tabBar.SetActive(true);

            SwitchToOcclusionPlane();
        }

        /// <summary>
        /// destroy all ar planes in the scene
        /// switch AR plane from "AR Default Plane" to "AR Occlusion Plane"
        /// </summary>
        private void SwitchToOcclusionPlane() 
        {
            foreach(ARPlane plane in arPlaneManager.trackables)
            {
                Destroy(plane.gameObject); // do not forget .gameObject
            }
            arPlaneManager.planePrefab = Resources.Load("Prefab/AR Occlusion Plane") as GameObject;
        }
    }
}