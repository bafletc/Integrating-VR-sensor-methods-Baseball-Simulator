using UnityEngine;
using LightBuzz.Vitruvius;
using Windows.Kinect;

public class GreenScreenSample : VitruviusSample
{
    #region Variables

    public VitruviusVideo vitruviusVideo;

    public bool liveWhilePlayback = false;
    public FrameView frameViewBack;

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

        if (frameViewBack != null)
        {
            frameViewBack.FrameTexture = null;
        }
    }

    void Update()
    {
        vitruviusVideo.UpdateVideo();

        if (!vitruviusVideo.videoPlayer.IsPlaying || liveWhilePlayback)
        {
            UpdateFrame();
        }
    }

    #endregion

    #region UpdateFrame

    void UpdateFrame()
    {
        byte[] pixels = null;

        if (colorFrameReader != null && depthFrameReader != null && bodyIndexFrameReader != null)
        {
            using (ColorFrame colorFrame = colorFrameReader.AcquireLatestFrame())
            using (DepthFrame depthFrame = depthFrameReader.AcquireLatestFrame())
            using (BodyIndexFrame bodyIndexFrame = bodyIndexFrameReader.AcquireLatestFrame())
            {
                if (colorFrame != null && depthFrame != null && bodyIndexFrame != null)
                {
                    frameView.FrameTexture = colorFrame.GreenScreen(depthFrame, bodyIndexFrame, out pixels);
                }
            }
        }

        if (pixels != null)
        {
            vitruviusVideo.videoRecorder.RecordFrame(pixels, visualization, resolution, null, null, true);
        }
    }

    #endregion

    #region OnVideoFrameArrived

    void OnVideoFrameArrived(Texture2D image, Visualization visualization, ColorFrameResolution resolution, BodyWrapper body, Face face)
    {
        if (liveWhilePlayback)
        {
            frameViewBack.FrameTexture = image;
        }
        else
        {
            frameView.FrameTexture = image;
        }
    }

    #endregion
}