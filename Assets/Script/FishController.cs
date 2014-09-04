using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using PState = PlayerStateController.PlayerState;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SphereCollider))]
public sealed class FishController : CreatureController
{
    /*ゲージ関連*/
    [SerializeField]
    private float gaugeSpeed = default(float);
    [SerializeField]
    private float permitWidth = default(float);

    private FishController()
    {
    }

    /// <summary>
    /// この魚のゲージ速度を返す
    /// </summary>
    public float GaugeSpeed { get { return gaugeSpeed; } }

    /// <summary>
    /// 許可範囲を返す
    /// </summary>
    public float PermitWidth { get { return permitWidth; } }

    /// <summary>
    /// 捕獲されたかどうか
    /// </summary>
    public bool IsCatched { get; set; }

    protected override void Awake()
    {
        base.Awake();
        bigChangeDirectionTag.Add("Ground");
    }

    protected internal override void Move()
    {
        //狙っている魚は停止
        if(psc.IsState(PState.AimFish) && ac.CompareAimObject(this.gameObject))
            aimedFlag = true;
        else
        {
            //既に狙われていたことがあった場合の処理
            if(aimedFlag)
            {
                //魚取りに成功した時の処理
                if(IsCatched)
                {
                    //プレイヤーがOKボタンを押したら状態が変わる。魚を破棄する
                    if(!psc.IsState(PState.HuntFish))
                        TransParents();
                }
                //魚取りに失敗した時は透明化させる
                else
                    TransParents();
            }

            //移動処理
            if(!IsCatched)
                if(moveFlag)
                    cc.SimpleMove(transform.forward * moveSpeed);
                else
                    transform.Rotate(transform.up * rotateAng * rotateDir / (FLAG_CHANGE_COUNT + addCount));
        }
    }
}