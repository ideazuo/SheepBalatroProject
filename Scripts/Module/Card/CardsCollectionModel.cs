﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 卡牌集合模型类，负责管理卡牌集合和牌型评估
/// 将原CardModel中的容器和评估逻辑抽离，使其更专注于集合管理
/// </summary>
public class CardsCollectionModel : BaseModel
{
    /// <summary>
    /// 容器A中的卡牌集合，键为花色和点数组合，值为卡牌对象
    /// </summary>
    private Dictionary<string, Card> containerACards = new Dictionary<string, Card>();
    
    /// <summary>
    /// 容器B中的卡牌集合，键为花色和点数组合，值为卡牌对象，最多5张
    /// </summary>
    private Dictionary<string, Card> containerBCards = new Dictionary<string, Card>();
    
    /// <summary>
    /// 当前检测到的扑克牌手牌类型
    /// </summary>
    public PokerHandType CurrentPokerHandType { get; private set; } = PokerHandType.HighCard;

    /// <summary>
    /// 手牌类型的改变事件
    /// </summary>
    public static event Action<PokerHandType> HandTypeChanged;

    /// <summary>
    /// 达到5张牌事件
    /// </summary>
    public static event Action<PokerHandType> HandCardMax;

    /// <summary>
    /// 获取容器A中的卡牌集合
    /// </summary>
    public Dictionary<string, Card> ContainerACards => containerACards;

    /// <summary>
    /// 容器A的改变事件
    /// </summary>
    public static event Action<int> ContainerACardsCountChanged;

    /// <summary>
    /// 容器A空牌事件委托
    /// </summary>
    public delegate void ContainerANoCardsHander();

    /// <summary>
    /// 容器A没有牌事件
    /// </summary>
    public static event ContainerANoCardsHander ContainerANoCardsCount;

    /// <summary>
    /// 游戏结束事件委托
    /// </summary>
    public delegate void CGameOverHander();

    /// <summary>
    /// 游戏结束事件
    /// </summary>
    public static event CGameOverHander GameOver;

    /// <summary>
    /// 获取容器B中的卡牌集合
    /// </summary>
    public Dictionary<string, Card> ContainerBCards => containerBCards;

    private LevelModel levelModel;

    /// <summary>
    /// 无参构造函数
    /// </summary>
    public CardsCollectionModel() : base()
    {
        // 默认初始化
    }
    
    /// <summary>
    /// 带控制器参数的构造函数
    /// </summary>
    /// <param name="controller">关联的控制器</param>
    public CardsCollectionModel(BaseController controller) : base(controller)
    {
        // 调用父类的带参数构造函数，并传递控制器引用
        levelModel = new LevelModel();
    }
    

    /// <summary>
    /// 向容器A添加卡牌
    /// </summary>
    /// <param name="key">卡牌键名（花色和点数组合）</param>
    /// <param name="card">卡牌对象</param>
    public void AddCardToContainerA(string key, Card card)
    {
        if (!containerACards.ContainsKey(key))
        {
            containerACards.Add(key, card);
            OnContainerACardsCountChanged();
        }
    }

    /// <summary>
    /// 从容器A移除卡牌
    /// </summary>
    /// <param name="key">卡牌键名（花色和点数组合）</param>
    /// <returns>是否移除成功</returns>
    public bool RemoveCardFromContainerA(string key)
    {
        if (containerACards.ContainsKey(key))
        {
            bool result = containerACards.Remove(key);
            if (result)
            {
                if(containerACards.Count == 0)
                {
                    if(levelModel.LeveNum == 2)
                    {
                        GameOver?.Invoke();
                    }
                    else
                    {
                        ContainerANoCardsCount?.Invoke();
                    }
                    
                }
                OnContainerACardsCountChanged();
            }
            return result;
        }
        return false;
    }

    private void OnContainerACardsCountChanged()
    {
        ContainerACardsCountChanged?.Invoke(containerACards.Count);
    }

