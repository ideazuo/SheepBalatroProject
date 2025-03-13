using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 卡牌重叠检测组件，用于检测卡牌是否被其他卡牌遮挡
/// </summary>
public class CardOverlapDetector : MonoBehaviour
{
    /// <summary>
    /// 重叠状态变化事件委托
    /// </summary>
    /// <param name="card">卡牌对象</param>
    /// <param name="isOverlapped">是否被遮挡</param>
    public delegate void OverlapStateChangedHandler(Card card, bool isOverlapped);
    
    /// <summary>
    /// 重叠状态变化事件
    /// </summary>
    public static event OverlapStateChangedHandler OnOverlapStateChanged;
    
    /// <summary>
    /// 卡牌对象
    /// </summary>
    private Card card;
    
    /// <summary>
    /// 卡牌图片组件
    /// </summary>
    private Image cardImage;
    
    /// <summary>
    /// 卡牌容器
    /// </summary>
    private Transform container;
    
    /// <summary>
    /// 卡牌矩形区域
    /// </summary>
    private RectTransform rectTransform;
    
    /// <summary>
    /// 是否被遮挡
    /// </summary>
    private bool isOverlapped = false;
    
    /// <summary>
    /// 获取卡牌是否被遮挡
    /// </summary>
    public bool IsOverlapped => isOverlapped;
    
    /// <summary>
    /// 设置卡牌遮挡状态
    /// </summary>
    /// <param name="overlapped">是否被遮挡</param>
    public void SetOverlapped(bool overlapped)
    {
        if (isOverlapped != overlapped)
        {
            isOverlapped = overlapped;
            UpdateCardState();
            
            // 触发重叠状态变化事件
            OnOverlapStateChanged?.Invoke(card, isOverlapped);
        }
    }
    
    /// <summary>
    /// 初始化
    /// </summary>
    void Awake()
    {
        card = GetComponent<Card>();
        cardImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }
    
    /// <summary>
    /// 初始化组件
    /// </summary>
    /// <param name="parent">卡牌容器</param>
    public void Initialize(Transform parent)
    {
        container = parent;
        StartCoroutine(CheckOverlap());
        
        // 添加一个事件触发器，在任何卡牌层级变化时检测重叠
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { CheckAllCards(); });
        trigger.triggers.Add(entry);
    }
    
    /// <summary>
    /// 检查卡牌是否被遮挡
    /// </summary>
    private IEnumerator CheckOverlap()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f); // 提高检测频率，每0.1秒检查一次
            
            bool overlapped = IsCardOverlapped();
            
            if (overlapped != isOverlapped)
            {
                isOverlapped = overlapped;
                UpdateCardState();
                
                // 触发重叠状态变化事件
                OnOverlapStateChanged?.Invoke(card, isOverlapped);
            }
        }
    }

    /// <summary>
    /// 检查容器中的所有卡牌
    /// </summary>
    private void CheckAllCards()
    {
        // 立即对所有卡牌执行一次重叠检测
        for (int i = 0; i < container.childCount; i++)
        {
            CardOverlapDetector detector = container.GetChild(i).GetComponent<CardOverlapDetector>();
            if (detector != null)
            {
                bool overlapped = detector.IsCardOverlapped();
                if (overlapped != detector.isOverlapped)
                {
                    detector.isOverlapped = overlapped;
                    detector.UpdateCardState();
                    
                    // 触发重叠状态变化事件
                    OnOverlapStateChanged?.Invoke(detector.card, detector.isOverlapped);
                }
            }
        }
    }
    
    /// <summary>
    /// 判断卡牌是否被其他卡牌遮挡
    /// </summary>
    /// <returns>是否被遮挡</returns>
    public bool IsCardOverlapped()
    {
        if (container == null || rectTransform == null)
            return false;
            
        // 获取当前卡牌在容器中的索引
        int myIndex = transform.GetSiblingIndex();
        
        // 获取当前卡牌的矩形区域
        Rect myRect = GetWorldRect(rectTransform);
        
        // 检查是否被其他卡牌遮挡
        for (int i = 0; i < container.childCount; i++)
        {
            // 只检查在我之上（层级更高）的卡牌
            if (i > myIndex)
            {
                Transform otherCard = container.GetChild(i);
                RectTransform otherRect = otherCard.GetComponent<RectTransform>();
                
                if (otherRect != null)
                {
                    Rect otherWorldRect = GetWorldRect(otherRect);
                    
                    // 如果两个矩形相交且重叠面积超过阈值，认为被遮挡
                    if (myRect.Overlaps(otherWorldRect))
                    {
                        Rect intersection = new Rect(
                            Mathf.Max(myRect.x, otherWorldRect.x),
                            Mathf.Max(myRect.y, otherWorldRect.y),
                            Mathf.Min(myRect.xMax, otherWorldRect.xMax) - Mathf.Max(myRect.x, otherWorldRect.x),
                            Mathf.Min(myRect.yMax, otherWorldRect.yMax) - Mathf.Max(myRect.y, otherWorldRect.y)
                        );
                        
                        float overlapArea = intersection.width * intersection.height;
                        float myArea = myRect.width * myRect.height;
                        
                        if (overlapArea / myArea > 0.0f) // 重叠面积阈值调整为0%，任何重叠都视为遮挡
                        {
                            return true;
                        }
                    }
                }
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 获取RectTransform在世界空间的矩形
    /// </summary>
    /// <param name="rectTransform">要转换的RectTransform</param>
    /// <returns>世界空间中的矩形</returns>
    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        
        float xMin = corners[0].x;
        float yMin = corners[0].y;
        float xMax = corners[2].x;
        float yMax = corners[2].y;
        
        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }
    
    /// <summary>
    /// 更新卡牌状态
    /// </summary>
    public void UpdateCardState()
    {
        if (isOverlapped)
        {
            // 被遮挡，禁用点击并变灰
            DisableCard();
        }
        else
        {
            // 未被遮挡，启用点击并恢复正常颜色
            EnableCard();
        }
    }
    
    /// <summary>
    /// 禁用卡牌
    /// </summary>
    private void DisableCard()
    {
        // 变灰
        Color grayColor = cardImage.color;
        grayColor.r *= 0.6f;
        grayColor.g *= 0.6f;
        grayColor.b *= 0.6f;
        cardImage.color = grayColor;
        
        // 禁用点击
        card.SetInteractable(false);
    }
    
    /// <summary>
    /// 启用卡牌
    /// </summary>
    private void EnableCard()
    {
        // 恢复颜色
        cardImage.color = Color.white;
        
        // 启用点击
        card.SetInteractable(true);
    }
} 