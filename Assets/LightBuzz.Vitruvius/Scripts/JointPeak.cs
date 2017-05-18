using Windows.Kinect;

[System.Serializable]
public class JointPeak
{
    public JointType start;
    public JointType center;
    public JointType end;
    public AngleArc arc;
    [UnityEngine.HideInInspector]
    public float jointAngle = 0;
}