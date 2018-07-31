﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ScrollbarController : MonoBehaviour
{
    private GetAPIData api;

    public Button showScrollbar;
    public Button hideScrollbar;
    public Scrollbar scrollbar;
    public GameObject hitTestObj;

    float zoomSpeed = 0.0001f;

    protected virtual void Awake()
    {        
        scrollbar.gameObject.SetActive(false);

        showScrollbar.onClick.AddListener(ShowTimeUI);
        hideScrollbar.onClick.AddListener(HideTimeUI);

        scrollbar.GetComponent<Scrollbar>().size = 0.05f;

        api = FindObjectOfType(typeof(GetAPIData)) as GetAPIData;

    }

    protected virtual void ShowTimeUI()
    {
        showScrollbar.gameObject.SetActive(false);
        scrollbar.gameObject.SetActive(true);
    }

    protected virtual void HideTimeUI()
    {
        showScrollbar.gameObject.SetActive(true);
        scrollbar.gameObject.SetActive(false);
    }


    protected virtual void ScaleScrollbar()
    {       
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;


            if (scrollbar.isActiveAndEnabled == true)
            {
                scrollbar.GetComponent<Scrollbar>().size -= deltaMagnitudeDiff * zoomSpeed;
                scrollbar.GetComponent<Scrollbar>().size = Mathf.Clamp(scrollbar.GetComponent<Scrollbar>().size, 0, 1.0f);
            }            
        }
    }
    /*
    IEnumerator AlterTime(int numDays)
    {    
        float n_size = api.numDays / 365;

        // days = scrollbar.GetComponent<Scrollbar>().size * 365
        // startDate = value - size/2;
        // endDate = value + size/2;

        //Debug.Log("dayyyyyyy: " + api.numDays);       

        yield return n_size;
       
        // also every change in scrollbar, re-pull the data from api
    }

    public virtual void Update()
    {
        ScaleScrollbar();

        //StartCoroutine(AlterTime(api.numDays));
        //Debug.Log("dayssss" + api.numDays);
    }
    */
}
