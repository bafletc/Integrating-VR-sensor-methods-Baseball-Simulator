using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System;
using System.Collections.Generic;
using DamienG.System;
using System.Linq;

public class StickmanController : MonoBehaviour
{
    #region Variables

    protected bool initialized = false;

    public StickmanJointList joints = new StickmanJointList();

    public Transform eyeLeft;
    public Transform eyeRight;
    public LineRenderer eyesLine;

    #endregion

    public virtual void InitializeJoints()
    {
        if (initialized)
        {
            return;
        }

        for (int i = 0; i < joints.Count; i++)
        {
            joints[i].DefineJointType();
            joints[i].DefineLink();

            if (joints[i].line == null)
            {
                joints[i].DefineLine();
            }
        }

        initialized = true;
    }

    public virtual IEnumerable GetJointList()
    {
        return joints.Joints;
    }

    public GameObject GetJointObject(int index)
    {
        return joints[index].Joint;
    }
    public GameObject GetJointObject(JointType jointType)
    {
        return joints[jointType].Joint;
    }

    public JointType GetJointType(int index)
    {
        return joints[index].JointType;
    }

    public LineRenderer GetJointLine(int index)
    {
        return joints[index].line;
    }
    public LineRenderer GetJointLine(JointType jointType)
    {
        return joints[jointType].line;
    }

    public Vector3 GetJointPosition(int index)
    {
        return joints[index].JointPosition;
    }
    public Vector3 GetJointPosition(JointType jointType)
    {
        return joints[jointType].JointPosition;
    }

    public void SetJointPosition(int index, Vector3 position)
    {
        joints[index].JointPosition = position;
    }

    public JointType GetLinkType(JointType originalType)
    {
        return joints[originalType].Link;
    }

    public Vector3 GetLinkPosition(int index)
    {
        return joints[joints[index].Link].JointPosition;
    }

    public Vector3 GetLinkPosition(JointType jointType)
    {
        return joints[joints[jointType].Link].JointPosition;
    }

    public int JointCount()
    {
        return joints.Count;
    }
}

[System.Serializable]
public class StickmanJointList
{
    #region Variables and Properties

    [SerializeField]
    List<StickmanJoint> joints = new List<StickmanJoint>();
    public IEnumerable<StickmanJoint> Joints
    {
        get
        {
            return joints;
        }
    }

    public StickmanJoint this[JointType index]
    {
        get
        {
            return joints.Find(x => x.JointType == index);
        }
    }

    public StickmanJoint this[int index]
    {
        get
        {
            return joints[index];
        }
    }

    public int Count
    {
        get
        {
            return joints.Count;
        }
    }

    #endregion
}

[System.Serializable]
public class StickmanJoint
{
    protected JointType jointType;
    public JointType JointType
    {
        get
        {
            return jointType;
        }
    }

    public Transform joint;
    public LineRenderer line;
    protected JointType link;
    public JointType Link
    {
        get
        {
            return link;
        }
    }

    public GameObject Joint
    {
        get
        {
            return joint.gameObject;
        }
    }

    public Vector3 JointPosition
    {
        get
        {
            return joint.position;
        }
        set
        {
            joint.position = value;
        }
    }

    public void DefineJointType()
    {
        jointType = (JointType)System.Enum.Parse(typeof(JointType), joint.name);
    }

    public void DefineLink()
    {
        JointType type;

        if (Enum<JointType>.TryParse(joint.parent.name, out type))
        {
            link = type;
        }
    }

    public void DefineLine()
    {
        Transform transform = joint.GetComponentsInChildren<Transform>().FirstOrDefault(y => y.ToString().Contains(joint.name) && y.ToString().Contains(link.ToString()));
        if (transform != null)
        {
            line = transform.GetComponent<LineRenderer>();
        }
    }
}