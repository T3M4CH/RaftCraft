using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Game.Scripts.Levels.Enums;
using System.Linq;
using UnityEngine;
using System;

public class BiomesSceneManager
{
    public BiomesSceneManager()
    {
        biomesConfig = Resources.Load<BiomesConfig>("LevelsConfig");
        _levelStructs = biomesConfig.Levels.ToDictionary(key => key.Scene.NameScene, value => value.BiomeType);
        _levels = _levelStructs.Select(pair => pair.Key).ToArray();
        CurrentScene = ES3.LoadString(SaveConstants.CurrentScene, CurrentScene);

        if (string.IsNullOrEmpty(CurrentScene))
        {
            CurrentScene = _levels[0];
            ES3.Save(SaveConstants.CurrentScene, CurrentScene);
        }

        Loop = ES3.Load(SaveConstants.GameLoop, 0);
        TriesNumber = ES3.Load(SaveConstants.AttemptsCount, 1);
    }

    private readonly string[] _levels;
    private readonly Dictionary<string, EBiomeType> _levelStructs = new();

    public event Action OnSceneChanged = () => { };

    public void SaveNextLevel()
    {
        var id = LevelId;
        TriesNumber += 1;

        if (id >= _levels.Length - 1)
        {
            id = 0;
            Loop += 1;
            ES3.Save(SaveConstants.GameLoop, Loop);
        }
        else
        {
            id += 1;
        }

        CurrentScene = _levels[id];
        ES3.Save(SaveConstants.CurrentScene, CurrentScene);
        ES3.Save(SaveConstants.AttemptsCount, TriesNumber);
    }

    public void CompleteLevel()
    {
        StartGame();
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(CurrentScene);
    }

    public int Loop { get; private set; }
    public int LevelId => Array.IndexOf(_levels, CurrentScene);

    public int TriesNumber { get; private set; }

    public BiomesConfig biomesConfig { get; private set; }
    public string CurrentScene { get; private set; } = string.Empty;
}