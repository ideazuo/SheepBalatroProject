using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 扑克牌花色枚举
/// </summary>
public enum CardSuit
{
    /// <summary>
    /// 黑桃
    /// </summary>
    Spade,

    /// <summary>
    /// 红桃
    /// </summary>
    Heart,

    /// <summary>
    /// 梅花
    /// </summary>
    Club,

    /// <summary>
    /// 方块
    /// </summary>
    Diamond
}

/// <summary>
/// 扑克牌点数枚举
/// </summary>
public enum CardRank
{
    /// <summary>
    /// A
    /// </summary>
    Ace = 1,

    /// <summary>
    /// 2
    /// </summary>
    Two = 2,

    /// <summary>
    /// 3
    /// </summary>
    Three = 3,

    /// <summary>
    /// 4
    /// </summary>
    Four = 4,

    /// <summary>
    /// 5
    /// </summary>
    Five = 5,

    /// <summary>
    /// 6
    /// </summary>
    Six = 6,

    /// <summary>
    /// 7
    /// </summary>
    Seven = 7,

    /// <summary>
    /// 8
    /// </summary>
    Eight = 8,

    /// <summary>
    /// 9
    /// </summary>
    Nine = 9,

    /// <summary>
    /// 10
    /// </summary>
    Ten = 10,

    /// <summary>
    /// J
    /// </summary>
    Jack = 11,

    /// <summary>
    /// Q
    /// </summary>
    Queen = 12,

    /// <summary>
    /// K
    /// </summary>
    King = 13
}

/// <summary>
/// 卡牌模型类，存储单张卡牌的数据
/// </summary>
public class CardModel : BaseModel
{
    // 卡牌模型现在仅处理单张卡牌数据，如需存储状态或数据
} 