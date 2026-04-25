using UnityEngine;

public class DungeonViewer : MonoBehaviour
{
    public void CenterCamera(int mapWidth, int mapHeight)
    {
        Vector3 newPosition;
        newPosition.x = Mathf.RoundToInt(mapWidth / 2);
        newPosition.y = Mathf.RoundToInt(mapHeight / 2);
        newPosition.z = -10f;

        transform.position = newPosition;
    }
}