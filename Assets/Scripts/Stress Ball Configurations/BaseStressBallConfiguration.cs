using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Stress Ball Configuration", menuName = "Create Stress Ball Configuration")]
public class BaseStressBallConfiguration : ScriptableObject
{
    [Space(20)]
    public string ConfigurationName;

    [Space(20)]
    public float MinimumSize;
    public float MaximumSize;
    public const float IncrementalSizeChange = 0.1f;

    [Space(20)]
    public List<Color> AvailableColors = new List<Color>() { Color.black };
}