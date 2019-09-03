using System.Collections;

using UnityEngine;
using UnityEngine.XR.iOS;

using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;

[RequireComponent(typeof(SturfeeArKitManager))]
public class ArKitImuProvider: ImuProviderBase
{
    private Vector3 _position;
    private Quaternion _orientation;

    private Vector3 _lastPosition = Vector3.zero;
    private Quaternion _lastOrientation = Quaternion.identity;
    private Quaternion _resetOffset = Quaternion.identity;

    private bool _sessionStarted;
    private float _theta;
    private float _trueHeading;

    private void Start()
    {
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;

        // ARKitWorldTrackingSessionConfiguration is deprecated in Xcode, so calculate pose readings using Compass
        Input.compass.enabled = true;
        StartCoroutine(InitialPositionRotation());
    }

    private void Update()
    {
        if(!_sessionStarted)
        {
            return;
        }

        Matrix4x4 matrix =  UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraPose();
        _position = _lastPosition + GetWorldPosition( UnityARMatrixOps.GetPosition(matrix));
        _orientation = _resetOffset * GetWorldRotation(UnityARMatrixOps.GetRotation(matrix));
    }

    public override Vector3 GetOffsetPosition()
    {
        return _position;
    }

    public override  Quaternion GetOrientation()
    {
        return _orientation;
    }

    public override ProviderStatus GetProviderStatus()
    {
        return GetComponent<SturfeeArKitManager>().GetProviderStatus();
    }

	public override void Destroy ()
	{
		UnityEngine.Object.Destroy(GameObject.Find("_OnAppFocusOrientationHelper"));
	}

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            StartCoroutine(ResetOrientation());
        }
        else
        {
            _lastPosition = _position;
            _lastOrientation = _orientation;
        }
    }

    private void FirstFrameUpdate(UnityARCamera cam)
    {
        _sessionStarted = true;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
    }

    private IEnumerator InitialPositionRotation()
    {
        // Wait until we start getting readings from ArKit
        yield return new WaitUntil(()=>_sessionStarted);

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

    private IEnumerator ResetOrientation()
    {        

        Matrix4x4 matrix =  UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraPose();

        var orientation = UnityARMatrixOps.GetRotation(matrix);
        while 
            (
                orientation == Quaternion.identity ||
                (
                   orientation.x == 0 && 
                   orientation.y == 0 && 
                   orientation.z == 0 && 
                   orientation.w == -1
                )
            )
        {            
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        _orientation = _resetOffset * orientation;

        var orientationHelper = new GameObject();
        orientationHelper.name = "_OnAppFocusOrientationHelper";

        orientationHelper.transform.rotation = _lastOrientation;
        var oldForward = orientationHelper.transform.forward;

        orientationHelper.transform.rotation = _orientation;
        var newForward = orientationHelper.transform.forward;  

        var angle = Quaternion.Angle(_lastOrientation, _orientation);
        //        Debug.Log(" [IOSTrackingProvider] : Angle is : " + angle);

        if (Mathf.Abs(angle) <= 30)
        {
            _resetOffset = Quaternion.FromToRotation(oldForward, newForward);
        }
    }

    //private void OnGUI()
    //{
    //    string guiText =
    //        "ArKit Pos : " + _position + "\n" +
    //        "ArKit Rot : " + _orientation.eulerAngles.ToString("f3") + "\n" +
    //        "Last Pos : " + _lastPosition + "\n" +
    //        "Reset Rot : " + _lastOrientation.eulerAngles.ToString("f3") + "\n" +
    //        "Status :" + _sessionStarted;

    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 40;

    //    GUI.Label(new Rect(Screen.width - 800, Screen.height - 500, 400, 400), guiText, style);
    //}
}
