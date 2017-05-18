using UnityEngine;
using LightBuzz.Vitruvius;
using Windows.Kinect;

public class CursorSample : VitruviusSample
{
    #region Variables

    BodyWrapper body;
    byte[] pixels;

    public VitruviusVideo vitruviusVideo;

    public KinectUI kinectUI;

    #endregion

    #region Reserved methods // Awake - OnApplicationQuit - Update

    protected override void Awake()
    {
        base.Awake();

        vitruviusVideo.Initialize(OnVideoFrameArrived, null, null);
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        vitruviusVideo.Dispose();
    }

    void Update()
    {
        vitruviusVideo.UpdateVideo();

        if (!vitruviusVideo.videoPlayer.IsPlaying)
        {
            UpdateFrame();
        }
    }

    #endregion

    #region UpdateFrame

    void UpdateFrame()
    {
        byte[] pixels = null;

        switch (visualization)
        {
            case Visualization.Color:
                UpdateColorFrame(out pixels);
                break;
            case Visualization.Depth:
                UpdateDepthFrame(out pixels);
                break;
            default:
                UpdateInfraredFrame(out pixels);
                break;
        }

        UpdateBodyFrame();

        RefreshFrame(body);

        vitruviusVideo.videoRecorder.RecordFrame(pixels, visualization, resolution, body, null, false);
    }

    #endregion

    #region OnBodyFrameReceived

    protected override void OnBodyFrameReceived(BodyFrame frame)
    {
        Body body = frame.Bodies().Closest();

        if (body != null && this.body == null)
        {
            this.body = new BodyWrapper();
        }
        else if (body == null && this.body != null)
        {
            this.body = null;
        }

        if (this.body != null)
        {
            this.body.Set(body, KinectSensor.CoordinateMapper, visualization);
        }
    }

    #endregion

    #region OnVideoFrameArrived

    void OnVideoFrameArrived(Texture2D image, Visualization visualization, ColorFrameResolution resolution, BodyWrapper body, Face face)
    {
        frameView.FrameTexture = image;

        this.body = body;

        RefreshFrame(body);
    }

    #endregion

    #region RefreshFrame

    void RefreshFrame(BodyWrapper body)
    {
        kinectUI.UpdateCursor(body, vitruviusVideo.videoPlayer.IsPlaying);
    }

    #endregion
}