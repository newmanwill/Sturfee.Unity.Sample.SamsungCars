using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCam : MonoBehaviour
{
    private int _camHeightFromUser;

    private void Update()
    {
        transform.position = MapManager.Instance.MapUser.transform.position + Vector3.up * _camHeightFromUser;
    }

    public void Initialize(int camHeight, int camDistBehindUser)
    {
        if (MapSettings.Orientation == MapOrientation.Landscape)
        {
            // This script will be used to hold the non-rotating camera in place over the Map User
            _camHeightFromUser = camHeight; //MapManager.
            this.enabled = true;
        }
        else
        {
            // This script no longer needs to be used, and will be parented to the Map User
            transform.parent = MapManager.Instance.MapUser.transform;
            transform.localPosition = new Vector3(0, camHeight, camDistBehindUser);
            this.enabled = false;
        }

        // This setup fixes a render texture bug
        gameObject.SetActive(true);
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
