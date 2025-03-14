using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 统一定义游戏中的管理器，在此类进行初始化
/// </summary>
public class GameApp : Singleton<GameApp>
{
    public static SoundManager SoundManager;//音频管理文件

    public static ControllerManager ControllerManager;//控制器管理器

    public static ViewManager ViewManager;//视图管理器

    public static ConfigManager ConfigManager;//配置表管理器

    public static CardManager CardManager;//卡牌管理器

    public override void Init()
    {
        SoundManager = new SoundManager();

        ControllerManager = new ControllerManager();

        ViewManager = new ViewManager();

        ConfigManager = new ConfigManager();

        // 创建并初始化CardManager
        CardManager = new CardManager();
        CardManager.Init();
        
        // 初始化CoroutineHelper
        Debug.Log("确保CoroutineHelper实例已创建");
        // 只需要访问一次instance属性就会触发CoroutineHelper的创建
        var helper = CoroutineHelper.instance;
    }

    public override void Update(float dt)
    {

    }
}
