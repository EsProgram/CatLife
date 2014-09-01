using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using PState = PlayerStateController.PlayerState;

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
    private bool groundTouchFlag;
    private bool aimedFlag;
    private Color alpha = new Color(0, 0, 0, 0.01f);
    private List<Material> materials = new List<Material>();

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

    public bool IsCatched { private get; set; }

    private void Awake()
    {
        addCount = Random.Range(0, ADD_COUNT_MAX);
        psc = PlayerStateController.GetInstance();
    }

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        ac = FindObjectOfType<AimControl>();

        //親を含めた子オブジェクトの全てのマテリアルを取得するための操作
        foreach(Transform child in transform)
        {
            if(child != null && child.renderer != null && child.renderer.material != null)
                foreach(Material mat in child.renderer.materials)
                    materials.Add(mat);
        }
    }

    private void Update()
    {
        //狙っている魚は停止
        if(psc.IsState(PState.AimFish) && ac.CompareAimObject(this.gameObject))
            aimedFlag = true;
        else
        {
            //既に狙われていたことがあった場合の処理
            if(aimedFlag)
            {
                //魚取りに成功した時の処理-----------------------------------------------------
                if(IsCatched)
                {
                }
                //魚取りに失敗した時の処理-----------------------------------------------------
                else
                    TransParents();
            }

            //移動処理
            if(!IsCatched)
                if(moveFlag)
                    cc.SimpleMove(transform.forward * speed * Time.deltaTime);
                else
                    transform.Rotate(transform.up * rotateAng * rotateDir / (FLAG_CHANGE_COUNT + addCount));
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
                if(groundTouchFlag)
                {
                    rotateAng = Random.Range(140, 180);
                    groundTouchFlag = false;
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
        if(hit.gameObject.tag == "Ground" && !groundTouchFlag)
            groundTouchFlag = true;
    }

    /// <summary>
    /// 透明化する
    /// 完全に透明だったらゲームオブジェクトを破棄する
    /// </summary>
    private void TransParents()
    {
        //透明にしていく
        if(materials.Any(m => m.color.a > 0))
            materials.ForEach(m => m.color -= alpha);
        else
            Destroy(this.gameObject);
    }
}