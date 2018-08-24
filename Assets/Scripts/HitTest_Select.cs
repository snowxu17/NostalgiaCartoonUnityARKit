﻿using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lean.Touch;

namespace UnityEngine.XR.iOS
{
    public class HitTest_Select : MonoBehaviour
    {        
        private static HitTest_Select ht;
        public static HitTest_Select Instance()
        {
            if (!ht)
            {
                ht = FindObjectOfType(typeof(HitTest_Select)) as HitTest_Select;
            }
            return ht;
        }

        public Transform m_HitTransform;
        public float maxRayDistance = 30.0f;
        public LayerMask collisionLayer = 1 << 10;  //ARKitPlane layer

        public Button scanButton;
        public Button placeButton;
        public Dropdown s_dropdown;        

        public GameObject floor;
        public GameObject wall1;
        public GameObject wall2;

        Quaternion newQuat; 

        public bool isDetecting = false;

        bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
        {
            List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
            if (hitResults.Count > 0)
            {
                foreach (var hitResult in hitResults)
                {
                    Debug.Log("Got hit!");
                    m_HitTransform.position = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                    m_HitTransform.rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
                    Debug.Log(string.Format("x:{0:0.######} y:{1:0.######} z:{2:0.######}",
                                              m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));
                    return true;
                }
            }
            return false;
        }

        public void Start()
        {
            placeButton.onClick.AddListener(PlaceWhenHitButton);            

            isDetecting = true;
            newQuat.Set(0, 50, 0, 1);

        }

        public void PlaceWhenHitButton()
        {
            foreach (Transform child in gameObject.transform)
            {               
                //Debug.Log(placeButton.name + " is pressed!");
                if (child.gameObject.GetComponent<LeanSelectable>().IsSelected == true)
                {
                    child.gameObject.GetComponent<LeanSelectable>().Deselect();
                }
            }
        }


        public void TransformSingleObject()
        {            
            if (isDetecting == false)
            {            
                foreach (Transform child in gameObject.transform)
                {                   
                    //Debug.Log("debug:" + child.gameObject.name);
                    //child.gameObject.GetComponent<LeanRotate>().enabled = true;
                    child.gameObject.GetComponent<LeanTranslate>().enabled = true;
                    child.gameObject.GetComponent<LeanScale>().enabled = true;                    
                }
            }            
        }


        private void DeselectOnTimeChange()
        {            
            if (s_dropdown.isActiveAndEnabled == true)
            {
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.GetComponent<LeanSelectable>().enabled = false;

                }
            }

            if (s_dropdown.isActiveAndEnabled == false)
            {
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.GetComponent<LeanSelectable>().enabled = true;
                }
            }          
        }

        public void DetectionOff()
        {            
            isDetecting = false;
            Debug.Log("Plane detection off!");                               
        }


        private bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }


        public void ARPlaceObjectsOnPlane()
        {            
            if (Input.GetMouseButtonDown(0) && isDetecting == true && ManagerScript.arKitPlaneRenderers.Count > 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Ray ray = Camera.main.ViewportPointToRay(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0));

                RaycastHit hit;

                //we'll try to hit one of the plane collider gameobjects that were generated by the plugin
                //effectively similar to calling HitTest with ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent
                if (Physics.Raycast(ray, out hit, maxRayDistance, collisionLayer))
                {

                    //we're going to get the position from the contact point
                    m_HitTransform.position = hit.point;
                    Debug.Log(string.Format("Position x:{0:0.######} y:{1:0.######} z:{2:0.######}",
                                            m_HitTransform.position.x, m_HitTransform.position.y, m_HitTransform.position.z));

                    //and the rotation from the transform of the plane collider
                    m_HitTransform.rotation = hit.transform.rotation;
                    //m_HitTransform.rotation = newQuat;
                    Debug.LogWarning("ARPlaceObjectsOnPlane about to call deactivate all planes");
                    ManagerScript.instance.debugString += " ARPlaceObjOnPlane ";
                    ManagerScript.instance.DeActivateAllPlanes();

                    isDetecting = false;
                }

                isDetecting = false;
            }
        }


        void Update()
        {
            if (scanButton.isActiveAndEnabled == true)
            {
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.GetComponent<LeanSelectable>().enabled = false;
                }
            }
                              
            if (scanButton.isActiveAndEnabled == false)
            {                                                           
                DeselectOnTimeChange();

                if (isDetecting == true)
                {
                    foreach (Transform child in gameObject.transform)
                    {
                        child.gameObject.GetComponent<LeanSelectable>().enabled = false;
                    }

                    ARPlaceObjectsOnPlane();
                }                         

                if (isDetecting == false)
                {
                    ShadowPlane.ShowShadowPlane(floor, wall1, wall2);

                    foreach (Transform child in gameObject.transform)
                    {                        
                        child.gameObject.GetComponent<LeanSelectable>().enabled = true;   

                        //if(child.gameObject.GetComponent<LeanSelectable>().IsSelected == true)
                        //{
                        //    placeButton.gameObject.SetActive(true);
                        //}
                           
                    }
                }
            }
        }
    }
}
