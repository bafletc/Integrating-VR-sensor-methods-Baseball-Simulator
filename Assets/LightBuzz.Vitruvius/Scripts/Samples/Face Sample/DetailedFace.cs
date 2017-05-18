using UnityEngine;
using System.Collections;
using LightBuzz.Vitruvius;
using Windows.Kinect;
using System.Collections.Generic;
using Microsoft.Kinect.Face;

public class DetailedFace : MonoBehaviour
{
    #region Variables and Properties

    public bool Initialized
    {
        get;
        private set;
    }

    public Transform forehead;
    Transform[] foreheadPoints;

    public Transform eyebrowLeft;
    Transform[] eyebrowLeftPoints;
    public Transform eyeLeft;
    Transform[] eyeLeftPoints;

    public Transform eyebrowRight;
    Transform[] eyebrowRightPoints;
    public Transform eyeRight;
    Transform[] eyeRightPoints;

    public Transform nose;
    Transform[] nosePoints;

    public Transform mouth;
    Transform[] mouthPoints;

    public Transform jaw;
    Transform[] jawPoints;

    Visualization visualization = Visualization.Color;
    CoordinateMapper coordinateMapper = null;

    #endregion

    #region Initialize

    public void Initialize()
    {
        if (Initialized)
        {
            return;
        }

        if (forehead != null)
        {
            foreheadPoints = new Transform[forehead.childCount];
            for (int i = 0; i < foreheadPoints.Length; i++)
            {
                foreheadPoints[i] = forehead.GetChild(i);
            }
        }

        if (eyebrowLeft != null)
        {
            eyebrowLeftPoints = new Transform[eyebrowLeft.childCount];
            for (int i = 0; i < eyebrowLeftPoints.Length; i++)
            {
                eyebrowLeftPoints[i] = eyebrowLeft.GetChild(i);
            }
        }

        if (eyeLeft != null)
        {
            eyeLeftPoints = new Transform[eyeLeft.childCount];
            for (int i = 0; i < eyeLeftPoints.Length; i++)
            {
                eyeLeftPoints[i] = eyeLeft.GetChild(i);
            }
        }

        if (eyebrowRight != null)
        {
            eyebrowRightPoints = new Transform[eyebrowRight.childCount];
            for (int i = 0; i < eyebrowRightPoints.Length; i++)
            {
                eyebrowRightPoints[i] = eyebrowRight.GetChild(i);
            }
        }

        if (eyeRight != null)
        {
            eyeRightPoints = new Transform[eyeRight.childCount];
            for (int i = 0; i < eyeRightPoints.Length; i++)
            {
                eyeRightPoints[i] = eyeRight.GetChild(i);
            }
        }

        if (nose != null)
        {
            nosePoints = new Transform[nose.childCount];
            for (int i = 0; i < nosePoints.Length; i++)
            {
                nosePoints[i] = nose.GetChild(i);
            }
        }

        if (mouth != null)
        {
            mouthPoints = new Transform[mouth.childCount];
            for (int i = 0; i < mouthPoints.Length; i++)
            {
                mouthPoints[i] = mouth.GetChild(i);
            }
        }

        if (jaw != null)
        {
            jawPoints = new Transform[jaw.childCount];
            for (int i = 0; i < jawPoints.Length; i++)
            {
                jawPoints[i] = jaw.GetChild(i);
            }
        }

        Initialized = true;
    }

    #endregion

    #region UpdateFace

    public void UpdateFace(Face face, FrameView frameView, Visualization visualization, CoordinateMapper coordinateMapper, bool playback = false)
    {
        if (!Initialized || eyeLeft == null || eyeRight == null)
        {
            return;
        }

        this.visualization = visualization;
        this.coordinateMapper = coordinateMapper;

        SetVertexAtPosition(face.Forehead, foreheadPoints[0], frameView);

        if (playback)
        {
            SetVertexAtPosition(face.EyeLeft, eyeLeftPoints[0], frameView);
            SetVertexAtPosition(face.EyeRight, eyeRightPoints[0], frameView);
            SetVertexAtPosition(face.Nose, nosePoints[0], frameView);
            SetVertexAtPosition(face.Mouth, mouthPoints[0], frameView);
            SetVertexAtPosition(face.Jaw, jawPoints[0], frameView);

            ClosePoints(0, eyebrowLeftPoints);
            ClosePoints(0, eyebrowRightPoints);
            ClosePoints(1, eyeLeftPoints);
            ClosePoints(1, eyeRightPoints);
            ClosePoints(1, nosePoints);
            ClosePoints(1, mouthPoints);
            ClosePoints(1, jawPoints);
        }
        else
        {
            List<CameraSpacePoint> eyebrowLeftPoints = face.EyebrowLeftPoints();
            for (int i = 0; i < eyebrowLeftPoints.Count; i++)
            {
                SetVertexAtPosition(eyebrowLeftPoints[i], this.eyebrowLeftPoints[i], frameView);
            }

            List<CameraSpacePoint> eyeLeftPoints = face.EyeLeftPoints();
            for (int i = 0; i < eyeLeftPoints.Count; i++)
            {
                SetVertexAtPosition(eyeLeftPoints[i], this.eyeLeftPoints[i], frameView);
            }

            List<CameraSpacePoint> eyebrowRightPoints = face.EyebrowRightPoints();
            for (int i = 0; i < eyebrowRightPoints.Count; i++)
            {
                SetVertexAtPosition(eyebrowRightPoints[i], this.eyebrowRightPoints[i], frameView);
            }

            List<CameraSpacePoint> eyeRightPoints = face.EyeRightPoints();
            for (int i = 0; i < eyeRightPoints.Count; i++)
            {
                SetVertexAtPosition(eyeRightPoints[i], this.eyeRightPoints[i], frameView);
            }

            List<CameraSpacePoint> nosePoints = face.NosePoints();
            for (int i = 0; i < nosePoints.Count; i++)
            {
                SetVertexAtPosition(nosePoints[i], this.nosePoints[i], frameView);
            }

            List<CameraSpacePoint> mouthPoints = face.MouthPoints();
            for (int i = 0; i < mouthPoints.Count; i++)
            {
                SetVertexAtPosition(mouthPoints[i], this.mouthPoints[i], frameView);
            }

            List<CameraSpacePoint> jawPoints = face.JawPoints();
            for (int i = 0; i < jawPoints.Count; i++)
            {
                SetVertexAtPosition(jawPoints[i], this.jawPoints[i], frameView);
            }
        }
    }

    #endregion

    #region SetVertexAtPosition

    void SetVertexAtPosition(CameraSpacePoint vertex, Transform vertexTransform, FrameView frameView)
    {
        Vector2 position = vertex.ToPoint(visualization, coordinateMapper);

        if (float.IsInfinity(position.x) || float.IsInfinity(position.y))
        {
            if (vertexTransform.gameObject.activeSelf)
            {
                vertexTransform.gameObject.SetActive(false);
            }
        }
        else
        {
            frameView.SetPositionOnFrame(ref position);

            if (!vertexTransform.gameObject.activeSelf)
            {
                vertexTransform.gameObject.SetActive(true);
            }

            vertexTransform.localPosition = position;
        }
    }

    #endregion

    #region ClosePoints

    void ClosePoints(int start, params Transform[] vertexTransforms)
    {
        for (int i = start; i < vertexTransforms.Length; i++)
        {
            if (vertexTransforms[i].gameObject.activeSelf)
            {
                vertexTransforms[i].gameObject.SetActive(false);
            }
        }
    }

    #endregion
}