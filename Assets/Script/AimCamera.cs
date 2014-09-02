using System.Collections;
using UnityEngine;
using PState = PlayerStateController.PlayerState;

public class AimCamera : MonoBehaviour
{
    public GameObject destination;//移動の目的地
    public GameObject lookAtPos;//移動後に向きたい方向
    [SerializeField]
    public float moveSpeed = 5;//カメラ移動速度

    private PlayerStateController psc;

    private void Awake()
    {
        psc = PlayerStateController.GetInstance();
    }

    private void Start()
    {
        if(!(GetComponent<Camera>() != null && this.tag == "AimCamera"))
        {
            Debug.LogError("AimCameraをアタッチするオブジェクトはカメラであり\"AimCamera\"タグを付与する必要があります");
            Destroy(this);
        }
        this.camera.enabled = false;
    }

    private void Update()
    {
        //AimStateなら
        if(psc.IsState(PState.AimFish))
        {
            //カメラを無効化/有効化
            if(!this.camera.enabled)
            {
                this.camera.enabled = true;
                ChangeCameraTrigger.CurrentCamera.enabled = false;
            }
            //カメラ移動
            if(Vector3.Distance(this.transform.position, destination.transform.position) > 0.01f)
            {
                transform.Translate((destination.transform.position - this.transform.position) * moveSpeed * Time.deltaTime, Space.World);
                transform.LookAt(lookAtPos.transform.position);
            }
            return;
        }
        //Idle状態に戻ったら(Huntでなかったら)
        else if(!psc.IsState(PState.Hunt))
        {
            //前の位置にカメラを戻す
            if(Vector3.Distance(this.transform.position, ChangeCameraTrigger.CurrentCamera.transform.position) > 0.01f)
            {
                transform.rotation = ChangeCameraTrigger.CurrentCamera.transform.rotation;
                transform.Translate((ChangeCameraTrigger.CurrentCamera.transform.position - this.transform.position) * moveSpeed * Time.deltaTime, Space.World);
            }
            //カメラの無効化/有効化
            else
            {
                ChangeCameraTrigger.CurrentCamera.enabled = true;
                this.camera.enabled = false;
            }
        }
    }
}