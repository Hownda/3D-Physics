using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject localPlayer = null;

    private void Update()
    {
        if (localPlayer == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player.GetComponent<NetworkObject>().IsLocalPlayer)
                {
                    localPlayer = player;
                }
            }
            if (localPlayer == null)
            {
                return;
            }
        }
        else
        {
            if (transform.parent != localPlayer.transform)
            {
                transform.SetParent(localPlayer.transform);
            }
            else
            {
                return;
            }
        }
    }
}
