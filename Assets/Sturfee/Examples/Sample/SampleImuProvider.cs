using UnityEngine;

using Sturfee.Unity.XR.Core.Utilities;
using Sturfee.Unity.XR.Core.Providers;
using Sturfee.Unity.XR.Core.Providers.Base;

public class SampleImuProvider : ImuProviderBase {

    public Vector3 OffsetPosition;
    private SampleManager _sampleManager;

    private void Start()
    {
        _sampleManager = GetComponent<SampleManager>();
    }

    public override Quaternion GetOrientation()
    {
        return OrientationUtils.WorldToUnity(_sampleManager.GetOrientationAtCurrentIndex());
    }

    public override Vector3 GetOffsetPosition()
    {
        return OffsetPosition;
    }

    public override ProviderStatus GetProviderStatus()
    {
        return ProviderStatus.Ready;
    }

    public override void Destroy()
    {

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
