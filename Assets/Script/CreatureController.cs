﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PState = PlayerStateController.PlayerState;

/// <summary>
/// ゲーム内での生物の動きを抽象化したクラス
/// 派生した生物クラスはプレイヤーステートを参照して動く
/// 派生した生物クラスは必要な固有のデータのみを保有していれば良い
///
/// インスペクターで接触方向転換タグを指定できる
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SphereCollider))]
public abstract class CreatureController : MonoBehaviour
{
    private int updateCounter;//FixedUpdateにより加算されるカウンタ
    private Vector3 snapGround;
    private Color alpha = new Color(0, 0, 0, 0.03f);
    private List<Material> materials = new List<Material>();
    private bool bigTurnFlag;//指定したタグをもつオブジェクトに接触しているかどうか
    private int ADD_COUNT_MAX = 30;//追加カウント数のMAX
    private int addCount;//追加カウント数(乱数)
    private float rotateAng;//１回の回転行動で回転する角度
    private float rotateDir;//回転方向(左回転、右回転)
    private bool moveFlag;//動く状態かをあらわすフラグ
    private bool aimedFlag;//狙われていたことがあるかどうか
    private bool isTrans;//透明化処理の実行中かどうか

    protected AimControl ac;
    protected CharacterController cc;
    protected PlayerStateController psc;
    protected const int FLAG_CHANGE_COUNT = 30;//moveFlagが反転する基本カウント数

    [SerializeField]
    protected float moveSpeed = 100;
    [SerializeField]
    protected List<string> DirectionBigChangeTag = new List<string>();//接触したら大きく方向転換したいオブジェクトのタグを指定する

    /// <summary>
    /// 捕獲されたかどうか
    /// </summary>
    public bool IsCatched { get; set; }

    /// <summary>
    /// 動ける状態であるかどうかを返す
    /// </summary>
    protected bool IsMovePossible { get { return moveFlag; } }

    protected virtual void Awake()
    {
        addCount = Random.Range(0, ADD_COUNT_MAX);
        psc = PlayerStateController.GetInstance();
        //親を含めた子オブジェクトの全てのマテリアルを取得するための操作
        GetMaterials(transform);
    }

    protected virtual void Start()
    {
        cc = GetComponent<CharacterController>();
        ac = FindObjectOfType<AimControl>();
    }

    /// <summary>
    /// マテリアルを取得
    /// Transformは自分自身と子の情報しか持たない(孫を知らない)ので
    /// 再帰してすべてを取得している
    /// </summary>
    private void GetMaterials(Transform parents)
    {
        foreach(Transform child in parents)
        {
            GetMaterials(child);
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
        Actions();

        if(!IsCatched)
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
        if(DirectionBigChangeTag.Contains(hit.gameObject.tag))
            bigTurnFlag = true;
    }

    /// <summary>
    /// 透明化する
    /// 完全に透明だったらゲームオブジェクトを破棄する
    /// </summary>
    protected void TransParents()
    {
        if(!isTrans)
            isTrans = true;

        //透明にしていく
        if(materials.Any(m => m.color.a > 0))
            materials.ForEach(m => m.color -= alpha);
        else
            Destroy(this.gameObject);
    }

    /// <summary>
    /// 生物の動きを定義する
    /// </summary>
    private void Actions()
    {
        //狙われたらフラグを立てる
        if(psc.IsState(PState.Aim) && ac.CompareAimObject(this.gameObject))
            aimedFlag = true;
        else
        {
            //移動処理
            if(!IsCatched)
                //動ける状態なら動く
                if(IsMovePossible)
                    CharactorMove();
                //そうでなければ回転処理
                else
                    CharactorRotate();

            //狙われたことがあったら
            if(aimedFlag)
                //狩に成功した時の処理
                if(IsCatched)
                {
                    //プレイヤーがOKボタンを押したら
                    if(!psc.IsState(PState.Hunt))
                        TransParents();
                }
                //狩に失敗した時は透明化させる
                else
                    TransParents();
        }
    }

    /// <summary>
    /// 生物の移動をサポートする
    /// </summary>
    protected void CharactorMove()
    {
        cc.SimpleMove(transform.forward * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 生物の回転をサポートする
    /// </summary>
    protected void CharactorRotate()
    {
        transform.Rotate(transform.up * rotateAng * rotateDir / (FLAG_CHANGE_COUNT + addCount));
    }

    /// <summary>
    /// 透明化処理を実行したかどうかを返す
    /// 既に狙ったオブジェクトかどうかを判定するのに使える
    /// </summary>
    /// <returns></returns>
    public bool IsTransed()
    {
        return isTrans;
    }
}