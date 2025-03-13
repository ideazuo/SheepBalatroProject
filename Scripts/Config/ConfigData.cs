using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于读取和解析CSV格式的配置数据表（逗号分隔值）
/// </summary>
public class ConfigData
{
    /// <summary>
    /// 存储配置表数据的字典。外层键为行ID，内层为每行数据的键值对
    /// </summary>
    private Dictionary<int, Dictionary<string, string>> datas;
    
    /// <summary>
    /// 配置表文件名称
    /// </summary>
    public string fileName;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="fileName">要加载的配置文件名称</param>
    public ConfigData(string fileName)
    {
        this.fileName = fileName;
        this.datas = new Dictionary<int, Dictionary<string, string>>();
    }

    /// <summary>
    /// 从Resources/Data目录下加载配置文件
    /// </summary>
    /// <returns>加载的文本资源</returns>
    public TextAsset LoadFile()
    {
        return Resources.Load<TextAsset>($"Data/{fileName}");
    }

    /// <summary>
    /// 解析配置文件内容并加载到内存中
    /// </summary>
    /// <remarks>
    /// 配置文件格式要求：
    /// - 第一行为字段名（表头）
    /// - 第二行为描述（会被忽略）
    /// - 从第三行开始为数据内容
    /// - 必须包含一个名为"Id"的列作为主键
    /// </remarks>
    /// <param name="txt">要解析的配置文件文本内容</param>
    public void Load(string txt)
    {
        // 按行分割文本
        string[] dataArr = txt.Split("\n", StringSplitOptions.RemoveEmptyEntries);
        // 获取表头作为字典的键
        string[] titleArr = dataArr[0].Trim().Split(",");
        // 从第三行开始读取数据内容（下标从2开始）
        for(int i = 2;i < dataArr.Length;i++)
        {
            string[] tempArr = dataArr[i].Trim().Split(",");
            Dictionary<string, string> tempData = new Dictionary<string, string>();
            for(int j = 0; j < tempArr.Length; j++)
            {
                tempData.Add(titleArr[j], tempArr[j]);
            }
            datas.Add(int.Parse(tempData["Id"]), tempData);
        }
    }

    /// <summary>
    /// 根据ID获取一行配置数据
    /// </summary>
    /// <param name="id">要查询的数据ID</param>
    /// <returns>对应ID的数据字典，不存在则返回null</returns>
    public Dictionary<string,string> GetDataById(int id)
    {
        if(datas.ContainsKey(id))
        {
            return datas[id];
        }
        return null;
    }

    /// <summary>
    /// 获取所有配置数据
    /// </summary>
    /// <returns>包含所有配置数据的字典</returns>
    public Dictionary<int,Dictionary<string,string>> GetLines()
    {
        return datas;
    }
}
