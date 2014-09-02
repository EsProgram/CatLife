using System.Collections;
using UnityEngine;

using PState = PlayerStateController.PlayerState;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController cc;
    private PlayerStateController psc;
    private GaugeController gc;
    private AimControl ac;
    private Vector3 snapGround;//接地時に下方向に加える力
    private FishController aimFishCtrl;//AimFish時に狙っている魚のコントロールを格納する
    private GameObject mouth = default(GameObject);//口(位置情報を使う)

    public float walkSpeed;
    public float rotateSpeed;
    public float runSpeed;

    [SerializeField]
    private GUITexture gauge = default(GUITexture);//ゲージ本体
    [SerializeField]
    private GUITexture frame = default(GUITexture);//ゲージの枠
    [SerializeField]
    private GUITexture permit = default(GUITexture);//ゲージの許可範囲

    private PlayerController()
    {
    }

    private void Awake()
    {
        psc = PlayerStateController.GetInstance();
        gc = GaugeController.CreateInstance(gauge, frame, permit);
        gc.GaugeEnabled(false);
    }

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        ac = FindObjectOfType<AimControl>();
        mouth = GameObject.FindGameObjectWithTag("Mouth");
    }

    /// <summary>
    /// Invokeでの呼び出し用
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
            case PState.Idle:
                //Idle処理
                break;

            case PState.WalkForward:
                cc.SimpleMove(transform.forward * psc.GetInputVertical() * walkSpeed);
                break;

            case PState.WalkBack:
                cc.SimpleMove(transform.forward * psc.GetInputVertical() * walkSpeed);
                break;

            case PState.Run:
                cc.SimpleMove(transform.forward * psc.GetInputVertical() * runSpeed);
                break;

            case PState.Rotate:
                transform.Rotate(transform.up * psc.GetInputHorizontal() * rotateSpeed);
                break;

            case PState.AimFish:
                AimingProc();
                break;

            case PState.Hunt:
                HuntProc();
                break;

            default:
                break;
        }

        Gravity();
    }

    /// <summary>
    /// Hunt時の処理
    /// </summary>
    private void HuntProc()
    {
        //ゲージの無効化
        if(gc.IsGaugeEnabled())
            Invoke("GaugeUnenabled", 1f);
        //魚が近くにいた場合
        if(aimFishCtrl != null)
        {
            //魚が取れた
            if(gc.IsGaugePermit())
            {
                Debug.Log("お魚が取れました");
                aimFishCtrl.IsCatched = true;
                //魚を口元に移動
                aimFishCtrl.gameObject.transform.position = mouth.transform.position;
                aimFishCtrl.gameObject.transform.rotation = mouth.transform.rotation;
            }
            //魚が取れなかった
            else
            {
                Debug.Log("お魚を取れませんでした");
                aimFishCtrl.IsCatched = false;
            }
        }
        else
            Debug.Log("お魚が近くにいませんでした");

        //Hunt状態を抜ける処理
        if(aimFishCtrl == null || aimFishCtrl.IsCatched == false || Input.GetButton("OK"))
        {
            //Idle状態に戻す処理
            psc.SetStateIdle();
            //しばらくAimFish状態になれなくなる処理
            psc.DisabledAimCertainTime();
        }
    }

    /// <summary>
    /// AimFish状態中での処理
    /// </summary>
    private void AimingProc()
    {
        if(psc.IsState(PState.AimFish))
        {
            //AimFish遷移初回時のみ呼び出される
            if(!gc.IsGaugeEnabled())
            {
                SetAimFishCtrl();
                SetPermitZone(aimFishCtrl);

                //ゲージの有効化
                gc.GaugeEnabled(true);
            }

            //ゲージを動かす処理
            gc.GaugeMove(aimFishCtrl != null ? aimFishCtrl.GaugeSpeed : 1f);
        }
    }

    /// <summary>
    /// AimFish状態でのみ作用する
    /// 指定したFishControllerを参照してゲージのパーミットを設定
    /// 引数がnullなら適当な値を設定する
    /// </summary>
    /// <param name="aimFishCtrl"></param>
    private void SetPermitZone(FishController aimFishCtrl)
    {
        if(psc.IsState(PState.AimFish))
            if(aimFishCtrl != null)
                gc.SetPermitXAndWidth(Random.Range(0f, GaugeController.MAX - aimFishCtrl.PermitWidth), aimFishCtrl.PermitWidth);
            else
                gc.SetPermitXAndWidth(Random.Range(0f, GaugeController.HALF), Random.Range(20f, 100f));
    }

    /// <summary>
    ///狙っている魚のコントロールを得る
    /// </summary>
    private void SetAimFishCtrl()
    {
        aimFishCtrl = default(FishController);
        var aimFish = ac.GetAimObject();
        if(aimFish != null)
            aimFishCtrl = aimFish.gameObject.GetComponent<FishController>();
    }

    /// <summary>
    /// 重力処理
    /// </summary>
    private void Gravity()
    {
        if(cc.isGrounded)
            snapGround = Vector3.down;
        else
            snapGround = Vector3.zero;
        cc.Move(Physics.gravity * Time.deltaTime + snapGround);
    }
}