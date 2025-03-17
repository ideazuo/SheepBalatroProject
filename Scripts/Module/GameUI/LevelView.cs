using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelView : BaseView
{
    protected override void OnAwake()
    {
        base.OnAwake();
        Find<Button>("bg/okBtn").onClick.AddListener(OnClickBtn);
    }

    private void OnClickBtn()
    {
        GameApp.ViewManager.Close(ViewId);
    }
}
