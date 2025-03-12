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
    }

    void Update()
    {
        dt = Time.deltaTime;
        GameApp.Instance.Update(dt);
    }
}
