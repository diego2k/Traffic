using System;
using UnityEngine;
using UnityEngine.UI;

public class ObserverResultScreenController : MonoBehaviour
{
    private DateTime _startTime;

    public GameObject nextDialog;

    public Text centerHUD;

    void Start()
    {
    }

    private void OnEnable()
    {
        _startTime = DateTime.Now;
        centerHUD.text = "";
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
