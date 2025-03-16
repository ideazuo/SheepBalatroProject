using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// 卡牌控制器，负责创建和管理卡牌，处理卡牌相关的交互逻辑
/// 实现了MVC架构中的Controller层，负责协调Model和View层之间的通信
/// 整合了卡牌管理器的所有功能，直接管理View层元素
/// </summary>
public class CardController : BaseController
{
    /// <summary>
    /// 卡牌集合模型，管理容器A和容器B中的卡牌集合
    /// </summary>
    private CardsCollectionModel cardsCollectionModel;

    /// <summary>
    /// 计算分数模型
    /// </summary>
    private ScoreModel scoreModel;

    /// <summary>
    /// 所有已创建的卡牌字典，键为唯一ID，值为卡牌对象
    /// </summary>
    private Dictionary<string, Card> cardInstances = new Dictionary<string, Card>();
    
    /// <summary>
    /// 卡牌牌组字典，键为唯一ID，值为卡牌信息
    /// </summary>
    private Dictionary<string, CardInfo> cardDeck = new Dictionary<string, CardInfo>();
    
    /// <summary>
    /// 卡牌预制体引用
    /// </summary>
    private GameObject cardPrefab;
    
    /// <summary>
    /// 容器A - 存放初始卡牌的容器
    /// </summary>
    private Transform containerA;
    
    /// <summary>
    /// 容器B - 存放选中卡牌的容器
    /// </summary>
    private Transform containerB;
    
    /// <summary>
    /// 构造函数，初始化卡牌控制器
    /// 创建卡牌模型并设置关联
    /// </summary>
    public CardController() : base()
    {
        // 初始化卡牌集合模型
        cardsCollectionModel = new CardsCollectionModel(this);
        scoreModel = new ScoreModel(this);

    }
    
    /// <summary>
    /// 初始化控制器，调用基类初始化方法并注册模块事件
    /// </summary>
    public override void Init()
    {
        base.Init();
        InitModuleEvent();
        
        // 加载卡牌预制体
        cardPrefab = Resources.Load<GameObject>("Models/Card");
        
        if (cardPrefab == null)
        {
            Debug.LogError("无法加载卡牌预制体：Models/Card");
        }
        
        // 注册卡牌点击事件处理
        Card.OnCardClicked += OnCardClicked;
        CardOverlapDetector.OnOverlapStateChanged += OnCardOverlapStateChanged;
    }
    
    /// <summary>
    /// 释放控制器资源
    /// </summary>
    public override void Destroy()
    {
        // 取消事件订阅
        Card.OnCardClicked -= OnCardClicked;
        CardOverlapDetector.OnOverlapStateChanged -= OnCardOverlapStateChanged;
        
        // 销毁所有卡牌
        DestroyAllCards();
        
        // 清空牌组
        cardDeck.Clear();
        
        base.Destroy();
    }
    
    /// <summary>
    /// 初始化模块事件，注册所有卡牌相关功能的回调函数
    /// </summary>
    public override void InitModuleEvent()
    {
        base.InitModuleEvent();
        // 注册各种卡牌操作的回调函数
        RegisterFunc(Defines.CreateCard, CreatePokerCard);
        RegisterFunc(Defines.GeneratePokerDecks, GeneratePokerDecks);
        RegisterFunc(Defines.RandomDealCards, RandomDealCards);
        RegisterFunc(Defines.SetContainerB, SetContainerB);
        RegisterFunc(Defines.EvaluatePokerHand, EvaluatePokerHand);
        RegisterFunc(Defines.ClearContainerBWithDelay, ClearContainerBWithDelay);
        
        // 注册CardManager需要的回调函数
        RegisterFunc(Defines.AddCardToContainerA, AddCardToContainerA);
        RegisterFunc(Defines.RemoveCardFromContainerA, RemoveCardFromContainerAWithReturn);
        RegisterFunc(Defines.AddCardToContainerB, AddCardToContainerB);
        RegisterFunc(Defines.GetContainerBCount, GetContainerBCount);
        RegisterFunc(Defines.GetContainerACount, GetContainerACount);
        RegisterFunc(Defines.OnCardOverlapped, OnCardOverlapped);
        RegisterFunc(Defines.OnCardRevealed, OnCardRevealed);
        RegisterFunc(Defines.ClearContainerA, ClearContainerA);
    }
    
