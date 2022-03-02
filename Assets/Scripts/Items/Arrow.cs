using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Arrow : MonoBehaviour {

    public float speed = 10;
    public Rigidbody arrow;
    public float destroyTime = 0;
    public float forceMultiplier = 0;
    public float initTime = 0;
    public float endTime = 0;
    public float time = 0;
    
    void Start() {
        arrow = GetComponent<Rigidbody>();
        arrow.AddRelativeForce(Vector3.forward * forceMultiplier, ForceMode.Impulse);
        initTime = Time.deltaTime;
        endTime = initTime + 30;
    }

    void Update() {
        time += Time.deltaTime;
        LimiteTimeRespawn();
    }

    private void OnCollisionEnter(Collision collision) {
        Destroy(gameObject, 1);
    }

    void LimiteTimeRespawn() {
        if(time >= endTime) {
            Destroy(gameObject, 1);
        }
    }

    void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.CompareTag("Pllayer") && collider.GetComponent<PhotonView>().IsMine) {
            collider.GetComponent<Player>().Damages(10);
        }
    }
}