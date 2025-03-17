using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelModel : BaseModel
{
    public int LeveNum;//关卡数

    public LevelModel() : base()
    {
        // 默认初始化
    }

    /// <summary>
    /// 带控制器参数的构造函数
    /// </summary>
    /// <param name="controller">关联的控制器</param>
    public LevelModel(BaseController controller) : base(controller)
    {
        // 调用父类的带参数构造函数，并传递控制器引用
        LeveNum = 1;
    }
}
