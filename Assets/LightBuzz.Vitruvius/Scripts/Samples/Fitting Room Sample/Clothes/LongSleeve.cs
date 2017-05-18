using LightBuzz.Vitruvius.Avateering;

public class LongSleeve : AvatarCloth
{
    public override void OnUpdate()
    {
        UpdateBone(Avateering.SpineBase);
        UpdateBone(Avateering.SpineMid);
        UpdateBone(Avateering.Neck);

        UpdateBone(Avateering.ShoulderLeft);
        UpdateBone(Avateering.ElbowLeft);
        UpdateBone(Avateering.WristLeft, 7);

        UpdateBone(Avateering.ShoulderRight);
        UpdateBone(Avateering.ElbowRight);
        UpdateBone(Avateering.WristRight, 7);
    }
}