using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

public class GazeGestureManager : MonoBehaviour
{
    private static bool _firstTime = true;
    private int _count = 0;
    private Animator _animator;
    private GestureRecognizer recognizer;
    public Text centerHUD;

    public GameObject FocusedObject { get; private set; }

    public GameObject traffic;

    public GameObject animationObject;

    public void Start()
    {
        Debug.Log("Start!");
        _animator = animationObject.GetComponent<Animator>();

        recognizer = new GestureRecognizer();
        recognizer.Tapped += (args) =>
        {
            if (FocusedObject != null)
            {
                FocusedObject.SendMessageUpwards("OnSelect", SendMessageOptions.DontRequireReceiver);
            }
        };
        recognizer.StartCapturingGestures();
    }

    private void OnEnable()
    {
        FocusedObject = null;
        centerHUD.text = "Watch out for traffic! Say: 'traffic'";
        TcpListner.ResetListner();
    }

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
            if (!_animator.GetBool("AnimateSphere"))
                _animator.SetBool("AnimateSphere", true);
        }
    }

    public void AnimationDone()
    {
        Debug.Log("AnimationDone!");

        if (_firstTime)
        {
            if (_count++ < 2)
            {
                _animator.SetBool("AnimateSphere", true);
                return;
            }
        }
        _firstTime = false;
#if WINDOWS_UWP
        TcpListner.SendReadyForTraffic();
#endif
        this.gameObject.SetActive(false);
        traffic.SetActive(true);
    }
}