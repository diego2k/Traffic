using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechCommander : MonoBehaviour
{
    private TextToSpeech _textToSpeech;

    void Start()
    {
        _textToSpeech = GetComponent<TextToSpeech>();
    }

    public void SayLeft()
    {
        _textToSpeech.StartSpeaking("Left");
    }

    public void SayRight()
    {
        _textToSpeech.StartSpeaking("Right");
    }

    public void SayDown()
    {
        _textToSpeech.StartSpeaking("Down");
    }

    public void SayUp()
    {
        _textToSpeech.StartSpeaking("Up");
    }

}