using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
} 