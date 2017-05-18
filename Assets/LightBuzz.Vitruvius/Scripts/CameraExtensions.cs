using UnityEngine;
using System.Collections;

public static class CameraExtensions
{
    public static Vector2 Size(this Camera camera)
    {
        float pixelFactor = camera.orthographicSize / ((float)camera.pixelHeight * 0.5f);

        Vector2 point = camera.transform.rotation * new Vector2(camera.pixelWidth, camera.pixelHeight);

        point.Set(
            point.x * pixelFactor * (camera.rect.width + camera.rect.x * 2f),
            point.y * pixelFactor * (camera.rect.height + camera.rect.y * 2f));

        return point;
    }
}