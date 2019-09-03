using UnityEngine;
using UnityEditor;

using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;
using Sturfee.Unity.XR.Core.Models.Location;

public class DebugGpsProvider : GpsProviderBase 
{

    [HideInInspector]
    public double LastKnownLatitude;
    [HideInInspector]
    public double LastKnownLongitude;
    [HideInInspector]
    public double LastKnownHeight;

    public double Latitude = 37.779044d;
    public double Longitude = -122.416594d;
    public double Height = 0;
    public ProviderStatus ProviderStatus = ProviderStatus.Ready;

    [Tooltip("Simulate the time it takes for GPS sensor until it is ready")]
    public float Delay = 5.0f;
    public bool SetLastKnownGps;

    private GpsPosition _testGps = new GpsPosition();
    private float _startTime;

    private void Awake()
    {
        _testGps.Latitude = Latitude;
        _testGps.Longitude = Longitude;
        _testGps.Height = Height;
    }

    private void Start()
    {
        _startTime = Time.time;
    }

    private void Update()
    {

        if(Time.time - _startTime < Delay)
        {
            _testGps = null;
            ProviderStatus = ProviderStatus.Initializing;
            return;
        }

        if(_testGps == null)
        {
            _testGps = new GpsPosition()
            {
                Latitude = Latitude,
                Longitude = Longitude,
                Height = Height
            };

            ProviderStatus = ProviderStatus.Ready;
        }
    }


    public override  GpsPosition GetGPSPosition()
    {
        return _testGps;
    }

    public override GpsPosition GetLastKnownGpsPosition()
    {

        var gps = GetGPSPosition();

        if (gps == null)
        {
            if (LastKnownLatitude == 0 || LastKnownLongitude == 0)
            {
                return null;
            }
            else
            {
                GpsPosition lastKnownGps = new GpsPosition
                {
                    Latitude = LastKnownLatitude,
                    Longitude = LastKnownLongitude,
                    Height = LastKnownHeight
                };

                return lastKnownGps;
            }
        }
        else
        {
            return gps;
        }

    }

    public override ProviderStatus GetProviderStatus()
    {
        return ProviderStatus;
    }
  

	public override void Destroy ()
	{
		
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(DebugGpsProvider))]
public class DebugGpsProvider_Editor : Editor
{
    private SerializedProperty _useLastKnown;
    private SerializedProperty _lastKnownLatitude;
    private SerializedProperty _lastKnownLongitude;
    private SerializedProperty _lastKnownHeight;

    private void OnEnable()
    {
        _useLastKnown = serializedObject.FindProperty("SetLastKnownGps");
        _lastKnownLatitude = serializedObject.FindProperty("LastKnownLatitude");
        _lastKnownLongitude = serializedObject.FindProperty("LastKnownLongitude");
        _lastKnownHeight = serializedObject.FindProperty("LastKnownHeight");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();
        
        if(_useLastKnown.boolValue)
        {
            _lastKnownLatitude.doubleValue = EditorGUILayout.DoubleField("Latitude", _lastKnownLatitude.doubleValue);
            _lastKnownLongitude.doubleValue = EditorGUILayout.DoubleField("Longitude", _lastKnownLongitude.doubleValue);
            _lastKnownHeight.doubleValue = EditorGUILayout.DoubleField("Height", _lastKnownHeight.doubleValue);
        }
        else
        {
            _lastKnownLatitude.doubleValue = 0;
            _lastKnownLongitude.doubleValue = 0;
            _lastKnownHeight.doubleValue = 0;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif