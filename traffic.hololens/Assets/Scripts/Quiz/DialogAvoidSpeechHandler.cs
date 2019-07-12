using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialogAvoidSpeechHandler : MonoBehaviour, ISpeechHandler
{
    private Button activeButton;

    public Text Answer;
    public Button button1;
    public Button button2;
    public GameObject compass;

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
        bool answer = false;
        switch (command)
        {
            case "left":
                {
                    answer = false;
                    activeButton = button1;
                }
                break;
            case "right":
                {
                    answer = true;
                    activeButton = button2;
                }
                break;
            default:
                return;

        }

        // Check Answer
        if (!TcpListner.IsScenarioDataValid) return;
        try
        {
            Image image = activeButton.GetComponent<Image>();
            image.color = Color.yellow;

            var result = TcpListner.ScenarioData.TurnRight;

            Answer.text = (answer == result) ? "Correct" : "Wrong";
            Answer.color = (answer == result) ? Color.green : Color.red;
            TcpListner.Results.TurnRight = answer;

            Wait(3, () =>
            {
                this.gameObject.SetActive(false);
                compass.SetActive(true);
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
