using UnityEngine;
using LightBuzz.Vitruvius.Avateering;
using Windows.Kinect;

[System.Serializable]
public class AvatarCloth : FBX
{
    #region Variables and Properties

    protected JointType pivot = JointType.SpineBase;
    public JointType Pivot
    {
        get
        {
            return pivot;
        }
    }
    Vector3 pivotOrigin;
    public Vector3 ScaleOrigin
    {
        get;
        private set;
    }

    public float colorScaleFactor;
    public float depthScaleFactor;

    #endregion

    #region OnInitialized

    protected override void OnInitialized()
    {
        pivotOrigin = Bones[(int)pivot].Transform.localPosition;
        ScaleOrigin = Body.transform.localScale;
    }

    #endregion

    #region OnReset

    protected override bool OnReset()
    {
        if (base.OnReset())
        {
            Bones[(int)pivot].Transform.localPosition = pivotOrigin;
            Body.transform.localScale = ScaleOrigin;

            return true;
        }

        return false;
    }

    #endregion
}