using UnityEngine;

public class SphereCombatSetup : MonoBehaviour
{
    [Header("Assign spheres in inspector")]
    public Transform sphereA;
    public Transform sphereB;

    [Header("Desired distance between spheres")]
    public float combatDistance = 3f;

    public GameObject Proute;

    void Start()
    {
        if (sphereA == null || sphereB == null)
        {
            Debug.LogError("Spheres not assigned!");
            return;
        }

        // Calcul direction de B par rapport � A
        Vector3 direction = sphereB.position - sphereA.position;

        if (direction.magnitude == 0)
        {
            Debug.LogWarning("Spheres are at same position. Moving sphereB along X axis.");
            direction = Vector3.right;
        }
        else
        {
            direction.Normalize();
        }

        // Nouvelle position de B � la distance souhait�e
        sphereB.position = sphereA.position + direction * combatDistance;
    }

    private void Update()
    {
    }
}
