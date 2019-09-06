using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

public class ObserveIndikatorController : MonoBehaviour
{
    private const int MIN_ATTEMPT_FIRST = 1;
    private const int MIN_ATTEMPT = 1;
    private const int MAX_ATTEMPT = 10;
    private static bool _firstTime = true;
    private int _count = 0, _time = 0, _timeHit = 0;
    private bool _isTimeRunning = false;
    public int _attempts = 0;
    private Animator _animator;
    private GestureRecognizer recognizer;

    public Text centerHUD;

    public GameObject FocusedObject { get; private set; }

    public GameObject nextDialog;

    public GameObject animationObject;

    public Text points;

    public GameObject hitPlane;

    public void Start()
    {
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
        Debug.Log("Start!");
        _isTimeRunning = false;
        _time = _timeHit = _attempts = _count = 0;
        FocusedObject = null;
        centerHUD.text = "Follow the ball!";
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
            Image image = hitPlane.GetComponent<Image>();
            image.color = new Color(0, 140, 0, 255);
        }
        else
        {
            FocusedObject = null;
            Image image = hitPlane.GetComponent<Image>();
            image.color = new Color(140, 0, 0, 255);
        }

        if (!TcpListner.IsScenarioDataValid) return;

        _time++;
        if (FocusedObject != null)
        {
            _timeHit++;
            if (!_animator.GetBool("AnimateSphere"))
                _animator.SetBool("AnimateSphere", true);
        }

        float hits = (float)_timeHit / (float)_time;
        centerHUD.text = string.Format("Pattern matched: {0:0.00}%", hits * 100);
    }

    public void AnimationDone()
    {
        Debug.Log("AnimationDone!");
        float hits = (float)_timeHit / (float)_time;
        points.text = string.Format("{0:0.00}%", hits * 100);
        bool continueAnimation = false;

        if (++_attempts < MAX_ATTEMPT)
        {
            if (_firstTime)
            {
                if (++_count < MIN_ATTEMPT_FIRST)
                    continueAnimation = true;
            }
            else
            {
                if (++_count < MIN_ATTEMPT)
                    continueAnimation = true;
            }
            if (hits < 0.2f)
                continueAnimation = true;
        }
        if (continueAnimation)
        {
            //_animator.SetBool("AnimateSphere", true);
            return;
        }
        _firstTime = false;

        TcpListner.Results.NumberOfAttempts = _attempts;
#if WINDOWS_UWP
        TcpListner.SendReadyForTraffic();
#endif
        this.gameObject.SetActive(false);
        nextDialog.SetActive(true);
    }
}