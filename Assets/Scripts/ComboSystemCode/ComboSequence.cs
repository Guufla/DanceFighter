using UnityEngine;

[CreateAssetMenu(menuName = "Combo/ComboSequence")]
public class ComboSequence : ScriptableObject
{
    public ComboData[] steps;
}