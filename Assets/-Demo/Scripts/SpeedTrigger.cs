using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedTrigger : MonoBehaviour {

    public float AdjustmentTime;
    public float TargetSpeed;

	void Start () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        print("Hit Speed Collider Trigger");

        if (other.tag == "HoverCar")
        {
            other.GetComponent<HoverCarController>().AdjustSpeedOverTime(TargetSpeed, AdjustmentTime);
        }
    }
}
