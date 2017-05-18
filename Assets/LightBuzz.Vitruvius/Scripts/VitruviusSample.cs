using UnityEngine;
using LightBuzz.Vitruvius;
using Microsoft.Kinect.Face;
using Windows.Kinect;

public class VitruviusSample : MonoBehaviour
{
    #region Variables and Properties

    protected BodyFrameReader bodyFrameReader = null;
    protected ColorFrameReader colorFrameReader = null;
    protected DepthFrameReader depthFrameReader = null;
    protected InfraredFrameReader infraredFrameReader = null;
    protected BodyIndexFrameReader bodyIndexFrameReader = null;
    protected HighDefinitionFaceFrameSource faceFrameSource = null;
    protected HighDefinitionFaceFrameReader faceFrameReader = null;

    public Visualization visualization = Visualization.Color;
    public ColorFrameResolution resolution = ColorFrameResolution.Resolution_1920x1080;
    public FrameView frameView;

    public KinectSensor KinectSensor
    {
        get;
        protected set;
    }

    #endregion

    #region Reserved methods // Awake - OnApplicationQuit

    protected virtual void Awake()
    {
        KinectSensor = KinectSensor.GetDefault();

        if (KinectSensor != null)
        {
            bodyFrameReader = KinectSensor.BodyFrameSource.OpenReader();
            colorFrameReader = KinectSensor.ColorFrameSource.OpenReader();
            depthFrameReader = KinectSensor.DepthFrameSource.OpenReader();
            infraredFrameReader = KinectSensor.InfraredFrameSource.OpenReader();
            bodyIndexFrameReader = KinectSensor.BodyIndexFrameSource.OpenReader();
            faceFrameSource = HighDefinitionFaceFrameSource.Create(KinectSensor);
            faceFrameReader = faceFrameSource.OpenReader();

            KinectSensor.Open();
        }
    }

    protected virtual void OnApplicationQuit()
    {
        if (bodyFrameReader != null)
        {
            bodyFrameReader.Dispose();
            bodyFrameReader = null;
        }

        if (colorFrameReader != null)
        {
            colorFrameReader.Dispose();
            colorFrameReader = null;
        }

        if (depthFrameReader != null)
        {
            depthFrameReader.Dispose();
            depthFrameReader = null;
        }

        if (infraredFrameReader != null)
        {
            infraredFrameReader.Dispose();
            infraredFrameReader = null;
        }

        if (bodyIndexFrameReader != null)
        {
            bodyIndexFrameReader.Dispose();
            bodyIndexFrameReader = null;
        }

        if (faceFrameReader != null)
        {
            faceFrameReader.Dispose();
            faceFrameReader = null;
        }

        if (frameView != null)
        {
            frameView.FrameTexture = null;
        }

        if (KinectSensor != null && KinectSensor.IsOpen)
        {
            KinectSensor.Close();
            KinectSensor = null;
        }
    }

    #endregion

    #region UpdateColorFrame - OnColorFrameReceived

    /// <summary>
    /// Acquires the latest color frame.
    /// It calles the OnColorFrameReceived only if the acquired frame is not null.
    /// </summary>
    protected void UpdateColorFrame(bool updateFrameView = true)
    {
        if (colorFrameReader != null)
        {
            using (ColorFrame frame = colorFrameReader.AcquireLatestFrame())
            {
                if (frame != null)
                {
                    if (updateFrameView)
                    {
                        frameView.FrameTexture = frame.ToBitmap(resolution);
                    }
                    OnColorFrameReceived(frame);
                }
            }
        }
    }

    /// <summary>
    /// Acquires the latest color frame.
    /// It calles the OnColorFrameReceived only if the acquired frame is not null.
    /// </summary>
    protected void UpdateColorFrame(out byte[] pixels, bool updateFrameView = true)
    {
        pixels = null;

        if (colorFrameReader != null)
        {
            using (ColorFrame frame = colorFrameReader.AcquireLatestFrame())
            {
                if (frame != null)
                {
                    if (updateFrameView)
                    {
                        frameView.FrameTexture = frame.ToBitmap(resolution, out pixels);
                    }
                    OnColorFrameReceived(frame);
                }
            }
        }
    }

