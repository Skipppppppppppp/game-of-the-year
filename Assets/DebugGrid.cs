using Unity.Collections;
using UnityEngine;

public class DebugGrid : MonoBehaviour
{
    public int gridSize = 100;
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,1,1,0.1f);
        for (int i = -gridSize; i<=gridSize; i++)
        {
            Gizmos.DrawLine(new Vector3 (i, -gridSize, 0), new Vector3 (i, gridSize, 0));
        }
        for (int i = -gridSize; i<=gridSize; i++)
        {
            Gizmos.DrawLine(new Vector3 (-gridSize, i, 0), new Vector3 (gridSize, i, 0));
        } 
    }
}
