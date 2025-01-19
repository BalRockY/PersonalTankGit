using UnityEngine;

[CreateAssetMenu(fileName = "WallPositionData", menuName = "ScriptableObjects/WallPositionData", order = 1)]
public class WallPositionData : ScriptableObject
{
    public Vector3[] positions;
}
