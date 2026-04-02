using UnityEngine;

public class Pointer : MonoBehaviour
{
    public Betting bettingScript;

    public LayerMask sectionLayer; // Assigned wheel sections to a specific Layer
    public Transform tip; // Tip of the pointer distinguished for detection purposed
    public Vector3 detectionSize = new Vector3(0.1f, 0.1f, 0.1f);

    public void PrintWinningSection()
    {
        GameObject winner = GetWinningSection();

        if (winner != null)
        {
            // Prints the name of the GameObject to the Console
            Debug.Log($"<color=green>Winner Detected:</color> {winner.name}");
        }
        else
        {
            Debug.LogWarning("No section detected! Is the arrow touching a collider?");
        }
    }

    public GameObject GetWinningSection()
    {        
        Vector3 boxSize = new Vector3(1f, 1f, 1f); 
    
        // Detects all colliders within the Arrow's space
        Collider[] hitColliders = Physics.OverlapBox(tip.position, boxSize / 2, tip.rotation, sectionLayer);


        GameObject bestSection = null;
        float closestDistance = Mathf.Infinity;

        // Decides which section is closer if the Arrow hits many
        foreach (Collider hit in hitColliders)
        {
            float distance = Vector3.Distance(tip.position, hit.bounds.center);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestSection = hit.gameObject;
            }
        }

        if (bestSection != null && bettingScript != null)
        {
            bettingScript.ResolveSpin(bestSection.name);
        }
        return bestSection;
    }

}