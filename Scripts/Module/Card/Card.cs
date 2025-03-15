using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 卡牌类，处理卡牌的基本属性和视图表现
/// 实现MVC架构中的View层，仅负责UI展示和用户输入捕获
/// </summary>
public class Card : BaseView, IPointerClickHandler
{
    /// <summary>
    /// 卡牌点击事件委托
    /// </summary>
    /// <param name="card">被点击的卡牌</param>
    public delegate void CardClickedHandler(Card card);
    
    /// <summary>
    /// 卡牌点击事件
    /// </summary>
    public static event CardClickedHandler OnCardClicked;
    
    /// <summary>
    /// 容器B对象
    /// </summary>
    private static Transform containerB;
    
    /// <summary>
    /// 卡牌唯一ID
    /// </summary>
    private string uniqueId;
    
    /// <summary>
    /// 设置容器B引用
    /// </summary>
    /// <param name="container">容器B对象</param>
    public static void SetContainerB(Transform container)
    {
        containerB = container;
    }
    
    /// <summary>
    /// 获取容器B引用
    /// </summary>
    public static Transform GetContainerB()
    {
        return containerB;
    }
    
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
    /// 是否可交互
    /// </summary>
    private bool isInteractable = true;
    
    /// <summary>
    /// 初始化卡牌
    /// </summary>
    private void Awake()
    {
        cardImage = GetComponent<Image>();
        
        // 生成唯一ID
        uniqueId = System.Guid.NewGuid().ToString();
        
    }
    
    /// <summary>
    /// 生成卡牌的唯一键名
    /// </summary>
    /// <returns>卡牌的唯一键名，格式为 "Suit_Rank_UniqueID"</returns>
    public string GetCardKey()
    {
        return $"{Suit}_{Rank}_{uniqueId}";
    }
    
    /// <summary>
    /// 设置卡牌的类型、花色和点数
    /// </summary>
    /// <param name="type">卡牌类型</param>
    /// <param name="suit">卡牌花色（仅扑克牌类型有效）</param>
    /// <param name="rank">卡牌点数（仅扑克牌类型有效）</param>
    public void SetCardInfo(CardSuit suit = CardSuit.Spade, CardRank rank = CardRank.Ace)
    {
            Suit = suit;
            Rank = rank;
            
            // 设置扑克牌图片
            UpdateCardImage();
    }
    
    /// <summary>
    /// 设置卡牌是否可交互
    /// </summary>
    /// <param name="interactable">是否可交互</param>
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
    }
    
    /// <summary>
    /// 更新卡牌图片
    /// </summary>
    private void UpdateCardImage()
    {
        if (cardImage == null)
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
        // 获取重叠检测组件
        CardOverlapDetector detector = GetComponent<CardOverlapDetector>();

        // 在点击时立即进行重叠检测
        if (detector != null)
        {
            bool overlapped = detector.IsCardOverlapped();
            if (overlapped)
            {
                // 如果被遮挡，直接返回，不处理点击
                return;
            }
        }

        // 如果卡牌不可交互，忽略点击
        if (!isInteractable)
            return;

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

        // 触发卡牌点击事件，由Controller处理后续业务逻辑
        OnCardClicked?.Invoke(this);
    }

    /// <summary>
    /// 在下一帧更新所有卡牌状态
    /// </summary>
    public void UpdateAllCardsState()
    {
        StartCoroutine(UpdateAllCardsNextFrame());
    }
    
    /// <summary>
    /// 设置卡牌为最顶层
    /// </summary>
    public void SetAsTopMost()
    {
        transform.SetAsLastSibling();
    }
    
    /// <summary>
    /// 将卡牌移动到指定容器，由Controller调用
    /// </summary>
    /// <param name="targetContainer">目标容器</param>
    public void MoveToContainer(Transform targetContainer)
    {
        // 仅处理视图层的移动逻辑
        transform.SetParent(targetContainer, false);
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }
    
    /// <summary>
    /// 在下一帧更新所有卡牌状态
    /// </summary>
    private IEnumerator UpdateAllCardsNextFrame()
    {
        yield return null; // 等待下一帧
        
        //备注一下，这里通过Unity加载层级来找所有卡牌不太合适，应该从当前牌堆的数组里面去找Card对象
        Transform container = transform.parent;
        if (container != null)
        {
            for (int i = 0; i < container.childCount; i++)
            {
                CardOverlapDetector detector = container.GetChild(i).GetComponent<CardOverlapDetector>();
                if (detector != null)
                {
                    bool overlapped = detector.IsCardOverlapped();
                    if (overlapped != detector.IsOverlapped)
                    {
                        detector.SetOverlapped(overlapped);
                    }
                }
            }
        }
    }
} 