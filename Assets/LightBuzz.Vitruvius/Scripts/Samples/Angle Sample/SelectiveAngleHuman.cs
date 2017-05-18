using UnityEngine;
using System.Collections;
using LightBuzz.Vitruvius.Avateering;
using Windows.Kinect;
using System.Linq;

public class SelectiveAngleHuman : FBX
{
    public JointType start;
    public JointType center;
    public JointType end;

    public override void OnUpdate()
    {
        if (IsJoint(JointType.SpineBase))
        {
            UpdateBone(Avateering.SpineBase);
        }

        if (IsJoint(JointType.SpineMid))
        {
            UpdateBone(Avateering.SpineMid);
        }

        if (IsJoint(JointType.Neck))
        {
            UpdateBone(Avateering.Neck);
        }

        if (IsJoint(JointType.ShoulderLeft))
        {
            UpdateBone(Avateering.ShoulderLeft);
        }
        if (IsJoint(JointType.ElbowLeft))
        {
            UpdateBone(Avateering.ElbowLeft);
        }
        if (IsJoint(JointType.WristLeft))
        {
            UpdateBone(Avateering.WristLeft, 7);
        }

        if (IsJoint(JointType.ShoulderRight))
        {
            UpdateBone(Avateering.ShoulderRight);
        }
        if (IsJoint(JointType.ElbowRight))
        {
            UpdateBone(Avateering.ElbowRight);
        }
        if (IsJoint(JointType.WristRight))
        {
            UpdateBone(Avateering.WristRight, 7);
        }

        if (IsJoint(JointType.HipLeft))
        {
            UpdateBone(Avateering.HipLeft);
        }
        if (IsJoint(JointType.KneeLeft))
        {
            UpdateBone(Avateering.KneeLeft);
        }
        if (IsJoint(JointType.AnkleLeft))
        {
            UpdateBone(Avateering.AnkleLeft, 7);
        }

        if (IsJoint(JointType.HipRight))
        {
            UpdateBone(Avateering.HipRight);
        }
        if (IsJoint(JointType.KneeRight))
        {
            UpdateBone(Avateering.KneeRight);
        }
        if (IsJoint(JointType.AnkleRight))
        {
            UpdateBone(Avateering.AnkleRight, 7);
        }
    }

    bool IsJoint(JointType joint)
    {
        return start == joint || center == joint || end == joint;
    }
}