using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class AimControl
{
    private static AimControl _singleton;
    private List<Fish> fishes;
    private ManagedFishInstantiate mfi;
    private GameObject aimTrigger;

    private AimControl()
    {
        mfi = GameObject.FindObjectOfType<ManagedFishInstantiate>();
        fishes = mfi.fishes;
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
    /// </summary>
    /// <param name="distance">魚とAimTriggerの距離</param>
    /// <returns>魚。いなければnull</returns>
    public Fish AimingFish(float distance)
    {
        var fish = fishes.FirstOrDefault(f => Vector3.Distance(aimTrigger.transform.position, f.gameObject.transform.position) < distance);
        return fish;
    }
}