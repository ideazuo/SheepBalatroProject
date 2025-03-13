using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    float dt;
    void Awake()
    {
        GameApp.Instance.Init();
    }

    void Start()
    {
        //播放背景音乐
        GameApp.SoundManager.PlayBGM("StartBgm");

        //注册配置表
        RegisterConfigs();

        GameApp.ConfigManager.LoadAllConfigs();//加载配置表

        RegisterController();//注册游戏中的控制器
        InitModule();
    }

    //注册控制器
    void RegisterController()
    {
        GameApp.ControllerManager.Register(ControllerType.GameUI, new GameUIController());
        GameApp.ControllerManager.Register(ControllerType.Game, new GameController());
        GameApp.ControllerManager.Register(ControllerType.Card, new CardController());
    }

    //执行所有控制器初始化
    void InitModule()
    {
        GameApp.ControllerManager.InitAllControllers();
    }

    //注册配置表
    void RegisterConfigs()
    {
        GameApp.ConfigManager.Register("hand", new ConfigData("hand"));
    }

    void Update()
    {
        dt = Time.deltaTime;
        GameApp.Instance.Update(dt);
    }
}
