using System.Collections;
using System.Linq;
using UnityEngine;

using PState = PlayerStateController.PlayerState;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController cc;
    private PlayerStateController psc;
    private GaugeController gc;
    private AimControl ac;
    private RendaController rc;
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
    [SerializeField]
    private GUITexture renda = default(GUITexture);

    private PlayerController()
    {
    }

    private void Awake()
    {
        psc = PlayerStateController.GetInstance();
        gc = GaugeController.GetInstance(gauge, frame, permit);
        gc.SetEnabled(false);
        rc = RendaController.GetInstance(renda);
        rc.SetEnabled(false);
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
        gc.SetEnabled(false);
    }

    private void Update()
    {
        //プレイヤーの状態の更新
        psc.UpdateState();
    }

    private void FixedUpdate()
    {
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
                AimFishProc();
                break;

            case PState.HuntFish:
                HuntFishProc();
                break;

            case PState.AimMouse:
                AimMouseProc();
                break;

            case PState.HuntMouse:
                if(psc.GetInputHunt())
                    Debug.Log("HuntMouse");
                break;

            default:
                break;
        }

        Gravity();
    }

    private void AimMouseProc()
    {
        //AimMouse初回時のみ呼ばれるはずの処理
        if(!rc.IsEnabled())
        {
            rc.SetEnabled(true);
            //ネズミから必要な連打数や連打可能時間を取得
        }
        //ボタンが押されていたらカウント
        if(psc.GetInputAim())
            rc.Increment();
        //カウントが必要数以上になったらHuntMouseに移行
        if(rc.GetCount() > 10)
            psc.SetHuntMouse();
    }

    /// <summary>
    /// HuntFish時の処理
    /// </summary>
    private void HuntFishProc()
    {
        //Hunt初回のみ呼び出される
        if(gc.IsEnabled() && !IsInvoking("GaugeUnenabled"))
        {
            //ゲージを非表示に
            Invoke("GaugeUnenabled", 0.5f);
            //魚が近くにいた場合
            if(aimFishCtrl != null)
            {
                //魚が取れた
                if(gc.IsPermit())
                {
                    PrintMessage.Add(aimFishCtrl.name.Split('(').FirstOrDefault() + "が取れました！");
                    aimFishCtrl.IsCatched = true;
                    //魚を口元に移動
                    aimFishCtrl.gameObject.transform.position = mouth.transform.position;
                    aimFishCtrl.gameObject.transform.rotation = mouth.transform.rotation;
                }
                //魚が取れなかった
                else
                {
                    PrintMessage.Add("お魚を逃したようです");
                    aimFishCtrl.IsCatched = false;
                }
            }
            else
                PrintMessage.Add("お魚が近くにいませんでした");
        }
        //Hunt状態を抜ける処理(OKボタンが押される)
        if(aimFishCtrl == null || aimFishCtrl.IsCatched == false || psc.GetInputOK())
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
    private void AimFishProc()
    {
        //AimFish遷移初回時のみ呼び出される
        if(!gc.IsEnabled())
        {
            SetAimFishCtrl();
            SetPermitZone(aimFishCtrl);

            //ゲージの有効化
            gc.SetEnabled(true);
        }

        //ゲージを動かす処理
        gc.GaugeMove(aimFishCtrl != null ? aimFishCtrl.GaugeSpeed : 1f);
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