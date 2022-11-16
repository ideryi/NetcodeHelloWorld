using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        //Vector3 pos = new Vector3();

        public static bool firstTime = true;
        DateTime start = new DateTime();
        DateTime end = new DateTime();
        DateTime start2 = new DateTime();
        DateTime end2 = new DateTime();
        public static double lantency = 0;
        public static double lantency2 = 0;

        static bool first = true;

        void OnValueChangedDelegate(Vector3 previousValue, Vector3 newValue)
        {
            Debug.Log("OnValueChangedDelegate");
            end2 = DateTime.Now;
            lantency2 = (end2 - start2).TotalMilliseconds / 2;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                Position.OnValueChanged = OnValueChangedDelegate;
                Move();
            }
        }

        public void Move()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                Position.Value = randomPosition;
            }
            else
            {
                Debug.Log("Move");
                start2 = DateTime.Now;
                SubmitPositionRequestServerRpc();
            }
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            Position.Value = GetRandomPositionOnPlane();
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(UnityEngine.Random.Range(-3f, 3f), 1f, UnityEngine.Random.Range(-3f, 3f));
        }

        [ServerRpc]
        void callServerRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            callClientRequestClientRpc();
        }

        [ClientRpc]
        void callClientRequestClientRpc(ClientRpcParams rpcParams = default)
        {
            end = DateTime.Now;
            lantency = (end - start).TotalMilliseconds / 2;
        }

        void getLatency()
        {
            start = DateTime.Now;

            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            var player = playerObject.GetComponent<HelloWorldPlayer>();
            player.callServerRequestServerRpc();
        }

        void Update()
        {
            Debug.Log("Update");

            transform.position = Position.Value;

            if (NetworkManager.Singleton.IsClient)
            {
                getLatency();
                //HelloWorldManager.latency = lantency;
                //HelloWorldManager.latency2 = lantency2;
            }
        }
    }
}