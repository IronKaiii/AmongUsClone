using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class roomHost : Photon.Pun.MonoBehaviourPunCallbacks
{
    
    
    public void Initialize(int num) {
        if (PhotonNetwork.IsMasterClient) StartCoroutine(pickImpostor(num));
    }

    private IEnumerator pickImpostor(int selectedNum) {
        GameObject[] players;
        List<int> playerListToSelect = new List<int>();
        int checkTime = 0;
        int impostorNum = 0;

        do {
            players = GameObject.FindGameObjectsWithTag("crewmateAlive");
            checkTime++;
            yield return new WaitForSeconds(0.25f);
        } while((players.Length < PhotonNetwork.CurrentRoom.PlayerCount) && checkTime < 5);

        for (int i = 0; i < players.Length; i++) playerListToSelect.Add(i);

        impostorNum = selectedNum;   
        
        while (impostorNum > 0) {
            int selectImpostor = playerListToSelect[Random.Range(0, playerListToSelect.Count)];
            playerListToSelect.Remove(selectImpostor);

            // send the setImpostor function only to the player who is selected to be Impostor
            PhotonView pv = players[selectImpostor].GetComponent<PhotonView>();
            
            pv.RPC("SetImpostor", RpcTarget.All);

            impostorNum--;
        }

    }
}
