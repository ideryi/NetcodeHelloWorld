
using Unity.Netcode;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldManager : MonoBehaviour
    {
        public static double latency = 0;

        public static long totalReceive = 0;
        public static long totalSend = 0;
        public static long frameReceive = 0;
        public static long frameSend = 0;

        void OnGUI()
        {
            if(NetworkCommandLine.thisIsHost == true)
            {
                return;
            }

            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();

                SubmitNewPosition();
            }

            GUILayout.EndArea();
        }

        static void StartButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);

            GUILayout.Label("Latency: " + latency.ToString());
            //GUILayout.Label("Total Receive: " + totalReceive.ToString());
            //GUILayout.Label("Total Send: " + totalSend.ToString());

            GUILayout.Label($"Total Bytes Received:{HelloWorldManager.totalReceive} byte");
            GUILayout.Label($"Total Bytes Sent: {HelloWorldManager.totalSend} byte");

            GUILayout.Label($"Frame Bytes Received:{HelloWorldManager.frameReceive} byte");
            GUILayout.Label($"Frame Bytes Sent: {HelloWorldManager.frameSend} byte");

            //ReliableUtility.Statistics;
        }

        static void SubmitNewPosition()
        {
            if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
            {
                if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
                {
                    foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                        NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<HelloWorldPlayer>().Move();
                }
                else
                {
                    var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                    var player = playerObject.GetComponent<HelloWorldPlayer>();
                    player.Move();
                }
            }
        }
    }
}