    protected virtual void OnColorFrameReceived(ColorFrame frame)
    {
    }

    #endregion

    #region UpdateDepthFrame - OnDepthFrameReceived

    /// <summary>
    /// Acquires the latest depth frame.
    /// It calles the OnDepthFrameReceived only if the acquired frame is not null.
    /// </summary>
    protected void UpdateDepthFrame(bool updateFrameView = true)
    {
        if (depthFrameReader != null)
        {
            using (DepthFrame frame = depthFrameReader.AcquireLatestFrame())
            {
                if (frame != null)
                {
                    if (updateFrameView)
                    {
                        frameView.FrameTexture = frame.ToBitmap();
                    }
                    OnDepthFrameReceived(frame);
                }
            }
        }
    }

    /// <summary>
    /// Acquires the latest depth frame.
    /// It calles the OnDepthFrameReceived only if the acquired frame is not null.
    /// </summary>
    protected void UpdateDepthFrame(out byte[] pixels, bool updateFrameView = true)
    {
        pixels = null;

        if (depthFrameReader != null)
        {
            using (DepthFrame frame = depthFrameReader.AcquireLatestFrame())
            {
                if (frame != null)
                {
                    if (updateFrameView)
                    {
                        frameView.FrameTexture = frame.ToBitmap(out pixels);
                    }
                    OnDepthFrameReceived(frame);
                }
            }
        }
    }

    protected virtual void OnDepthFrameReceived(DepthFrame frame)
    {
    }

    #endregion

    #region UpdateInfraredFrame - OnInfraredFrameReceived

    /// <summary>
    /// Acquires the latest infrared frame.
    /// It calles the OnInfraredFrameReceived only if the acquired frame is not null.
    /// </summary>
    protected void UpdateInfraredFrame(bool updateFrameView = true)
    {
        if (infraredFrameReader != null)
        {
            using (InfraredFrame frame = infraredFrameReader.AcquireLatestFrame())
            {
                if (frame != null)
                {
                    if (updateFrameView)
                    {
                        frameView.FrameTexture = frame.ToBitmap();
                    }
                    OnInfraredFrameReceived(frame);
                }
            }
        }
    }

    /// <summary>
    /// Acquires the latest infrared frame.
    /// It calles the OnInfraredFrameReceived only if the acquired frame is not null.
    /// </summary>
    protected void UpdateInfraredFrame(out byte[] pixels, bool updateFrameView = true)
    {
        pixels = null;

        if (infraredFrameReader != null)
        {
            using (InfraredFrame frame = infraredFrameReader.AcquireLatestFrame())
            {
                if (frame != null)
                {
                    if (updateFrameView)
                    {
                        frameView.FrameTexture = frame.ToBitmap(out pixels);
                    }
                    OnInfraredFrameReceived(frame);
                }
            }
        }
    }

    protected virtual void OnInfraredFrameReceived(InfraredFrame frame)
    {
    }

    #endregion

    #region UpdateBodyIndexFrame - OnBodyIndexFrameReceived

    /// <summary>
    /// Acquires the latest body index frame.
    /// It calles the OnBodyIndexFrameReceived only if the acquired frame is not null.
    /// </summary>
    protected void UpdateBodyIndexFrame()
    {
        if (bodyIndexFrameReader != null)
        {
            using (BodyIndexFrame frame = bodyIndexFrameReader.AcquireLatestFrame())
            {
                if (frame != null)
                {
                    OnBodyIndexFrameReceived(frame);
                }
            }
        }
    }

    protected virtual void OnBodyIndexFrameReceived(BodyIndexFrame frame)
    {
    }

    #endregion

    #region UpdateBodyFrame - OnBodyFrameReceived

    /// <summary>
    /// Acquires the latest body frame.
    /// It calles the OnBodyFrameReceived only if the acquired frame is not null.
    /// </summary>
    protected void UpdateBodyFrame()
    {
        if (bodyFrameReader != null)
        {
            using (BodyFrame frame = bodyFrameReader.AcquireLatestFrame())
            {
                if (frame != null)
                {
                    OnBodyFrameReceived(frame);
                }
            }
        }
    }

    protected virtual void OnBodyFrameReceived(BodyFrame frame)
    {
    }

    #endregion
}