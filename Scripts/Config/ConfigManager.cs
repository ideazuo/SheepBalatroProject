using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理游戏中所有配置表的加载和访问
/// </summary>
public class ConfigManager
{
    /// <summary>
    /// 存储待加载的配置表
    /// </summary>
    private Dictionary<string, ConfigData> loadList;

    /// <summary>
    /// 存储已加载完成的配置表
    /// </summary>
    private Dictionary<string, ConfigData> configs;

    /// <summary>
    /// 构造函数，初始化字典
    /// </summary>
    public ConfigManager()
    {
        loadList = new Dictionary<string, ConfigData>();
        configs = new Dictionary<string, ConfigData>();
    }

    /// <summary>
    /// 注册需要加载的配置表
    /// </summary>
    /// <param name="file">配置表文件名</param>
    /// <param name="config">配置表对象</param>
    public void Register(string file, ConfigData config)
    {
        loadList[file] = config;
    }

    /// <summary>
    /// 加载所有已注册的配置表
    /// </summary>
    /// <remarks>
    /// 会遍历所有已注册的配置表，加载并解析它们，然后存入configs字典
    /// </remarks>
    public void LoadAllConfigs()
    {
        foreach (var item in loadList)
        {
            TextAsset textAsset = item.Value.LoadFile();
            item.Value.Load(textAsset.text);
            configs.Add(item.Value.fileName, item.Value);
        }
    }

    /// <summary>
    /// 获取指定名称的配置表
    /// </summary>
    /// <param name="file">配置表文件名</param>
    /// <returns>对应的配置表对象，不存在则返回null</returns>
    public ConfigData GetConfigData(string file)
    {
        if(configs.ContainsKey(file))
        {
            return configs[file];
        }

        return null;
    }
}