    /// <summary>
    /// 设置卡牌容器
    /// </summary>
    /// <param name="containerA">容器A - 存放初始卡牌</param>
    /// <param name="containerB">容器B - 存放选中卡牌</param>
    public void SetContainers(Transform containerA, Transform containerB = null)
    {
        this.containerA = containerA;
        
        if (containerB != null)
        {
            this.containerB = containerB;
            Card.SetContainerB(containerB);
        }
    }
    
    /// <summary>
    /// 清空容器A中的卡牌集合
    /// </summary>
    /// <param name="args">不需要参数</param>
    private void ClearContainerA(object[] args)
    {
        cardsCollectionModel.ClearContainerA();
        Debug.Log("清空了容器A中的卡牌集合");
    }
    
    /// <summary>
    /// 创建卡牌实例
    /// </summary>
    /// <param name="args">
    /// args[0]: CardSuit - 卡牌花色（扑克牌类型时有效）
    /// args[1]: CardRank - 卡牌点数（扑克牌类型时有效）
    /// args[2]: Transform - 父物体
    /// </param>
    /// <remarks>
    /// 该方法加载卡牌预制体并实例化，设置卡牌属性
    /// </remarks>
    private void CreatePokerCard(object[] args)
    {
        // 检查参数
        if (args.Length < 3)
        {
            Debug.LogError("CreateCard参数不足");
            return;
        }

        CardSuit suit = (CardSuit)args[0];
        CardRank rank = (CardRank)args[1];
        Transform parent = (Transform)args[2];
        
        // 创建卡牌并返回
        Card card = CreateCardInstance(suit, rank, parent);
    }
    
    /// <summary>
    /// 创建卡牌实例
    /// </summary>
    /// <param name="suit">卡牌花色</param>
    /// <param name="rank">卡牌点数</param>
    /// <param name="parent">父物体</param>
    /// <returns>创建的卡牌对象</returns>
    private Card CreateCardInstance(CardSuit suit, CardRank rank, Transform parent)
    {
        if (cardPrefab == null)
        {
            cardPrefab = Resources.Load<GameObject>("Models/Card");
            
            if (cardPrefab == null)
            {
                Debug.LogError("无法加载卡牌预制体：Models/Card");
                return null;
            }
        }
        
        // 创建卡牌实例
        GameObject cardObj = GameObject.Instantiate(cardPrefab, parent);
        
        // 确保卡牌有Image组件
        Image cardImage = cardObj.GetComponent<Image>();
        if (cardImage == null)
        {
            cardImage = cardObj.AddComponent<Image>();
        }
        
        // 获取或添加Card组件
        Card card = cardObj.GetComponent<Card>();
        if (card == null)
        {
            card = cardObj.AddComponent<Card>();
        }
        
        // 设置卡牌信息
        card.SetCardInfo(suit, rank);
        
        // 添加重叠检测组件
        CardOverlapDetector detector = cardObj.GetComponent<CardOverlapDetector>();
        if (detector == null)
        {
            detector = cardObj.AddComponent<CardOverlapDetector>();
            detector.Initialize(parent);
        }
        
        // 添加到卡牌字典
        string cardKey = card.GetCardKey();
        cardInstances[cardKey] = card;
        
        return card;
    }
    
    /// <summary>
    /// 生成指定副数的扑克牌并存储在字典中
    /// </summary>
    /// <param name="args">
    /// args[0]: int - 扑克牌副数
    /// </param>
    /// <remarks>
    /// 该方法根据指定的副数生成完整的扑克牌组，每副包含所有花色和点数的组合
    /// 生成的卡牌信息存储在卡牌控制器中，供后续使用
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
        
