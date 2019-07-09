using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialogCompassSpeechHandler : MonoBehaviour, ISpeechHandler
{
    private Button activeButton;

    public Text Answer;
    public Button button1;
    public Button button2;
    public GameObject observeIndikator;
    public Text t_n4;
    public Text t_n3;
    public Text t_n2;
    public Text t_n1;
    public Text t_0;
    public Text t_1;
    public Text t_2;
    public Text t_3;
    public Text t_4;

    public void Start()
    {
        if (!TcpListner.IsScenarioDataValid) return;
        t_n4.text = (TcpListner.ScenarioData.CompassCurrent - 40).ToString();
        t_n3.text = (TcpListner.ScenarioData.CompassCurrent - 30).ToString();
        t_n2.text = (TcpListner.ScenarioData.CompassCurrent - 20).ToString();
        t_n1.text = (TcpListner.ScenarioData.CompassCurrent - 10).ToString();
        t_0.text = TcpListner.ScenarioData.CompassCurrent.ToString();
        t_1.text = (TcpListner.ScenarioData.CompassCurrent + 20).ToString();
        t_2.text = (TcpListner.ScenarioData.CompassCurrent + 30).ToString();
        t_3.text = (TcpListner.ScenarioData.CompassCurrent + 40).ToString();
        t_4.text = (TcpListner.ScenarioData.CompassCurrent + 50).ToString();
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

            var result = TcpListner.ScenarioData.CompassTurnRight;

            Answer.text = (answer == result) ? "Correct" : "Wrong";
            Answer.color = (answer == result) ? Color.green : Color.red;

            Wait(3, () =>
            {
                gameObject.SetActive(false);
                observeIndikator.SetActive(true);
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
