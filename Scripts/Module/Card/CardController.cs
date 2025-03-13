using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 卡牌控制器，负责创建和管理卡牌，处理卡牌相关的交互逻辑
/// 实现了MVC架构中的Controller层，负责协调Model和View层之间的通信
/// </summary>
public class CardController : BaseController
{
    /// <summary>
    /// 卡牌模型，存储卡牌数据和相关业务逻辑
    /// </summary>
    private CardModel cardModel;
    
    /// <summary>
    /// 构造函数，初始化卡牌控制器
    /// 创建卡牌模型并设置关联
    /// </summary>
    public CardController() : base()
    {
        // 初始化卡牌模型
        cardModel = new CardModel();
        SetModel(cardModel);
    }
    
    /// <summary>
    /// 初始化控制器，调用基类初始化方法并注册模块事件
    /// </summary>
    public override void Init()
    {
        base.Init();
        InitModuleEvent();
        
        // 订阅卡牌点击事件
        Card.OnCardClicked += HandleCardClicked;
        
        // 订阅卡牌重叠状态变化事件
        CardOverlapDetector.OnOverlapStateChanged += HandleCardOverlapStateChanged;
    }
    
    /// <summary>
    /// 释放控制器资源
    /// </summary>
    public override void Destroy()
    {
        // 取消订阅卡牌点击事件
        Card.OnCardClicked -= HandleCardClicked;
        
        // 取消订阅卡牌重叠状态变化事件
        CardOverlapDetector.OnOverlapStateChanged -= HandleCardOverlapStateChanged;
        
        base.Destroy();
    }
    
    /// <summary>
    /// 处理卡牌点击事件
    /// </summary>
    /// <param name="card">被点击的卡牌</param>
    private void HandleCardClicked(Card card)
    {
        // 获取容器B
        Transform containerB = Card.GetContainerB();
        
        // 如果容器B已设置，尝试移动卡牌到容器B
        if (containerB != null)
        {
            MoveCardToContainerB(card);
        }
        else
        {
            // 否则，将卡牌设置为最顶层
            card.SetAsTopMost();
            
            // 更新所有卡牌的重叠状态
            card.UpdateAllCardsState();
        }
    }
    
    /// <summary>
    /// 将卡牌从容器A移动到容器B
    /// </summary>
    /// <param name="card">要移动的卡牌</param>
    private void MoveCardToContainerB(Card card)
    {
        // 获取卡牌键
        string cardKey = card.GetCardKey();
        
        // 检查容器B是否已满
        if (cardModel.ContainerBCards.Count >= 5)
        {
            Debug.LogWarning("容器B已满，最多只能放置5张卡牌");
            return;
        }
        
        // 从容器A移除卡牌
        if (cardModel.RemoveCardFromContainerA(cardKey))
        {
            // 获取容器B引用
            Transform containerB = Card.GetContainerB();
            
            // 更新视图层：移动卡牌到容器B
            card.MoveToContainer(containerB);
            
            // 更新模型层：将卡牌添加到容器B的集合中
            cardModel.AddCardToContainerB(cardKey, card);
            
            Debug.Log($"卡牌 {cardKey} 已从容器A移动到容器B");
        }
        else
        {
            Debug.LogWarning($"卡牌 {cardKey} 不在容器A中，无法移动");
        }
    }
    
    /// <summary>
    /// 初始化模块事件，注册所有卡牌相关功能的回调函数
    /// </summary>
    public override void InitModuleEvent()
    {
        base.InitModuleEvent();
        // 注册各种卡牌操作的回调函数
        RegisterFunc(Defines.CreateCard, CreateCard);
        RegisterFunc(Defines.GeneratePokerDecks, GeneratePokerDecks);
        RegisterFunc(Defines.RandomDealCards, RandomDealCards);
        RegisterFunc(Defines.SetContainerB, SetContainerB);
        RegisterFunc(Defines.EvaluatePokerHand, EvaluatePokerHand);
        RegisterFunc(Defines.ClearContainerBWithDelay, ClearContainerBWithDelay);
    }
    
