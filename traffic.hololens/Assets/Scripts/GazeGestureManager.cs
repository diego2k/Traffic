﻿using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class GazeGestureManager : MonoBehaviour
{
    public static GazeGestureManager Instance { get; private set; }

    // Represents the hologram that is currently being gazed at.
    public GameObject FocusedObject { get; private set; }

    GestureRecognizer recognizer;

    private static int Count = 0;

    public GameObject traffic;


    // Use this for initialization
    void Awake()
    {
        Instance = this;

        // Set up a GestureRecognizer to detect Select gestures.
        recognizer = new GestureRecognizer();
        recognizer.Tapped += (args) =>
        {
            // Send an OnSelect message to the focused object and its ancestors.
            if (FocusedObject != null)
            {
                FocusedObject.SendMessageUpwards("OnSelect", SendMessageOptions.DontRequireReceiver);
            }
        };
        recognizer.StartCapturingGestures();
    }

    // Update is called once per frame
    void Update()
    {
        // Figure out which hologram is focused this frame.
        GameObject oldFocusObject = FocusedObject;

        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            // If the raycast hit a hologram, use that as the focused object.
            FocusedObject = hitInfo.collider.gameObject;
        }
        else
        {
            // If the raycast did not hit a hologram, clear the focused object.
            FocusedObject = null;
        }

        // If the focused object changed this frame,
        // start detecting fresh gestures again.
        if (FocusedObject == null) return;
        if (FocusedObject != oldFocusObject)
        {
            Debug.Log(FocusedObject.tag);
            if (FocusedObject.tag == "Last_Left")
            {
                BroadcastMessage("OnShowRight", SendMessageOptions.DontRequireReceiver);
            }
            if (FocusedObject.tag == "Last_Right")
            {
                Debug.Log("Intaration count: " + Count);
                if (Count++ < 2)
                {
                    BroadcastMessage("OnShowLeft", SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    traffic.SetActive(true);
                }
            }
            //FocusedObject.SetActive(false);
            FocusedObject.SendMessageUpwards("OnLookAt", SendMessageOptions.DontRequireReceiver);
            //recognizer.CancelGestures();
            //recognizer.StartCapturingGestures();
        }
    }
}