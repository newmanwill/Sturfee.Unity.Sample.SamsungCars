using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour {

    public Button FullMapButton;

	void Start () {
        FullMapButton.onClick.AddListener(delegate { MapManager.Instance.MapModeToggle(true); });
    }
}
