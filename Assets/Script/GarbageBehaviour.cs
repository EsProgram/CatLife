﻿using System.Collections;
using UnityEngine;

using PState = PlayerStateController.PlayerState;

public class GarbageBehaviour : MonoBehaviour
{
    private bool isLooked;//既に見たことがあるかどうか
    private AimControl ac;
    private PlayerStateController psc;

    private void Awake()
    {
        psc = PlayerStateController.GetInstance();
    }

    private void Start()
    {
        if(this.tag != "Garbage")
            Destroy(this);

        ac = FindObjectOfType<AimControl>();
    }

    private void Update()
    {
        //条件が細かいが
        //今後それぞれの条件で色々付け足していく予定なので気にしない

        //ゴミが狙われていたら
        if(!psc.IsState(PState.AimFish | PState.Hunt))
            if(ac.CompareAimObject(this.gameObject))
            {
                //OKボタンが押されたら
                if(psc.GetInputOK())
                {
                    //初回の探索だったら
                    if(!isLooked)
                    {
                        PrintMessage.Add("ゴミから食べ物を見つけた");
                        isLooked = true;
                    }
                    else
                        PrintMessage.Add("何も見つからなかった");
                }
            }
    }
}