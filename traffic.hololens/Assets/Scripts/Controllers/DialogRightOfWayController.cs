using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialogRightOfWayController : MonoBehaviour, ISpeechHandler
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
        Debug.Log("DialogRightOfWaySpeechHandler.OnSpeechKeywordRecognized: " + eventData.RecognizedText);
        SpeechCommands(eventData.RecognizedText.ToLower());
    }

    public void SpeechCommands(string command)
    {
        int answer = 0;
        switch (command)
        {
            case "alpha":
                {
                    answer = 1;
                    _activeButton = button1;
                }
                break;
            case "bravo":
                {
                    answer = 2;
                    _activeButton = button2;
                }
                break;
            case "charlie":
                {
                    answer = 3;
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
            
            int result = TcpListner.ScenarioData.RightOfWay;

            Answer.text = (answer == result) ? "Correct" : "Wrong";
            Answer.color = (answer == result) ? Color.green : Color.red;
            TcpListner.Results.RightOfWay = answer;

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
