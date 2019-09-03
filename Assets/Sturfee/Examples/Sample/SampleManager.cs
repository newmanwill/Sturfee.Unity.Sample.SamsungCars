﻿using System.IO;
using System.Collections;

using UnityEngine;

using Sturfee.Unity.XR.Core.Utilities;

public class SampleManager : MonoBehaviour
{
    [Header("Sample Data")]
    public TextAsset Data;
    public Texture2D[] Frames;

    public bool CacheFrames = true;

    [HideInInspector]
    private string _dataJson;
    private OrientationData[] _orientationData;
    private string _cachedDataDirectory;

    public int Index
    {
        get
        {
            return _count;
        }
    }

    private int _count;

    private void Awake()
    {
        _cachedDataDirectory = Path.Combine(Application.persistentDataPath, "Sample Scene Frames");
    }

    private void Start()
    {

        if(CacheFrames)
        {
            if(!Directory.Exists(_cachedDataDirectory))
            {
                Directory.CreateDirectory(_cachedDataDirectory);
            }
        }

        _dataJson = Data.text;
        _orientationData = JsonHelper.FromJson<OrientationData>(_dataJson);

        PopulateFrames();
    }

    private void Update()
    {

        //Detect Left swipe
        if(Input.GetAxis("Mouse X") < -0.5f)
        {
            if(_count == 0)
            {
                _count = _orientationData.Length - 1;
            }
            else
            {
                _count--;
            }
        }

        //Detect Right swipe
        if (Input.GetAxis("Mouse X") > 0.5f)
        {
            if (_count == (_orientationData.Length - 1))
            {
                _count = 0;
            }
            else
            {
                _count++;
            }
        }

    }

    public Quaternion GetOrientationAtCurrentIndex()
    {
        return _orientationData[_count].quaternion;
    }

    public Quaternion GetOrientationAtIndex(int i)
    {
        return _orientationData[i].quaternion;
    }

    public Texture2D GetFrameAtCurrentIndex()
    {
        return Frames[_count];
    }

    public Texture2D GetFrameAtIndex(int i)
    {
        return Frames[i];
    }


    private void PopulateFrames()
    {
        Frames = new Texture2D[181];

        for (int i = 0; i < 181; i++)
        {
            if (File.Exists(Path.Combine(_cachedDataDirectory, "Image_" + i + ".jpg")))
            {
                StartCoroutine(DownloadFrameFromCache(i));
            }
            else
            {
                StartCoroutine(DownloadFrame(i));
            }
        }
    }

    private IEnumerator DownloadFrame(int i)
    {
        WWW www = new WWW("https://s3-us-west-1.amazonaws.com/sdk-sample-scene/" + i + ".jpg");

        yield return www;

        if(string.IsNullOrEmpty(www.error))
        {
            Texture2D texture2D = new Texture2D(0, 0);
            texture2D.LoadImage(www.bytes);

            Frames[i] = texture2D;

            if (!File.Exists(Path.Combine(_cachedDataDirectory, "Image_" + i + ".jpg")))
            {
                //Debug.Log("Caching Image_" + i + ".jpg");
                File.WriteAllBytes(Path.Combine(_cachedDataDirectory, "Image_" + i + ".jpg"), www.bytes);
            }

        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    private IEnumerator DownloadFrameFromCache(int i)
    {
        //Debug.Log("Loading Image_" + i + ".jpg from Cache");

        Texture2D texture2D = new Texture2D(0, 0);
        texture2D.LoadImage(File.ReadAllBytes(Path.Combine(_cachedDataDirectory, "Image_" + i + ".jpg")));

        Frames[i] = texture2D;

        yield return null;
    }


    [System.Serializable]
    private class OrientationData
    {
        public double latitude;
        public double longitude;
        public string imName;
        public Quaternion quaternion;
    }

}
