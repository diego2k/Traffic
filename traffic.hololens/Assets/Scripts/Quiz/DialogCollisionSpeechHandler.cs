using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using HoloToolkit.Unity;
using System.Collections;

public class DialogCollisionSpeechHandler : MonoBehaviour, ISpeechHandler
{
    private Button activeButton;

    public Text Answer;
    public Button button1;
    public Button button2;
    public Sprite check;
    public Sprite cross;
    public GameObject nextDialog;

    public void Start()
    {
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (eventData == null || string.IsNullOrEmpty(eventData.RecognizedText)) return;
        Debug.Log("OnSpeechKeywordRecognized: " + eventData.RecognizedText);
        SpeechCommands(eventData.RecognizedText.ToLower());
    }

    public void SpeechCommands(string command)
    {
        int answer = 0;
        switch (command)
        {
            case "yes":
                {
                    answer = 1;
                    //transform.position += Vector3.forward;
                    activeButton = button1;
                }
                break;
            case "no":
                {
                    answer = 2;
                    //transform.position += Vector3.back;
                    activeButton = button2;
                }
                break;
            default:
                return;

        }

        // Check Answer
        try
        {
            Image image = activeButton.GetComponent<Image>();
            image.color = Color.yellow;

            int result = GetCorrectAnswer();

            Answer.text = (answer == result) ? "Correct" : "Wrong";
            Answer.color = (answer == result) ? Color.green : Color.red;

            Wait(3, () =>
            {
                this.gameObject.SetActive(false);
                nextDialog.SetActive(true);
            });

        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public int GetCorrectAnswer()
    {
        return 1;
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
