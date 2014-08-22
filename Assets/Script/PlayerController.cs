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
    public float walkSpeed;
    public float rotateSpeed;
    public float runSpeed;
    public GUITexture gauge;
    public GUITexture frame;
    public float gaugeSpeed;

    private void Awake()
    {
        psc = PlayerStateController.GetInstance();
        gc = GaugeController.CreateInstance(gauge, frame);
        gc.GaugeEnabled(false);
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
                    Debug.Log(gc.GetGaugeValue());
                    //魚への処理

                    //ゲージの非表示
                    Invoke("GaugeUnenabled", 1f);
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