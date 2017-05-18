using UnityEngine;
using LightBuzz.Vitruvius;
using LightBuzz.Vitruvius.Avateering;
using Windows.Kinect;

public class FittingRoomSample : VitruviusSample
{
    #region Variables

    BodyWrapper body;

    public VitruviusVideo vitruviusVideo;

    public bool useGreenScreen = true;
    public GameObject greenScreenView;

    public AvatarCloth[] clothes = new AvatarCloth[5];
    [HideInInspector]
    public bool[] selected;
    public Vector2[] buttonPositions = new Vector2[5];
    public Vector2 buttonSize;

    #endregion

    #region Reserved methods // Awake - OnApplicationQuit - Update - OnGUI

    protected override void Awake()
    {
        base.Awake();

        vitruviusVideo.Initialize(OnVideoFrameArrived, null, null);

        Avateering.Enable();
        for (int i = 0; i < clothes.Length; i++)
        {
            clothes[i].Initialize();
        }

        selected = new bool[clothes.Length];
    }

    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();

        vitruviusVideo.Dispose();

        Avateering.Disable();
        for (int i = 0; i < clothes.Length; i++)
        {
            clothes[i].Dispose();
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

    void OnGUI()
    {
        for (int i = 0; i < clothes.Length; i++)
        {
            if (!clothes[i].IsInitialized)
            {
                continue;
            }

            Vector3 clothPosition = Camera.main.WorldToScreenPoint(buttonPositions[i]);
            clothPosition.y = Screen.height - clothPosition.y;

            Rect rect = new Rect(
                clothPosition.x,
                clothPosition.y,
                buttonSize.x * (float)Screen.width,
                buttonSize.y * (float)Screen.height);
            rect.x -= rect.width * 0.5f;
            rect.y -= rect.height * 0.5f;

            if (GUI.Button(rect, clothes[i].Body.name))
            {
                selected[i] = !selected[i];

                if (!selected[i])
                {
                    clothes[i].Reset();
                }
            }
        }
    }

    #endregion

    #region UpdateFrame

    void UpdateFrame()
    {
        byte[] pixels = null;

        if (useGreenScreen)
        {
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
        }
        else
        {
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
        }

        UpdateBodyFrame();

        RefreshFrame(body, useGreenScreen);

        vitruviusVideo.videoRecorder.RecordFrame(pixels, visualization, resolution, body, null, useGreenScreen);
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
            this.body.Set(body, KinectSensor.CoordinateMapper, useGreenScreen ? Visualization.Depth : visualization);
        }
    }

    #endregion

    #region OnVideoFrameArrived

    void OnVideoFrameArrived(Texture2D image, Visualization visualization, ColorFrameResolution resolution, BodyWrapper body, Face face)
    {
        frameView.FrameTexture = image;

        this.body = body;

        RefreshFrame(body, visualization == Visualization.Depth);
    }

    #endregion

    #region RefreshFrame

    void RefreshFrame(BodyWrapper body, bool useGreenScreen)
    {
        if (useGreenScreen)
        {
            if (!greenScreenView.activeSelf)
            {
                greenScreenView.SetActive(true);
            }
        }
        else if (greenScreenView.activeSelf)
        {
            greenScreenView.SetActive(false);
        }

        if (body != null)
        {
            for (int i = 0; i < selected.Length; i++)
            {
                if (selected[i])
                {
                    AvatarCloth cloth = clothes[i];

                    Avateering.Update(cloth, body);

                    Vector2 position = vitruviusVideo.videoPlayer.IsPlaying ? body.Map2D[cloth.Pivot].ToVector2() :
                        body.Joints[cloth.Pivot].Position.ToPoint(useGreenScreen ? Visualization.Depth : visualization, KinectSensor.CoordinateMapper);
                    
                    if (!float.IsInfinity(position.x) && !float.IsInfinity(position.y))
                    {
                        frameView.SetPositionOnFrame(ref position);

                        cloth.SetBonePosition(cloth.Pivot, position);

                        float distance = cloth.JointInfos[(int)cloth.Pivot].RawPosition.z;
                        if (distance != 0)
                        {
                            cloth.Body.transform.localScale = cloth.ScaleOrigin * (cloth.colorScaleFactor / distance) * frameView.ViewScale;
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < clothes.Length; i++)
            {
                clothes[i].Reset();
            }
        }
    }

    #endregion
}