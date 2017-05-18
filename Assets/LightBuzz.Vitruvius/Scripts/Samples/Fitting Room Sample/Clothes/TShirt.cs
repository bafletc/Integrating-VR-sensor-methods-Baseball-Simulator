using LightBuzz.Vitruvius.Avateering;

public class TShirt : AvatarCloth
{
    public override void OnUpdate()
    {
        UpdateBone(Avateering.SpineBase);
        UpdateBone(Avateering.SpineMid);
        UpdateBone(Avateering.Neck);

        UpdateBone(Avateering.ShoulderLeft);
        UpdateBone(Avateering.ElbowLeft);

        UpdateBone(Avateering.ShoulderRight);
        UpdateBone(Avateering.ElbowRight);
    }
}