        // 清空现有牌组
        cardDeck.Clear();

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
                        Suit = suit,
                        Rank = rank
                    };
                    
                    // 添加到字典
                    cardDeck.Add(cardKey, cardInfo);
                }
            }
        }
        
        Debug.Log($"成功生成 {deckCount} 副扑克牌，共 {cardDeck.Count} 张");
    }
    
    /// <summary>
    /// 随机发牌到容器A
    /// </summary>
    private void DealCardsToContainerA()
    {
        if (containerA == null)
        {
            Debug.LogError("容器A未设置，无法发牌");
            return;
        }
        
        if (cardDeck == null || cardDeck.Count == 0)
        {
            Debug.LogError("卡牌集合为空，无法发牌");
            return;
        }
        
        // 先获取containerA的所有子对象并保存在List中
        List<Transform> cardPositions = new List<Transform>();
        for (int i = 0; i < containerA.childCount; i++)
        {
            cardPositions.Add(containerA.GetChild(i));
        }
        
        // 检查是否有子对象作为位置标记
        if (cardPositions.Count == 0)
        {
            Debug.LogWarning("容器A没有子对象作为位置标记，将使用随机位置");
        }
        
        // 清空容器A现有卡牌实例（但保留位置标记）
        for (int i = containerA.childCount - 1; i >= 0; i--)
        {
            // 只删除Card组件的游戏对象，保留没有Card组件的位置标记
            Transform child = containerA.GetChild(i);
            if (child.GetComponent<Card>() != null)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        
        // 清空卡牌字典
        cardInstances.Clear();
        
        // 通知控制器清空容器A的卡牌集合
        ClearContainerA(new object[] { });
        
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
        
        // 加载预制体，获取卡牌尺寸
        if (cardPrefab == null)
        {
            cardPrefab = Resources.Load<GameObject>("Models/Card");
        }
        
        RectTransform cardRectTemp = cardPrefab.GetComponent<RectTransform>();
        float cardWidth = cardRectTemp.rect.width;
        float cardHeight = cardRectTemp.rect.height;
        
        // 准备随机发牌
        List<string> cardKeys = new List<string>(cardDeck.Keys);
        System.Random random = new System.Random();
        
        // 位置索引，用于循环使用cardPositions中的位置
        int positionIndex = 0;
        
        // 逐一创建所有卡牌
        while (cardKeys.Count > 0)
        {
            // 随机抽取一张牌
            int randomIndex = random.Next(cardKeys.Count);
            string cardKey = cardKeys[randomIndex];
            CardInfo cardInfo = cardDeck[cardKey];
            cardKeys.RemoveAt(randomIndex);
            
            // 创建卡牌
            Card card = CreateCardInstance(cardInfo.Suit, cardInfo.Rank, containerA);
            
            // 设置卡牌位置
            RectTransform cardRect = card.GetComponent<RectTransform>();
            
            if (cardPositions.Count > 0)
            {
                // 使用位置标记的坐标
                // 当List中对象用完时循环使用第一个位置
                Transform positionMarker = cardPositions[positionIndex];
                RectTransform markerRect = positionMarker as RectTransform;
                
                if (markerRect != null)
                {
                    // 使用位置标记的坐标
                    cardRect.anchoredPosition = markerRect.anchoredPosition;
                }
                else
                {
                    // 如果位置标记没有RectTransform，使用局部坐标
                    cardRect.localPosition = positionMarker.localPosition;
                }
                
                // 更新位置索引，循环使用位置列表
                positionIndex = (positionIndex + 1) % cardPositions.Count;
            }
            else
            {
                // 如果没有位置标记，使用随机位置
                float maxX = (containerWidth - cardWidth) / 2;
                float maxY = (containerHeight - cardHeight) / 2;
                float randomX = Random.Range(-maxX, maxX);
                float randomY = Random.Range(-maxY, maxY);
                
                cardRect.anchoredPosition = new Vector2(randomX, randomY);
            }
            
            // 添加到模型
            object[] args = new object[] { card.GetCardKey(), card };
            AddCardToContainerA(args);
        }
        
        Debug.Log($"成功发放所有卡牌到容器A");
    }
    
    /// <summary>
    /// 随机从牌堆中抽取卡牌并克隆到指定区域
    /// </summary>
    /// <param name="args">
    /// args[0]: Transform - 放置卡牌的容器A
    /// args[1]: Transform - (可选) 容器B，用于移动选中的卡牌
    /// </param>
    /// <remarks>
    /// 该方法处理随机发牌的逻辑
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
            // 设置容器
            SetContainers(containerA, containerB);
        }
        else
        {
            // 只设置容器A
            SetContainers(containerA);
        }
        
        // 检查卡牌集合是否为空
        if (cardDeck == null || cardDeck.Count == 0)
        {
            Debug.LogError("卡牌集合为空，请先调用GeneratePokerDecks生成卡牌");
            return;
        }
        
        // 处理发牌逻辑
        DealCardsToContainerA();
        
        // 清空临时卡牌集合
        cardDeck.Clear();
    }
    
    /// <summary>
    /// 将卡牌从容器A移动到容器B
    /// </summary>
    /// <param name="card">要移动的卡牌</param>
    /// <returns>是否移动成功</returns>
    public bool MoveCardToContainerB(Card card)
    {
        if (containerB == null)
        {
            Debug.LogWarning("容器B未设置，无法移动卡牌");
            return false;
        }
        
        string cardKey = card.GetCardKey();
        
        // 通知Controller将卡牌从容器A移除
        object[] removeArgs = new object[] { cardKey };
        RemoveCardFromContainerAWithReturn(removeArgs);
            
        // 更新视图层：移动卡牌到容器B
        card.MoveToContainer(containerB);
        
        // 通知Controller添加卡牌到容器B
        object[] addArgs = new object[] { cardKey, card };
        AddCardToContainerB(addArgs);
        
        Debug.Log($"卡牌 {cardKey} 已从容器A移动到容器B");
        return true;
    }
    
    /// <summary>
    /// 处理卡牌点击事件
    /// </summary>
    /// <param name="card">被点击的卡牌</param>
    private void OnCardClicked(Card card)
    {
        if (containerB != null)
        {
            // 直接检查容器B的子物体数量
            int containerBCount = containerB.childCount;
            if (containerBCount >= 5)
            {
                Debug.LogWarning("容器B已满，最多只能放置5张卡牌");
                return;
            }
            
            // 移动卡牌到容器B
            MoveCardToContainerB(card);
        }
        else
        {
            // 将卡牌设为最顶层
            card.SetAsTopMost();
            
            // 更新所有卡牌重叠状态
            card.UpdateAllCardsState();
        }
    }
    
    /// <summary>
    /// 处理卡牌重叠状态变化
    /// </summary>
    /// <param name="card">卡牌对象</param>
    /// <param name="isOverlapped">是否被遮挡</param>
    private void OnCardOverlapStateChanged(Card card, bool isOverlapped)
    {
        // 状态变化的处理逻辑
        if (isOverlapped)
        {
            // 通知Controller卡牌被遮挡
            object[] args = new object[] { card.GetCardKey() };
            OnCardOverlapped(args);
        }
        else
        {
            // 通知Controller卡牌取消遮挡
            object[] args = new object[] { card.GetCardKey() };
            OnCardRevealed(args);
        }
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
        this.containerB = containerB;
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
        PokerHandType handType = cardsCollectionModel.EvaluatePokerHand();
        
        // 当容器B中没有牌时，不输出任何信息
        if (cardsCollectionModel.ContainerBCards.Count == 0)
        {
            return;
        }
        
        // 将牌型枚举转换为对应的中文名称
        string handName = GetPokerHandTypeName(handType);
        
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
            PokerHandType handType = cardsCollectionModel.CurrentPokerHandType;
            string handName = GetPokerHandTypeName(handType);
            
            // 清理并销毁容器B中的卡牌
            cardsCollectionModel.ClearContainerBWithDestroy();
            
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
    /// 添加卡牌到容器A的集合中
    /// </summary>
    /// <param name="args">
    /// args[0]: string - 卡牌键
    /// args[1]: Card - 卡牌对象
    /// </param>
    private void AddCardToContainerA(object[] args)
    {
        if (args.Length < 2 || !(args[0] is string) || !(args[1] is Card))
        {
            Debug.LogError("AddCardToContainerA参数不足或类型错误");
            return;
        }
        
        string cardKey = (string)args[0];
        Card card = (Card)args[1];
        
        cardsCollectionModel.AddCardToContainerA(cardKey, card);
    }
    
    /// <summary>
    /// 从容器A的集合中移除卡牌并返回是否成功
    /// </summary>
    /// <param name="args">
    /// args[0]: string - 卡牌键
    /// </param>
    /// <returns>是否成功移除</returns>
    private void RemoveCardFromContainerAWithReturn(object[] args)
    {
        if (args.Length < 1 || !(args[0] is string))
        {
            Debug.LogError("RemoveCardFromContainerA参数不足或类型错误");
            return;
        }
        
        string cardKey = (string)args[0];
        bool success = cardsCollectionModel.RemoveCardFromContainerA(cardKey);
        
        Debug.Log($"从容器A移除卡牌 {cardKey}: {(success ? "成功" : "失败")}");
    }
    
    /// <summary>
    /// 添加卡牌到容器B的集合中
    /// </summary>
    /// <param name="args">
    /// args[0]: string - 卡牌键
    /// args[1]: Card - 卡牌对象
    /// </param>
    private void AddCardToContainerB(object[] args)
    {
        if (args.Length < 2 || !(args[0] is string) || !(args[1] is Card))
        {
            Debug.LogError("AddCardToContainerB参数不足或类型错误");
            return;
        }
        
        string cardKey = (string)args[0];
        Card card = (Card)args[1];
        
        cardsCollectionModel.AddCardToContainerB(cardKey, card);
    }
    
    /// <summary>
    /// 获取容器B中卡牌数量
    /// </summary>
    /// <param name="args">不需要参数</param>
    /// <returns>容器B中卡牌数量</returns>
    private void GetContainerBCount(object[] args)
    {
        int count = cardsCollectionModel.ContainerBCards.Count;
        Debug.Log($"容器B中的卡牌数量: {count}");
    }

    /// <summary>
    /// 获取容器A中卡牌数量
    /// </summary>
    /// <param name="args">不需要参数</param>
    private void GetContainerACount(object[] args)
    {
        int count = cardsCollectionModel.ContainerACards.Count;
        Debug.Log($"容器A中的卡牌数量: {count}");
    }

    /// <summary>
    /// 根据键获取卡牌
    /// </summary>
    /// <param name="cardKey">卡牌键</param>
    /// <returns>卡牌对象，若不存在则返回null</returns>
    public Card GetCard(string cardKey)
    {
        if (cardInstances.TryGetValue(cardKey, out Card card))
        {
            return card;
        }
        return null;
    }
    
    /// <summary>
    /// 销毁所有卡牌
    /// </summary>
    public void DestroyAllCards()
    {
        foreach (var card in cardInstances.Values)
        {
            if (card != null && card.gameObject != null)
            {
                GameObject.Destroy(card.gameObject);
            }
        }
        cardInstances.Clear();
    }
    
    /// <summary>
    /// 处理卡牌被遮挡事件
    /// </summary>
    /// <param name="args">
    /// args[0]: string - 卡牌键
    /// </param>
    private void OnCardOverlapped(object[] args)
    {
        if (args.Length < 1 || !(args[0] is string))
        {
            Debug.LogError("OnCardOverlapped参数不足或类型错误");
            return;
        }
        
        string cardKey = (string)args[0];
        Debug.Log($"Controller: 卡牌 {cardKey} 被遮挡");
        
        // 在这里可以添加卡牌被遮挡时的业务逻辑
    }
    
    /// <summary>
    /// 处理卡牌取消遮挡事件
    /// </summary>
    /// <param name="args">
    /// args[0]: string - 卡牌键
    /// </param>
    private void OnCardRevealed(object[] args)
    {
        if (args.Length < 1 || !(args[0] is string))
        {
            Debug.LogError("OnCardRevealed参数不足或类型错误");
            return;
        }
        
        string cardKey = (string)args[0];
        Debug.Log($"Controller: 卡牌 {cardKey} 变为可见");
        
        // 在这里可以添加卡牌变为可见时的业务逻辑
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
    /// 卡牌花色（黑桃、红心等）
    /// </summary>
    public CardSuit Suit;
    
    /// <summary>
    /// 卡牌点数（A、2、3...K）
    /// </summary>
    public CardRank Rank;
} 