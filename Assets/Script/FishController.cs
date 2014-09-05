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

    protected override void Awake()
    {
        base.Awake();
        DirectionBigChangeTag.Add("Ground");
    }
}