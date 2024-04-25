using System.Collections.Generic;
using UnityEngine;
using YG;

public class PlayerPrefsMapProgressManager : IMapProgressManager
{
    private string GetLevelKey(int number)
    {
        return $"Level.{number:000}.StarsCount";
    }

    public int LoadLevelStarsCount(int level)
    {
        return YandexGame.savesData.LevelStars.GetValueOrDefault(GetLevelKey(level), 0);
    }

    public void SaveLevelStarsCount(int level, int starsCount)
    {
        YandexGame.savesData.LevelStars[GetLevelKey(level)] = starsCount;
        YandexGame.SaveProgress();
    }

    public void ClearLevelProgress(int level)
    {
        YandexGame.savesData.LevelStars[GetLevelKey(level)] = 0;
        YandexGame.SaveProgress();
    }
}