    /// <summary>
    /// 创建卡牌实例并初始化其属性
    /// </summary>
    /// <param name="args">
    /// args[0]: CardType - 卡牌类型
    /// args[1]: CardSuit - 卡牌花色（扑克牌类型时有效）
    /// args[2]: CardRank - 卡牌点数（扑克牌类型时有效）
    /// args[3]: Transform - 父物体
    /// </param>
    /// <remarks>
    /// 该方法加载卡牌预制体并实例化，设置卡牌属性，并将其添加到模型中
    /// </remarks>
    private void CreateCard(object[] args)
    {
        // 检查参数
        if (args.Length < 4)
        {
            Debug.LogError("CreateCard参数不足");
            return;
        }
        
        CardType cardType = (CardType)args[0];
        Transform parent = (Transform)args[3];
        
        // 加载卡牌预制体
        GameObject cardPrefab = Resources.Load<GameObject>("Models/Card");
        if (cardPrefab == null)
        {
            Debug.LogError("找不到Card预制体：Resources/Models/Card");
            return;
        }
        
        // 实例化卡牌对象
        GameObject cardObj = GameObject.Instantiate(cardPrefab, parent);
        if (cardObj == null)
        {
            Debug.LogError("创建Card预制体失败");
            return;
        }
        
        // 确保卡牌有Image组件用于显示
        Image cardImage = cardObj.GetComponent<Image>();
        if (cardImage == null)
        {
            cardImage = cardObj.AddComponent<Image>();
            Debug.Log("已添加 Image 组件到卡牌");
        }
        
        // 获取或添加Card组件
        Card card = cardObj.GetComponent<Card>();
        if (card == null)
        {
            card = cardObj.AddComponent<Card>();
        }
        
        // 根据卡牌类型设置属性
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
        
        // 将创建的卡牌添加到模型中
        cardModel.AddCard(card);
    }

