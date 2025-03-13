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
        //Find<Button>("StartBtn").onClick.AddListener(onStartGameBtn);
        
        Button setBtn = Find<Button>("SetBtn");
        if (setBtn != null)
        {
            setBtn.onClick.AddListener(onSetBtn);
        }
        else
        {
            Debug.LogError("在StartView中找不到SetBtn按钮，请检查预制体结构");
        }
        
        Button quitBtn = Find<Button>("QuitBtn");
        if (quitBtn != null)
        {
            quitBtn.onClick.AddListener(onQuitBtn);
        }
        else
        {
            Debug.LogError("在StartView中找不到QuitBtn按钮，请检查预制体结构");
        }
    }

    //开始游戏
    //private void onStartGameBtn()
    //{
    //    //关闭开始界面
    //    GameApp.ViewManager.Close(ViewId);

    //    LoadingModel loadingModel = new LoadingModel();
    //    loadingModel.SceneName = "map";
    //    loadingModel.callback = delegate ()
    //    {
    //        //打开选择关卡界面
    //        Controller.ApplyControllerFunc(ControllerType.Level, Defines.OpenSelectLevelView);
    //    };
    //    Controller.ApplyControllerFunc(ControllerType.Loading, Defines.LoadingScene, loadingModel);
    //}

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
