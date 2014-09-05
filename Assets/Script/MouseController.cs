using System.Collections;
using UnityEngine;
using PState = PlayerStateController.PlayerState;

public class MouseController : CreatureController
{
    [SerializeField]
    private int requireCount = 20;//捕まえるのに必要なカウント数
    [SerializeField]
    private int limitTime = 180;//捕まえる制限時間

    /// <summary>
    /// 必要なボタン連打数を返す
    /// </summary>
    public int RequireCount { get { return requireCount; } }

    //制限時間を返す
    public int LimitTime { get { return limitTime; } }
}