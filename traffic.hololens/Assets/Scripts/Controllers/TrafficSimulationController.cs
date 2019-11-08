using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrafficSimulationController : MonoBehaviour, ISpeechHandler
{
    public GameObject nextDialog;
    public Text centerHUD;
    private bool _enableSpeechinput = false;

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (!_enableSpeechinput || eventData == null || string.IsNullOrEmpty(eventData.RecognizedText)) return;
        Debug.Log("TrafficController.OnSpeechKeywordRecognized: " + eventData.RecognizedText);
        SpeechCommands(eventData.RecognizedText.ToLower());
    }

    private void OnEnable()
    {
        _enableSpeechinput = true;
        centerHUD.text = "Decide now! Press the button!";
    }

    public void SpeechCommands(string command)
    {
        if (command == "decided")
        {
            Debug.Log("User has seen traffic.");
            centerHUD.text = string.Empty;
            TcpListner.Results.CallDecidedTicks = DateTime.Now.Ticks;
            TcpListner.Results.CallDecided = TcpListner.TrafficData;

            _enableSpeechinput = false;
            this.gameObject.SetActive(false);
            nextDialog.SetActive(true);
        }
    }

    void Update()
    {
        if (TcpListner.IsTrafficDataValid)
        {
            transform.position = new Vector3(
                TcpListner.TrafficData.PosX,
                TcpListner.TrafficData.PosY,
                TcpListner.TrafficData.PosZ);
            transform.localRotation = Quaternion.Euler(
                TcpListner.TrafficData.RotationX,
                TcpListner.TrafficData.RotationY,
                TcpListner.TrafficData.RotationZ);
        }
        else
        {
            transform.position = new Vector3(0, 0, -10000);
        }
    }

}
