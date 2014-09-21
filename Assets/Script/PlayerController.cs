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
    private Animator anim;
    private Vector3 snapGround;//接地時に下方向に加える力
    private CreatureController aimCtrl;//AimFish時に狙っている魚のコントロール
    //private MouseController aimMouseCtrl;//AimMouse時に狙っているネズミのコントロール
    private int countAimTime;//ネズミを狙っている間カウントする
    private bool isHunting;//Hunt中かどうか

    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private GameObject catchEffect;//飛沫
    [SerializeField]
    private GameObject mouth;//口(位置情報を使う)
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
        anim = GetComponent<Animator>();
    }

    private void OnCollisionWater()
    {
        Destroy(GameObject.Instantiate(catchEffect, aimCtrl.gameObject.transform.position + transform.forward * 4, Quaternion.identity), 5);
    }

    private void OnHuntComplete()
    {
        if(aimCtrl != null && aimCtrl.IsCatched)
            MoveOnMouth(aimCtrl);
        isHunting = false;
    }

    /// <summary>
    /// Invokeでの呼び出し用
    /// </summary>
    private void GaugeUnenabled()
    {
        gc.SetEnabled(false);
    }

    /// <summary>
    /// Invokeでの呼び出し用
    /// </summary>
    private void RendaUnenabled()
    {
        rc.SetEnabled(false);
    }

    private void PlayerMove(float speed)
    {
        cc.SimpleMove(transform.forward * psc.GetInputVertical() * speed);
    }

    private void PlayerRotate()
    {
        transform.Rotate(transform.up * psc.GetInputHorizontal() * rotateSpeed);
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
                SetAnimTrigger("IdleTrigger");
                break;

            case PState.Walk:
                SetAnimTrigger("WalkTrigger");
                PlayerMove(walkSpeed);
                break;

            case PState.Run:
                SetAnimTrigger("RunTrigger");
                PlayerMove(runSpeed);
                break;

            case PState.Rotate:
                PlayerRotate();
                break;

            case PState.WalkRotate:
                SetAnimTrigger("WalkTrigger");
                PlayerMove(walkSpeed);
                PlayerRotate();
                break;

            case PState.RunRotate:
                SetAnimTrigger("RunTrigger");
                PlayerMove(runSpeed);
                PlayerRotate();
                break;

            case PState.AimFish:
                SetAnimTrigger("AimTrigger");
                AimFishProc();
                break;

            case PState.HuntFish:
                SetAnimTrigger("HuntTrigger");
                HuntFishProc();
                break;

            case PState.AimMouse:
                SetAnimTrigger("AimTrigger");
                AimMouseProc();
                break;

            case PState.HuntMouse:
                SetAnimTrigger("HuntTrigger");
                HuntMouseProc();
                break;

            default:
                break;
        }

        Gravity();
    }

    /// <summary>
    /// 指定した名前のアニメーショントリガーをセットする
    /// </summary>
    /// <param name="name"></param>
    private void SetAnimTrigger(string name)
    {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName(name))
            anim.SetTrigger(name);
    }

    private void HuntMouseProc()
    {
        //捕獲出来たら初回のみ呼び出される
        if(rc.IsEnabled() && !IsInvoking("RendaUnenabled"))
        {
            isHunting = true;//Hunt中
            Invoke("RendaUnenabled", 0.5f);
            PrintMessage.Add(aimCtrl.name.Split('(').FirstOrDefault() + "が取れました");
        }
        //OKボタンが押されたかつHuntモーションが終了したら
        if(psc.GetInputOK() && !isHunting)
            psc.SetStateIdle();
    }

    private void AimMouseProc()
    {
        //AimMouse初回時のみ呼ばれるはずの処理
        if(!rc.IsEnabled())
        {
            //ネズミのコントロールを得る
            SetAimCtrl<CreatureController>(out aimCtrl);
            rc.SetEnabled(true);
            countAimTime = 0;
            //ネズミから必要な連打数や連打可能時間を取得
        }
        //Huntボタンが押されたらカウント
        if(psc.GetInputHunt())
            rc.Increment();
        //狩が成功したら
        if(rc.GetCount() > ((MouseController)aimCtrl).RequireCount)
        {
            aimCtrl.IsCatched = true;
            psc.SetHuntMouse();
        }
        //時間が過ぎれば逃げられてしまう
        if(countAimTime > ((MouseController)aimCtrl).LimitTime)
        {
            psc.SetStateIdle();
            Invoke("RendaUnenabled", 0.5f);
            PrintMessage.Add("逃げられてしまった...");
        }
        ++countAimTime;
    }

    private void HuntFishProc()
    {
        //Hunt初回のみ呼び出される
        if(gc.IsEnabled() && !IsInvoking("GaugeUnenabled"))
        {
            isHunting = true;
            //ゲージを非表示に
            Invoke("GaugeUnenabled", 0.5f);
            //魚が近くにいた場合
            if(aimCtrl != null)
            {
                //魚が取れた
                if(gc.IsPermit())
                {
                    PrintMessage.Add(aimCtrl.name.Split('(').FirstOrDefault() + "が取れました！");
                    aimCtrl.IsCatched = true;
                }
                //魚が取れなかった
                else
                {
                    PrintMessage.Add("お魚を逃したようです");
                    aimCtrl.IsCatched = false;
                }
            }
            else
                PrintMessage.Add("お魚が近くにいませんでした");
        }
        //Hunt状態を抜ける処理(OKボタンが押される)
        if(aimCtrl == null || aimCtrl.IsCatched == false || psc.GetInputOK() && !isHunting)
        {
            //Idle状態に戻す処理
            psc.SetStateIdle();
            //しばらくAimFish状態になれなくなる処理
            psc.DisabledAimCertainTime();
        }
    }

    private void AimFishProc()
    {
        //AimFish遷移初回時のみ呼び出される
        if(!gc.IsEnabled())
        {
            SetAimCtrl<CreatureController>(out aimCtrl);

            SetPermitZone((FishController)aimCtrl);
            //ゲージの有効化
            gc.SetEnabled(true);
        }

        //ゲージを動かす処理
        gc.GaugeMove(aimCtrl != null ? ((FishController)aimCtrl).GaugeSpeed : 1f);
    }

    /// <summary>
    /// 指定した生物を口に移動する
    /// </summary>
    private void MoveOnMouth(CreatureController creature)
    {
        creature.gameObject.transform.rotation = mouth.transform.rotation;
        creature.gameObject.transform.position = mouth.transform.position;
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
    ///狙っている生物のコントロールを得る
    /// </summary>
    private void SetAimCtrl<T>(out T aimCtrl) where T : CreatureController
    {
        aimCtrl = default(T);
        var aimFish = ac.GetAimObject();
        if(aimFish != null)
            aimCtrl = aimFish.gameObject.GetComponent<T>();
        else
            aimCtrl = null;
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