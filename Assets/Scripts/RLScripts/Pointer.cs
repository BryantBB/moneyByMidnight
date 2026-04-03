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
        Vector2 boxSize = new Vector2(1f, 1f); 
    
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(tip.position, boxSize, tip.eulerAngles.z, sectionLayer);    

        GameObject bestSection = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D hit in hitColliders)
        {
            print("hit");
            
            float distance = Vector2.Distance(tip.position, hit.bounds.center);

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