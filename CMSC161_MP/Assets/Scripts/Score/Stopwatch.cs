using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stopwatch : MonoBehaviour
{
    public float currentTime;

    void Start()
    {
        currentTime = 0;        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
    }
}
