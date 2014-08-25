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
    public float walkSpeed;
    public float rotateSpeed;
    public float runSpeed;
    public float aimFishDistance;//魚を狙える距離
    public GUITexture gauge;//ゲージ本体
    public GUITexture frame;//ゲージの枠
    public float gaugeSpeed;//ゲージ速度(後に魚によって変更させる)

    private void Awake()
    {
        psc = PlayerStateController.GetInstance();
        gc = GaugeController.CreateInstance(gauge, frame);
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
                //ゲージの処理(表示・伸び縮み)
                if(!gc.IsGaugeEnabled())
                    gc.GaugeEnabled(true);
                gc.GaugeMove(gaugeSpeed);
                //"AimFish"ボタンを押したら(２度目)アニメーションして魚にメッセージ送って状態遷移フラグを立てる
                if(Input.GetButton("Hunt"))
                {
                    Debug.Log("ゲージ停止位置 : " + gc.GetGaugeValue());
                    //ゲージの非表示
                    Invoke("GaugeUnenabled", 1f);
                    //狙った魚の取得
                    var aimfish = ac.AimingFish(aimFishDistance);
                    if(aimfish != null)
                    {
                        //if(/*ゲージが一定値ならば魚にメッセージ等*/) { }
                        Debug.Log("お魚が取れました（仮）");
                    }

                    //Idle状態に戻す処理
                    psc.EndAimFishState();
                    //しばらくAimFish状態になれなくなる処理
                    psc.ResetUpdateCounterForAimToAim();
                }
                break;

            default:
                break;
        }
        //重力処理
        cc.Move(new Vector3(0f, Physics.gravity.y * Time.deltaTime, 0f));
    }
}