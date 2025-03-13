using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 卡牌类，处理卡牌的基本属性和行为
/// </summary>
public class Card : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// 卡牌类型
    /// </summary>
    public CardType Type { get; private set; }
    
    /// <summary>
    /// 卡牌花色
    /// </summary>
    public CardSuit Suit { get; private set; }
    
    /// <summary>
    /// 卡牌点数
    /// </summary>
    public CardRank Rank { get; private set; }
    
    /// <summary>
    /// 卡牌图片组件
    /// </summary>
    private Image cardImage;
    
    /// <summary>
    /// 初始化卡牌
    /// </summary>
    private void Awake()
    {
        cardImage = GetComponent<Image>();
        
        // 确保卡牌可以接收射线检测
        if (!gameObject.GetComponent<CanvasRenderer>())
        {
            gameObject.AddComponent<CanvasRenderer>();
        }
        
        // UI 元素需要有 GraphicRaycaster 处理点击
        Canvas rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null && rootCanvas.GetComponent<GraphicRaycaster>() == null)
        {
            rootCanvas.gameObject.AddComponent<GraphicRaycaster>();
            Debug.Log("已添加 GraphicRaycaster 到 Canvas");
        }
    }
    
    /// <summary>
    /// 设置卡牌的类型、花色和点数
    /// </summary>
    /// <param name="type">卡牌类型</param>
    /// <param name="suit">卡牌花色（仅扑克牌类型有效）</param>
    /// <param name="rank">卡牌点数（仅扑克牌类型有效）</param>
    public void SetCardInfo(CardType type, CardSuit suit = CardSuit.Spade, CardRank rank = CardRank.Ace)
    {
        Type = type;
        
        if (type == CardType.Poker)
        {
            Suit = suit;
            Rank = rank;
            
            // 设置扑克牌图片
            UpdateCardImage();
        }
        else
        {
            // 宝箱牌逻辑，预留
        }
    }
    
    /// <summary>
    /// 更新卡牌图片
    /// </summary>
    private void UpdateCardImage()
    {
        if (Type != CardType.Poker || cardImage == null)
            return;
        
        string suitPrefix = "";
        switch (Suit)
        {
            case CardSuit.Club:
                suitPrefix = "c";
                break;
            case CardSuit.Diamond:
                suitPrefix = "d";
                break;
            case CardSuit.Heart:
                suitPrefix = "h";
                break;
            case CardSuit.Spade:
                suitPrefix = "s";
                break;
        }
        
        // 构建图片文件名，如 "c01" 表示梅花A
        string fileName = $"{suitPrefix}{(int)Rank:D2}";
        
        // 加载并设置图片
        Sprite cardSprite = Resources.Load<Sprite>($"Images/{fileName}");
        if (cardSprite != null)
        {
            cardImage.sprite = cardSprite;
        }
        else
        {
            Debug.LogError($"找不到卡牌图片: Images/{fileName}");
        }
    }
    
    /// <summary>
    /// 处理 UI 元素点击
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Type == CardType.Poker)
        {
            string suitName = "";
            switch (Suit)
            {
                case CardSuit.Club:
                    suitName = "梅花";
                    break;
                case CardSuit.Diamond:
                    suitName = "方块";
                    break;
                case CardSuit.Heart:
                    suitName = "红桃";
                    break;
                case CardSuit.Spade:
                    suitName = "黑桃";
                    break;
            }
            
            string rankName = "";
            switch (Rank)
            {
                case CardRank.Ace:
                    rankName = "A";
                    break;
                case CardRank.Jack:
                    rankName = "J";
                    break;
                case CardRank.Queen:
                    rankName = "Q";
                    break;
                case CardRank.King:
                    rankName = "K";
                    break;
                default:
                    rankName = ((int)Rank).ToString();
                    break;
            }
            
            Debug.Log($"点击了{suitName}{rankName}");
        }
    }
} 