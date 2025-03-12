using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制器基类
/// </summary>
public class BaseController
{
    private Dictionary<string, System.Action<object[]>> message;//事件字典

    protected BaseModel model;//模板数据

    /// <summary>
    /// 构造函数，初始化事件字典
    /// </summary>
    public BaseController()
    {
        message = new Dictionary<string, System.Action<object[]>>();
    }

    //注册后调用的初始化函数（要所有控制器初始化后执行）
    public virtual void Init()
    {

    }

    //视图相关方法

    //加载视图
    public virtual void OnLoadView(IBaseView view) { }

    /// <summary>
    /// 打开视图
    /// </summary>
    /// <param name="view">要打开的视图</param>
    public virtual void OpenView(IBaseView view)
    {

    }

    //关闭视图
    public virtual void CloseView(IBaseView view)
    {

    }

    //事件相关方法

    //注册模板事件
    public void RegisterFunc(string eventName, System.Action<object[]> callback)
    {
        if(message.ContainsKey(eventName))
        {
            message[eventName] += callback;
        }
        else
        {
            message.Add(eventName, callback);
        }
    }

    //移除模板事件
    public void UnRegisterFunc(string eventName)
    {
        if (message.ContainsKey(eventName))
        {
            message.Remove(eventName);
        }
    }

    //触发本模块事件
    public void ApplyFunc(string eventName, params object[] args)
    {
        if(message.ContainsKey(eventName))
        {
            message[eventName].Invoke(args);
        }
        else
        {
            Debug.Log($"error:{eventName}");
        }
    }

    //触发其它模板的事件
    public void ApplyControllerFunc(int controllerKey,string eventName, params object[] args)
    {
        GameApp.ControllerManager.ApplyFunc(controllerKey, eventName, args);
    }

    /// <summary>
    /// 触发其它控制器的事件（通过控制器类型）
    /// </summary>
    /// <param name="type">控制器类型</param>
    /// <param name="eventName">事件名称</param>
    /// <param name="args">事件参数</param>
    public void ApplyControllerFunc(ControllerType type, string eventName,params object[] args)
    {
        ApplyControllerFunc((int)type, eventName, args);
    }

    //模型相关方法

    /// <summary>
    /// 设置模型数据并建立模型与控制器的关联
    /// </summary>
    /// <param name="model">要设置的模型</param>
    public void SetModel(BaseModel model)
    {
        this.model = model;
        this.model.controller = this;
    }

    /// <summary>
    /// 获取控制器关联的模型
    /// </summary>
    /// <returns>关联的模型实例</returns>
    public BaseModel GetModel()
    {
        return model;
    }

    /// <summary>
    /// 获取控制器关联的模型并转换为指定类型
    /// </summary>
    /// <typeparam name="T">目标模型类型</typeparam>
    /// <returns>转换后的模型实例</returns>
    public T GetModel<T>() where T : BaseModel
    {
        return model as T;
    }

    /// <summary>
    /// 获取指定控制器的模型
    /// </summary>
    /// <param name="controllerKey">控制器的键值</param>
    /// <returns>指定控制器的模型实例</returns>
    public BaseModel GetControllerModel(int controllerKey)
    {
        return GameApp.ControllerManager.GetControllerModel(controllerKey);
    }

    //控制器相关方法

    //删除控制器
    public virtual void Destroy()
    {
        RemoveModuleEvent();
        RemoveGlobalEvent();
    }

    //初始化模板事件
    public virtual void InitModuleEvent()
    {

    }

    //移除模板事件
    public virtual void RemoveModuleEvent()
    {

    }

    //初始化全局事件
    public virtual void InitGlobalEvent()
    {

    }

    //移除全局事件
    public virtual void RemoveGlobalEvent()
    {

    }
}
