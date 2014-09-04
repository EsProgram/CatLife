using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ゲーム内での生物の動きを抽象化したクラス
/// このクラスを派生した生物のクラスではMoveメソッドをoverrideして
/// 動きをその生物特有のものにするだけで良い(あとはゲーム上固有の動きをしてくれる)
/// 回転角度、回転方向などは勝手にアップデートしてくれるので気にせず派生クラスで使える
///
/// インスペクターで接触方向転換タグを指定できる
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SphereCollider))]
public abstract class CreatureController : MonoBehaviour
{
    private int updateCounter;//FixedUpdateにより加算されるカウンタ
    private Vector3 snapGround;
    private Color alpha = new Color(0, 0, 0, 0.01f);
    private List<Material> materials = new List<Material>();
    private bool bigTurnFlag;//指定したタグをもつオブジェクトに接触しているかどうか
    private int ADD_COUNT_MAX = 30;//追加カウント数のMAX

    protected AimControl ac;
    protected CharacterController cc;
    protected PlayerStateController psc;
    protected int addCount;//追加カウント数(乱数)
    protected bool moveFlag;//動く状態で歩かないかをあらわすフラグ
    protected float rotateAng;//１回の回転行動で回転する角度
    protected float rotateDir;//回転方向(左回転、右回転)
    protected bool aimedFlag;//狙われているかどうか
    protected const int FLAG_CHANGE_COUNT = 30;//moveFlagが反転する基本カウント数

    [SerializeField]
    protected float moveSpeed = 100;
    [SerializeField]
    protected List<string> bigChangeDirectionTag = new List<string>();//接触したら大きく方向転換したいオブジェクトのタグを指定する

    protected virtual void Awake()
    {
        addCount = Random.Range(0, ADD_COUNT_MAX);
        psc = PlayerStateController.GetInstance();
        //親を含めた子オブジェクトの全てのマテリアルを取得するための操作
        GetMaterials();
    }

    protected virtual void Start()
    {
        cc = GetComponent<CharacterController>();
        ac = FindObjectOfType<AimControl>();
    }

    /// <summary>
    /// オブジェクトの持つマテリアルを取得
    /// </summary>
    private void GetMaterials()
    {
        foreach(Transform child in transform)
        {
            if(child != null && child.renderer != null && child.renderer.material != null)
                foreach(Material mat in child.renderer.materials)
                    materials.Add(mat);
        }
    }

    //キャラクターの動き

    //重力処理
    private void Gravity()
    {
        if(cc.isGrounded)
            snapGround = Vector3.down;
        else
            snapGround = Vector3.zero;
        cc.Move(Physics.gravity * Time.deltaTime + snapGround);
    }

    private void Update()
    {
        //動きの処理
        Move();
        //重力処理
        Gravity();
    }

    /// <summary>
    /// カウントのみで使用
    /// </summary>
    private void FixedUpdate()
    {
        CountAndSwitchAction();
    }

    /// <summary>
    /// カウントし、そのカウント数に応じてフラグを変化させる
    /// Moveでの挙動を変えるためのmoveFlagや回転行動で必要になる
    /// rotateAng,rotateDirを設定する
    /// </summary>
    private void CountAndSwitchAction()
    {
        //カウント
        ++updateCounter;

        //フラグスイッチ(アクションの変更)
        if(updateCounter > FLAG_CHANGE_COUNT + addCount)
        {
            moveFlag = !moveFlag;
            updateCounter = 0;
            addCount = Random.Range(0, ADD_COUNT_MAX);
            //回転行動にスイッチしたら回転変数を決定する
            if(!moveFlag)
            {
                //指定したタグを持つオブジェクトに接触してしまっていたら大きく方向転換する
                if(bigTurnFlag)
                {
                    rotateAng = Random.Range(140, 180);
                    bigTurnFlag = false;
                }
                else
                    rotateAng = Random.Range(50, 100);
                rotateDir = Mathf.Cos(Mathf.PI * Random.Range(-1, 2));
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //指定したタグのオブジェクトに接触したら大きく旋回するようにフラグを立てる
        if(bigChangeDirectionTag.Contains(hit.gameObject.tag))
            bigTurnFlag = true;
    }

    /// <summary>
    /// 透明化する
    /// 完全に透明だったらゲームオブジェクトを破棄する
    /// </summary>
    protected void TransParents()
    {
        //透明にしていく
        if(materials.Any(m => m.color.a > 0))
            materials.ForEach(m => m.color -= alpha);
        else
            Destroy(this.gameObject);
    }

    /// <summary>
    /// 生物の動き
    /// 回転行動以外で、どのように動かすかを定義できる
    /// </summary>
    protected internal abstract void Move();
}