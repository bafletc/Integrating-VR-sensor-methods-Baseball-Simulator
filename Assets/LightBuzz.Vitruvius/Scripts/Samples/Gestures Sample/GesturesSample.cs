using UnityEngine;
using UnityEngine.UI;
using LightBuzz.Vitruvius;
using Windows.Kinect;

public class GesturesSample : VitruviusSample
{
    #region Variables

    BodyWrapper body;

    public VitruviusVideo vitruviusVideo;

    GestureController gestureController;
    public Text gestureText;

    #endregion

    #region Reserved methods // Awake - OnApplicationQuit - Update

    protected override void Awake()
    {
        base.Awake();

        vitruviusVideo.Initialize(OnVideoFrameArrived, null, null);

        gestureController = new GestureController();
        gestureController.GestureRecognized += GestureRecognized;
        gestureController.Start();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        vitruviusVideo.Dispose();

        if (gestureController != null)
        {
            gestureController.Stop();
            gestureController.GestureRecognized -= GestureRecognized;
            gestureController = null;
        }
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
        if (body != null)
        {
            gestureController.Update(body);
        }
    }

    #endregion

    #region GestureRecognized

    void GestureRecognized(object sender, GestureEventArgs e)
    {
        gestureText.text = "Gesture: <b>" + e.GestureType.ToString() + "</b>";
    }

    #endregion
}