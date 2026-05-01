using UnityEngine;

public enum GameDifficulty
{
    Easy,
    Hard
}

public static class GameSettings
{
    public static GameDifficulty selectedDifficulty = GameDifficulty.Easy;
}