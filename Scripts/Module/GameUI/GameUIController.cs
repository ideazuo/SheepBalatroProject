using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 处理一些游戏通用UI的控制器（设置面板、提示面板、开始游戏面板等在这个控制器注册）
/// </summary>
public class GameUIController : BaseController
{
    public GameUIController() : base()
    {
        //注册开始游戏面板
        GameApp.ViewManager.Register(ViewType.StartView, new ViewInfo()
        {
            PrefabName = "StartView",
            controller = this,
            parentTf = GameApp.ViewManager.canvasTf
        });

        //注册设置面板
        GameApp.ViewManager.Register(ViewType.SetView, new ViewInfo()
        {
            PrefabName = "SetView",
            controller = this,
            Sorting_Order = 1,//挡住开始面板，比他层级高一点
            parentTf = GameApp.ViewManager.canvasTf
        });

        //注册提示面板
        GameApp.ViewManager.Register(ViewType.MessageView, new ViewInfo()
        {
            PrefabName = "MessageView",
            controller = this,
            Sorting_Order = 999,
            parentTf = GameApp.ViewManager.canvasTf
        });

        //注册游戏面板
        GameApp.ViewManager.Register(ViewType.GameView, new ViewInfo()
        {
            PrefabName = "GameView",
            controller = this,
            parentTf = GameApp.ViewManager.canvasTf
        });

        //注册关卡面板
        GameApp.ViewManager.Register(ViewType.LevelView, new ViewInfo()
        {
            PrefabName = "LevelView",
            controller = this,
            Sorting_Order = 1,
            parentTf = GameApp.ViewManager.canvasTf
        });

        //注册胜利面板
        GameApp.ViewManager.Register(ViewType.WinView, new ViewInfo()
        {
            PrefabName = "WinView",
            controller = this,
            Sorting_Order = 1,
            parentTf = GameApp.ViewManager.canvasTf
        });

        //注册失败面板
        GameApp.ViewManager.Register(ViewType.LoseView, new ViewInfo()
        {
            PrefabName = "LoseView",
            controller = this,
            Sorting_Order = 1,
            parentTf = GameApp.ViewManager.canvasTf
        });

        InitModuleEvent();//初始化模板事件
        InitGlobalEvent();//初始化全局事件
    }

    public override void InitModuleEvent()
    {
        RegisterFunc(Defines.OpenStartView, openStartView);//注册打开开始面板
        RegisterFunc(Defines.OpenSetView, openSetView);//注册打开设置面板
        RegisterFunc(Defines.OpenMessageView, openMessageView);//注册打开提示面板
        RegisterFunc(Defines.OpenGameView, openGameView);//注册打开游戏面板
        RegisterFunc(Defines.OpenLevelView, openLevelView);//注册打开关卡面板
        RegisterFunc(Defines.OpenWinView, openWinView);//注册打开胜利面板
        RegisterFunc(Defines.OpenLoseView, openLoseView);//注册打开失败面板
    }

    //打开开始游戏面板
    private void openStartView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.StartView, arg);
    }

    //打开设置面板
    private void openSetView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.SetView, arg);
    }

    //打开提示面板
    private void openMessageView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.MessageView, arg);
    }

    //打开游戏面板
    private void openGameView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.GameView, arg);
    }

    //打开关卡面板
    private void openLevelView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.LevelView, arg);
    }

    //打开胜利面板
    private void openWinView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.WinView, arg);
    }

    //关打开失败面板
    private void openLoseView(System.Object[] arg)
    {
        GameApp.ViewManager.Open(ViewType.LoseView, arg);
    }

}
