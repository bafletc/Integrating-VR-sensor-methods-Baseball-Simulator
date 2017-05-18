using UnityEngine;
using UnityEngine.UI;
using LightBuzz.Vitruvius;
using System.IO;

[System.Serializable]
public class VitruviusVideo
{
    #region Variables and Properties

    bool initialized = false;

    readonly Color GREEN_COLOR = new Color(0f, 255f, 0f, 1f);
    readonly Color RED_COLOR = new Color(255f, 0f, 0f, 1f);

    public KinectVideoRecorder videoRecorder;
    public KinectVideoPlayer videoPlayer;
    public KinectVideoSlider playbackSlider;
    public Button buttonRecord;
    public Button buttonCompress;
    public Text buttonCompressText;
    public Button buttonLoad;
    public float videoDuration = 0;
    public bool videoCompressed = false;

    KinectVideoPlayer.OnFinishedPlaying onFinishedPlaying = null;
    KinectVideoPlayer.OnSeeking onSeeking = null;

    public string DefaultVideoPath
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, "video");
        }
    }

    #endregion

    #region Initialize

    public void Initialize(KinectVideoPlayer.OnFrameUpdate frameUpdateCallback,
        KinectVideoPlayer.OnFinishedPlaying finishedPlayingCallback, KinectVideoPlayer.OnSeeking seekingCallback)
    {
        Initialize(DefaultVideoPath, frameUpdateCallback, finishedPlayingCallback, seekingCallback);
    }

    public void Initialize(string videoPath, KinectVideoPlayer.OnFrameUpdate frameUpdateCallback,
        KinectVideoPlayer.OnFinishedPlaying finishedPlayingCallback, KinectVideoPlayer.OnSeeking seekingCallback)
    {
        if (initialized)
        {
            return;
        }

        if (!Directory.Exists(videoPath))
        {
            Directory.CreateDirectory(videoPath);
        }

        if (videoPlayer == null)
        {
            videoPlayer = KinectVideoPlayer.Create();
        }
        videoPlayer.Path = videoPath;
        videoPlayer.onFrameUpdateCallback = frameUpdateCallback;

        onFinishedPlaying = finishedPlayingCallback;
        onSeeking = seekingCallback;

        videoPlayer.onFinishedPlayingCallback += OnFinishedPlaying;
        if (finishedPlayingCallback != null)
        {
            videoPlayer.onFinishedPlayingCallback += onFinishedPlaying;
        }
        if (onSeeking != null)
        {
            videoPlayer.onSeekingEvent += onSeeking;
        }

        if (videoRecorder == null)
        {
            videoRecorder = KinectVideoRecorder.Create();
        }
        videoRecorder.Path = videoPath;

        playbackSlider.videoPlayer = videoPlayer;

        buttonRecord.onClick.AddListener(OnRecord);
        buttonCompress.onClick.AddListener(OnCompress);
        buttonLoad.onClick.AddListener(OnLoad);

        initialized = true;
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        if (!initialized)
        {
            return;
        }

        videoPlayer.onFrameUpdateCallback = null;
        videoPlayer.onFinishedPlayingCallback -= OnFinishedPlaying;
        if (onFinishedPlaying != null)
        {
            videoPlayer.onFinishedPlayingCallback -= onFinishedPlaying;
        }
        if (onSeeking != null)
        {
            videoPlayer.onSeekingEvent -= onSeeking;
        }

        initialized = false;
    }

    #endregion

    #region UpdateVideo

    public void UpdateVideo()
    {
        if (!initialized)
        {
            return;
        }

        if (videoPlayer.IsPlaying)
        {
            videoPlayer.UpdateFrame();
        }

        if (KinectVideoCompressor.IsCompressing)
        {
            KinectVideoCompressor.UpdateCompression();

            buttonCompressText.text = "Compressing (" + (KinectVideoCompressor.Progress * 100f).ToString("N0") + "%)";

            if (!KinectVideoCompressor.IsCompressing)
            {
                buttonCompress.enabled = true;
                buttonCompressText.text = "Start Compression";
            }
        }
    }

    #endregion

    #region OnFinishedPlaying

    void OnFinishedPlaying()
    {
        if (!initialized)
        {
            return;
        }

        playbackSlider.Stop();
    }

    #endregion

    #region GUI Button Events

    public void OnRecord()
    {
        if (!initialized)
        {
            return;
        }

        if (videoPlayer.IsPlaying || KinectVideoCompressor.IsCompressing)
        {
            return;
        }

        if (!videoRecorder.IsRecording)
        {
#if UNITY_EDITOR || !UNITY_WINRT

            if (Directory.Exists(videoRecorder.Path))
            {
                Directory.Delete(videoRecorder.Path, true);
                Directory.CreateDirectory(videoRecorder.Path);
            }

#endif
            videoRecorder.StartRecorder();

            buttonCompress.gameObject.SetActive(false);
            buttonRecord.GetComponentInChildren<Text>().text = "Stop Recording";
            buttonRecord.GetComponentsInChildren<Image>()[1].color = GREEN_COLOR;

            playbackSlider.gameObject.SetActive(false);
        }
        else
        {
            videoRecorder.Stop(out videoDuration);

            videoPlayer.Play(videoDuration, videoCompressed);

            if (videoPlayer.IsPlaying)
            {
                playbackSlider.gameObject.SetActive(true);
            }

            buttonCompress.gameObject.SetActive(true);
            buttonRecord.GetComponentInChildren<Text>().text = "Start Recording";
            buttonRecord.GetComponentsInChildren<Image>()[1].color = RED_COLOR;
        }
    }

    public void OnCompress()
    {
        if (!initialized)
        {
            return;
        }

        if (KinectVideoCompressor.IsCompressing)
        {
            KinectVideoCompressor.StopCompression();

            buttonCompress.enabled = true;
            buttonCompress.GetComponentInChildren<Text>().text = "Start Compression";
        }
        else
        {
            videoRecorder.Compress();

            if (KinectVideoCompressor.IsCompressing)
            {
                buttonCompress.enabled = false;
                buttonCompressText.text = "Compressing (0%)";
            }
        }
    }

    public void OnLoad()
    {
        if (!initialized)
        {
            return;
        }

#if UNITY_EDITOR

        string path = UnityEditor.EditorUtility.OpenFolderPanel("Load Vitruvius Kinect Video", videoPlayer.Path, "");

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        videoPlayer.Path = path;
        videoPlayer.Play(videoDuration, videoCompressed);

        bool usePathInRecording = UnityEditor.EditorUtility.DisplayDialog("Folder usage", "Use folder path as a recording destination?", "Yes", "No");

        if (usePathInRecording)
        {
            videoRecorder.Path = path;
        }

        if (videoPlayer.IsPlaying)
        {
            playbackSlider.gameObject.SetActive(true);
        }

        if (!KinectVideoCompressor.IsCompressing && usePathInRecording)
        {
            buttonCompress.gameObject.SetActive(true);
            buttonCompress.enabled = true;
            buttonCompressText.text = "Start Compression";
        }

#else

        videoPlayer.Play(videoDuration, videoCompressed);
        videoRecorder.Path = videoPlayer.Path;

        if (videoPlayer.IsPlaying)
        {
            playbackSlider.gameObject.SetActive(true);
        }

        if (!KinectVideoCompressor.IsCompressing)
        {
            buttonCompress.gameObject.SetActive(true);
            buttonCompress.enabled = true;
            buttonCompressText.text = "Start Compression";
        }

#endif
    }

    #endregion
}
