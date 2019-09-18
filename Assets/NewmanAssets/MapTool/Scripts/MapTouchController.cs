using UnityEngine;
using UnityEngine.EventSystems;

public class MapTouchController : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public static MapTouchController Instance;

    public float MapMoveSensitivity = 0.3f;
    public float BaseMapCamBound = 100;

    private int _camBoundsUp;
    private int _camBoundsDown;
    private int _camBoundsRight;
    private int _camBoundsLeft;

    private Vector2 _prevTouchPos;

    void Awake()
    {
        Instance = this;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        _prevTouchPos = data.position;
    }

    public void OnDrag(PointerEventData data)
    {
        Vector2 touchDifference = (_prevTouchPos - data.position) * MapMoveSensitivity;

        Vector3 camPos = MapManager.Instance.FullMapCam.transform.position;
        camPos.x += touchDifference.x;
        camPos.z += touchDifference.y;

        //Keep the camera in the boundaries of the map
        if (camPos.x > _camBoundsRight)
        {
            camPos.x = _camBoundsRight;
        }
        else if (camPos.x < _camBoundsLeft)
        {
            camPos.x = _camBoundsLeft;
        }

        if (camPos.z > _camBoundsUp)
        {
            camPos.z = _camBoundsUp;
        }
        else if (camPos.z < _camBoundsDown)
        {
            camPos.z = _camBoundsDown;
        }

        MapManager.Instance.FullMapCam.transform.position = camPos;
        _prevTouchPos = data.position;
    }

    public void ComputeMapboxCamBounds()
    {
        Vector3 mapCenterCoord = MapManager.Instance.Map.transform.GetChild(0).position;
        float distFromMapCenterToEdge = Mathf.Abs((mapCenterCoord.x - MapManager.Instance.Map.transform.GetChild(1).position.x));

        float camRectHeight = MapManager.Instance.FullMapCam.GetComponent<Camera>().pixelHeight;
        float camRectWidth = MapManager.Instance.FullMapCam.GetComponent<Camera>().pixelWidth;
        float camRectRatio;

        float vertOffset = BaseMapCamBound;
        float horOffset = 0;

        if(camRectWidth > camRectHeight)
        {
            // Landscape
            camRectRatio = 1 - (camRectHeight / camRectWidth);
            horOffset = BaseMapCamBound * camRectRatio * 8;
        }
        // Current setup has no changes for Portrait mode

        _camBoundsUp = (int)(mapCenterCoord.z + distFromMapCenterToEdge - vertOffset);
        _camBoundsDown = (int)(mapCenterCoord.z - distFromMapCenterToEdge + vertOffset);
        _camBoundsRight = (int)(mapCenterCoord.x + distFromMapCenterToEdge - horOffset);
        _camBoundsLeft = (int)(mapCenterCoord.x - distFromMapCenterToEdge + horOffset);

        //print("Cam Bounds Up: " + _camBoundsUp);
        //print("Cam Bounds Down: " + _camBoundsDown);
        //print("Cam Bounds Right: " + _camBoundsRight);
        //print("Cam Bounds Left: " + _camBoundsLeft);
    }
}
