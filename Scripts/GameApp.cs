﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 统一定义游戏中的管理器，在此类进行初始化
/// </summary>
public class GameApp : Singleton<GameApp>
{
    public static SoundManager SoundManager;//音频管理文件

    public static ControllerManager ControllerManager;//控制器管理器

    public override void Init()
    {
        SoundManager = new SoundManager();

        ControllerManager = new ControllerManager();
    }

    public override void Update(float dt)
    {

    }
}
