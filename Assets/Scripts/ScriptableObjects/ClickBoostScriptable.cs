using UnityEngine;

[CreateAssetMenu(fileName = "New Click Boost", menuName = "Plants/ClickBoost")]
public class ClickBoostScriptable : ScriptableObject
{
    [Header("Type Of Booster")]
    public ClickBooster typeBooster;
   
    [Header("Value Boost (area or int)")]
    public float amount;
}
