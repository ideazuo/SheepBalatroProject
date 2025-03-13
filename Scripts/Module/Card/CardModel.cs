using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 卡牌模型类，存储卡牌相关数据
/// </summary>
public class CardModel : BaseModel
{
    /// <summary>
    /// 卡牌列表
    /// </summary>
    public List<Card> Cards { get; private set; } = new List<Card>();
    
    /// <summary>
    /// 卡牌牌组字典
    /// </summary>
    private Dictionary<string, CardInfo> cardDeck = new Dictionary<string, CardInfo>();
    
    /// <summary>
    /// 容器A中的卡牌集合，键为花色和点数组合，值为卡牌对象
    /// </summary>
    private Dictionary<string, Card> containerACards = new Dictionary<string, Card>();
    
    /// <summary>
    /// 容器B中的卡牌集合，键为花色和点数组合，值为卡牌对象，最多5张
    /// </summary>
    private Dictionary<string, Card> containerBCards = new Dictionary<string, Card>();
    
    /// <summary>
    /// 获取容器A中的卡牌集合
    /// </summary>
    public Dictionary<string, Card> ContainerACards => containerACards;
    
    /// <summary>
    /// 获取容器B中的卡牌集合
    /// </summary>
    public Dictionary<string, Card> ContainerBCards => containerBCards;
    
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
            return containerACards.Remove(key);
        }
        return false;
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
            return containerBCards.Remove(key);
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
    }
    
    /// <summary>
    /// 设置卡牌牌组
    /// </summary>
    /// <param name="deck">卡牌牌组字典</param>
    public void SetCardDeck(Dictionary<string, CardInfo> deck)
    {
        cardDeck = deck;
    }
    
    /// <summary>
    /// 获取卡牌牌组
    /// </summary>
    /// <returns>卡牌牌组字典</returns>
    public Dictionary<string, CardInfo> GetCardDeck()
    {
        return cardDeck;
    }
    
    /// <summary>
    /// 清空卡牌牌组
    /// </summary>
    public void ClearCardDeck()
    {
        cardDeck.Clear();
    }
    
    /// <summary>
    /// 添加卡牌到模型
    /// </summary>
    /// <param name="card">要添加的卡牌</param>
    public void AddCard(Card card)
    {
        if (card != null && !Cards.Contains(card))
        {
            Cards.Add(card);
        }
    }
    
    /// <summary>
    /// 从模型中移除卡牌
    /// </summary>
    /// <param name="card">要移除的卡牌</param>
    public void RemoveCard(Card card)
    {
        if (card != null && Cards.Contains(card))
        {
            Cards.Remove(card);
        }
    }
    
    /// <summary>
    /// 清空卡牌列表
    /// </summary>
    public void ClearCards()
    {
        Cards.Clear();
    }
} 