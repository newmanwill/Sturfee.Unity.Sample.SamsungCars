using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Unity.Map.TileProviders;
using Mapbox.Utils;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("Settings")]
    [Tooltip("True if you need to test landscape in the Unity Editor")]
    public bool DebugLandscape;
    //public bool PerspectiveBuildings;  // TODO: unimplemented currently

    [Header("Outside Components")]
    public AbstractMap Map;
    public MapUser MapUser;
    public LocalizationUI LocalizationUi;  

    [Header("Base Prefab Components")]
    public CanvasScaler MapCanvasScaler;
    public GameObject MapTouchPanel;
    public GameObject ToMiniMapButton;
    public GameObject CenterFullMapOnUserButton;
    public GameObject MiniMapCam;    
    public GameObject FullMapCam;

    [Header("Landscape Components")]
    [SerializeField]
    private GameObject _lMiniMap;

    [Header("Portrait Components")]
    [SerializeField]
    private GameObject _pMiniMap;
    [SerializeField]
    private int _pMiniMapCamDistBehindUser;

    // Values determined by orientation 
    [HideInInspector]
    public GameObject MiniMap;

    //private int _miniMapCamHeight;  // TODO: This doesn't really matter unless you're using perspective mode
    private int _miniMapCamDistBehindUser;
    private RenderTexture _miniMapRenderTexture;
    private bool _localized;    // TODO: Change this when the SDK gets updated to have a single check if localized or not

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
#if !UNITY_EDITOR
        DebugLandscape = false;
#endif

        SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
        SturfeeEventManager.Instance.OnLocalizationSuccessful += OnLocalizationSuccessful;
        LocalizationManager.OnLocalizationScanStatusChanged += OnLocalizationScanStatusChanged;

        _miniMapRenderTexture = MiniMapCam.GetComponent<Camera>().targetTexture;

        OrientationSetup();

        MapSettings.Mode = MapMode.None;
        MiniMap.SetActive(false);
        ToMiniMapButton.SetActive(false);
        CenterFullMapOnUserButton.SetActive(false);
    }

    private void OnDestroy()
    {
        SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
        SturfeeEventManager.Instance.OnLocalizationSuccessful -= OnLocalizationSuccessful;
        LocalizationManager.OnLocalizationScanStatusChanged -= OnLocalizationScanStatusChanged;
    }

    #region Events
    private void OnSessionReady()
    {
        InitializeMap();
        MapTouchPanel.GetComponent<MapTouchController>().ComputeMapboxCamBounds();  // TODO: Move this into map initialization function?
    }

    private void OnLocalizationSuccessful()
    {
        _localized = true;
        MapUser.ShowVisionCone();
    }

    void OnLocalizationScanStatusChanged(ScanStatus scanStatus)
    {
        if (scanStatus == ScanStatus.Scanning || scanStatus == ScanStatus.Loading)
        {
            MiniMap.GetComponent<MiniMap>().FullMapButton.interactable = false;
        }
        else if (scanStatus == ScanStatus.PreScan || scanStatus == ScanStatus.PostScan)
        {
            MiniMap.GetComponent<MiniMap>().FullMapButton.interactable = true;
        }
    }
    #endregion

    //// Used to cap zoom out height from custom changes added to the WRLD scripts
    //public float GetHeightOfFullCamFromUser()
    //{
    //    float val = FullMapCam.transform.position.y - User.Instance.transform.position.y;
    //    return val;
    //}

    #region ButtonPresses
    public void CenterFullMapCamOnPlayer()
    {
        FullMapCam.transform.position = XRSessionManager.GetSession().GetXRCameraPosition() + (Vector3.up * 100);
    }

    public void MapModeToggle(bool toFullMap)
    {
        FullMapCam.SetActive(toFullMap);
        MapTouchPanel.SetActive(toFullMap);
        MiniMap.SetActive(!toFullMap);
        ToMiniMapButton.SetActive(toFullMap);
        CenterFullMapOnUserButton.SetActive(toFullMap);

        if (!_localized)
        {
            if (toFullMap)
            {
                LocalizationUi.HideScanButton();
            }
            else
            {
                LocalizationUi.ShowScanButton();
            }
        }

        if (toFullMap)
        {
            CenterFullMapCamOnPlayer();
            MapSettings.Mode = MapMode.Full;
        }
        else
        {
            MapSettings.Mode = MapMode.Mini;
        }
    }
    #endregion

    public void InitializeMap()
    {
        // TODO: Adjust this? Take a variable from the outside?
        Vector3 mapPos;
        mapPos = XRSessionManager.GetSession().GetXRCameraPosition();
        Map.transform.position = mapPos;

        var gpsPos = XRSessionManager.GetSession().LocalPositionToGps(mapPos);
        Vector2d mapboxCoord = new Vector2d(gpsPos.Latitude, gpsPos.Longitude);
        Map.Initialize(mapboxCoord, 16);
        Map.transform.position += Vector3.down * 0.3f;

        MiniMap.SetActive(true);
        MiniMapCam.GetComponent<MiniMapCam>().Initialize(/*_miniMapCamHeight,*/ 100, _miniMapCamDistBehindUser);
        MapSettings.Mode = MapMode.Mini;
    }

    private void OrientationSetup()
    {
        if (DebugLandscape || Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            MapSettings.Orientation = MapOrientation.Landscape;

            MapCanvasScaler.referenceResolution = new Vector2(1920, 1080);
            MapCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            MapCanvasScaler.matchWidthOrHeight = 0.5f;

            MiniMap = Instantiate(_lMiniMap, MapCanvasScaler.transform);

            //_miniMapCamHeight = _lMiniMapCamHeight;
            _miniMapCamDistBehindUser = 0;

            MiniMapCam.GetComponent<Camera>().targetTexture.height = 550;
            MiniMapCam.GetComponent<Camera>().targetTexture.width = 550;
        }
        else if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            MapSettings.Orientation = MapOrientation.Portrait;

            MapCanvasScaler.referenceResolution = new Vector2(1080, 1920);
            MapCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            MiniMap = Instantiate(_pMiniMap, MapCanvasScaler.transform);

            //_miniMapCamHeight = _pMiniMapCamHeight;
            _miniMapCamDistBehindUser = _pMiniMapCamDistBehindUser;

            MiniMapCam.GetComponent<Camera>().targetTexture.height = 512;
            MiniMapCam.GetComponent<Camera>().targetTexture.width = 1080;
            //MiniMapCam.GetComponent<Camera>().targetTexture = _pRenderTex;
            //MiniMapCam.GetComponentInChildren<Camera>().targetTexture = _pRenderTex;
        }
        else
        {
            Debug.LogError("Incorrect device orientation for Map UI");
            //return;
        }

        MiniMap.SetActive(true);
    }
}
