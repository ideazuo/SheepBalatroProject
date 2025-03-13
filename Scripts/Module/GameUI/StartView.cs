using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 开始游戏界面
/// </summary>
public class StartView : BaseView
{
    protected override void OnAwake()
    {
        base.OnAwake();
        Find<Button>("StartBtn").onClick.AddListener(onStartGameBtn);
        Find<Button>("SetBtn").onClick.AddListener(onSetBtn); 
        Find<Button>("QuitBtn").onClick.AddListener(onQuitBtn);

    }

    //开始游戏
    private void onStartGameBtn()
    {
        ApplyFunc(Defines.OpenGameView);
        GameApp.ViewManager.Close(ViewId);
    }

    //打开设置界面
    private void onSetBtn()
    {
        ApplyFunc(Defines.OpenSetView);
    }

    //退出游戏
    private void onQuitBtn()
    {
        Controller.ApplyControllerFunc(ControllerType.GameUI, Defines.OpenMessageView, new MessageInfo()
        {
            okCallback = delegate ()
            {
                Application.Quit();//退出游戏
            },
            MsgTxt = "确定退出游戏吗？",

        });
    }
}
