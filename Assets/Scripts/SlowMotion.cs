// unity only file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    public float slowMotionTimescale = 0.1f;
    public bool enableSlowMo = false;
    private bool isInSlowMo = false;

    private float startTimescale;
    private float startFixedDeltaTime;

    void Start()
    {
        startTimescale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
    }

    void Update()
    {
        if (enableSlowMo && !isInSlowMo)
        {
            StartSlowMotion();
        }

        if (!enableSlowMo && isInSlowMo)
        {
            StopSlowMotion();
        }
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     StartSlowMotion();
        // }
        // 
        // if (Input.GetKeyUp(KeyCode.Space))
        // {
        //     StopSlowMotion();
        // }
    }

    private void StartSlowMotion()
    {
        Time.timeScale = slowMotionTimescale;
        //Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimescale;
        isInSlowMo = true;
    }

    private void StopSlowMotion()
    {
        Time.timeScale = startTimescale;
        //Time.fixedDeltaTime = startFixedDeltaTime;
        isInSlowMo = false;
    }
}
