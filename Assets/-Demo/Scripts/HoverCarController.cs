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
    [SerializeField]
    private GameObject[] JetFlames;

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

    private void TurnOffJetFlames()
    {
        foreach(GameObject jetFlame in JetFlames)
        {
            jetFlame.SetActive(false);
        }
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

        // If the float is supposed to be 0, then turn off the jet flames
        if (_pathFollower.speed < 0.001f)
        {
            TurnOffJetFlames();
        }

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
