using System.Collections;
using UnityEngine;

public class CameraAttention : MonoBehaviour
{
    public GameObject destination;
    public GameObject lookAtPos;
    public float moveSpeed;

    // Use this for initialization
    private void Start()
    {
        //アタッチされたオブジェクトがカメラでなければスクリプトを破棄
        if(GetComponent<Camera>() == null)
            Destroy(this);
    }

    // Update is called once per frame
    private void Update()
    {
        //何かキーを押してる間回転移動する
        if(Input.anyKey)
            //目的の位置まで距離があれば移動＆回転
            if(Vector3.Distance(this.transform.position, destination.transform.position) > 0.1f)
            {
                //2通りの回転方法(速度あまり変化なかった。どっちでもあんまり変わんない)
                transform.LookAt(Vector3.Lerp(this.transform.position, lookAtPos.transform.position, Time.time));
                //transform.LookAt(LerpAngle(this.transform.rotation, lookAtPos.transform.rotation, Time.time));

                //移動
                transform.Translate((destination.transform.position - this.transform.position) * moveSpeed * Time.deltaTime);
            }
    }

    /// <summary>
    /// クォータニオンを保管して返す
    /// </summary>
    /// <param name="a">origin</param>
    /// <param name="b">destination</param>
    /// <param name="t">[0 ~ 1]</param>
    /// <returns></returns>
    private Vector3 LerpAngle(Quaternion a, Quaternion b, float t)
    {
        var x = Mathf.LerpAngle(a.x, b.x, t);
        var y = Mathf.LerpAngle(a.y, b.y, t);
        var z = Mathf.LerpAngle(a.z, b.z, t);
        return new Vector3(x, y, z);
    }
}