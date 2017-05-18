using UnityEngine;
using System.Collections;
using Windows.Kinect;
using LightBuzz.Vitruvius;

public enum CursorState { None, Up, Down, Pressing }

[ExecuteInEditMode]
public class KinectUI : MonoBehaviour
{
    #region Singleton

    static KinectUI instance = null;
    static KinectUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<KinectUI>();
            }

            return instance;
        }
    }
    public static bool Assigned
    {
        get
        {
            return Instance != null;
        }
    }

    #endregion

    #region Variables

    [SerializeField]
    Transform cursor;
    public Transform Cursor
    {
        get
        {
            return cursor;
        }
    }

#if UNITY_EDITOR
    new
#endif
    SpriteRenderer renderer;
    [SerializeField]
    Sprite cursorOpenState;
    [SerializeField]
    Sprite cursorClosedState;

    [SerializeField]
    Animator gaugeAnimator;
    Coroutine gaugeLoad = null;

    [SerializeField]
    float stillnessThreshold = 0.05f;
    public static float StillnessThreshold
    {
        get
        {
            return Instance.stillnessThreshold;
        }
    }

    Vector2 prevCursorPosition;
    Vector2 cursorPosition;
    public static Vector2 CursorPosition
    {
        get
        {
            return Instance.cursor.position;
        }
    }
    public static Vector2 Direction
    {
        get
        {
            return CursorPosition - Instance.prevCursorPosition;
        }
    }
    public Vector2 cursorAreaCenter = Vector2.zero;
    public Vector2 cursorAreaSize = Vector2.one;
    Vector2 areaCenter = Vector2.zero;
    Vector2 areaSize = Vector2.zero;
    Vector2 cameraSize = Vector2.one;
#if UNITY_EDITOR
    Color drawAreaColor = new Color(1, 1, 1, 0.5f);
#endif

    HandState prevHandState = HandState.NotTracked;
    CursorState cursorState = CursorState.None;

    bool isPlaying = false;

    public FrameView frameView;

    KinectButton[] buttons = null;

    public delegate void OnGaugeEnd();

    #endregion

    #region Reserved methods // Awake - OnApplicationQuit - Update - OnDrawGizmos

    void Awake()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif

        renderer = cursor.GetComponent<SpriteRenderer>();

        buttons = FindObjectsOfType<KinectButton>();
    }

    void OnApplicationQuit()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return;
        }
#endif

        frameView.FrameTexture = null;
    }

    void Update()
    {
        float ratio = Screen.width / (float)Screen.height;
        cameraSize.y = Camera.main.orthographicSize * 2f;
        cameraSize.x = cameraSize.y * ratio;
        areaSize.Set(cameraSize.x * cursorAreaSize.x, cameraSize.y * cursorAreaSize.y);
        areaCenter.Set(cameraSize.x * 0.5f * cursorAreaCenter.x, cameraSize.y * 0.5f * cursorAreaCenter.y);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = drawAreaColor;
        Gizmos.DrawCube(areaCenter, areaSize);
    }
#endif

    #endregion

    #region UpdateCursor

    public void UpdateCursor(BodyWrapper body, bool isPlaying = false)
    {
        this.isPlaying = isPlaying;

        if (body != null && body.HandRightState != HandState.NotTracked)
        {
            if (!cursor.gameObject.activeSelf)
            {
                cursor.gameObject.SetActive(true);
            }

            HandState handState = body.HandRightState;

            if (handState != prevHandState)
            {
                if (handState == HandState.Open)
                {
                    renderer.sprite = cursorOpenState;
                }
                else if (handState == HandState.Closed)
                {
                    renderer.sprite = cursorClosedState;
                }

                prevHandState = handState;
            }

            switch (handState)
            {
                case HandState.Open:

                    if (cursorState == CursorState.Pressing)
                    {
                        cursorState = CursorState.Up;
                    }
                    else if (cursorState == CursorState.Up)
                    {
                        cursorState = CursorState.None;
                    }

                    break;
                case HandState.Closed:

                    if (cursorState == CursorState.None)
                    {
                        cursorState = CursorState.Down;
                    }
                    else if (cursorState == CursorState.Down)
                    {
                        cursorState = CursorState.Pressing;
                    }

                    break;
            }

            RefreshCursorPosition(body);
        }
        else
        {
            if (cursor.gameObject.activeSelf)
            {
                cursor.gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region RefreshCursorPosition

    void RefreshCursorPosition(BodyWrapper body)
    {
        cursorPosition = isPlaying ? body.Map2D[JointType.HandLeft].ToVector2() : body.Joints[JointType.HandLeft].Position.ToPoint(Visualization.Color);

        Vector2 handTipPos = isPlaying ? body.Map2D[JointType.HandTipLeft].ToVector2() : body.Joints[JointType.HandTipLeft].Position.ToPoint(Visualization.Color);
        cursor.up = new Vector3(handTipPos.x - cursorPosition.x, cursorPosition.y - handTipPos.y, 0);

        frameView.SetPositionOnFrame(ref cursorPosition);

        float x = (cursorPosition.x + cameraSize.x * 0.5f) / cameraSize.x;
        float y = (cursorPosition.y + cameraSize.y * 0.5f) / cameraSize.y;

        cursorPosition.Set(x * areaSize.x - areaSize.x * 0.5f + areaCenter.x, y * areaSize.y - areaSize.y * 0.5f + areaCenter.y);
        cursorPosition = Vector3.Lerp(cursor.position, cursorPosition, 0.35f);
        cursor.position = cursorPosition;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].ValidateButton(cursorPosition, cursorState);
        }

        prevCursorPosition = cursorPosition;
    }

    #endregion

    #region ValidateGauge

    public static void ValidateGauge(OnGaugeEnd onGaugeEnd)
    {
        if (Direction.magnitude < StillnessThreshold)
        {
            Instance.Internal_ValidateGauge(onGaugeEnd);
        }
        else
        {
            HideGauge();
        }
    }
    void Internal_ValidateGauge(OnGaugeEnd onGaugeEnd)
    {
        if (gaugeLoad != null)
        {
            return;
        }

        gaugeLoad = StartCoroutine(Coroutine_GaugeLoad(onGaugeEnd));
    }

    IEnumerator Coroutine_GaugeLoad(OnGaugeEnd onGaugeEnd)
    {
        gaugeAnimator.gameObject.SetActive(true);

        yield return new WaitForSeconds(1);

        gaugeAnimator.gameObject.SetActive(false);

        onGaugeEnd();
    }

    #endregion

    #region HideGauge

    public static void HideGauge()
    {
        Instance.Internal_HideGauge();
    }
    void Internal_HideGauge()
    {
        if (gaugeLoad != null)
        {
            StopCoroutine(gaugeLoad);
            gaugeLoad = null;

            if (gaugeAnimator.gameObject.activeSelf)
            {
                gaugeAnimator.gameObject.SetActive(false);
            }
        }
    }

    #endregion
}