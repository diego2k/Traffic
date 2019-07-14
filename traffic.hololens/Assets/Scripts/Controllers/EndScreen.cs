using System;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    private DateTime _startTime;

    public GameObject nextDialog;
    public Text points;

    void Start()
    {
    }

    private void OnEnable()
    {
        _startTime = DateTime.Now;
        points.text = TcpListner.Points.ToString();
        Debug.Log("EndScreen Start " + TcpListner.Points.ToString());
    }

    void Update()
    {
        if(_startTime.AddSeconds(5) < DateTime.Now)
        {
            this.gameObject.SetActive(false);
            nextDialog.SetActive(true);
        }
    }
}
