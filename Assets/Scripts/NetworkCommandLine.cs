using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

public class NetworkCommandLine : MonoBehaviour
{
    public static string listenAddress;
    public static string address;
    public static string port;
    public static ushort portNum = 0;

    

    private NetworkManager netManager;

    public static bool thisIsHost = false;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        listenAddress = "0.0.0.0";
        //address = "172.21.109.60";
        //address = "43.139.96.90";
        address = "127.0.0.1";
        //address = "192.168.0.106";
        port = "10000";
        portNum = 10000;
    }

    void Start()
    {
        netManager = GetComponentInParent<NetworkManager>();

        if (Application.isEditor)
        {
            var ut1 = netManager.NetworkConfig.NetworkTransport as UnityTransport;
            if (ut1 != null)
            {
                ut1.SetConnectionData(address, portNum, listenAddress);
            }
            return;
        }

        var args = GetCommandlineArgs();

        //使用命令行设置服务器ip及端口

        //string listenAddress;
        //string address;
        //string port;
        args.TryGetValue("-listenAddress", out listenAddress);
        args.TryGetValue("-address", out address);
        args.TryGetValue("-port", out port);

        if(listenAddress == null || listenAddress.Trim().Length == 0)
        {
            listenAddress = "0.0.0.0";
        }
        if (address == null || address.Trim().Length == 0)
        {
            address = "127.0.0.1";
        }
        if (port == null || port.Trim().Length == 0)
        {
            port = "10000";
        }
        //ushort portNum = 0;
        ushort.TryParse(port,out portNum);

        var ut = netManager.NetworkConfig.NetworkTransport as UnityTransport;
        if (ut != null)
        {
            ut.SetConnectionData(address, portNum,listenAddress);
        }

        //ReliableUtility.Statistics;

        if (args.TryGetValue("-mlapi", out string mlapiValue))
        {
            switch (mlapiValue)
            {
                case "server":
                    thisIsHost = true;
                    netManager.StartServer();
                    break;
                case "host":
                    thisIsHost = true;
                    netManager.StartHost();
                    break;
                case "client":

                    netManager.StartClient();
                    break;
            }
        }
    }

    private Dictionary<string, string> GetCommandlineArgs()
    {
        Dictionary<string, string> argDictionary = new Dictionary<string, string>();

        var args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; ++i)
        {
            var arg = args[i].ToLower();
            if (arg.StartsWith("-"))
            {
                var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                value = (value?.StartsWith("-") ?? false) ? null : value;

                argDictionary.Add(arg, value);
            }
        }
        return argDictionary;
    }
}