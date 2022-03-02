using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerInfos : MonoBehaviour {
    [SerializeField]     private PhotonView pv;
    [SerializeField] private Text playerName;
    [SerializeField] private GameObject canvasPlayers;

    void Start() {
        pv = GetComponent<PhotonView>();
        playerName.text = pv.Owner.NickName;
    }

    void Update() {
        canvasPlayers.gameObject.transform.LookAt(Camera.main.transform);
    }
}