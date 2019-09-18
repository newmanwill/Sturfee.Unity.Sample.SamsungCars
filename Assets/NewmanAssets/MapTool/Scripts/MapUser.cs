using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUser : MonoBehaviour {

    [SerializeField]
    private GameObject _visionCone;
    private Vector3 _rotation;

    void Start()
    {
        _visionCone.SetActive(false);
    }

    private void LateUpdate()
    {
        // Map Rep
        _rotation = transform.rotation.eulerAngles;
        _rotation.x = 0;
        _rotation.z = 0;
        transform.rotation = Quaternion.Euler(_rotation);
    }

    public void ShowVisionCone()
    {
        _visionCone.SetActive(true);
    }
}
