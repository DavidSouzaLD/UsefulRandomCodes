using System.Collections;
using UnityEngine;

public class FireWithRayAndGravity : MonoBehaviour
{
    public LayerMask hittableMask;
    public Transform shootPoint;
    public float maxBulletDistance;
    public float effectiveBulletDistance;
    public float bulletGravityScale;
    public float bulletVelocity;
    public float bulletHitForce;

    [Header("Gizmos")]
    [SerializeField] private bool showGizmos;
    [SerializeField] private Color bulletTracingColor = new Color32(255, 0, 0, 255);
    [SerializeField] private Color effectiveDistanceColor = new Color32(0, 255, 0, 255);


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            ShootFire();
    }

    private void ShootFire()
    {
        Vector3 p1 = shootPoint.position;
        Vector3 predictedVelocity = shootPoint.forward * maxBulletDistance;
        float stepSize = 0.01f;

        for (float step = 0f; step < 1; step += stepSize)
        {
            // Apply gravity in bullet
            if (step > (effectiveBulletDistance / maxBulletDistance))
            {
                predictedVelocity += bulletGravityScale * stepSize * Physics.gravity;
            }

            Vector3 p2 = p1 + predictedVelocity * stepSize;
            Ray ray = new(p1, p2 - p1);

            if (Physics.Raycast(ray, out RaycastHit hit, (p2 - p1).magnitude, hittableMask))
            {
                if (hit.transform)
                {
                    // Calculate delay time
                    float distance = (shootPoint.position - hit.point).sqrMagnitude;
                    float time = distance / (bulletVelocity * 1000f);

                    StartCoroutine(CalculateFireDelay(time, hit));
                    break;
                }
            }
            p1 = p2;
        }
    }

    private IEnumerator CalculateFireDelay(float time, RaycastHit hit)
    {
        yield return new WaitForSeconds(time);

        // Apply force in physics bodys
        if (hit.transform.TryGetComponent<Rigidbody>(out var hittedBody))
        {
            hittedBody.AddForceAtPosition(shootPoint.forward * bulletHitForce, hit.point);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = effectiveDistanceColor;
        Vector3 p1 = shootPoint.position;
        Vector3 predictedBulletVelocity = shootPoint.forward * maxBulletDistance;
        float stepSize = 0.01f;

        for (float step = 0f; step < 1; step += stepSize)
        {
            if (step > (effectiveBulletDistance / maxBulletDistance))
            {
                Gizmos.color = bulletTracingColor;
                predictedBulletVelocity += bulletGravityScale * stepSize * Physics.gravity;
            }

            Vector3 p2 = p1 + predictedBulletVelocity * stepSize;
            Gizmos.DrawLine(p1, p2);
            p1 = p2;
        }
    }
}
