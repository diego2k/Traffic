using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlaneSpeechHandler : MonoBehaviour, ISpeechHandler
{
    public GameObject nextDialog;
    public Text centerHUD;
    private bool _enableSpeechinput = false;

    public void OnEnable()
    {
        _enableSpeechinput = true;
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (!_enableSpeechinput || eventData == null || string.IsNullOrEmpty(eventData.RecognizedText)) return;
        Debug.Log("PlaneSpeech.OnSpeechKeywordRecognized: " + eventData.RecognizedText);
        SpeechCommands(eventData.RecognizedText.ToLower());
    }

    public void SpeechCommands(string command)
    {
        if (command == "traffic")
        {
            Debug.Log("User has decided.");
            centerHUD.text = "Decide now! Say 'decided'";
            TcpListner.Results.CallTrafficTicks = DateTime.Now.Ticks;
            TcpListner.Results.CallTraffic = TcpListner.TrafficData;
        }
        else if (command == "decided")
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
}
