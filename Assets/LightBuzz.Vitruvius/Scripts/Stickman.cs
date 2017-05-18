using UnityEngine;
using System.Collections;
using Windows.Kinect;
using LightBuzz.Vitruvius;

public class Stickman : MonoBehaviour
{
    #region Variables
    
    public bool smoothJoints = false;
    [SerializeField, Range(0f, 1f)]
    float smoothness = 0.25f;
    public float Smoothness
    {
        get
        {
            return smoothness;
        }
        set
        {
            smoothness = Mathf.Clamp01(value);
        }
    }

    public StickmanController controller;

    bool eyesEnabledExternal = false;

    public delegate Vector2 MapCameraPointToKinectSpace(ref CameraSpacePoint point);

    public bool Initialized
    {
        get;
        private set;
    }

    public bool Mirrored
    {
        get;
        private set;
    }

    public bool UpperBodyActive
    {
        get
        {
            return controller.GetJointObject(JointType.SpineMid).activeSelf;
        }
    }

    public bool LowerBodyActive
    {
        get
        {
            return controller.GetJointObject(JointType.SpineBase).activeSelf;
        }
    }

    public bool HasEyes
    {
        get
        {
            return controller.eyeLeft != null && controller.eyeRight != null;
        }
    }

    #endregion

    #region Initialize

    public void Initialize()
    {
        if (Initialized)
        {
            return;
        }

        controller.InitializeJoints();

        Initialized = true;

        OnInitialized();
    }

    #endregion

    #region OnInitialized

    protected virtual void OnInitialized()
    {

    }

    #endregion

    #region UpdateBody

    public void UpdateBody(Body body, FrameView frameView, CoordinateMapper coordinateMapper, Visualization visualization)
    {
        if (!Initialized)
        {
            return;
        }

        Mirrored = frameView.MirroredView;

        for (int i = 0; i < controller.JointCount(); i++)
        {
            Vector2 position = visualization == Visualization.Color ?
                coordinateMapper.MapCameraPointToColorSpace(body.Joints[controller.GetJointType(i)].Position).ToPoint() :
                coordinateMapper.MapCameraPointToDepthSpace(body.Joints[controller.GetJointType(i)].Position).ToPoint();

            if (float.IsInfinity(position.x))
            {
                position.x = 0f;
            }

            if (float.IsInfinity(position.y))
            {
                position.y = 0f;
            }

            frameView.SetPositionOnFrame(ref position);

            controller.SetJointPosition(i, smoothJoints ? Vector3.Lerp(controller.GetJointPosition(i), position, smoothness) : (Vector3)position);
        }

        UpdateLines();
    }

    public void UpdateBody(BodyWrapper body, FrameView frameView)
    {
        if (!Initialized)
        {
            return;
        }

        Mirrored = frameView.MirroredView;

        for (int i = 0; i < controller.JointCount(); i++)
        {
            Vector2 position = body.Map2D[controller.GetJointType(i)].ToVector2();

            if (float.IsInfinity(position.x))
            {
                position.x = 0f;
            }

            if (float.IsInfinity(position.y))
            {
                position.y = 0f;
            }

            frameView.SetPositionOnFrame(ref position);

            controller.SetJointPosition(i, smoothJoints ? Vector3.Lerp(controller.GetJointPosition(i), position, smoothness) : (Vector3)position);
        }

        UpdateLines();
    }

    #endregion

    #region UpdateFace

