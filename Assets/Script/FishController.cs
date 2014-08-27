using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FishController : MonoBehaviour
{
    public float speed;

    private AimControl ac;
    private CharacterController cc;
    private bool moveFlag;
    private int updateCounter;//FixedUpdateにより加算されるカウンタ
    private const int FLAG_CHANGE_COUNT = 200;//moveFlagが反転するカウント数
    private Vector3 snapGround;

    /*ゲージ関連*/
    [SerializeField]
    private float gaugeSpeed;
    [SerializeField]
    private float permitWidth;

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
        ac = AimControl.GetInstance();
    }

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        //Aim範囲内なら何もしない(停止)
        if(ac.IsAim(this.gameObject)) { /*何もしない(魚停止)*/}
        else
        {
            //moveFlagがtrueなら動く
            //falseなら何もしない
            if(moveFlag)
            {
                cc.SimpleMove(transform.forward * speed * Time.deltaTime);
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
        ++updateCounter;

        //フラグスイッチ
        if(updateCounter > FLAG_CHANGE_COUNT)
        {
            moveFlag = !moveFlag;
            updateCounter = 0;
        }
    }
}