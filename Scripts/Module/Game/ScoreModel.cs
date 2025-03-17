using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreModel : BaseModel 
{
    private ConfigData handData;
    private int _score;
    private int _totalScore;

    //在 PlayerPrefs 中存储总分的键
    private const string TOTAL_SCORE_KEY = "PokerGameTotalScore";

    /// <summary>
    /// 分数改变事件
    /// </summary>
    public static event Action<int> ScoreChanged;

    /// <summary>
    /// 历史最高分改变事件
    /// </summary>
    public static event Action<int> TotalScoreChanged;

    public int Score
    {
        get
        {
            return _score;
        }
    }

    public int TotalScore
    {
        get
        {
            return _totalScore;
        }
    }

    public ScoreModel(BaseController crl) : base(crl)
    {
        _score = 0;
        LoadTotalScore();
        CardsCollectionModel.HandCardMax += HandScore;
        CardsCollectionModel.GameOver += GetTotalScore;
        CardsCollectionModel.ContainerANoCardsCount += FirstLevelDown;
    }

    private void LoadTotalScore()
    {
        // 如果存在保存的分数，则加载它
        if (PlayerPrefs.HasKey(TOTAL_SCORE_KEY))
        {
            _totalScore = PlayerPrefs.GetInt(TOTAL_SCORE_KEY);
        }
        else
        {
            _totalScore = 0;
        }
    }

    /// <summary>
    /// 保存历史最高分数到PlayerPrefs
    /// </summary>
    private void SaveTotalScore()
    {
        PlayerPrefs.SetInt(TOTAL_SCORE_KEY, _totalScore);
        PlayerPrefs.Save();
        Debug.Log($"保存历史最高分: {_totalScore}");
    }

    public void HandScore(PokerHandType handType)
    {
        handData = GameApp.ConfigManager.GetConfigData("hand");
        foreach (var item in handData.GetLines())
        {
            if (item.Value["HandType"] == handType.ToString())
            {
                _score += int.Parse(item.Value["Score"]) * int.Parse(item.Value["Multiplier"]);
                ScoreChanged?.Invoke(_score);
                return;
            }
        }
    }

    private void GetTotalScore()
    {
        if (_score > _totalScore)
        {
            _totalScore = _score;
            TotalScoreChanged?.Invoke(_totalScore);
            
            // 保存新的历史最高分
            SaveTotalScore();
        }
    }

    private void FirstLevelDown()
    {
        _score = 0;
    }
}
