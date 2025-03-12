using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 控制器管理器
/// </summary>
public class ControllerManager
{
    private Dictionary<int, BaseController> _controllers;//存储控制器的字典

    public ControllerManager()
    {
        _controllers = new Dictionary<int, BaseController>();
    }

    public void Register(ControllerType type,BaseController ctl)
    {
        Register((int)type, ctl);
    }

    //注册控制器
    public void Register(int controllerKey,BaseController ctl)
    {
        if(_controllers.ContainsKey(controllerKey) == false)
        {
            _controllers.Add(controllerKey, ctl);
        }
    }

    //执行所有控制器Init函数
    public void InitAllControllers()
    {
        foreach(var item in _controllers)
        {
            item.Value.Init();
        }
    }

    //移除控制器
    public void UnRegister(int controllerKey)
    {
        if(_controllers.ContainsKey(controllerKey))
        {
            _controllers.Remove(controllerKey);
        }
    }

    //清除
    public void Clear()
    {
        _controllers.Clear();
    }

    //清楚所有控制器
    public void ClearAllControllers()
    {
        List<int> keys = _controllers.Keys.ToList();
        for(int i = 0; i <keys.Count; i++)
        {
            _controllers[keys[i]].Destroy();
            _controllers.Remove(keys[i]);
        }
    }

    //跨模板触发消息
    public void ApplyFunc(int controllerKey, string eventName, System.Object[] args)
    {
        if(_controllers.ContainsKey(controllerKey))
        {
            _controllers[controllerKey].ApplyFunc(eventName, args);
        }
    }

    //获取某控制器Model对象
    public BaseModel GetControllerModel(int controllerKey)
    {
        if(_controllers.ContainsKey(controllerKey))
        {
            return _controllers[controllerKey].GetModel();
        }
        else
        {
            return null;
        }
    }
}
