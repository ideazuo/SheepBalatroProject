using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : BaseView
{
    Transform cardNodeParent;//初始牌存放节点
    Transform selectCardNodeParent;//选择的牌存放节点
    Text originalCardDeckText;//源牌库剩余数量
    Text handTypeText;//当前手牌类型
    Text scoreText;//当前分数
    Text totalScoreText;//历史最高分
    Button getCardButton;

    //在 PlayerPrefs 中存储总分的键
    private const string TOTAL_SCORE_KEY = "PokerGameTotalScore";


    void Start()
    {
        CardsCollectionModel.ContainerACardsCountChanged += OnContainerACardsCountChanged;
        CardsCollectionModel.HandTypeChanged += OnHandTypeChanged;
        ScoreModel.ScoreChanged += GetScore;
        ScoreModel.TotalScoreChanged += GetTotalScore;
        cardNodeParent = Find<Transform>("CardNode");
        selectCardNodeParent = Find<Transform>("SelectCardNode");
        originalCardDeckText = Find<Text>("OriginalCardDeck/Text");
        handTypeText = Find<Text>("HandTypeText");
        scoreText = Find<Text>("ScoreNode/Text");
        totalScoreText = Find<Text>("TotalScoreNode/Text");

        getCardButton = Find<Button>("GetCardButton");
        getCardButton.onClick.AddListener(GetCard);

        LoadTotalScore();
        Controller.ApplyControllerFunc((int)ControllerType.Card, Defines.GeneratePokerDecks, 1);
        Controller.ApplyControllerFunc((int)ControllerType.Card, Defines.RandomDealCards, cardNodeParent, selectCardNodeParent);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnContainerACardsCountChanged(int count)
    {
        originalCardDeckText.text = $"{count}";
    }

    private void OnHandTypeChanged(PokerHandType handType)
    {
        ConfigData handData = GameApp.ConfigManager.GetConfigData("hand");
        foreach (var item in handData.GetLines())
        {
            if (item.Value["HandType"] == handType.ToString())
            {
                handTypeText.text = item.Value["Name"];
                return;
            }
            handTypeText.text = "";
        }
    }

    private void GetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    private void GetTotalScore(int score)
    {
        totalScoreText.text = score.ToString();
    }

    private void LoadTotalScore()
    {
        // 如果存在保存的分数，则加载它
        if (PlayerPrefs.HasKey(TOTAL_SCORE_KEY))
        {
            totalScoreText.text = PlayerPrefs.GetInt(TOTAL_SCORE_KEY).ToString();
        }
        else
        {
            totalScoreText.text = "0";
        }
    }

    private void GetCard()
    {
        Controller.ApplyControllerFunc((int)ControllerType.Card, Defines.GeneratePokerDecks, 1);
        Controller.ApplyControllerFunc((int)ControllerType.Card, Defines.RandomDealCards, cardNodeParent, selectCardNodeParent);
    }
}
