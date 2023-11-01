using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(Stage))]
public class Stage : ScriptableObject
{
    public Color stageBackgroundColor = Color.white;
    public Color stageLevelPartColor = Color.white;
    public Color stageBallColor = Color.white;
    public List<Level> levels;
}

[Serializable]
public class Level
{
    [Range(1, 11)]
    public int partCount = 11;

    [Range(0, 11)]
    public int deathPartCount = 1;
}
