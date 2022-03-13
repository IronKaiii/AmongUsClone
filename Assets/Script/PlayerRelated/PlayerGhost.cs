using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerGhost : Photon.Pun.MonoBehaviourPun, IPunObservable
{
    [SerializeField] private SpriteRenderer ghost;

    // Follow the color of the specific character
    public void setColor(Color color) {
        ghost.color = color;
    }

    // synchronize all the player dead body color
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(ghost.color.r);
            stream.SendNext(ghost.color.g);
            stream.SendNext(ghost.color.b);
        } else {
            float r = (float)stream.ReceiveNext();
            float g = (float)stream.ReceiveNext();
            float b = (float)stream.ReceiveNext();
            ghost.color = new Color(r, g, b, 1);
        }
    }
}
