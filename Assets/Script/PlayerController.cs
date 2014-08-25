/*
 * プレイヤーの動きを制御する。
 * コントローラー(キーボード)による操作の制御
 */

using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Represents the player's behavior
    /// </summary>

    private CharacterController cc;
    private PlayerStateController psc;
    private GaugeController gc;
    private AimControl ac;
    private Vector3 snapGround;//接地時に下方向に加える力
    public float walkSpeed;
    public float rotateSpeed;
    public float runSpeed;
    public float aimFishDistance;//魚を狙える距離
    public GUITexture gauge;//ゲージ本体
    public GUITexture frame;//ゲージの枠
    public GUITexture permit;//ゲージの許可範囲
    public float gaugeSpeed;//ゲージ速度(後に魚によって変更させる)

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
                //ゲージの処理(表示・パーミット設定)
                if(!gc.IsGaugeEnabled())
                {
                    //本来なら魚によって幅をまちまちにする。今はとりあえずランダム(不正な値を取る場合がある)
                    gc.SetPermitXAndWidth(Random.Range(0, 244), Random.Range(0, 244));
                    gc.GaugeEnabled(true);
                }
                //"Hunt"ボタンを押したらアニメーションして魚にメッセージ送って状態遷移フラグを立てる
                if(Input.GetButton("Hunt"))
                {
                    //Debug.Log("ゲージ停止位置 : " + gc.GetGaugeValue());
                    //Debug.Log("ゲージパーミット : " + gc.IsGaugePermit());

                    //ゲージの非表示
                    Invoke("GaugeUnenabled", 1f);
                    //狙った魚の取得
                    var aimfish = ac.AimingFish(aimFishDistance);
                    //魚が近くにいてたら
                    if(aimfish != null)
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
                }
                else
                    //ゲージを動かす
                    gc.GaugeMove(gaugeSpeed);
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