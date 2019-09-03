using UnityEngine;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Utilities;


/// <summary>
/// Helper class to use values received from tracking plugins like ArCore/ArKit with Sturfee 
/// </summary>
public class TrackingUtils : MonoBehaviour
{
    // XrCameraPos = AbsolutePos + RelativePos
    // AbsolutePos = Current GPS/VPS position converted into Local Coordinate system (Unity space).             
    // RelativePos = Position obtained from Tracking plugins like ArKit/ArCore (Read from ImuProvider.GetOffsetPosition())

    // Current GPS/VPS position converted into local coordinate system.             
    private static Vector3 _absolutePos;

    // Position from where we calculate new relativePos
    private static Vector3 _relativeOrigin;

    private bool _localized;

    private void Awake()
    {
        SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
    }

    private void Update()
    {
        if (!_localized)
        {

            _absolutePos = XRSessionManager.GetSession().GetXRCameraPosition();
        }
    }

    private void OnDestroy()
    {
        SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
    }

    /// <summary>
    /// Updates position obtained from Tracking Plugin to Sturfee local position
    /// </summary>
    /// <returns>The rotated position based on offset correction received from VPS.</returns>
    /// <param name="position">Position.</param>
    public static Vector3 TrackingPosToSturfeePos(Vector3 pos)
    {
        // To align the position value from tracking plugins with Sturfee 3D models we rotate this
        // value using orientation correction received from VPS and subtract tracking
        // plugin's position value at the time of localization 
        Vector3 relativePos = (RotatePoint(pos) - RotatePoint(_relativeOrigin)); 

        return _absolutePos + relativePos;
    }

    private void OnLocalizationSuccessful()
    {
        _localized = true;

        // We set _relativeOrigin to current position from tracking plugin. This enables us to calculate new relativePos from
        // this point onwards.
        _relativeOrigin = XRSessionManager.GetSession().ImuProvider.GetOffsetPosition();    

        //AbsolutePos is now set to VPS and at this stage XrCamPos = VPS --> Local and relativePos = 0.
        _absolutePos = XRSessionManager.GetSession().GpsToLocalPosition(XRSessionManager.GetSession().GetLocationCorrection());
    }

    /// <summary>
    /// Rotates a point based on orientation correction received from VPS service.
    /// </summary>
    /// <returns>The point.</returns>
    /// <param name="pos">Position.</param>
    private static Vector3 RotatePoint(Vector3 pos)
    {
        Vector3 rotatedPos = new Vector3();

        //Create a parent object to rotate pos around this object
        GameObject parent = new GameObject();

        GameObject posToRotate = new GameObject();
        posToRotate.transform.parent = parent.transform;

        //Before rotation
        posToRotate.transform.position = pos;

        //GetOrientationCorrection() is identity before Localization
        Quaternion rotInWorld = XRSessionManager.GetSession().GetOrientationCorrection() * OrientationUtils.UnityToWorld(parent.transform.rotation);

        //Rotate parent object which will rotate its child(posToRotate) around it
        parent.transform.rotation = OrientationUtils.WorldToUnity(rotInWorld);

        rotatedPos = posToRotate.transform.position;    // Unity WorldPos of pos

        Destroy(posToRotate);
        Destroy(parent);

        return rotatedPos;
    }
}
