using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Photon.Pun;
using Photon.Realtime;

public class ShooterSystems : MonoBehaviour {
    public GameObject aimCamera;
    public float rotateSpeed = 15;
    public float aimMaxDistance = 100;
    public GameObject ammunitionsPrefab;
    public Transform ammunitionsRespawn;
    public GameObject aimCanvas;
    StarterAssetsInputs input;
    ThirdPersonController tpController;
    Camera mainCamera;
    Vector3 aimPosition = Vector3.zero;
    Animator animator;
    PhotonView pv;

    void Start() {
        input = GetComponent<StarterAssetsInputs>();
        tpController = GetComponent<ThirdPersonController>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
    }

    void Update() {
        OnCollisionEnter();
        OnAim();
        pv.RPC("OnShot", RpcTarget.All);
        //OnShot();
    }

    void OnCollisionEnter() {
        // verificando colisão do projetil
        Vector2 screenCenterPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = mainCamera.ScreenPointToRay(screenCenterPos);// pegando a origem do raio no centro da tela

        if(Physics.Raycast(ray, out RaycastHit hit, aimMaxDistance)) {
            aimPosition = hit.point;
        } else {
            aimPosition = ray.origin + ray.direction * aimMaxDistance;
        }
    }

    void OnAim() {
        Vector2 screenCenterPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = mainCamera.ScreenPointToRay(screenCenterPos);
        if(Physics.Raycast(ray, out RaycastHit hit, aimMaxDistance)) {
            aimPosition = hit.point;
        } else {
            aimPosition = ray.origin + ray.direction * aimMaxDistance;
        }
        if(input.aim) {
            aimCanvas.SetActive(true);
            animator.SetLayerWeight(1, 1);
            tpController.SetRotateOnMove(false);
            aimCamera.SetActive(true);
            float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), Time.deltaTime * rotateSpeed);
        } else {
            aimCanvas.SetActive(false);
            animator.SetLayerWeight(1, 0);
            aimCamera.SetActive(false);
            tpController.SetRotateOnMove(true);
        }
    }

    [PunRPC]
    public void OnShot() {
        if (input.shot && input.aim) {
            input.shot = false;
            Vector3 bulletDirection = (aimPosition - ammunitionsRespawn.position).normalized;
            Instantiate(ammunitionsPrefab, ammunitionsRespawn.position, Quaternion.LookRotation(bulletDirection)); //Quaternion.LookRotation(): Olha pra uma direção e transforma em um quaternion
            //PhotonNetwork.Instantiate(ammunitionsPrefab.name, ammunitionsRespawn.position, Quaternion.LookRotation(bulletDirection));
        }
    }
}
