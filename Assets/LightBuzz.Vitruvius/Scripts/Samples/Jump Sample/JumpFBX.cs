using UnityEngine;
using LightBuzz.Vitruvius.Avateering;

public class JumpFBX : FBX
{
    #region Variables

    float minZ = float.MaxValue;
    float maxZ = float.MinValue;
    float yAtMin = 0;
    float yAtMax = 0;
    public float jumpHeightMultiplier = 15;
    public float jumpSmoothness = 0.1f;

    float prevJumpHeight = 0;
    public float JumpHeight
    {
        get;
        private set;
    }

    #endregion

    #region FBX reserved methods // OnPreUpdate - OnPostUpdate

    public override void OnPreUpdate()
    {
        // Reset y axis
        Vector3 localPosition = PreBaseJoint.localPosition;
        localPosition.y = 0;
        PreBaseJoint.localPosition = localPosition;

        base.OnPreUpdate();
    }

    public override void OnPostUpdate()
    {
        base.OnPostUpdate();

        UpdateJumpData();
        
        // Add jump height to y axis
        Vector3 localPosition = PreBaseJoint.localPosition;
        localPosition.y += Mathf.Lerp(prevJumpHeight, JumpHeight, jumpSmoothness);
        prevJumpHeight = JumpHeight;
        PreBaseJoint.localPosition = localPosition;
    }

    #endregion

    #region UpdateJumpData

    void UpdateJumpData()
    {
        Vector3 leftAnkle = GetJointInfo(Avateering.AnkleLeft).RawPosition;
        Vector3 rightAnkle = GetJointInfo(Avateering.AnkleRight).RawPosition;

        if (leftAnkle.z > maxZ)
        {
            maxZ = leftAnkle.z;
            yAtMax = leftAnkle.y;
        }

        if (leftAnkle.z < minZ)
        {
            minZ = leftAnkle.z;
            yAtMin = leftAnkle.y;
        }

        if (rightAnkle.z > maxZ)
        {
            maxZ = rightAnkle.z;
            yAtMax = rightAnkle.y;
        }

        if (rightAnkle.z < minZ)
        {
            minZ = rightAnkle.z;
            yAtMin = rightAnkle.y;
        }

        float standHeight = Mathf.Lerp(yAtMin, yAtMax, (Mathf.Lerp(leftAnkle.z, rightAnkle.z, 0.5f) - minZ) / (maxZ - minZ));

        JumpHeight = leftAnkle.y > (standHeight + 0.1f) && rightAnkle.y > (standHeight + 0.1f) ?
            ((leftAnkle.y < rightAnkle.y ? leftAnkle.y : rightAnkle.y) - standHeight) * jumpHeightMultiplier : 0;
    }

    #endregion
}