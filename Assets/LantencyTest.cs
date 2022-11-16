using HelloWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;


public class LantencyTest : NetworkBehaviour
{
    public bool enable = true;
    NetworkVariable<int> testObj = new NetworkVariable<int>(1);

    DateTime start = new DateTime();
    DateTime end = new DateTime();
    public static double lantency = 0;
    bool inLantencyTesting = false;


    void OnValueChangedDelegate(int previousValue, int newValue)
    {
        end = DateTime.Now;
        lantency = (end - start).TotalMilliseconds / 2;
        inLantencyTesting = false;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            testObj.OnValueChanged = OnValueChangedDelegate;
        }
    }

    void getLatency()
    {
        inLantencyTesting = true;
        start = DateTime.Now;

        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        var player = playerObject.GetComponent<LantencyTest>();
        player.SubmitValueChangeServerRpc();
    }

    [ServerRpc]
    void SubmitValueChangeServerRpc(ServerRpcParams rpcParams = default)
    {
        if (testObj.Value < 10000)
            testObj.Value = testObj.Value + 1;
        else
            testObj.Value = 0;
    }

    void Update()
    {
        if (enable && NetworkManager.Singleton.IsClient && inLantencyTesting == false)
        {
            getLatency();
        }
    }
}