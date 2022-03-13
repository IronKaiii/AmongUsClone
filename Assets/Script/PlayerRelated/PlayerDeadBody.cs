using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerDeadBody : Photon.Pun.MonoBehaviourPun, IPunObservable
{
    [SerializeField] private SpriteRenderer deadBody;

    // Follow the color of the specific character
    public void setColor(Color color) {
        deadBody.color = color;
    }

    // synchronize all the player dead body color
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(deadBody.color.r);
            stream.SendNext(deadBody.color.g);
            stream.SendNext(deadBody.color.b);
        } else {
            float r = (float)stream.ReceiveNext();
            float g = (float)stream.ReceiveNext();
            float b = (float)stream.ReceiveNext();
            deadBody.color = new Color(r, g, b, 1);
        }
    }
}
