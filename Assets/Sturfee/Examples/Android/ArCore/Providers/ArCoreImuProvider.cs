using System.Collections;

using UnityEngine;

using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;

using GoogleARCore;

[RequireComponent(typeof(SturfeeArCoreManager))]

public class ArCoreImuProvider :  ImuProviderBase
{
    private Quaternion _headingOffset = Quaternion.identity;
    private Quaternion _orientation = Quaternion.identity;
    private Vector3 _position = Vector3.zero;

    private float _theta;

    private float _trueHeading;

    private void Awake()
    {
        transform.position = Vector3.zero;
        transform.rotation = new Quaternion();
    }

    private void Start()
    {
        Input.compass.enabled = true;

        StartCoroutine(InitialPositionRotation());
    }

    private void Update()
    {
        _position = GetWorldPosition(Frame.Pose.position);
        _orientation = GetWorldRotation(Frame.Pose.rotation);
    }

    public override  Vector3 GetOffsetPosition()
    {        
        return _position;
    }

    public override  Quaternion GetOrientation()
    {   
        return _orientation;   
    }

    public override ProviderStatus GetProviderStatus()
    {
        return GetComponent<SturfeeArCoreManager>().GetProviderStatus();
    }

	public override void Destroy ()
	{
		UnityEngine.Object.Destroy(GameObject.Find("_rotationHelper"));
	}

    private IEnumerator InitialPositionRotation()
    {
        // Wait until we start getting readings from ArCore
        while (Session.Status != SessionStatus.Tracking)
        {
            yield return null;
        }
          
        _trueHeading = Input.compass.trueHeading;
        _theta = -((Mathf.PI * _trueHeading) / 180);
    }

    private Vector3 GetWorldPosition(Vector3 relativePos)
    {
        Vector3 worldPos = new Vector3
        {
            x = (relativePos.x * Mathf.Cos(_theta)) - (relativePos.z * Mathf.Sin(_theta)),
            y = relativePos.y,
            z = (relativePos.z * Mathf.Cos(_theta)) + (relativePos.x * Mathf.Sin(_theta))
        };

        return worldPos;
    }

    private Quaternion GetWorldRotation(Quaternion relative)
    {
        var euler = relative.eulerAngles;
        euler.y += _trueHeading;

        return Quaternion.Euler(euler);
    }

    private void OnGUI()
    {
        //string guiText = 
        //    "World Pos : " + _position + "\n" + 
        //    "World Rot : " + _orientation.eulerAngles.ToString("f3") + "\n" +
        //    "True Heading(Start) : " + _trueHeading + "\n" +
        //    "True Heading : " + Input.compass.trueHeading + "\n" +
        //    "Status :" + Session.Status.ToString();

        //GUIStyle style = new GUIStyle();
        //style.fontSize = 40;

        //GUI.Label(new Rect(Screen.width - 800, Screen.height -  500, 400, 400), guiText, style);
    }
}
