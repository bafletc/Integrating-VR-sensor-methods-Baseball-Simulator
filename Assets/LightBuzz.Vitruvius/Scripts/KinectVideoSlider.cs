using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LightBuzz.Vitruvius;

public class KinectVideoSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    protected readonly Color WHITE_COLOR = new Color(255f, 255f, 255f, 1f);
    protected readonly Color WHITE_COLOR_FADE = new Color(255f, 255f, 255f, 0.5f);

    public KinectVideoPlayer videoPlayer;

    public Button buttonPlay;

    public Button buttonPause;

    public Button buttonLoop;

    public float videoSpeed = 1f;

    public float videoDuration = 0f;

    public bool videoCompressed = false;
    
    private Slider slider;

    public float Value
    {
        get
        {
            return slider.value;
        }
        set
        {
            slider.value = value;
        }
    }

    void Awake()
    {
        slider = GetComponent<Slider>();

        if (videoPlayer != null)
        {
            buttonPlay.gameObject.SetActive(!videoPlayer.IsPlaying);
            buttonPause.gameObject.SetActive(videoPlayer.IsPlaying);
            buttonLoop.image.color = videoPlayer.Loop ? WHITE_COLOR : WHITE_COLOR_FADE;
        }
    }

    public void Stop()
    {
        if (!videoPlayer.Loop)
        {
            buttonPlay.gameObject.SetActive(true);
            buttonPause.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (videoPlayer.IsPlaying)
        {
            videoPlayer.Speed = videoSpeed;
            Value = videoPlayer.Time / videoPlayer.Duration;
        }
    }

    public void OnValueChanged()
    {
        if (videoPlayer.UseExtendedSeeking)
        {
            videoPlayer.Seek = Mathf.Clamp01(Value) * videoPlayer.Duration;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!videoPlayer.IsPlaying)
            {
                videoPlayer.Play();
                videoPlayer.Pause();
            }

            videoPlayer.UseExtendedSeeking = true;
            videoPlayer.Seek = Mathf.Clamp01(Value) * videoPlayer.Duration;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            videoPlayer.UseExtendedSeeking = false;
        }
    }

    public void OnPlayClick()
    {
        videoPlayer.Speed = videoSpeed;
        videoPlayer.Play(videoDuration, videoCompressed);

        buttonPlay.gameObject.SetActive(false);
        buttonPause.gameObject.SetActive(true);
    }

    public void OnPauseClick()
    {
        videoPlayer.Pause();

        buttonPlay.gameObject.SetActive(true);
        buttonPause.gameObject.SetActive(false);
    }

    public void OnLoopClick()
    {
        videoPlayer.Loop = !videoPlayer.Loop;

        buttonLoop.image.color = videoPlayer.Loop ? WHITE_COLOR : WHITE_COLOR_FADE;
    }
}