    /// <summary>
    /// 向容器B添加卡牌，最多5张
    /// </summary>
    /// <param name="key">卡牌键名（花色和点数组合）</param>
    /// <param name="card">卡牌对象</param>
    /// <returns>是否添加成功</returns>
    public bool AddCardToContainerB(string key, Card card)
    {
        // 检查容器B是否已满（最多5张卡牌）
        if (containerBCards.Count >= 5)
        {
            Debug.LogWarning("容器B已达到上限（5张卡牌）");
            return false;
        }
        
        if (!containerBCards.ContainsKey(key))
        {
            containerBCards.Add(key, card);
            
            // 当容器B中有卡牌时，评估当前的牌型
            if (containerBCards.Count >= 1)
            {
                EvaluatePokerHand();
            }
            
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 从容器B移除卡牌
    /// </summary>
    /// <param name="key">卡牌键名（花色和点数组合）</param>
    /// <returns>是否移除成功</returns>
    public bool RemoveCardFromContainerB(string key)
    {
        if (containerBCards.ContainsKey(key))
        {
            bool result = containerBCards.Remove(key);
            
            // 当容器B中有卡牌时，评估当前的牌型
            if (containerBCards.Count >= 1)
            {
                EvaluatePokerHand();
            }
            else
            {
                // 如果没有卡牌，重置为高牌
                CurrentPokerHandType = PokerHandType.Null;
            }
            
            return result;
        }
        return false;
    }
    
    /// <summary>
    /// 清空容器A中的卡牌
    /// </summary>
    public void ClearContainerA()
    {
        containerACards.Clear();
    }
    
    /// <summary>
    /// 清空容器B中的卡牌
    /// </summary>
    public void ClearContainerB()
    {
        containerBCards.Clear();
        // 重置为高牌
        CurrentPokerHandType = PokerHandType.Null;
    }
    
    /// <summary>
    /// 清空容器B中的卡牌并销毁对应的游戏对象
    /// </summary>
    public void ClearContainerBWithDestroy()
    {
        // 存储所有需要销毁的卡牌对象
        List<Card> cardsToDestroy = new List<Card>(containerBCards.Values);
        
        // 先清空容器B字典
        containerBCards.Clear();
        
        // 重置为高牌
        CurrentPokerHandType = PokerHandType.Null;
        HandTypeChanged?.Invoke(CurrentPokerHandType);

        // 销毁所有卡牌对象
        foreach (Card card in cardsToDestroy)
        {
            if (card != null && card.gameObject != null)
            {
                GameObject.Destroy(card.gameObject);
            }
        }
        
        Debug.Log("容器B中的卡牌已被移除并销毁");
    }
    
    /// <summary>
    /// 评估容器B中的卡牌组合，判断扑克牌手牌类型
    /// </summary>
    /// <returns>检测到的扑克牌手牌类型</returns>
    public PokerHandType EvaluatePokerHand()
    {
        // 如果容器B中没有卡牌，返回Null
        if (containerBCards.Count == 0)
        {
            CurrentPokerHandType = PokerHandType.Null;
            return CurrentPokerHandType;
        }
        
        // 收集容器B中的所有卡牌信息
        List<Card> cards = new List<Card>(containerBCards.Values);
        
        // 按照规则从高到低检查各种牌型
        
        // 检查是否为同花五条（5张牌花色和点数都相同）
        if (IsFlushFiveOfAKind(cards))
        {
            CurrentPokerHandType = PokerHandType.FlushFiveOfAKind;
            Debug.Log("同花五条");
        }
        // 检查是否为同花葫芦（5张牌花色相同，3张点数相同，另2张点数相同）
        else if (IsFlushFullHouse(cards))
        {
            CurrentPokerHandType = PokerHandType.FlushFullHouse;
            Debug.Log("同花葫芦");
        }
        // 检查是否为五条（5张牌点数相同）
        else if (IsFiveOfAKind(cards))
        {
            CurrentPokerHandType = PokerHandType.FiveOfAKind;
            Debug.Log("五条");
        }
        // 检查是否为同花顺（5张牌花色相同且点数连续）
        else if (IsStraightFlush(cards))
        {
            CurrentPokerHandType = PokerHandType.StraightFlush;
            Debug.Log("同花顺");
        }
        // 检查是否为四条（4张牌点数相同）
        else if (IsFourOfAKind(cards))
        {
            CurrentPokerHandType = PokerHandType.FourOfAKind;
            Debug.Log("四条");
        }
        // 检查是否为葫芦（3张牌点数相同，另2张点数相同）
        else if (IsFullHouse(cards))
        {
            CurrentPokerHandType = PokerHandType.FullHouse;
            Debug.Log("葫芦");
        }
        // 检查是否为同花（5张牌花色相同）
        else if (IsFlush(cards))
        {
            CurrentPokerHandType = PokerHandType.Flush;
            Debug.Log("同花");
        }
        // 检查是否为顺子（5张牌点数连续）
        else if (IsStraight(cards))
        {
            CurrentPokerHandType = PokerHandType.Straight;
            Debug.Log("顺子");
        }
        // 检查是否为三条（3张牌点数相同）
        else if (IsThreeOfAKind(cards))
        {
            CurrentPokerHandType = PokerHandType.ThreeOfAKind;
            Debug.Log("三条");
        }
        // 检查是否为两对（2对不同点数的牌）
        else if (IsTwoPair(cards))
        {
            CurrentPokerHandType = PokerHandType.TwoPair;
            Debug.Log("两对");
        }
        // 检查是否为对子（2张牌点数相同）
        else if (IsOnePair(cards))
        {
            CurrentPokerHandType = PokerHandType.OnePair;
            Debug.Log("对子");
        }
        // 如果以上都不是，则为高牌
        else
        {
            CurrentPokerHandType = PokerHandType.HighCard;
            Debug.Log("高牌");
        }

        
        // 检查容器B是否已满5张牌，如果是，则计算分数然后调用控制器进行延迟清理
        if (containerBCards.Count >= 5)
        {
            HandCardMax?.Invoke(CurrentPokerHandType);
            // 延迟清理容器B中的卡牌，让玩家有时间看到结果
            GameApp.ControllerManager.ApplyFunc((int)ControllerType.Card, Defines.ClearContainerBWithDelay, new object[0]);
        }

        HandTypeChanged?.Invoke(CurrentPokerHandType);
        return CurrentPokerHandType;
    }
    
    /// <summary>
    /// 检查是否为同花五条（5张牌花色和点数都相同）
    /// </summary>
    private bool IsFlushFiveOfAKind(List<Card> cards)
    {
        if (cards.Count < 5) return false;
        
        // 如果是5张牌，检查是否所有牌的花色和点数都相同
        CardSuit firstSuit = cards[0].Suit;
        CardRank firstRank = cards[0].Rank;
        
        foreach (Card card in cards)
        {
            if (card.Suit != firstSuit || card.Rank != firstRank)
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 检查是否为同花葫芦（5张牌花色相同，3张点数相同，另2张点数相同）
    /// </summary>
    private bool IsFlushFullHouse(List<Card> cards)
    {
        if (cards.Count != 5) return false;
        
        // 先检查是否为同花
        CardSuit firstSuit = cards[0].Suit;
        bool isFlush = cards.All(card => card.Suit == firstSuit);
        
        if (!isFlush) return false;
        
        // 再检查是否为葫芦结构
        // 统计每个点数出现的次数
        Dictionary<CardRank, int> rankCounts = new Dictionary<CardRank, int>();
        foreach (Card card in cards)
        {
            if (!rankCounts.ContainsKey(card.Rank))
                rankCounts[card.Rank] = 0;
            rankCounts[card.Rank]++;
        }
        
        // 葫芦结构必须有且仅有两种点数，一种出现3次，另一种出现2次
        return rankCounts.Count == 2 && rankCounts.Values.Contains(3) && rankCounts.Values.Contains(2);
    }
    
    /// <summary>
    /// 检查是否为五条（5张牌点数相同）
    /// </summary>
    private bool IsFiveOfAKind(List<Card> cards)
    {
        if (cards.Count < 5) return false;
        
        // 检查所有牌的点数是否相同
        CardRank firstRank = cards[0].Rank;
        return cards.All(card => card.Rank == firstRank);
    }
    
    /// <summary>
    /// 检查是否为同花顺（5张牌花色相同且点数连续）
    /// </summary>
    private bool IsStraightFlush(List<Card> cards)
    {
        if (cards.Count != 5) return false;
        
        // 同时满足同花和顺子的条件
        return IsFlush(cards) && IsStraight(cards);
    }
    
    /// <summary>
    /// 检查是否为四条（4张牌点数相同）
    /// </summary>
    private bool IsFourOfAKind(List<Card> cards)
    {
        if (cards.Count < 4) return false;
        
        // 统计每个点数出现的次数
        Dictionary<CardRank, int> rankCounts = new Dictionary<CardRank, int>();
        foreach (Card card in cards)
        {
            if (!rankCounts.ContainsKey(card.Rank))
                rankCounts[card.Rank] = 0;
            rankCounts[card.Rank]++;
        }
        
        // 如果有任何一个点数出现了4次或更多，则为四条
        return rankCounts.Values.Any(count => count >= 4);
    }
    
    /// <summary>
    /// 检查是否为葫芦（3张牌点数相同，另2张点数相同）
    /// </summary>
    private bool IsFullHouse(List<Card> cards)
    {
        if (cards.Count != 5) return false;
        
        // 统计每个点数出现的次数
        Dictionary<CardRank, int> rankCounts = new Dictionary<CardRank, int>();
        foreach (Card card in cards)
        {
            if (!rankCounts.ContainsKey(card.Rank))
                rankCounts[card.Rank] = 0;
            rankCounts[card.Rank]++;
        }
        
        // 葫芦结构必须有且仅有两种点数，一种出现3次，另一种出现2次
        return rankCounts.Count == 2 && rankCounts.Values.Contains(3) && rankCounts.Values.Contains(2);
    }
    
    /// <summary>
    /// 检查是否为同花（5张牌花色相同）
    /// </summary>
    private bool IsFlush(List<Card> cards)
    {
        if (cards.Count < 5) return false;
        
        // 检查所有牌的花色是否相同
        CardSuit firstSuit = cards[0].Suit;
        return cards.All(card => card.Suit == firstSuit);
    }
    
    /// <summary>
    /// 检查是否为顺子（5张牌点数连续）
    /// </summary>
    private bool IsStraight(List<Card> cards)
    {
        if (cards.Count != 5) return false;
        
        // 获取所有牌的点数，并排序
        List<int> ranks = cards.Select(card => (int)card.Rank).OrderBy(r => r).ToList();
        
        // 特殊情况：A-2-3-4-5 顺子，A的点数为14，但在这种情况下当作1
        bool isLowStraight = ranks.Contains((int)CardRank.Ace) && 
                             ranks.Contains((int)CardRank.Two) && 
                             ranks.Contains((int)CardRank.Three) && 
                             ranks.Contains((int)CardRank.Four) && 
                             ranks.Contains((int)CardRank.Five);
        
        if (isLowStraight) return true;
        
        // 特殊情况：10-J-Q-K-A 顺子
        bool isHighStraight = ranks.Contains((int)CardRank.Ten) && 
                              ranks.Contains((int)CardRank.Jack) && 
                              ranks.Contains((int)CardRank.Queen) && 
                              ranks.Contains((int)CardRank.King) && 
                              ranks.Contains((int)CardRank.Ace);
        
        if (isHighStraight) return true;
        
        // 一般情况：检查排序后的点数是否连续
        for (int i = 1; i < ranks.Count; i++)
        {
            if (ranks[i] != ranks[i - 1] + 1)
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 检查是否为三条（3张牌点数相同）
    /// </summary>
    private bool IsThreeOfAKind(List<Card> cards)
    {
        if (cards.Count < 3) return false;
        
        // 统计每个点数出现的次数
        Dictionary<CardRank, int> rankCounts = new Dictionary<CardRank, int>();
        foreach (Card card in cards)
        {
            if (!rankCounts.ContainsKey(card.Rank))
                rankCounts[card.Rank] = 0;
            rankCounts[card.Rank]++;
        }
        
        // 如果有任何一个点数出现了3次或更多，则为三条
        return rankCounts.Values.Any(count => count >= 3);
    }
    
    /// <summary>
    /// 检查是否为两对（2对不同点数的牌）
    /// </summary>
    private bool IsTwoPair(List<Card> cards)
    {
        if (cards.Count < 4) return false;
        
        // 统计每个点数出现的次数
        Dictionary<CardRank, int> rankCounts = new Dictionary<CardRank, int>();
        foreach (Card card in cards)
        {
            if (!rankCounts.ContainsKey(card.Rank))
                rankCounts[card.Rank] = 0;
            rankCounts[card.Rank]++;
        }
        
        // 计算有多少对子
        int pairCount = rankCounts.Values.Count(count => count >= 2);
        
        // 如果有2对或更多，则为两对
        return pairCount >= 2;
    }
    
    /// <summary>
    /// 检查是否为对子（2张牌点数相同）
    /// </summary>
    private bool IsOnePair(List<Card> cards)
    {
        if (cards.Count < 2) return false;
        
        // 统计每个点数出现的次数
        Dictionary<CardRank, int> rankCounts = new Dictionary<CardRank, int>();
        foreach (Card card in cards)
        {
            if (!rankCounts.ContainsKey(card.Rank))
                rankCounts[card.Rank] = 0;
            rankCounts[card.Rank]++;
        }
        
        // 如果有任何一个点数出现了2次或更多，则为对子
        return rankCounts.Values.Any(count => count >= 2);
    }
} 