    public virtual void UpdateFace(Face face, FrameView frameView, CoordinateMapper coordinateMapper)
    {
        if (controller.eyeLeft == null || controller.eyeRight == null)
        {
            return;
        }

        if (!eyesEnabledExternal)
        {
            if (face == null)
            {
                if (controller.eyeLeft.gameObject.activeSelf)
                {
                    controller.eyeLeft.gameObject.SetActive(false);
                    controller.eyeRight.gameObject.SetActive(false);
                }

                return;
            }

            Vector2 leftEyePosition = face.EyeLeft2D.ToVector2();
            Vector2 rightEyePosition = face.EyeRight2D.ToVector2();

            if (!float.IsInfinity(leftEyePosition.x) && !float.IsInfinity(leftEyePosition.y) &&
                   !float.IsInfinity(rightEyePosition.x) && !float.IsInfinity(rightEyePosition.y))
            {
                if (!controller.eyeLeft.gameObject.activeSelf)
                {
                    controller.eyeLeft.gameObject.SetActive(true);
                    controller.eyeRight.gameObject.SetActive(true);
                }

                frameView.SetPositionOnFrame(ref leftEyePosition);
                frameView.SetPositionOnFrame(ref rightEyePosition);

                controller.eyeLeft.position = leftEyePosition;
                controller.eyeRight.position = rightEyePosition;

                if (controller.eyesLine != null)
                {
                    controller.eyesLine.SetPosition(0, leftEyePosition);
                    controller.eyesLine.SetPosition(1, rightEyePosition);
                }
            }
            else if (controller.eyeLeft.gameObject.activeSelf)
            {
                controller.eyeLeft.gameObject.SetActive(false);
                controller.eyeRight.gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region UpdateLines

    protected virtual void UpdateLines()
    {
        for (int i = controller.JointCount() - 1; i >= 0; i--)
        {
            if (controller.GetJointLine(i) != null)
            {
                controller.GetJointLine(i).SetPosition(0, controller.GetJointPosition(i));
                controller.GetJointLine(i).SetPosition(1, controller.GetLinkPosition(i));
            }
        }
    }

    #endregion

    #region Toggle methods

    #region SetHead

    public void SetHead(bool visible)
    {
        if (!Initialized)
        {
            return;
        }

        eyesEnabledExternal = !visible;

        controller.eyeLeft.gameObject.SetActive(visible);
        controller.eyeRight.gameObject.SetActive(visible);

        controller.GetJointObject(JointType.Head).SetActive(visible);
        controller.GetJointObject(JointType.Neck).SetActive(visible);

        controller.GetJointLine(JointType.SpineShoulder).GetComponent<MeshRenderer>().enabled = visible;
    }

    #endregion

    #region SetUpperBody

    public void SetUpperBody(bool visible)
    {
        if (!Initialized)
        {
            return;
        }

        controller.GetJointObject(JointType.SpineMid).SetActive(visible);

        if (visible)
        {
            if (controller.GetJointObject(JointType.SpineBase).activeSelf)
            {
                controller.GetJointLine(JointType.SpineMid).GetComponent<MeshRenderer>().enabled = true;
            }

            controller.GetJointLine(JointType.ShoulderLeft).GetComponent<MeshRenderer>().enabled = true;
            controller.GetJointLine(JointType.ShoulderRight).GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            controller.GetJointLine(JointType.SpineMid).GetComponent<MeshRenderer>().enabled = false;
        }

        controller.GetJointObject(JointType.SpineShoulder).SetActive(visible);

        controller.GetJointObject(JointType.ShoulderLeft).SetActive(visible);
        controller.GetJointObject(JointType.ElbowLeft).SetActive(visible);
        controller.GetJointObject(JointType.WristLeft).SetActive(visible);

        controller.GetJointObject(JointType.ShoulderRight).SetActive(visible);
        controller.GetJointObject(JointType.ElbowRight).SetActive(visible);
        controller.GetJointObject(JointType.WristRight).SetActive(visible);
    }

    #endregion

    #region SetLowerBody

    public void SetLowerBody(bool visible)
    {
        if (!Initialized)
        {
            return;
        }

        controller.GetJointObject(JointType.SpineBase).SetActive(visible);

        if (visible)
        {
            if (controller.GetJointObject(JointType.SpineMid).activeSelf)
            {
                controller.GetJointLine(JointType.SpineMid).GetComponent<MeshRenderer>().enabled = true;
            }
        }
        else
        {
            controller.GetJointLine(JointType.SpineMid).GetComponent<MeshRenderer>().enabled = false;
        }

        controller.GetJointObject(JointType.HipLeft).SetActive(visible);
        controller.GetJointObject(JointType.KneeLeft).SetActive(visible);
        controller.GetJointObject(JointType.AnkleLeft).SetActive(visible);
        controller.GetJointObject(JointType.FootLeft).SetActive(visible);

        controller.GetJointObject(JointType.HipRight).SetActive(visible);
        controller.GetJointObject(JointType.KneeRight).SetActive(visible);
        controller.GetJointObject(JointType.AnkleRight).SetActive(visible);
        controller.GetJointObject(JointType.FootRight).SetActive(visible);
    }

    #endregion

    #region SetLeftSide

    public void SetLeftSide(bool visible)
    {
        if (!Initialized)
        {
            return;
        }

        if (visible)
        {
            controller.GetJointObject(JointType.SpineMid).SetActive(true);

            if (controller.GetJointObject(JointType.SpineBase).activeSelf)
            {
                controller.GetJointLine(JointType.SpineMid).GetComponent<MeshRenderer>().enabled = true;
            }

            controller.GetJointObject(JointType.SpineShoulder).SetActive(true);
        }

        controller.GetJointLine(JointType.ShoulderLeft).GetComponent<MeshRenderer>().enabled = visible;

        controller.GetJointObject(JointType.ShoulderLeft).SetActive(visible);
        controller.GetJointObject(JointType.ElbowLeft).SetActive(visible);
        controller.GetJointObject(JointType.WristLeft).SetActive(visible);

        controller.GetJointObject(JointType.HipLeft).SetActive(visible);
        controller.GetJointObject(JointType.KneeLeft).SetActive(visible);
        controller.GetJointObject(JointType.AnkleLeft).SetActive(visible);
        controller.GetJointObject(JointType.FootLeft).SetActive(visible);
    }

    #endregion

    #region SetRightSide

    public void SetRightSide(bool visible)
    {
        if (!Initialized)
        {
            return;
        }

        if (visible)
        {
            controller.GetJointObject(JointType.SpineMid).SetActive(true);

            if (controller.GetJointObject(JointType.SpineBase).activeSelf)
            {
                controller.GetJointLine(JointType.SpineMid).GetComponent<MeshRenderer>().enabled = true;
            }

            controller.GetJointObject(JointType.SpineShoulder).SetActive(true);
        }

        controller.GetJointLine(JointType.ShoulderRight).GetComponent<MeshRenderer>().enabled = visible;

        controller.GetJointObject(JointType.ShoulderRight).SetActive(visible);
        controller.GetJointObject(JointType.ElbowRight).SetActive(visible);
        controller.GetJointObject(JointType.WristRight).SetActive(visible);

        controller.GetJointObject(JointType.HipRight).SetActive(visible);
        controller.GetJointObject(JointType.KneeRight).SetActive(visible);
        controller.GetJointObject(JointType.AnkleRight).SetActive(visible);
        controller.GetJointObject(JointType.FootRight).SetActive(visible);
    }

    #endregion

    #endregion
}