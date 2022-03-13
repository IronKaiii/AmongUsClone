using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Movement : Photon.Pun.MonoBehaviourPun
{
    // SerializeField speed for further adjustment in Unity
    [SerializeField] private float speed = 10;

    private Vector2 position;
    private Rigidbody2D body;

    

    // Awake is called for the initialization
    private void Awake() {
        body = GetComponent<Rigidbody2D>();
        position = Vector2.zero;
    }

    // Update is called once per frame
    private void Update()
    {
        // the player can only move their own character
        if (photonView.IsMine) {
            position.x = Input.GetAxisRaw("Horizontal");
            position.y = Input.GetAxisRaw("Vertical");
        }
    }

    // This function is used to unify the speed.
    // Character may move faster due to the processing speed of computer
    private void FixedUpdate() {
        // the player can only move their own character
        if (photonView.IsMine) {
            body.MovePosition(body.position + position.normalized * speed * Time.fixedDeltaTime);
        }     
    }
}
