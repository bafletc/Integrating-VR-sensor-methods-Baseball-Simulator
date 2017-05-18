using UnityEngine;
using System.Collections;

public class ScrollableList : KinectButton
{
    public Transform listParent;

    protected override void OnDragging()
    {
        listParent.position += new Vector3(KinectUI.Direction.x, 0, 0);
    }

    protected override void OnOutsideDragging()
    {
        OnDragging();
    }
}