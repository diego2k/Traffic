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

        // Mark choise
        try
        {
            Image image = activeButton.GetComponent<Image>();
            string hexCode = "#84D0FF";
            Color lightblue;
            ColorUtility.TryParseHtmlString(hexCode, out lightblue);

            if (image != null)
            {
                if ((int)(image.color.r * 1000) == (int)(lightblue.r * 1000))

                {
                    image.color = Color.yellow;
                }
                else
                {
                    image.color = lightblue;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }

        // Check Answer
        try
        {
            int result = GetCorrectAnswer();

            /*Debug.Log("1");
            var checkSprite = Resources.Load<Sprite>("check");
            var crossSprite = Resources.Load<Sprite>("cross");
            Debug.Log("2");

            var ch1 = GameObject.Find("Image 1").GetComponent<Image>();
            var ch2 = GameObject.Find("Image 2").GetComponent<Image>();
            var ch3 = GameObject.Find("Image 3").GetComponent<Image>();
            Debug.Log("3");

            ch1.enabled = true;
            ch1.sprite = crossSprite;
            ch2.enabled = true;
            ch2.sprite = crossSprite;
            ch3.enabled = true;
            ch3.sprite = crossSprite;
            Debug.Log("4");

            Debug.Log("5");
            switch (result)
            {
                case 1:
                    ch1.enabled = true;
                    ch1.sprite = checkSprite;
                    break;
                case 2:
                    ch2.enabled = true;
                    ch2.sprite = checkSprite;
                    break;
                case 3:
                    ch3.enabled = true;
                    ch3.sprite = checkSprite;
                    break;
            }
            Debug.Log("6");*/

            Answer.text = (answer == result) ? "Correct" : "Wrong";
            Debug.Log("1");

            Wait(3, () =>
            {
                this.gameObject.SetActive(false);
                Debug.Log("4");
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

    public void validateAnswers()
    {
        var ch1 = GameObject.Find("Image 1").GetComponent<Image>();
        var ch2 = GameObject.Find("Image 2").GetComponent<Image>();
        var ch3 = GameObject.Find("Image 3").GetComponent<Image>();


        //UserScore = 0;
        //temporary
        List<string> answers = new List<string>();

        List<bool> user_answers = new List<bool>();
        for (var i = 0; i < 6; i++)
        {
            user_answers.Add(false);
        }

        GameObject[] buttons = GameObject.FindGameObjectsWithTag("button");
        string hexCode = "#84D0FF";
        Color lightblue;
        ColorUtility.TryParseHtmlString(hexCode, out lightblue);

        var index = 0;
        foreach (var button in buttons)
        {
            user_answers[index] = false;
            if (button.GetComponent<Image>().color == Color.yellow)
            {
                user_answers[index] = true;
                answers.Add(button.GetComponentInChildren<Text>().text);
                Image image = button.GetComponent<Image>();
                // image.color = lightblue;
            }
            index++;
        }
        /*
    #if WINDOWS_UWP

        try
        {
          if (DataListener.socket != null)
          {
            var time = "";
            if (UserData.time != null)
            {
              time = (float.Parse(UserData.time) / 1000).ToString();
            }
            else
            {
              UserData.time = "7";
            }



            //var data = GetCorrectAnswers();
            //button.Color = Red;

            text.text = "YOUR RESULTS";

            ch1.enabled = true;
            ch2.enabled = true;
            ch3.enabled = true;
            ch4.enabled = true;
            ch5.enabled = true;
            ch6.enabled = true;

            var checkSprite = Resources.Load<Sprite>("check");
            var crossSprite = Resources.Load<Sprite>("cross");

            ch1.sprite = (DataListener.criteria.TooLeft == user_answers[0]) ? checkSprite : crossSprite;
            ch2.sprite = (DataListener.criteria.TooRight == user_answers[1]) ? checkSprite : crossSprite;
            ch3.sprite = (DataListener.criteria.TooLow == user_answers[2]) ? checkSprite : crossSprite;
            ch4.sprite = (DataListener.criteria.TooHigh == user_answers[3]) ? checkSprite : crossSprite;
            ch5.sprite = (DataListener.criteria.TooSlow == user_answers[4]) ? checkSprite : crossSprite;
            ch6.sprite = (DataListener.criteria.TooFast == user_answers[5]) ? checkSprite : crossSprite;

            int score = 0;
            score += (DataListener.criteria.TooLeft == user_answers[0]) ? 2 : 0;
            score += (DataListener.criteria.TooRight == user_answers[1]) ? 2 : 0;
            score += (DataListener.criteria.TooLow == user_answers[2]) ? 2 : 0;
            score += (DataListener.criteria.TooHigh == user_answers[3]) ? 2 : 0;
            score += (DataListener.criteria.TooSlow == user_answers[4]) ? 2 : 0;
            score += (DataListener.criteria.TooFast == user_answers[5]) ? 2 : 0;


            UserData.score = score;
            finish.text = "Continue";
            UserData userData = new UserData(DataListener.criteria.Usercode, DataListener.criteria.CorrectAnswers, user_answers, UserData.time, UserData.score);
            var json = userData.ToString();

            DataListener.SendDataToCLient(json);

          }
        }
        catch (Exception e)
        {
        }
    #endif*/
    }

    public void FinishSession()
    {
        var ch1 = GameObject.Find("Image 1").GetComponent<Image>();
        var ch2 = GameObject.Find("Image 2").GetComponent<Image>();
        var ch3 = GameObject.Find("Image 3").GetComponent<Image>();
        var ch4 = GameObject.Find("Image 4").GetComponent<Image>();
        var ch5 = GameObject.Find("Image 5").GetComponent<Image>();
        var ch6 = GameObject.Find("Image 6").GetComponent<Image>();
        var text = GameObject.Find("Question").GetComponent<Text>();
        var finish = GameObject.Find("Finish").GetComponentInChildren<Text>();

        GameObject[] buttons = GameObject.FindGameObjectsWithTag("button");
        string hexCode = "#84D0FF";
        Color lightblue;
        ColorUtility.TryParseHtmlString(hexCode, out lightblue);

        var index = 0;
        foreach (var button in buttons)
        {
            if (button.GetComponent<Image>().color == Color.yellow)
            {
                Image image = button.GetComponent<Image>();
                image.color = lightblue;
            }
            index++;
        }


        //StartFlightScript.QuizManager.SetActive(false);

        //StartFlightScript.DoneManager.SetActive(true);

        ch1.enabled = false;
        ch2.enabled = false;
        ch3.enabled = false;
        ch4.enabled = false;
        ch5.enabled = false;
        ch6.enabled = false;
        finish.text = "Finish";
        text.text = "Rate your approach!";

    }
}
