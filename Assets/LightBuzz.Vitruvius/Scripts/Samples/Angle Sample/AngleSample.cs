using UnityEngine;
using LightBuzz.Vitruvius;
using LightBuzz.Vitruvius.Avateering;
using Joint = Windows.Kinect.Joint;
using Windows.Kinect;

public class AngleSample : VitruviusSample
{
    #region Variables

    BodyWrapper body;

    public VitruviusVideo vitruviusVideo;

    public Stickman stickman;
    public JointPeak[] jointPeaks;

    public SelectiveAngleHuman model;

    public AngleArc frameViewArc;
    public AngleArc modelArc;

    GUIStyle guiStyle;

    #endregion

    #region Reserved methods // Awake - OnApplicationQuit - Update - OnGUI

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
        if (!stickman.isActiveAndEnabled)
        {
            return;
        }

        if (guiStyle == null)
        {
            guiStyle = new GUIStyle(GUI.skin.textArea);
            guiStyle.alignment = TextAnchor.MiddleCenter;
        }

        for (int i = 0; i < jointPeaks.Length; i++)
        {
            Vector2 jointPosition = Camera.main.WorldToScreenPoint(jointPeaks[i].arc.transform.position);

            GUI.Label(new Rect(jointPosition.x, Screen.height - jointPosition.y, 50, 25), jointPeaks[i].jointAngle.ToString("N0") + "°", guiStyle);
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
        frameViewArc.gameObject.SetActive(body != null);
        modelArc.gameObject.SetActive(body != null);

        if (body != null)
        {
            if (model.updateModel)
            {
                Avateering.Update(model, body);
            }

            stickman.UpdateBody(body, frameView);

            Joint startJoint;
            Joint centerJoint;
            Joint endJoint;
            Vector2 centerJointPosition;
            Vector2 startJointDir;
            Vector2 endJointDir;

            #region JointPeaks

            for (int i = 0; i < jointPeaks.Length; i++)
            {
                startJoint = body.Joints[jointPeaks[i].start];
                centerJoint = body.Joints[jointPeaks[i].center];
                endJoint = body.Joints[jointPeaks[i].end];

                centerJointPosition = stickman.controller.GetJointPosition(jointPeaks[i].center);
                startJointDir = ((Vector2)stickman.controller.GetJointPosition(jointPeaks[i].start) - centerJointPosition).normalized;
                endJointDir = ((Vector2)stickman.controller.GetJointPosition(jointPeaks[i].end) - centerJointPosition).normalized;

                jointPeaks[i].arc.Angle = Vector2.Angle(startJointDir, endJointDir);

                jointPeaks[i].arc.transform.position = centerJointPosition;
                jointPeaks[i].arc.transform.up = Quaternion.Euler(0, 0, jointPeaks[i].arc.Angle) *
                    (Vector2.Dot(Quaternion.Euler(0, 0, 90) * startJointDir, endJointDir) > 0 ? startJointDir : endJointDir);

                jointPeaks[i].jointAngle = (float)centerJoint.Angle(startJoint, endJoint);
            }

            #endregion

            startJoint = body.Joints[model.start];
            centerJoint = body.Joints[model.center];
            endJoint = body.Joints[model.end];

            #region FrameView Arc

            if (vitruviusVideo.videoPlayer.IsPlaying)
            {
                startJointDir = body.Map2D[model.start].ToVector2();
                centerJointPosition = body.Map2D[model.center].ToVector2();
                endJointDir = body.Map2D[model.end].ToVector2();
            }
            else
            {
                startJointDir = startJoint.Position.ToPoint(visualization, KinectSensor.CoordinateMapper);
                centerJointPosition = centerJoint.Position.ToPoint(visualization, KinectSensor.CoordinateMapper);
                endJointDir = endJoint.Position.ToPoint(visualization, KinectSensor.CoordinateMapper);
            }

            frameView.SetPositionOnFrame(ref startJointDir);
            frameView.SetPositionOnFrame(ref centerJointPosition);
            frameView.SetPositionOnFrame(ref endJointDir);

            startJointDir = (startJointDir - centerJointPosition).normalized;
            endJointDir = (endJointDir - centerJointPosition).normalized;

            frameViewArc.Angle = Vector2.Angle(startJointDir, endJointDir);

            frameViewArc.transform.position = centerJointPosition;
            frameViewArc.transform.up = Quaternion.Euler(0, 0, frameViewArc.Angle) *
                (Vector2.Dot(Quaternion.Euler(0, 0, 90) * startJointDir, endJointDir) > 0 ? startJointDir : endJointDir);

            #endregion

            #region Model Arc

            Vector3 arcPosition = model.GetBone(model.center).Transform.position;
            arcPosition.z -= 2;

            centerJointPosition = centerJoint.Position.ToVector3();
            startJointDir = ((Vector2)startJoint.Position.ToVector3() - centerJointPosition).normalized;
            endJointDir = ((Vector2)endJoint.Position.ToVector3() - centerJointPosition).normalized;

            modelArc.Angle = Vector2.Angle(startJointDir, endJointDir);

            modelArc.transform.position = arcPosition;
            modelArc.transform.up = Quaternion.Euler(0, 0, modelArc.Angle) *
                (Vector2.Dot(Quaternion.Euler(0, 0, 90) * startJointDir, endJointDir) > 0 ? startJointDir : endJointDir);

            #endregion
        }
    }

    #endregion
}