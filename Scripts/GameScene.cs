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

    void Update()
    {
        dt = Time.deltaTime;
        GameApp.Instance.Update(dt);
    }
}
