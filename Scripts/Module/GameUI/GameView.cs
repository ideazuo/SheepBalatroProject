using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameView : BaseView
{
    Transform cardNodeParent;//初始牌存放节点
    Transform selectCardNodeParent;//选择的牌存放节点

    void Start()
    {
        cardNodeParent = Find<Transform>("CardNode"); // 父物体
        Controller.ApplyControllerFunc((int)ControllerType.Card, Defines.CreateCard,
            CardType.Poker, CardSuit.Heart, CardRank.Ten, cardNodeParent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
