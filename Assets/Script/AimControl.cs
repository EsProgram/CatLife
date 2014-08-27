using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class AimControl
{
    private static AimControl _singleton;
    private ManagedFishInstantiate mfi;
    private PlayerStateController psc;
    private GameObject aimTrigger;
    private const float AIM_FISH_DISTANCE = 0.8f;
    private Fish fish_buf;

    private AimControl()
    {
        mfi = GameObject.FindObjectOfType<ManagedFishInstantiate>();
        aimTrigger = GameObject.FindGameObjectWithTag("AimTrigger");
        psc = PlayerStateController.GetInstance();
    }

    public static AimControl GetInstance()
    {
        if(_singleton == null)
            _singleton = new AimControl();
        return _singleton;
    }

    /// <summary>
    /// 距離で判定し、全魚から狙っている魚を走査して最初にヒットした1匹を返す
    /// 呼び出し後、結果にかかわらず任意の場所でSetAimFlagFalseを呼び出す必要がある
    /// PlayerStateがAimFishでなければ常にnullを返す
    /// </summary>
    /// <param name="distance">魚とAimTriggerの距離</param>
    /// <returns>魚。いなければnull</returns>
    public Fish AimingFish()
    {
        if(psc.GetState() != PlayerStateController.PlayerState.AimFish)
            return null;

        var fish = mfi.fishes.FirstOrDefault(f => Vector3.Distance(aimTrigger.transform.position, f.gameObject.transform.position) < AIM_FISH_DISTANCE);
        return fish_buf = fish;
    }

    /// <summary>
    ///引数に指定した魚が狙われている状態かどうかを返す
    ///最後に呼び出されたAimingFishで返却されたFishオブジェクトと比較されることに注意
    ///PlayerStateがAimFishでなければ常にfalseを返す
    /// </summary>
    /// <param name="targetFish">対象の魚ゲームオブジェクト</param>
    /// <returns>引数に指定した魚が狙われた状態かどうか</returns>
    public bool IsAim(GameObject targetFish)
    {
        //プレイヤーがAimFish状態でなければ常にfalse
        if(psc.GetState() != PlayerStateController.PlayerState.AimFish)
            return false;
        if(targetFish == null || fish_buf == null)
            return false;

        if(targetFish.gameObject.tag != "Fish")
        {
            Debug.LogError("不正なタグをもつオブジェクトが引数に渡されました");
            return false;
        }
        return ReferenceEquals(fish_buf.gameObject, targetFish);
    }
}