using UnityEngine;
using System.Collections;

public class DraggableElement : KinectButton
{
    protected override void OnDragging()
    {
        transform.position = KinectUI.CursorPosition;
    }

    protected override void OnOutsideDragging()
    {
        OnDragging();
    }
}