using System;
using UnityEngine;
using UnityEngine.UI;

public class ObserverResultScreenController : MonoBehaviour
{
    private DateTime _startTime;

    public GameObject nextDialog;

    void Start()
    {
    }

    private void OnEnable()
    {
        _startTime = DateTime.Now;
        Debug.Log("Result screen Start " + TcpListner.Points.ToString());
    }

    void Update()
    {
        if (_startTime.AddSeconds(7) < DateTime.Now)
        {
            this.gameObject.SetActive(false);
            nextDialog.SetActive(true);
        }
    }
}
