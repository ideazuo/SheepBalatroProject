using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 卡牌控制器，负责创建和管理卡牌
/// </summary>
public class CardController : BaseController
{
    /// <summary>
    /// 卡牌模型
    /// </summary>
    private CardModel cardModel;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public CardController() : base()
    {
        // 初始化模型
        cardModel = new CardModel();
        SetModel(cardModel);
    }
    
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        base.Init();
        InitModuleEvent();
    }
    
    /// <summary>
    /// 初始化模块事件
    /// </summary>
    public override void InitModuleEvent()
    {
        base.InitModuleEvent();
        RegisterFunc(Defines.CreateCard, CreateCard);
        RegisterFunc(Defines.GeneratePokerDecks, GeneratePokerDecks);
        RegisterFunc(Defines.RandomDealCards, RandomDealCards);
        RegisterFunc(Defines.SetContainerB, SetContainerB);
    }
    
    /// <summary>
    /// 创建卡牌
    /// </summary>
    /// <param name="args">
    /// args[0]: CardType - 卡牌类型
    /// args[1]: CardSuit - 卡牌花色（扑克牌类型时有效）
    /// args[2]: CardRank - 卡牌点数（扑克牌类型时有效）
    /// args[3]: Transform - 父物体
    /// </param>
    private void CreateCard(object[] args)
    {
        if (args.Length < 4)
        {
            Debug.LogError("CreateCard参数不足");
            return;
        }
        
        CardType cardType = (CardType)args[0];
        Transform parent = (Transform)args[3];
        
        // 加载Card预制体
        GameObject cardPrefab = Resources.Load<GameObject>("Models/Card");
        if (cardPrefab == null)
        {
            Debug.LogError("找不到Card预制体：Resources/Models/Card");
            return;
        }
        
        // 创建卡牌实例
        GameObject cardObj = GameObject.Instantiate(cardPrefab, parent);
        if (cardObj == null)
        {
            Debug.LogError("创建Card预制体失败");
            return;
        }
        
        // 确保卡牌有 Image 组件
        Image cardImage = cardObj.GetComponent<Image>();
        if (cardImage == null)
        {
            cardImage = cardObj.AddComponent<Image>();
            Debug.Log("已添加 Image 组件到卡牌");
        }
        
        // 添加Card组件
        Card card = cardObj.GetComponent<Card>();
        if (card == null)
        {
            card = cardObj.AddComponent<Card>();
        }
        
        // 根据卡牌类型设置信息
        if (cardType == CardType.Poker && args.Length >= 3)
        {
            CardSuit suit = (CardSuit)args[1];
            CardRank rank = (CardRank)args[2];
            card.SetCardInfo(cardType, suit, rank);
        }
        else
        {
            card.SetCardInfo(cardType);
        }
        
        // 将卡牌添加到模型
        cardModel.AddCard(card);
    }

    /// <summary>
    /// 生成指定副数的扑克牌并存储在字典中
    /// </summary>
    /// <param name="args">
    /// args[0]: int - 扑克牌副数
    /// </param>
    /// <returns>Dictionary<string, CardInfo> - 扑克牌集合，键为花色和点数的组合</returns>
    private void GeneratePokerDecks(object[] args)
    {
        if (args.Length < 1)
        {
            Debug.LogError("GeneratePokerDecks参数不足");
            return;
        }

        int deckCount = (int)args[0];
        Dictionary<string, CardInfo> cardDeck = new Dictionary<string, CardInfo>();

        // 生成指定副数的扑克牌
        for (int deck = 0; deck < deckCount; deck++)
        {
            foreach (CardSuit suit in System.Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardRank rank in System.Enum.GetValues(typeof(CardRank)))
                {
                    // 生成卡牌唯一键名，例如: "Spade_Ace_1" 表示第1副牌的黑桃A
                    string cardKey = $"{suit}_{rank}_{deck}";
                    
                    // 创建卡牌信息对象
                    CardInfo cardInfo = new CardInfo
                    {
                        Type = CardType.Poker,
                        Suit = suit,
                        Rank = rank
                    };
                    
                    // 添加到字典
                    cardDeck.Add(cardKey, cardInfo);
                }
            }
        }

        // 将生成的牌组添加到模型中
        cardModel.SetCardDeck(cardDeck);
        
        Debug.Log($"成功生成 {deckCount} 副扑克牌，共 {cardDeck.Count} 张");
    }
    
    /// <summary>
    /// 随机从牌堆中抽取卡牌并克隆到指定区域
    /// </summary>
    /// <param name="args">
    /// args[0]: Transform - 放置卡牌的容器A
    /// args[1]: Transform - (可选) 容器B，用于移动选中的卡牌
    /// </param>
    private void RandomDealCards(object[] args)
    {
        if (args.Length < 1 || !(args[0] is Transform))
        {
            Debug.LogError("RandomDealCards参数不足或类型错误");
            return;
        }
        
        Transform containerA = (Transform)args[0];
        
        // 如果提供了容器B，设置容器B引用
        if (args.Length > 1 && args[1] is Transform containerB)
        {
            Card.SetContainerB(containerB);
        }
        
        Dictionary<string, CardInfo> cardDeck = cardModel.GetCardDeck();
        
        if (cardDeck == null || cardDeck.Count == 0)
        {
            Debug.LogError("卡牌集合为空，请先调用GeneratePokerDecks生成卡牌");
            return;
        }
        
        // 清空容器现有卡牌
        for (int i = containerA.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(containerA.GetChild(i).gameObject);
        }
        
        // 清空集合A中的卡牌
        cardModel.ClearContainerA();
        
        // 加载Card预制体
        GameObject cardPrefab = Resources.Load<GameObject>("Models/Card");
        if (cardPrefab == null)
        {
            Debug.LogError("找不到Card预制体：Resources/Models/Card");
            return;
        }
        
        // 获取容器的尺寸
        RectTransform containerRect = containerA as RectTransform;
        if (containerRect == null)
        {
            Debug.LogError("容器必须有RectTransform组件");
            return;
        }
        
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;
        
        // 获取卡牌预制体的尺寸
        RectTransform cardRectTemp = cardPrefab.GetComponent<RectTransform>();
        float cardWidth = cardRectTemp.rect.width;
        float cardHeight = cardRectTemp.rect.height;
        
        // 克隆所有卡牌
        List<string> cardKeys = cardDeck.Keys.ToList();
        System.Random random = new System.Random();
        
        while (cardKeys.Count > 0)
        {
            // 随机抽取一张牌
            int randomIndex = random.Next(cardKeys.Count);
            string cardKey = cardKeys[randomIndex];
            CardInfo cardInfo = cardDeck[cardKey];
            cardKeys.RemoveAt(randomIndex);
            
            // 创建卡牌实例
            GameObject cardObj = GameObject.Instantiate(cardPrefab, containerA);
            
            // 设置随机位置（确保卡牌不超出容器范围）
            RectTransform cardRect = cardObj.GetComponent<RectTransform>();
            float maxX = (containerWidth - cardWidth) / 2;
            float maxY = (containerHeight - cardHeight) / 2;
            float randomX = Random.Range(-maxX, maxX);
            float randomY = Random.Range(-maxY, maxY);
            
            cardRect.anchoredPosition = new Vector2(randomX, randomY);
            
            // 添加Card组件
            Card card = cardObj.GetComponent<Card>();
            if (card == null)
            {
                card = cardObj.AddComponent<Card>();
            }
            
            // 设置卡牌信息
            card.SetCardInfo(cardInfo.Type, cardInfo.Suit, cardInfo.Rank);
            
            // 为卡牌添加重叠检测组件
            CardOverlapDetector overlapDetector = cardObj.AddComponent<CardOverlapDetector>();
            overlapDetector.Initialize(containerA);
            
            // 将卡牌添加到集合A中
            string uniqueKey = card.GetCardKey();
            cardModel.AddCardToContainerA(uniqueKey, card);
        }
        
        // 从字典中移除所有卡牌
        cardModel.ClearCardDeck();
        
        Debug.Log($"成功发放所有卡牌到容器A并添加到集合A中");
    }
    
    /// <summary>
    /// 设置容器B，用于接收被移动的卡牌
    /// </summary>
    /// <param name="args">
    /// args[0]: Transform - 容器B对象
    /// </param>
    private void SetContainerB(object[] args)
    {
        if (args.Length < 1 || !(args[0] is Transform))
        {
            Debug.LogError("SetContainerB参数不足或类型错误");
            return;
        }
        
        Transform containerB = (Transform)args[0];
        Card.SetContainerB(containerB);
        
        Debug.Log("已设置容器B引用");
    }
}

/// <summary>
/// 卡牌信息类，用于存储卡牌的基本信息
/// </summary>
public class CardInfo
{
    public CardType Type;
    public CardSuit Suit;
    public CardRank Rank;
} 