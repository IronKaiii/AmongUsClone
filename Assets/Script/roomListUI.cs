using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class roomListUI : MonoBehaviour
{
    public lobbyNetwork lobbyHost;
    [SerializeField] private Text roomName;

    public void setName(string name) {
        roomName.text = name;
    }

    public void joinRoom() {
        lobbyHost.joinRoom(roomName.text);
    }
}
