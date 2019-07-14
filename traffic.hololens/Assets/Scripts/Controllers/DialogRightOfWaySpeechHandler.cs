using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialogRightOfWaySpeechHandler : MonoBehaviour, ISpeechHandler
{
    private Button activeButton;

    public Text Answer;
    public Button button1;
    public Button button2;
    public Button button3;
    public GameObject nextDialog;

    void Start()
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
            case "one":
                {
                    answer = 1;
                    activeButton = button1;
                }
                break;
            case "two":
                {
                    answer = 2;
                    activeButton = button2;
                }
                break;
            case "three":
                {
                    answer = 2;
                    activeButton = button3;
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
            var oldColor = image.color;
            image.color = Color.yellow;
            
            int result = TcpListner.ScenarioData.RightOfWay;

            Answer.text = (answer == result) ? "Correct" : "Wrong";
            Answer.color = (answer == result) ? Color.green : Color.red;
            TcpListner.Points += (answer == result) ? 1 : 0;
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
