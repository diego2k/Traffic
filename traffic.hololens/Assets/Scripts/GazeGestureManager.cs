using UnityEngine;
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
        GameObject oldFocusObject = FocusedObject;

        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            FocusedObject = hitInfo.collider.gameObject;
        }
        else
        {
            FocusedObject = null;
        }

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
            FocusedObject.SendMessageUpwards("OnLookAt", SendMessageOptions.DontRequireReceiver);
        }
    }
}