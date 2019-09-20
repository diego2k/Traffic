using HoloToolkit.Unity.InputModule;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeScreenController : MonoBehaviour, ISpeechHandler
{
    public Button button1;
    public GameObject nextDialog;

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (eventData == null || string.IsNullOrEmpty(eventData.RecognizedText)) return;
        Debug.Log("WelcomeScreenController.OnSpeechKeywordRecognized: " + eventData.RecognizedText);
        SpeechCommands(eventData.RecognizedText.ToLower());
    }

    public void SpeechCommands(string command)
    {
        if (command != "start") return;

        Image image = button1.GetComponent<Image>();
        var oldColor = image.color;
        image.color = Color.yellow;

        Wait(3, () =>
        {
            this.gameObject.SetActive(false);
            image.color = oldColor;

            nextDialog.SetActive(true);
        });
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
