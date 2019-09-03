using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class PathFollow : MonoBehaviour {

    public PathCreator PathCreator;
    public float Speed = 5;
    private float _distanceTravelled;

	void Start () {
		
	}

    private void Update()
    {
        _distanceTravelled += Speed * Time.deltaTime; //* _distanceTravelled;
        transform.position = PathCreator.path.GetPointAtDistance(_distanceTravelled);
        transform.rotation = PathCreator.path.GetRotationAtDistance(_distanceTravelled);
    }
}
