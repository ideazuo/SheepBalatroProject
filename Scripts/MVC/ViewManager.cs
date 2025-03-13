using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

/// <summary>
/// 视图信息类，存储视图的基本配置
/// </summary>
public class ViewInfo
{
    public string PrefabName;//视图预制体名称
    public Transform parentTf;//所在父级
    public BaseController controller;//视图所属控制器
    public int Sorting_Order;//显示层级，改变显示顺序
}

/// <summary>
/// 视图管理器，负责UI视图的注册、加载、显示和销毁
/// </summary>
public class ViewManager
{
    public Transform canvasTf;//画布组件
    public Transform worldCanvasTf;//世界画布组件
    Dictionary<int, IBaseView> _opens;//开启中的视图
    Dictionary<int, IBaseView> _viewCache;//视图缓存
    Dictionary<int, ViewInfo> _view;//注册的视图信息

    public ViewManager()
    {
        canvasTf = GameObject.Find("Canvas").transform;
        worldCanvasTf = GameObject.Find("WorldCanvas").transform;
        _opens = new Dictionary<int, IBaseView>();
        _view = new Dictionary<int, ViewInfo>();
        _viewCache = new Dictionary<int, IBaseView>();
    }

    /// <summary>
    /// 注册视图信息，使用整数键
    /// </summary>
    /// <param name="key">视图唯一标识键</param>
    /// <param name="viewInfo">视图信息</param>
    public void Register(int key, ViewInfo viewInfo)
    {
        if(_view.ContainsKey(key) == false)
        {
            _view.Add(key, viewInfo);
        }
    }

    /// <summary>
    /// 注册视图信息，使用ViewType枚举值
    /// </summary>
    /// <param name="viewType">视图类型枚举</param>
    /// <param name="viewInfo">视图信息</param>
    public void Register(ViewType viewType, ViewInfo viewInfo)
    {
        Register((int)viewType, viewInfo);
    }

    /// <summary>
    /// 注销视图信息
    /// </summary>
    /// <param name="key">视图唯一标识键</param>
    public void UnRegister(int key)
    {
        if(_view.ContainsKey(key))
        {
            _view.Remove(key);
        }
    }

    /// <summary>
    /// 移除视图及其所有引用
    /// </summary>
    /// <param name="key">视图唯一标识键</param>
    public void RemoveView(int key)
    {
        _view.Remove(key);
        _viewCache.Remove(key);
        _opens.Remove(key);
    }

    /// <summary>
    /// 移除指定控制器关联的所有视图
    /// </summary>
    /// <param name="ctl">目标控制器</param>
    public void RemoveViewByController(BaseController ctl)
    {
        foreach(var item in _view)
        {
            if(item.Value.controller == ctl)
            {
                RemoveView(item.Key);
            }
        }
    }

    /// <summary>
    /// 检查指定视图是否已打开
    /// </summary>
    /// <param name="key">视图唯一标识键</param>
    /// <returns>视图是否已打开</returns>
    public bool IsOpen(int key)
    {
        return _opens.ContainsKey(key);
    }

    /// <summary>
    /// 获取指定视图的引用
    /// </summary>
    /// <param name="key">视图唯一标识键</param>
    /// <returns>找到的视图实例，未找到则返回null</returns>
    public IBaseView GetView(int key)
    {
        if(_opens.ContainsKey(key))
        {
            return _opens[key];
        }
        if(_viewCache.ContainsKey(key))
        {
            return _viewCache[key];
        }
        return null;
    }

    /// <summary>
    /// 获取指定视图的引用并转换为目标类型
    /// </summary>
    /// <typeparam name="T">视图类型</typeparam>
    /// <param name="key">视图唯一标识键</param>
    /// <returns>找到的视图实例，未找到则返回null</returns>
    public T GetView<T>(int key) where T : class, IBaseView
    {
        IBaseView view = GetView(key);
        if(view != null)
        {
            return view as T;
        }
        return null;
    }

    /// <summary>
    /// 销毁指定视图及其游戏对象
    /// </summary>
    /// <param name="key">视图唯一标识键</param>
    public void Destroy(int key)
    {
        IBaseView oldView = GetView(key);
        if(oldView != null)
        {
            UnRegister(key);
            oldView.DestroyView();
            _viewCache.Remove(key);
        }
    }

    /// <summary>
    /// 关闭指定视图
    /// </summary>
    /// <param name="key">视图唯一标识键</param>
    /// <param name="args">关闭时传递的参数</param>
    public void Close(int key, params object[] args)
    {
        // 视图未打开则直接返回
        if(IsOpen(key) == false)
        {
            return;
        }
        IBaseView view = GetView(key);
        if(view != null)
        {
            _opens.Remove(key);
            view.Close(args);
            _view[key].controller.CloseView(view);
        }
    }

    /// <summary>
    /// 关闭所有已打开的视图
    /// </summary>
    public void CloseAll()
    {
        List<IBaseView> list = _opens.Values.ToList();

        for (int i = list.Count - 1; i >= 0; i--)
        {
            Close(list[i].ViewId);
        }
    }

    /// <summary>
    /// 使用ViewType枚举打开视图
    /// </summary>
    /// <param name="type">视图类型枚举</param>
    /// <param name="args">打开时传递的参数</param>
    public void Open(ViewType type, params object[] args)
    {
        Open((int)type, args);
    }

    /// <summary>
    /// 打开指定视图，如果视图未加载则先加载
    /// </summary>
    /// <param name="key">视图唯一标识键</param>
    /// <param name="args">打开时传递的参数</param>
    public void Open(int key, params object[] args)
    {
        IBaseView view = GetView(key);
        ViewInfo viewInfo = _view[key];
        if(view == null)
        {
            // 视图未加载，从Resources加载预制体
            string type = ((ViewType)key).ToString(); // 类型字符串与脚本名称对应
            GameObject uiObj = UnityEngine.Object.Instantiate(Resources.Load($"Views/{viewInfo.PrefabName}"), viewInfo.parentTf) as GameObject;
            
            if (uiObj == null)
            {
                Debug.LogError($"没有找到该界面：/Views/{viewInfo.PrefabName}");
            }

            Canvas canvas = uiObj.GetComponent<Canvas>();
            if(canvas == null)
            {
                canvas = uiObj.AddComponent<Canvas>();
            }
            if(uiObj.GetComponent<GraphicRaycaster>() == null)
            {
                uiObj.AddComponent<GraphicRaycaster>();
            }
            canvas.overrideSorting = true; // 启用排序覆盖
            canvas.sortingOrder = viewInfo.Sorting_Order; // 设置排序层级
            view = uiObj.AddComponent(Type.GetType(type)) as IBaseView; // 添加对应view脚本
            view.ViewId = key; // 设置视图ID
            view.Controller = viewInfo.controller; // 设置控制器
            // 添加到视图缓存
            _viewCache.Add(key, view);
            viewInfo.controller.OnLoadView(view);
        }

        // 已经打开则直接返回
        if (this._opens.ContainsKey(key) == true)
        {
            return;
        }
        this._opens.Add(key, view);

        // 根据初始化状态执行不同操作
        if (view.IsInit())
        {
            view.SetVisable(true); // 显示视图
            view.Open(args); // 调用打开方法
            viewInfo.controller.OpenView(view);
        }
        else
        {
            view.InitUI();
            view.InitData();
            view.Open(args);
            viewInfo.controller.OpenView(view);
        }
    }
}


