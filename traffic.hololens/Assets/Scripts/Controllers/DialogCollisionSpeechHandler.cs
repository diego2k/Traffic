﻿using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialogCollisionSpeechHandler : MonoBehaviour, ISpeechHandler
{
    private Button _activeButton;

    public Text Answer;
    public Button button1;
    public Button button2;
    public GameObject nextDialog;
    public GameObject lastDialog;

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (eventData == null || string.IsNullOrEmpty(eventData.RecognizedText)) return;
        Debug.Log("DialogCollisionSpeechHandler.OnSpeechKeywordRecognized: " + eventData.RecognizedText);
        SpeechCommands(eventData.RecognizedText.ToLower());
    }

    public void SpeechCommands(string command)
    {
        bool answer = false;
        switch (command)
        {
            case "yes":
                {
                    answer = true;
                    _activeButton = button1;
                }
                break;
            case "no":
                {
                    answer = false;
                    _activeButton = button2;
                }
                break;
            default:
                return;

        }

        // Check Answer
        if (!TcpListner.IsScenarioDataValid) return;
        try
        {
            Image image = _activeButton.GetComponent<Image>();
            var oldColor = image.color;
            image.color = Color.yellow;

            var result = TcpListner.ScenarioData.Collide;

            Answer.text = (answer == result) ? "Correct" : "Wrong";
            Answer.color = (answer == result) ? Color.green : Color.red;
            TcpListner.Results.Collide = answer;

            Wait(3, () =>
            {
                this.gameObject.SetActive(false);
                image.color = oldColor;
                Answer.text = string.Empty;

                if (result)
                    nextDialog.SetActive(true);
                else
                    lastDialog.SetActive(true);
            });

        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public void Wait(float seconds, Action action)
    {
        StartCoroutine(_wait(seconds, action));
    }

    IEnumerator _wait(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }

}
