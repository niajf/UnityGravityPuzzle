using UnityEngine;

/// <summary>
/// ステージごとの設定を保持する ScriptableObject。
/// ステージ名・インデックス・目標クリアタイム・初期重力方向を定義する。
/// </summary>
[CreateAssetMenu(fileName = "StageData", menuName = "GravityPuzzle/StageData")]
public class StageData : ScriptableObject
{
    public string stageName;
    public int stageIndex;
    public float parTime;                              // 目標クリアタイム（秒）
    public Vector3 initialGravity = Vector3.down;      // ステージ開始時の重力方向
}
