using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : BaseView
{
    Transform cardNodeParent;//初始牌存放节点
    Transform selectCardNodeParent;//选择的牌存放节点
    Text originalCardDeckText;//源牌库剩余数量


    void Start()
    {
        cardNodeParent = Find<Transform>("CardNode");
        selectCardNodeParent = Find<Transform>("SelectCardNode");
        originalCardDeckText = Find<Text>("OriginalCardDeck/Text");
        Controller.ApplyControllerFunc((int)ControllerType.Card, Defines.GeneratePokerDecks, 1);
        Controller.ApplyControllerFunc((int)ControllerType.Card, Defines.RandomDealCards, cardNodeParent, selectCardNodeParent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetOriginalCardDeckText(object[] args)
    {
        originalCardDeckText.text = (string)args[0];
    }
}
