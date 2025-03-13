using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 协程辅助类，负责处理需要协程的操作
/// 这是一个单例模式的实现，可以在非MonoBehaviour类中使用
/// </summary>
public class CoroutineHelper : MonoBehaviour
{
    private static CoroutineHelper _instance;
    
    /// <summary>
    /// 单例实例
    /// </summary>
    public static CoroutineHelper instance
    {
        get
        {
            if (_instance == null)
            {
                // 创建一个新的GameObject来挂载CoroutineHelper组件
                GameObject go = new GameObject("CoroutineHelper");
                // 确保该对象在场景切换时不会被销毁
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<CoroutineHelper>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 延迟执行一个操作
    /// </summary>
    /// <param name="delay">延迟时间（秒）</param>
    /// <param name="action">要执行的操作</param>
    /// <returns>协程的引用，可用于停止</returns>
    public Coroutine DelayedAction(float delay, Action action)
    {
        return StartCoroutine(DelayedActionCoroutine(delay, action));
    }

    /// <summary>
    /// 延迟执行操作的协程
    /// </summary>
    private IEnumerator DelayedActionCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    /// <summary>
    /// 停止一个协程
    /// </summary>
    /// <param name="coroutine">要停止的协程</param>
    public void StopCoroutineAction(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
}
