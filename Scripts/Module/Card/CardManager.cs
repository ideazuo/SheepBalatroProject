using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 卡牌管理器，负责管理所有卡牌相关操作
/// 作为一个中间层连接CardController和卡牌视图元素
/// </summary>
public class CardManager
{
    /// <summary>
    /// 所有已创建的卡牌字典，键为唯一ID，值为卡牌对象
    /// </summary>
    private Dictionary<string, Card> cardInstances = new Dictionary<string, Card>();
    
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
    /// 构造函数
    /// </summary>
    public CardManager()
    {
        // 构造函数留空，初始化工作在Init方法中进行
    }
    
    /// <summary>
    /// 初始化卡牌管理器
    /// </summary>
    public void Init()
    {
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
    /// 创建卡牌实例
    /// </summary>
    /// <param name="cardType">卡牌类型</param>
    /// <param name="suit">卡牌花色</param>
    /// <param name="rank">卡牌点数</param>
    /// <param name="parent">父物体</param>
    /// <returns>创建的卡牌对象</returns>
    public Card CreateCard(CardSuit suit, CardRank rank, Transform parent)
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
    /// 随机发牌到容器A
    /// </summary>
    /// <param name="cardDeck">卡牌信息字典</param>
    public void DealCardsToContainerA(Dictionary<string, CardInfo> cardDeck)
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
        
        // 清空容器A现有卡牌
        for (int i = containerA.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(containerA.GetChild(i).gameObject);
        }
        
        // 清空卡牌字典
        cardInstances.Clear();
        
        // 通知控制器清空容器A的卡牌集合
        object[] clearArgs = new object[] { };
        GameApp.ControllerManager.ApplyFunc((int)ControllerType.Card, Defines.ClearContainerA, clearArgs);
        
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
        
        // 逐一创建所有卡牌
        while (cardKeys.Count > 0)
        {
            // 随机抽取一张牌
            int randomIndex = random.Next(cardKeys.Count);
            string cardKey = cardKeys[randomIndex];
            CardInfo cardInfo = cardDeck[cardKey];
            cardKeys.RemoveAt(randomIndex);
            
            // 创建卡牌
            Card card = CreateCard(cardInfo.Suit, cardInfo.Rank, containerA);
            
            // 设置随机位置
            RectTransform cardRect = card.GetComponent<RectTransform>();
            float maxX = (containerWidth - cardWidth) / 2;
            float maxY = (containerHeight - cardHeight) / 2;
            float randomX = Random.Range(-maxX, maxX);
            float randomY = Random.Range(-maxY, maxY);
            
            cardRect.anchoredPosition = new Vector2(randomX, randomY);
            
            // 通知Controller添加到模型
            object[] args = new object[] { card.GetCardKey(), card };
            GameApp.ControllerManager.ApplyFunc((int)ControllerType.Card, Defines.AddCardToContainerA, args);
        }
        
        Debug.Log($"成功发放所有卡牌到容器A");
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
        GameApp.ControllerManager.ApplyFunc((int)ControllerType.Card, Defines.RemoveCardFromContainerA, removeArgs);
            
        // 更新视图层：移动卡牌到容器B
        card.MoveToContainer(containerB);
        
        // 通知Controller添加卡牌到容器B
        object[] addArgs = new object[] { cardKey, card };
        GameApp.ControllerManager.ApplyFunc((int)ControllerType.Card, Defines.AddCardToContainerB, addArgs);
        
        Debug.Log($"卡牌 {cardKey} 已从容器A移动到容器B");
        return true;
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
            GameApp.ControllerManager.ApplyFunc((int)ControllerType.Card, Defines.OnCardOverlapped, args);
        }
        else
        {
            // 通知Controller卡牌取消遮挡
            object[] args = new object[] { card.GetCardKey() };
            GameApp.ControllerManager.ApplyFunc((int)ControllerType.Card, Defines.OnCardRevealed, args);
        }
    }
    
    /// <summary>
    /// 生成指定副数的扑克牌组
    /// </summary>
    /// <param name="deckCount">扑克牌副数</param>
    /// <returns>生成的卡牌信息字典</returns>
    public Dictionary<string, CardInfo> GeneratePokerDecks(int deckCount)
    {
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
                        Suit = suit,
                        Rank = rank
                    };
                    
                    // 添加到字典
                    cardDeck.Add(cardKey, cardInfo);
                }
            }
        }
        
        Debug.Log($"成功生成 {deckCount} 副扑克牌，共 {cardDeck.Count} 张");
        return cardDeck;
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
    /// 释放资源
    /// </summary>
    public void Cleanup()
    {
        // 取消事件订阅
        Card.OnCardClicked -= OnCardClicked;
        CardOverlapDetector.OnOverlapStateChanged -= OnCardOverlapStateChanged;
        
        // 销毁所有卡牌
        DestroyAllCards();
    }
} 