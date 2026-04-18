using UnityEngine;

[CreateAssetMenu(fileName = "UIConfig", menuName = "GravityPuzzle/UIConfig")]
public class UIConfig : ScriptableObject
{
    [Header("Common Fade Setting")]
    public float commonFadeInTime = 0.2f;
    public float commonFadeOutTime = 1.0f;

    [Header("Pause Fade Setting")]
    public float pauseFadeInTime = 0.2f;
    public float pauseFadeOutTime = 0.2f;

    [Header("Mission Panel Setting")]
    public int untilShowTime = 500;
    public int displayTime = 2000;
    public float missionPanelFadeInTime = 0.1f;
    public float missionPanelFadeOutTime = 1.0f;
}