    /// <summary>
    /// 生成指定副数的扑克牌并存储在字典中
    /// </summary>
    /// <param name="args">
    /// args[0]: int - 扑克牌副数
    /// </param>
    /// <remarks>
    /// 该方法根据指定的副数生成完整的扑克牌组，每副包含所有花色和点数的组合
    /// 生成的卡牌信息存储在卡牌模型中，供后续使用
    /// </remarks>
    private void GeneratePokerDecks(object[] args)
    {
        // 检查参数
        if (args.Length < 1)
        {
            Debug.LogError("GeneratePokerDecks参数不足");
            return;
        }

        int deckCount = (int)args[0];
        Dictionary<string, CardInfo> cardDeck = new Dictionary<string, CardInfo>();

        // 为每副牌生成所有花色和点数的组合
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
    /// <remarks>
    /// 该方法清空指定容器中的现有卡牌，然后随机抽取牌堆中的卡牌实例化到容器中
    /// 同时为每张卡牌添加重叠检测组件，并将其添加到集合A中
    /// </remarks>
    private void RandomDealCards(object[] args)
    {
        // 检查参数
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
        
        // 获取卡牌集合
        Dictionary<string, CardInfo> cardDeck = cardModel.GetCardDeck();
        
        // 检查卡牌集合是否为空
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
        
        // 获取容器的尺寸信息
        RectTransform containerRect = containerA as RectTransform;
        if (containerRect == null)
        {
            Debug.LogError("容器必须有RectTransform组件");
            return;
        }
        
        // 计算容器的宽高
        float containerWidth = containerRect.rect.width;
        float containerHeight = containerRect.rect.height;
        
        // 获取卡牌预制体的尺寸
        RectTransform cardRectTemp = cardPrefab.GetComponent<RectTransform>();
        float cardWidth = cardRectTemp.rect.width;
        float cardHeight = cardRectTemp.rect.height;
        
        // 准备随机抽取卡牌
        List<string> cardKeys = cardDeck.Keys.ToList();
        System.Random random = new System.Random();
        
        // 逐一创建所有卡牌
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
            
            // 获取或添加Card组件
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
        
        // 清空临时卡牌集合
        cardModel.ClearCardDeck();
        
        Debug.Log($"成功发放所有卡牌到容器A并添加到集合A中");
    }
    
    /// <summary>
    /// 设置容器B，用于接收被移动的卡牌
    /// </summary>
    /// <param name="args">
    /// args[0]: Transform - 容器B对象
    /// </param>
    /// <remarks>
    /// 该方法设置用于接收用户选择的卡牌的容器B的引用
    /// </remarks>
    private void SetContainerB(object[] args)
    {
        // 检查参数
        if (args.Length < 1 || !(args[0] is Transform))
        {
            Debug.LogError("SetContainerB参数不足或类型错误");
            return;
        }
        
        Transform containerB = (Transform)args[0];
        Card.SetContainerB(containerB);
        
        Debug.Log("已设置容器B引用");
    }

    /// <summary>
    /// 评估容器B中的扑克牌牌型
    /// </summary>
    /// <param name="args">不需要参数</param>
    /// <remarks>
    /// 该方法评估容器B中卡牌的牌型（如对子、顺子等）
    /// 如果容器B中没有卡牌，不会输出任何信息
    /// </remarks>
    private void EvaluatePokerHand(object[] args)
    {
        // 评估扑克牌型
        PokerHandType handType = cardModel.EvaluatePokerHand();
        
        // 当容器B中没有牌时，不输出任何信息
        if (cardModel.ContainerBCards.Count == 0)
        {
            return;
        }
        
        // 将牌型枚举转换为对应的中文名称
        string handName = "";
        switch (handType)
        {
            case PokerHandType.HighCard:
                handName = "高牌";
                break;
            case PokerHandType.OnePair:
                handName = "对子";
                break;
            case PokerHandType.TwoPair:
                handName = "两对";
                break;
            case PokerHandType.ThreeOfAKind:
                handName = "三条";
                break;
            case PokerHandType.Straight:
                handName = "顺子";
                break;
            case PokerHandType.Flush:
                handName = "同花";
                break;
            case PokerHandType.FullHouse:
                handName = "葫芦";
                break;
            case PokerHandType.FourOfAKind:
                handName = "四条";
                break;
            case PokerHandType.StraightFlush:
                handName = "同花顺";
                break;
            case PokerHandType.FiveOfAKind:
                handName = "五条";
                break;
            case PokerHandType.FlushFullHouse:
                handName = "同花葫芦";
                break;
            case PokerHandType.FlushFiveOfAKind:
                handName = "同花五条";
                break;
        }
        
        Debug.Log($"当前牌型: {handName}");
    }

    /// <summary>
    /// 执行延迟清理容器B的逻辑
    /// </summary>
    /// <param name="args">
    /// args[0]: float - (可选) 延迟时间（秒），默认为1秒
    /// </param>
    /// <remarks>
    /// 该方法在指定的延迟时间后清空并销毁容器B中的所有卡牌
    /// 通过CoroutineHelper实现非MonoBehaviour类中的延迟操作
    /// </remarks>
    private void ClearContainerBWithDelay(object[] args)
    {
        Debug.Log("执行延迟清理容器B的逻辑");
        
        // 设置默认延迟时间为1秒，可通过参数覆盖
        float delay = 1f;
        if (args.Length > 0 && args[0] is float)
        {
            delay = (float)args[0];
        }
        
        // 使用CoroutineHelper实现延迟操作
        CoroutineHelper.instance.DelayedAction(delay, () => {
            // 获取当前的牌型信息（用于日志）
            PokerHandType handType = cardModel.CurrentPokerHandType;
            string handName = GetPokerHandTypeName(handType);
            
            // 清理并销毁容器B中的卡牌
            cardModel.ClearContainerBWithDestroy();
            
            Debug.Log($"已完成牌型'{handName}'的评估，容器B中的卡牌已被移除并销毁");
        });
    }
    
    /// <summary>
    /// 获取扑克牌型的中文名称
    /// </summary>
    /// <param name="handType">牌型枚举值</param>
    /// <returns>牌型的中文名称</returns>
    /// <remarks>
    /// 将PokerHandType枚举值转换为对应的中文描述
    /// </remarks>
    private string GetPokerHandTypeName(PokerHandType handType)
    {
        switch (handType)
        {
            case PokerHandType.HighCard:
                return "高牌";
            case PokerHandType.OnePair:
                return "对子";
            case PokerHandType.TwoPair:
                return "两对";
            case PokerHandType.ThreeOfAKind:
                return "三条";
            case PokerHandType.Straight:
                return "顺子";
            case PokerHandType.Flush:
                return "同花";
            case PokerHandType.FullHouse:
                return "葫芦";
            case PokerHandType.FourOfAKind:
                return "四条";
            case PokerHandType.StraightFlush:
                return "同花顺";
            case PokerHandType.FiveOfAKind:
                return "五条";
            case PokerHandType.FlushFullHouse:
                return "同花葫芦";
            case PokerHandType.FlushFiveOfAKind:
                return "同花五条";
            default:
                return "未知牌型";
        }
    }

    /// <summary>
    /// 处理卡牌重叠状态变化
    /// </summary>
    /// <param name="card">卡牌对象</param>
    /// <param name="isOverlapped">是否被遮挡</param>
    private void HandleCardOverlapStateChanged(Card card, bool isOverlapped)
    {
        // 可以在这里添加额外的业务逻辑，如统计可见卡牌数量、触发特殊效果等
        if (isOverlapped)
        {
            // 卡牌被遮挡时的逻辑
            Debug.Log($"卡牌 {card.GetCardKey()} 被遮挡");
        }
        else
        {
            // 卡牌取消遮挡时的逻辑
            Debug.Log($"卡牌 {card.GetCardKey()} 变为可见");
        }
    }
}

/// <summary>
/// 卡牌信息类，用于存储卡牌的基本信息
/// </summary>
/// <remarks>
/// 包含卡牌类型、花色和点数等属性
/// 主要用于在生成卡牌前存储卡牌信息
/// </remarks>
public class CardInfo
{
    /// <summary>
    /// 卡牌类型（如扑克牌）
    /// </summary>
    public CardType Type;
    
    /// <summary>
    /// 卡牌花色（黑桃、红心等）
    /// </summary>
    public CardSuit Suit;
    
    /// <summary>
    /// 卡牌点数（A、2、3...K）
    /// </summary>
    public CardRank Rank;
} 