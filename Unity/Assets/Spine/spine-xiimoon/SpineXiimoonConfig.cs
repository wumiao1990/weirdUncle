using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineXiimoonConfig : ScriptableObject
{
    //要处理的路径
    public List<string> dopaths = new List<string>();
    //动画数量超过该值才需要拆分
    public int needSplitCount = 0;
    // public bool ingorePathContainBattle = true;
}