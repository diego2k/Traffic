using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using HoloToolkit.Unity;

public class CompassController : MonoBehaviour, ISpeechHandler
{
    private Button _activeButton;
    private TextToSpeech _textToSpeech;
    private bool _isSend; // HACK: Speech handler gets executed twice. Research needed. 

    public Text Answer;
    public Button button1;
    public Button button2;
    public GameObject nextDialog;
    public Text t_n4;
    public Text t_n3;
    public Text t_n2;
    public Text t_n1;
    public Text t_0;
    public Text t_1;
    public Text t_2;
    public Text t_3;
    public Text t_4;

    public void OnEnable()
    {
        if (!TcpListner.IsScenarioDataValid) return;
        _isSend = false;
        t_n4.text = CompassPosition(TcpListner.ScenarioData.CompassCurrent - 40);
        t_n3.text = CompassPosition(TcpListner.ScenarioData.CompassCurrent - 30);
        t_n2.text = CompassPosition(TcpListner.ScenarioData.CompassCurrent - 20);
        t_n1.text = CompassPosition(TcpListner.ScenarioData.CompassCurrent - 10);
        t_0.text = CompassPosition(TcpListner.ScenarioData.CompassCurrent);
        t_1.text = CompassPosition(TcpListner.ScenarioData.CompassCurrent + 10);
        t_2.text = CompassPosition(TcpListner.ScenarioData.CompassCurrent + 20);
        t_3.text = CompassPosition(TcpListner.ScenarioData.CompassCurrent + 30);
        t_4.text = CompassPosition(TcpListner.ScenarioData.CompassCurrent + 40);
    }

    private string CompassPosition(int degree)
    {
        if (degree > 360)
        {
            return (degree - 360).ToString();
        }
        else if (degree < 0)
        {
            return (360 + degree).ToString();
        }
        else
            return degree.ToString();
    }

    private void Awake()
    {
        _textToSpeech = GetComponent<TextToSpeech>();
        if (!TcpListner.IsScenarioDataValid) return;

        _textToSpeech.StartSpeaking(string.Format("Turn to heading {0}.", TcpListner.ScenarioData.CompassTarget));
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (eventData == null || string.IsNullOrEmpty(eventData.RecognizedText)) return;
        Debug.Log("DialogCompassSpeechHandler.OnSpeechKeywordRecognized: " + eventData.RecognizedText);
        SpeechCommands(eventData.RecognizedText.ToLower());
    }

    public void SpeechCommands(string command)
    {
        string answer = string.Empty;
        switch (command)
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

            string result = TcpListner.ScenarioData.CompassTurn;

            Debug.Log("Answer: " + answer + " - " + result);
            Answer.text = (answer == result) ? "Correct" : "Wrong";
            Answer.color = (answer == result) ? Color.green : Color.red;
            TcpListner.Results.CompassTurn = answer;

            // We are done, lets send the results.
            if (!_isSend)
            {
                _isSend = true;
#if WINDOWS_UWP
            TcpListner.SendResults();
#endif
            }
            Wait(3, () =>
            {
                gameObject.SetActive(false);
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
