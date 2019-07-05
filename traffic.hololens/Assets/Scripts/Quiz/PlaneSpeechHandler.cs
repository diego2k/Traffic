using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneSpeechHandler : MonoBehaviour, ISpeechHandler
{
    public GameObject nextDialog;
    public Text centerHUD;

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (eventData == null || string.IsNullOrEmpty(eventData.RecognizedText)) return;
        Debug.Log("PlaneSpeech: " + eventData.RecognizedText);
        SpeechCommands(eventData.RecognizedText.ToLower());
    }

    public void SpeechCommands(string command)
    {
        if (command == "traffic")
        {
            Debug.Log("User has decided.");
            centerHUD.text = "Decide now!";
        }
        else if (command == "decided")
        {
            Debug.Log("User has seen traffic.");
            centerHUD.text = string.Empty;

            this.gameObject.SetActive(false);
            nextDialog.SetActive(true);
        }
    }
}
