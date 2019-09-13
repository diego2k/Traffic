using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;

public class ObserveIndikatorController : MonoBehaviour
{
    private const int MIN_ATTEMPT_FIRST = 3;
    private const int MIN_ATTEMPT = 1;
    private const int MAX_ATTEMPT = 10;
    private const float MIN_SUCCESS = 0.7f;
    private static bool _firstTime = true;
    private int _count = 0, _time = 0, _timeHit = 0;
    private float _hitsLastAttempt = 0.0f;
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
        _time = _timeHit = _attempts = _count = 0;
        FocusedObject = null;
        centerHUD.text = "Catch and chase the ball!";
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

        if (_timeHit > 0)
        {
            float hits = (float)_timeHit / (float)_time;
            centerHUD.text = string.Format("Pattern matched: {0:0.00}%", hits * 100);
        }
    }

    public void AnimationDone()
    {
        Debug.Log("AnimationDone!");
        float hits = (float)_timeHit / (float)_time;
        if (_firstTime)
            points.text = string.Format("{0:0.00}%", hits * 100);
        else
            points.text = string.Format("{0:0.00}% (Last attempt {1:0.00}%)", hits * 100, _hitsLastAttempt * 100);
        _hitsLastAttempt = hits;

        if (++_attempts < MAX_ATTEMPT)
        {
            if (_firstTime)
            {
                if (++_count < MIN_ATTEMPT_FIRST)
                    return;
            }
            else
            {
                if (++_count < MIN_ATTEMPT)
                    return;
            }
            if (hits < MIN_SUCCESS)
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