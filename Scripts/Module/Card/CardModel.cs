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