using UnityEngine;
using System.Collections;

public class PaperPlaneSpawner : MonoBehaviour
{
    public float verticalRange = 1;

    public bool isLeftSide = true;

    public PaperPlane paperPlanePrefab;

    IEnumerator Start()
    {
        while (true)
        {
            Vector3 from = new Vector3(transform.position.x, GetRandomY(), 0);
            Vector3 to = new Vector3(-from.x, GetRandomY(), 0);
            Vector3 flyDirection = (to - from).normalized;

            PaperPlane paperPlane = Instantiate(paperPlanePrefab);
            paperPlane.transform.position = from;
            paperPlane.gameObject.layer = InFrontOfPlayer() ? 8 : 9;
            paperPlane.spriteRenderer.sortingOrder = 1;
            paperPlane.SetOff(flyDirection);

            yield return new WaitForSeconds(Random.Range(3, 5));
        }
    }

    float GetRandomY()
    {
        return Random.Range(-verticalRange * 0.5f, verticalRange * 0.5f);
    }

    bool InFrontOfPlayer()
    {
        return Random.Range(0, 100) > 50;
    }
}