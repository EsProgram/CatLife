using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// 水場を管理するクラス
/// </summary>
public class ManagedWaterPlace : MonoBehaviour
{
    public WaterPlace[] waterPlaces;

    /// <summary>
    /// デフォルトで生成されるスクリプト以外に
    /// このクラスを生成できなくする
    /// 他のスクリプトからはこのオブジェクトスクリプトの参照しかできない
    /// 事実上のシングルトン
    /// </summary>
    private ManagedWaterPlace()
    {
    }

    /// <summary>
    /// ゲームオブジェクトに設定された名前から水場の参照を走査する
    /// </summary>
    /// <param name="name">ゲームオブジェクトの名前</param>
    /// <returns>
    /// 水場
    /// 見つからなければnull
    /// </returns>
    public WaterPlace FindWithName(string name)
    {
        WaterPlace place = waterPlaces.FirstOrDefault(w => w != null && w.gameObject != null ? w.name == name : false);
        if(place == null)
            Debug.LogError("指定した名前の水場が見つかりませんでした");
        return place;
    }
}