using UnityEngine;
using System.Collections;

public class PaperPlane : MonoBehaviour
{
    public float flySpeed = 1;
    public Rigidbody2D myRigidbody;
    public SpriteRenderer spriteRenderer;

    public float destroyAfter = 5;

    public void SetOff(Vector3 flyDirection)
    {
        myRigidbody.velocity = flyDirection * flySpeed;
        
        if (flyDirection.x > 0)
        {
            spriteRenderer.flipX = true;
            flyDirection = -flyDirection;
        }

        float angle = Vector3.Angle(Vector3.left, flyDirection);
        angle = Vector3.Dot(Vector3.up, flyDirection) > 0 ? -angle : angle;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, destroyAfter);
    }
}