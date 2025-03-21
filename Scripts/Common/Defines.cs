﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 常量类
/// </summary>
public static class Defines
{
    //控制器相关的事件字符串
    public static readonly string OpenStartView = "OpenStartView";//打开开始面板
    public static readonly string OpenSetView = "OpenSetView";//打开设置面板
    public static readonly string OpenMessageView = "OpenMessageView";//打开提示面板
    public static readonly string OpenGameView = "OpenGameView";//打开游戏面板
    public static readonly string OpenLevelView = "OpenLevelView";//打开关卡面板
    public static readonly string OpenWinView = "OpenWinView";//打开关卡面板
    public static readonly string OpenLoseView = "OpenLoseView";//打开关卡面板

    //卡牌相关
    public static readonly string CreateCard = "CreateCard";//创建卡牌
    public static readonly string GeneratePokerDecks = "GeneratePokerDecks";//生成扑克牌组
    public static readonly string RandomDealCards = "RandomDealCards";//随机发牌
    public static readonly string SetContainerB = "SetContainerB";//设置容器B
    public static readonly string EvaluatePokerHand = "EvaluatePokerHand";//评估扑克牌手牌
    public static readonly string ClearContainerBWithDelay = "ClearContainerBWithDelay";//延迟清理容器B
    
    // CardManager相关常量
    public static readonly string AddCardToContainerA = "AddCardToContainerA";//添加卡牌到容器A
    public static readonly string RemoveCardFromContainerA = "RemoveCardFromContainerA";//从容器A移除卡牌
    public static readonly string AddCardToContainerB = "AddCardToContainerB";//添加卡牌到容器B
    public static readonly string GetContainerBCount = "GetContainerBCount";//获取容器B中卡牌数量
    public static readonly string GetContainerACount = "GetContainerACount";//获取容器A中卡牌数量
    public static readonly string OnCardOverlapped = "OnCardOverlapped";//卡牌被遮挡事件
    public static readonly string OnCardRevealed = "OnCardRevealed";//卡牌取消遮挡事件
    public static readonly string ClearContainerA = "ClearContainerA";//清空容器A中的卡牌集合
}
