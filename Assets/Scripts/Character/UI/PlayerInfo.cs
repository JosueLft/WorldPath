using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerInfo : MonoBehaviour {

    [SerializeField] private Text name;
    [SerializeField] private GameObject canvas;
    private PhotonView pv;

    void Start() {
        pv = GetComponent<PhotonView>();
        name.text = pv.Owner.NickName;
    }

    void Update() {
        if(pv.IsMine) {
            name.color = Color.red;
        }
        canvas.gameObject.transform.LookAt(Camera.main.transform);
    }
}