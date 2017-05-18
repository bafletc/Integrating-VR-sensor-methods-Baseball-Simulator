using LightBuzz.Vitruvius.Avateering;

public class Pants : AvatarCloth
{
    public override void OnUpdate()
    {
        UpdateBone(Avateering.SpineBase);

        UpdateBone(Avateering.HipLeft);
        UpdateBone(Avateering.KneeLeft);
        UpdateBone(Avateering.AnkleLeft, 7);

        UpdateBone(Avateering.HipRight);
        UpdateBone(Avateering.KneeRight);
        UpdateBone(Avateering.AnkleRight, 7);
    }
}