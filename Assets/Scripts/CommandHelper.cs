using System.Collections;
using System.Collections.Generic;
using ED.SC;
using Unity.Netcode;
using UnityEngine;

public class CommandHelper : MonoBehaviour
{
    [Command]
    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        GameObject detector = GameObject.Find("Detectors");
        detector.SetActive(true);
    }
    [Command]
    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
    [Command]
    void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
}
