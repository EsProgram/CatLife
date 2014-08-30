using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// アタッチされたゲームオブジェクトの位置に理論球を生成し最初にヒットしたオブジェクトの情報を取り扱う
///
/// </summary>
public sealed class AimControl : MonoBehaviour
{
    private GameObject aim;
    private RaycastHit rch;

    [SerializeField]
    private float rayDistance = 0.1f;
    public string[] ignoreTag;//衝突判定を無視するゲームオブジェクトのタグを設定する

    private AimControl()
    {
    }

    private void Update()
    {
        //レイによってヒットしたオブジェクトの情報を得る
        Debug.DrawRay(transform.position, transform.forward, Color.yellow);
        rch = Physics.SphereCastAll(transform.position, 1, transform.forward, rayDistance).FirstOrDefault(_ => !ignoreTag.Contains(_.collider.tag));

        aim = rch.collider != default(Collider) ? rch.collider.gameObject : null;
        //////////////////////////////////////////////////////////////////////////
        if(aim != null)
            Debug.Log(aim.tag + "がレイによって取得されました");
        //////////////////////////////////////////////////////////////////////////
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
        return aim.gameObject.tag;
    }

    public bool CompareAimObject(GameObject target)
    {
        return ReferenceEquals(aim, target);
    }
}