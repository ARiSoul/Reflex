using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Reflex/Level Catalog", fileName = "LevelCatalog")]
public sealed class LevelCatalog : ScriptableObject
{
    public List<LevelConfig> Levels = new();
}