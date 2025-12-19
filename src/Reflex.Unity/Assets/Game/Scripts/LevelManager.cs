using UnityEngine;

public sealed class LevelManager : MonoBehaviour
{
    public LevelCatalog Catalog;

    public LevelConfig CurrentLevel { get; private set; }
    private int _levelIndex;

    public void StartFirstLevel()
    {
        _levelIndex = 0;
        LoadLevelByIndex(_levelIndex);
    }

    public void LoadNextLevel()
    {
        if (Catalog == null || Catalog.Levels.Count == 0)
            return;

        _levelIndex = Mathf.Min(_levelIndex + 1, Catalog.Levels.Count - 1);
        LoadLevelByIndex(_levelIndex);
    }

    public void LoadLevelByIndex(int index)
    {
        if (Catalog == null || Catalog.Levels.Count == 0)
            return;

        index = Mathf.Clamp(index, 0, Catalog.Levels.Count - 1);
        _levelIndex = index;

        CurrentLevel = Catalog.Levels[_levelIndex];
        Debug.Log($"Starting Level {CurrentLevel.Id}");
    }
}
