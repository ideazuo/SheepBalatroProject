using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 扑克牌手牌类型
/// </summary>
public enum PokerHandType
{
    HighCard,       // 高牌
    OnePair,        // 对子
    TwoPair,        // 两对
    ThreeOfAKind,   // 三条
    Straight,       // 顺子
    Flush,          // 同花
    FullHouse,      // 葫芦
    FourOfAKind,    // 四条
    StraightFlush,  // 同花顺
    FiveOfAKind,    // 五条
    FlushFullHouse, // 同花葫芦
    FlushFiveOfAKind // 同花五条
}
