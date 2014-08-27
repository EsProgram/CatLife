/*
 * プレイヤーの動きを制御する。
 * コントローラー(キーボード)による操作の制御
 */

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController cc;
    private PlayerStateController psc;
    private GaugeController gc;
    private AimControl ac;
    private Vector3 snapGround;//接地時に下方向に加える力
    private FishController aimFishCtrl;//AimFish時に狙っている魚のコントロールを格納する

    public float walkSpeed;
    public float rotateSpeed;
    public float runSpeed;

    [SerializeField]
    private GUITexture gauge;//ゲージ本体
    [SerializeField]
    private GUITexture frame;//ゲージの枠
    [SerializeField]
    private GUITexture permit;//ゲージの許可範囲

    private void Awake()
    {
        psc = PlayerStateController.GetInstance();
        gc = GaugeController.CreateInstance(gauge, frame, permit);
        gc.GaugeEnabled(false);
        ac = AimControl.GetInstance();
    }

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Invokeメソッドでの呼び出し用
    /// </summary>
    private void GaugeUnenabled()
    {
        gc.GaugeEnabled(false);
    }

    private void FixedUpdate()
    {
        //プレイヤーの状態を更新
        psc.Update();

        //状態における処理
        switch(psc.GetState())
        {
            case PlayerStateController.PlayerState.Idle:
                //Idle処理
                break;

            case PlayerStateController.PlayerState.WalkForward:
                cc.SimpleMove(transform.forward * psc.GetInputVertical() * walkSpeed);
                break;

            case PlayerStateController.PlayerState.WarkBack:
                cc.SimpleMove(transform.forward * psc.GetInputVertical() * walkSpeed);
                break;

            case PlayerStateController.PlayerState.Run:
                cc.SimpleMove(transform.forward * psc.GetInputVertical() * runSpeed);
                break;

            case PlayerStateController.PlayerState.Rotate:
                transform.Rotate(transform.up * psc.GetInputHorizontal() * rotateSpeed);
                break;

            case PlayerStateController.PlayerState.AimFish:

                //ゲージがまだ表示されてなければゲージの表示
                if(!gc.IsGaugeEnabled())
                {
                    //魚のコントロールを得る(狙い中の魚を得たらAimフラグをtrueにし、魚を動けない状態に遷移させる)
                    aimFishCtrl = default(FishController);
                    var aimFish = ac.AimingFish();
                    if(aimFish != null)
                        aimFishCtrl = aimFish.gameObject.GetComponent<FishController>();

                    //付近に魚がいた場合はその魚の値を参照してゲージのパーミットを設定する
                    //それ以外なら適当に設定する
                    if(aimFishCtrl != null)
                        gc.SetPermitXAndWidth(Random.Range(0f, GaugeController.MAX - aimFishCtrl.PermitWidth), aimFishCtrl.PermitWidth);
                    else
                        gc.SetPermitXAndWidth(Random.Range(0f, GaugeController.HALF), Random.Range(20f, 100f));

                    //ゲージの有効化
                    gc.GaugeEnabled(true);
                }

                //"Hunt"ボタンを押したらアニメーションして魚にメッセージ送って状態遷移フラグを立てる
                if(Input.GetButton("Hunt"))
                {
                    //ゲージの非表示
                    Invoke("GaugeUnenabled", 1f);
                    //魚が近くにいてたら
                    if(aimFishCtrl != null)
                    {
                        //ゲージが許可範囲内で停止したら
                        if(gc.IsGaugePermit())
                            Debug.Log("お魚が取れました");
                        else
                            Debug.Log("お魚を取れませんでした");
                    }
                    else
                        Debug.Log("お魚が近くにいませんでした");

                    //Idle状態に戻す処理
                    psc.EndAimFishState();
                    //しばらくAimFish状態になれなくなる処理
                    psc.ResetUpdateCounterForAimToAim();
                    //魚のAim状態を解除する(Aimフラグを解除し魚が動き回れる状態に遷移する)
                    ac.SetAimFlagFalse();
                }
                else
                    //ゲージを動かす
                    gc.GaugeMove(aimFishCtrl != null ? aimFishCtrl.GaugeSpeed : 1f);
                break;

            default:
                break;
        }

        //重力処理
        if(cc.isGrounded)
            snapGround = Vector3.down;
        else
            snapGround = Vector3.zero;
        cc.Move(Physics.gravity * Time.deltaTime + snapGround);
    }
}