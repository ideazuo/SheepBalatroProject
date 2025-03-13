using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 基础视图类，实现MVC架构中的View部分
/// 所有UI视图的基础类，提供视图显示、隐藏和与Controller通信的功能
/// </summary>
public class BaseView : MonoBehaviour,IBaseView
{
    /// <summary>
    /// 视图唯一标识ID
    /// </summary>
    public int ViewId { get; set; }
    
    /// <summary>
    /// 关联的控制器引用
    /// </summary>
    public BaseController Controller { get; set; }

    /// <summary>
    /// 当前视图的Canvas组件
    /// </summary>
    protected Canvas _canvas;

    /// <summary>
    /// 缓存GameObject的字典，用于快速查找子物体
    /// </summary>
    protected Dictionary<string, GameObject> m_cache_gos = new Dictionary<string, GameObject>();//缓存的物体的字典

    /// <summary>
    /// 标记视图是否已经初始化
    /// </summary>
    private bool _isInit = false;//是否初始化

    /// <summary>
    /// Unity生命周期方法，在Start之前执行一次
    /// 获取Canvas组件并调用OnAwake自定义方法
    /// </summary>
    void Awake()
    {
        _canvas = gameObject.GetComponent<Canvas>();
        OnAwake();
    }

    /// <summary>
    /// Unity生命周期方法，在Awake之后执行一次
    /// 调用OnStart自定义方法
    /// </summary>
    void Start()
    {
        OnStart();
    }

    /// <summary>
    /// 可被子类重写的Awake方法
    /// </summary>
    protected virtual void OnAwake()
    {

    }

    /// <summary>
    /// 可被子类重写的Start方法
    /// </summary>
    protected virtual void OnStart()
    {

    }

    /// <summary>
    /// 向指定控制器发送事件
    /// </summary>
    /// <param name="controllerKey">控制器标识</param>
    /// <param name="eventName">事件名称</param>
    /// <param name="args">事件参数</param>
    public void ApplyControllerFunc(int controllerKey, string eventName, params object[] args)
    {
        this.Controller.ApplyControllerFunc(controllerKey, eventName, args);
    }

    /// <summary>
    /// 向当前关联的控制器发送事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    /// <param name="args">事件参数</param>
    public void ApplyFunc(string eventName, params object[] args)
    {
        this.Controller.ApplyFunc(eventName, args);
    }

    /// <summary>
    /// 关闭视图，可被子类重写以实现自定义关闭行为
    /// </summary>
    /// <param name="args">关闭参数</param>
    public virtual void Close(params object[] args)
    {
        SetVisable(false);//隐藏
    }

    /// <summary>
    /// 销毁视图，清除控制器引用并销毁游戏对象
    /// </summary>
    public void DestroyView()
    {
        Controller = null;
        Destroy(gameObject);
    }

    /// <summary>
    /// 初始化视图数据，标记视图为已初始化状态
    /// 子类应重写此方法以实现自定义数据初始化
    /// </summary>
    public virtual void InitData()
    {
        _isInit = true;
    }

    /// <summary>
    /// 初始化视图UI
    /// 子类应重写此方法以实现自定义UI初始化
    /// </summary>
    public virtual void InitUI()
    {
        
    }

    /// <summary>
    /// 检查视图是否已初始化
    /// </summary>
    /// <returns>是否已初始化</returns>
    public bool IsInit()
    {
        return _isInit;
    }

    /// <summary>
    /// 检查视图是否显示中
    /// </summary>
    /// <returns>是否显示</returns>
    public bool IsShow()
    {
        return _canvas.enabled == true;
    }

    /// <summary>
    /// 打开视图，可被子类重写以实现自定义打开行为
    /// </summary>
    /// <param name="args">打开参数</param>
    public virtual void Open(params object[] args)
    {
        
    }

    /// <summary>
    /// 设置视图的显示状态
    /// </summary>
    /// <param name="value">是否显示</param>
    public void SetVisable(bool value)
    {
        this._canvas.enabled = value;
    }

    /// <summary>
    /// 查找子物体并缓存，提高后续查找效率
    /// </summary>
    /// <param name="res">子物体路径</param>
    /// <returns>找到的GameObject</returns>
    public GameObject Find(string res)
    {
        if(m_cache_gos.ContainsKey(res))
        {
            return m_cache_gos[res];
        }
        
        Transform foundTransform = FindRecursively(transform, res);
        if (foundTransform == null)
        {
            Debug.LogError($"在 {gameObject.name} 中找不到子物体：{res}");
            return null;
        }
        
        m_cache_gos.Add(res, foundTransform.gameObject);
        return m_cache_gos[res];
    }

    /// <summary>
    /// 递归查找子物体
    /// </summary>
    /// <param name="parent">父级Transform</param>
    /// <param name="name">要查找的物体名称</param>
    /// <returns>找到的Transform，未找到则返回null</returns>
    private Transform FindRecursively(Transform parent, string name)
    {
        // 先尝试直接查找
        Transform child = parent.Find(name);
        if (child != null)
        {
            return child;
        }

        // 递归查找所有子物体
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform result = FindRecursively(parent.GetChild(i), name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    /// <summary>
    /// 查找子物体上的指定类型组件并缓存，提高后续查找效率
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="res">子物体路径</param>
    /// <returns>找到的组件</returns>
    public T Find<T>(string res) where T : Component
    {
        GameObject obj = Find(res);
        if (obj == null)
        {
            return null;
        }
        
        T component = obj.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError($"在 {gameObject.name} 的子物体 {res} 上找不到 {typeof(T).Name} 组件");
        }
        
        return component;
    }
}