using UnityEngine;
using LightBuzz.Vitruvius;
using LightBuzz.Vitruvius.Avateering;
using Windows.Kinect;

public class FaceSample : VitruviusSample
{
    #region Variables

    BodyWrapper body;
    Face face;

    public DetailedFace detailedFace;

    #endregion

    #region Reserved methods // Awake - Update

    protected override void Awake()
    {
        base.Awake();

        Avateering.Enable();
        detailedFace.Initialize();
    }

    void Update()
    {
        UpdateFrame();
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

    #region RefreshFrame

    void RefreshFrame(BodyWrapper body)
    {
        if (body != null)
        {
            if (faceFrameReader != null)
            {
                if (!faceFrameSource.IsTrackingIdValid)
                {
                    faceFrameSource.TrackingId = body.TrackingId;
                }

                using (var faceFrame = faceFrameReader.AcquireLatestFrame())
                {
                    if (faceFrame != null)
                    {
                        face = faceFrame.Face();

                        if (face != null)
                        {
                            if (!detailedFace.gameObject.activeSelf)
                            {
                                detailedFace.gameObject.SetActive(true);
                            }

                            detailedFace.UpdateFace(face, frameView, visualization, KinectSensor.CoordinateMapper);
                        }
                        else if (detailedFace.gameObject.activeSelf)
                        {
                            detailedFace.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    #endregion
}