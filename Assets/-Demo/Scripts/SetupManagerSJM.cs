using System;
using System.Collections;
using System.Collections.Generic;
using Sturfee.Unity.XR.Core.Events;
using UnityEngine;

public class SetupManagerSJM : MonoBehaviour {

    public GameObject Car1;
    public GameObject Car2;

    [Header("Testing")]
    public bool StartOnSessionReady;

    private void Awake()
    {
        Car1.SetActive(false);
        Car2.SetActive(false);
    }

    void Start () {
        SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;         SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
        Hashtable test = new Hashtable();
        print("Car 1 Instance ID: " + Car1.GetInstanceID());
    }

    #region EventFunctions
    private void OnLocalizationSuccessful()
    {
        StartCoroutine(StartAr());
    }

    private void OnSessionReady()
    {
        if (StartOnSessionReady)
        {
            StartCoroutine(StartAr());
        }
    }
    #endregion

    private IEnumerator StartAr()
    {
        Car1.SetActive(true);
        yield return new WaitForSeconds(6);
        Car2.SetActive(true);
    }

}
