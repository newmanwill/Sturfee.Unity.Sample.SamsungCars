using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PathCreation.Examples;
using UnityEngine;

public class HoverCarController : MonoBehaviour {

    [SerializeField]
    private TrailRenderer _trailRenderer;

    private PathFollower _pathFollower;

	void Start () {
        _pathFollower = GetComponent<PathFollower>();

        //StartCoroutine(StartSetup());
        StartSetupAA();
    }

    public void AdjustSpeedOverTime(float targetSpeed, float adjustmentTime)
    {
        print("Adjusting Speed");
        StartCoroutine(SpeedAdjustment(targetSpeed, adjustmentTime));
    }

    private IEnumerator SpeedAdjustment(float targetSpeed, float adjustmentTime)
    {
        float startSpeed = _pathFollower.speed;
        float speedDiff = startSpeed - targetSpeed;
        float endTime = Time.time + adjustmentTime;
        //float curSpeed = startSpeed;

        print("Speed Difference: " + speedDiff);


        while(Time.time < endTime)
        {
            _pathFollower.speed = targetSpeed + (speedDiff * ( (/*adjustmentTime -*/ (endTime - Time.time)) / adjustmentTime) );
            print("Car Speed: " + _pathFollower.speed);
            yield return null;
        }

        _pathFollower.speed = targetSpeed;
        print("Completed speed adjustment");
    }

    #region StartSetups
    private IEnumerator StartSetupCR()
    {
        _trailRenderer.time = 0;
        yield return new WaitForSeconds(1);
        _trailRenderer.time = 1;
    }

    private async void StartSetupAA()
    {
        _trailRenderer.time = 0;
        await Task.Delay(TimeSpan.FromSeconds(1));
        _trailRenderer.time = 1;
    }
    #endregion

}
