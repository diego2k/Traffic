using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialogAvoidController : MonoBehaviour, ISpeechHandler
{
    private Button _activeButton;

    public Text Answer;
    public Button button1;
    public Button button2;
    public Button button3;
    public GameObject nextDialog;

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (eventData == null || string.IsNullOrEmpty(eventData.RecognizedText)) return;
        Debug.Log("DialogAvoidSpeechHandler.OnSpeechKeywordRecognized: " + eventData.RecognizedText);
        SpeechCommands(eventData.RecognizedText.ToLower());
    }

    public void SpeechCommands(string command)
    {
        string answer = string.Empty;
        switch (command.ToLower())
        {
            case "left":
                {
                    answer = "L";
                    _activeButton = button1;
                }
                break;
            case "right":
                {
                    answer = "R";
                    _activeButton = button2;
                }
                break;
            case "none":
                {
                    answer = "N";
                    _activeButton = button3;
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

            var result = TcpListner.ScenarioData.Turn;

            Debug.Log("Answer1: " + answer + " " + result);
            Answer.text = (answer == result) ? "Correct" : "Wrong";
            Answer.color = (answer == result) ? Color.green : Color.red;
            TcpListner.Results.Turn = answer;

            Wait(3, () =>
            {
                this.gameObject.SetActive(false);
                image.color = oldColor;
                Answer.text = string.Empty;
                nextDialog.SetActive(true);
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
