using UnityEngine;

/// <summary>
/// 游戏生命周期管理器，处理应用程序暂停、退出等事件
/// 确保在应用程序关闭时保存数据
/// </summary>
public class GameLifecycleManager : MonoBehaviour
{
    // 单例实例
    private static GameLifecycleManager _instance;
    public static GameLifecycleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 在场景中查找已有实例
                _instance = FindObjectOfType<GameLifecycleManager>();
                
                // 如果没有找到实例，则创建新的
                if (_instance == null)
                {
                    GameObject obj = new GameObject("GameLifecycleManager");
                    _instance = obj.AddComponent<GameLifecycleManager>();
                    DontDestroyOnLoad(obj); // 确保在场景切换时不被销毁
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // 单例模式确保只有一个实例
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("GameLifecycleManager已初始化");
    }

    /// <summary>
    /// 当应用程序退出时调用
    /// </summary>
    private void OnApplicationQuit()
    {
        Debug.Log("应用程序即将退出，保存分数数据");
        
        // 确保当前分数已保存到PlayerPrefs
        SaveAllGameData();
    }

    /// <summary>
    /// 当应用程序暂停时调用（包括失去焦点）
    /// </summary>
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("应用程序暂停，保存分数数据");
            
            // 确保当前分数已保存到PlayerPrefs
            SaveAllGameData();
        }
    }

    /// <summary>
    /// 保存所有游戏数据
    /// </summary>
    public void SaveAllGameData()
    {
        // 确保PlayerPrefs中的所有数据都被保存到磁盘
        PlayerPrefs.Save();
        
        Debug.Log("游戏数据已保存");
    }
} 