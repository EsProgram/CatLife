using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class AimControl
{
    private static AimControl _singleton;
    private ManagedFishInstantiate mfi;
    private GameObject aimTrigger;
    private const float AIM_FISH_DISTANCE = 0.8f;
    private bool isAimFlag;//AimingFishにより魚を取得できたらtrue,プレイヤーHunt時にFalseAimFlagを呼び出す必要がある

    private AimControl()
    {
        mfi = GameObject.FindObjectOfType<ManagedFishInstantiate>();
        aimTrigger = GameObject.FindGameObjectWithTag("AimTrigger");
    }

    public static AimControl GetInstance()
    {
        if(_singleton == null)
            _singleton = new AimControl();
        return _singleton;
    }

    /// <summary>
    /// 距離で判定し、狙っている魚1匹を返す
    /// 呼び出し後、結果にかかわらず任意の場所でSetAimFlagFalseを呼び出す必要がある
    /// </summary>
    /// <param name="distance">魚とAimTriggerの距離</param>
    /// <returns>魚。いなければnull</returns>
    public Fish AimingFish()
    {
        var fish = mfi.fishes.FirstOrDefault(f => Vector3.Distance(aimTrigger.transform.position, f.gameObject.transform.position) < AIM_FISH_DISTANCE);
        if(fish != null)
            isAimFlag = true;
        else
            isAimFlag = false;
        return fish;
    }

    public void SetAimFlagFalse()
    {
        isAimFlag = false;
    }

    /// <summary>
    /// 引数に指定した魚がAim範囲内かどうかを返す
    /// AimimgFishメソッド呼び出し時の結果を返すことに注意
    /// </summary>
    /// <returns>魚がAim範囲内かどうか</returns>
    public bool IsAim()
    {
        return isAimFlag;
    }
}