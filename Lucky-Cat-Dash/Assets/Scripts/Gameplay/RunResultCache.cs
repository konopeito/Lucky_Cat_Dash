using UnityEngine;

public static class RunResultCache
{
    public static bool HasResult;

    public static float timeRemaining;
    public static float timeElapsed;
    public static int charmsCollected;
    public static int yenCollected;
    public static int stampsCount;

    public static int finalScore;

    public static void Clear()
    {
        HasResult = false;
        timeRemaining = 0f;
        timeElapsed = 0f;
        charmsCollected = 0;
        yenCollected = 0;
        stampsCount = 0;
        finalScore = 0;
    }

    public static void Set(
        float timeRemainingValue,
        float timeElapsedValue,
        int charmsCollectedValue,
        int yenCollectedValue,
        int stampsCountValue,
        int finalScoreValue)
    {
        HasResult = true;
        timeRemaining = timeRemainingValue;
        timeElapsed = timeElapsedValue;
        charmsCollected = charmsCollectedValue;
        yenCollected = yenCollectedValue;
        stampsCount = stampsCountValue;
        finalScore = finalScoreValue;
    }
}