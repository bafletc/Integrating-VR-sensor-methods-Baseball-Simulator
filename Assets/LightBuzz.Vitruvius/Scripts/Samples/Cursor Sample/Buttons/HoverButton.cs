using UnityEngine;
using System.Collections;

public class HoverButton : KinectButton
{
    protected override void OnHoverExit()
    {
        KinectUI.HideGauge();
    }

    protected override void OnHoverStay()
    {
        KinectUI.ValidateGauge(OnGaugeEnd);
    }

    void OnGaugeEnd()
    {
        Debug.Log("Selected " + name);
    }
}