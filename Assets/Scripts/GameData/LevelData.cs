using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class LevelData
{
    public string name;
    public int waves;
    public List<SpawnData> spawns;
}
