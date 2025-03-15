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


    void Start()
    {
        CardsCollectionModel.ContainerACardsCountChanged += OnContainerACardsCountChanged;
        CardsCollectionModel.HandTypeChanged += OnHandTypeChanged;
        cardNodeParent = Find<Transform>("CardNode");
        selectCardNodeParent = Find<Transform>("SelectCardNode");
        originalCardDeckText = Find<Text>("OriginalCardDeck/Text");
        handTypeText = Find<Text>("HandTypeText");
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
            int key = item.Key;
            if (item.Value["HandType"] == handType.ToString())
            {
                handTypeText.text = item.Value["Name"];
                return;
            }
            handTypeText.text = "";
        }
    }
}
