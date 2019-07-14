using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficControl : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (TcpListner.IsTrafficDataValid)
        {
            transform.position = new Vector3(
                TcpListner.TrafficData.PosX,
                TcpListner.TrafficData.PosY,
                TcpListner.TrafficData.PosZ);
        }
    }
}
