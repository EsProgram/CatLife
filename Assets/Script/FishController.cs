using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FishController : MonoBehaviour
{
    public float speed;

    private AimControl ac;
    private CharacterController cc;
    private PlayerStateController psc;
    private bool moveFlag;
    private int updateCounter;//FixedUpdateにより加算されるカウンタ
    private const int FLAG_CHANGE_COUNT = 30;//moveFlagが反転する基本カウント数
    private int ADD_COUNT_MAX = 30;//追加カウント数のMAX
    private int addCount;//追加カウント数(乱数)
    private Vector3 snapGround;
    private float rotateAng;//１回の回転行動で回転する角度
    private float rotateDir;//回転方向(左回転、右回転)
    private bool groundOrPlayerTouchFlag;

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

    private void Awake()
    {
        addCount = Random.Range(0, ADD_COUNT_MAX);
        psc = PlayerStateController.GetInstance();
    }

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        ac = FindObjectOfType<AimControl>();
    }

    private void Update()
    {
        //狙っている魚は停止
        if(psc.GetState() == PlayerStateController.PlayerState.AimFish && ac.CompareAimObject(this.gameObject)) { /*何もしない(魚停止)*/}
        else
        {
            //moveFlagがtrueなら動く
            //falseなら回転
            if(moveFlag)
            {
                cc.SimpleMove(transform.forward * speed * Time.deltaTime);
            }
            else
            {
                transform.Rotate(transform.up * rotateAng * rotateDir / (FLAG_CHANGE_COUNT + addCount));
            }
        }

        //重力処理
        if(cc.isGrounded)
            snapGround = Vector3.down;
        else
            snapGround = Vector3.zero;
        cc.Move(Physics.gravity * Time.deltaTime + snapGround);
    }

    private void FixedUpdate()
    {
        //カウント
        ++updateCounter;

        //フラグスイッチ
        if(updateCounter > FLAG_CHANGE_COUNT + addCount)
        {
            moveFlag = !moveFlag;
            updateCounter = 0;
            addCount = Random.Range(0, ADD_COUNT_MAX);
            //回転行動にスイッチしたら回転変数を決定する
            if(!moveFlag)
            {
                //大地に接触してしまっていたら大きく方向転換する
                if(groundOrPlayerTouchFlag)
                {
                    rotateAng = Random.Range(140, 180);
                    groundOrPlayerTouchFlag = false;
                }
                else
                    rotateAng = Random.Range(50, 100);
                rotateDir = Mathf.Cos(Mathf.PI * Random.Range(-1, 2));
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //"Ground"と接触していたら強制的に回転方向を変更する
        if(hit.gameObject.tag == "Ground" || hit.gameObject.tag == "Player" && !groundOrPlayerTouchFlag)
            groundOrPlayerTouchFlag = true;
    }
}