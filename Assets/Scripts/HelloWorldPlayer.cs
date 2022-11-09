using System;
using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
        Vector3 pos = new Vector3();

        public static bool firstTime = true;
        DateTime start = new DateTime();
        DateTime end = new DateTime();
        double lantency = 0;

        static bool first = true;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
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
                SubmitPositionRequestServerRpc();
            }
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            Position.Value = pos = GetRandomPositionOnPlane();
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            //return new Vector3(UnityEngine.Random.Range(-3f, 3f), 1f, UnityEngine.Random.Range(-3f, 3f));
            if(first)
            {
                return new Vector3(1,1,1);
            }
            else
            {
                return new Vector3(-1, 1, -1);
            }
            first = !first;
        }

        [ServerRpc]
        void callServerRequestServerRpc(ServerRpcParams rpcParams = default)
        {
            Debug.Log("callServerRequestServerRpc");
            Debug.Log("to callClientRequestClientRpc");
            callClientRequestClientRpc(Position.Value);
            Debug.Log("finish callClientRequestClientRpc");
        }

        [ClientRpc]
        void callClientRequestClientRpc(Vector3 pos,ClientRpcParams rpcParams = default)
        {
            Debug.Log("callClientRequestClientRpc");
            end = DateTime.Now;
            lantency = (end - start).TotalMilliseconds / 2;

            this.pos = pos;
        }

        double getLatency()
        {
            start = DateTime.Now;

            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            var player = playerObject.GetComponent<HelloWorldPlayer>();
            player.callServerRequestServerRpc();

            return lantency;
        }

        void Update()
        {
            //transform.position = Position.Value;

            transform.position = pos;

            if (NetworkManager.Singleton.IsClient)
            {
                //HelloWorldManager.latency = getLatency();
            }
        }
    }
}