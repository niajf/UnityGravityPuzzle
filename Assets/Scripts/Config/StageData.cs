using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "GravityPuzzle/StageData")]
public class StageData : ScriptableObject
{
    public string stageName;
    public int stageIndex;
    public float parTime;           // 目標クリアタイム
    public Vector3 initialGravity = Vector3.down;
}
