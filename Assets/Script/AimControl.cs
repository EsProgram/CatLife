using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using PState = PlayerStateController.PlayerState;

/// <summary>
/// アタッチされたゲームオブジェクトの位置に理論球を生成し最初にヒットしたオブジェクトの情報を取り扱う
///
/// </summary>
public sealed class AimControl : MonoBehaviour
{
    private GameObject aim;//ヒットしたゲームオブジェクト
    private RaycastHit rch;
    private GameObject aimPtr;
    private PlayerStateController psc;

    public string[] ignoreTag;//衝突判定を無視するゲームオブジェクトのタグを設定出来る

    [SerializeField]
    private GameObject aimPointerPrefab = default(GameObject);
    [SerializeField]
    private float rayRadius = 1;//レイの半径
    [SerializeField]
    private float rayDistance = 1;//レイの飛距離

    private AimControl()
    {
    }

    private void Awake()
    {
        psc = PlayerStateController.GetInstance();
    }

    private void Start()
    {
        if(this.tag != "AimControl")
            Destroy(this);
        aimPtr = Instantiate(aimPointerPrefab) as GameObject;
        aimPtr.SetActive(false);
    }

    private void Update()
    {
        //レイによってヒットしたオブジェクトの情報を得る
        if(!psc.IsState(PState.AimFish))//狙っている状態の時は特別で、他のオブジェクトを参照しないようにする
            rch = Physics.SphereCastAll(transform.position, rayRadius, transform.forward, rayDistance)
                         .FirstOrDefault(_ => !ignoreTag.Contains(_.collider.tag));

        aim = rch.collider != null ? rch.collider.gameObject : null;

        //AimPointerの表示/非表示
        SwitchAimPtr();
    }

    /// <summary>
    /// 特定の条件でAimPointerの表示/非表示を切り替える
    /// </summary>
    private void SwitchAimPtr()
    {
        if(aim != null && !psc.IsState(PState.HuntFish))
        {
            aimPtr.SetActive(true);
            aimPtr.transform.position = aim.gameObject.transform.position + Vector3.up * 2;
        }
        else if(aim == null || psc.IsState(PState.HuntFish))
            aimPtr.SetActive(false);
    }

    /// <summary>
    /// 狙っているゲームオブジェクトを返す
    /// 無ければnull
    /// </summary>
    /// <returns>
    /// 狙っているゲームオブジェクト
    /// なければNull
    /// </returns>
    public GameObject GetAimObject()
    {
        return aim;
    }

    /// <summary>
    /// 狙っているゲームオブジェクトのタグを返す
    /// </summary>
    /// <returns></returns>
    public string GetAimObjecctTag()
    {
        if(aim == null)
            return string.Empty;
        return aim.tag;
    }

    /// <summary>
    /// 指定したオブジェクトと現在狙ってるオブジェクトが等しいか判定する
    /// </summary>
    /// <param name="target">調べたいゲームオブジェクト</param>
    /// <returns></returns>
    public bool CompareAimObject(GameObject target)
    {
        return ReferenceEquals(aim, target);
    }

    /// <summary>
    /// 指定したタグをもつオブジェクトが狙われているかどうかを返す
    /// 狙っているオブジェクトがなければ常にfalseを返す
    /// </summary>
    /// <param name="tag">調べたいタグ</param>
    /// <returns></returns>
    public bool CompareAimObjectTag(string tag)
    {
        string buf = GetAimObjecctTag();
        return buf != string.Empty ? buf == tag : false;
    }
}