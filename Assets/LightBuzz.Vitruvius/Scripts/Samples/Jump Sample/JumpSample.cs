using UnityEngine;
using UnityEngine.UI;
using LightBuzz.Vitruvius;
using LightBuzz.Vitruvius.Avateering;
using Windows.Kinect;

public class JumpSample : VitruviusSample
{
    #region Variables

    BodyWrapper body;

    public VitruviusVideo vitruviusVideo;

    public Stickman stickman;

    public JumpFBX model;

    public Text playerStateText;
    string playerState = "None";

    #endregion

    #region Reserved methods // Awake - OnApplicationQuit - Update

    protected override void Awake()
    {
        base.Awake();

        vitruviusVideo.Initialize(OnVideoFrameArrived, null, null);

        Avateering.Enable();
        stickman.Initialize();
        model.Initialize();
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        vitruviusVideo.Dispose();

        Avateering.Disable();
        model.Dispose();
    }

    protected void Update()
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
        stickman.gameObject.SetActive(body != null);

        if (body != null)
        {
            if (model.updateModel)
            {
                Avateering.Update(model, body);
            }

            stickman.UpdateBody(body, frameView);

            playerState = "Player State: " + (model.JumpHeight > 0 ? "Jumping: " + model.JumpHeight.ToString("N2") : "Floored");

            if (playerStateText.text != playerState)
            {
                playerStateText.text = playerState;
            }
        }
    }

    #endregion
}