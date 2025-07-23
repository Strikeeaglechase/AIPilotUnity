using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !HSGE
using NativeWebSocket;
#else
using WebSocketClient;
#endif

using System.Threading;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Decompress.V5;
using System;

public class ClientId
{
    public string id;
    public string type;
}

public class RPC
{
    public string className;
    public string method;
    public string id;
    public object[] args;

    private int curArgIdx = 0;

    public T arg<T>(int index)
    {
        var selectedArg = args[index];
        return (T)Convert.ChangeType(selectedArg, typeof(T));
    }

    public T arg<T>()
    {
        var selectedArg = args[curArgIdx++];
        return (T)Convert.ChangeType(selectedArg, typeof(T));
    }

    public override string ToString()
    {
        return $"{className}.{method}({string.Join(", ", args)})";
    }
}

public abstract class CommandPacket
{
    public string type;
}

public class ConfigureAIP : CommandPacket { }

public class KinematicsPacket : CommandPacket
{
    public NetVector position;
    public NetVector velocity;
    public NetVector acceleration;
    public NetVector rotation;
}

public class AIPData : KinematicsPacket
{
    public NetVector pyr;
    public float throttle;
}

public class AIPMissileData : KinematicsPacket
{
    public int entityId;
    public bool detonate;
}

public class SpawnAIPWeapon : CommandPacket
{
    public string path;
    public int hpIndex;
}

public class RWRPing : CommandPacket
{
    public string rwrTargetOwnerId;
    public int actorId;
    public float signalStrength;
    public float frequency;
    public NetVector position;
    public NetVector velocity;
    public bool isLock;
}

public class AIPKillEntity : CommandPacket
{
    public int entityId;
}

public class UseCountermeasure : CommandPacket
{
    // 0=flare
    // 1=chaff
    // 2=both
    public int cmsType;
}

public class WrappedOutboundPacket
{
    public enum OutType
    {
        RPC,
        CommandPacket
    }

    public OutType type;
    public object packet;
}

public interface IVehicleReadyNotificationHandler
{
    void OnVehicleReadyNotification();
}

public class HCConnector : MonoBehaviour
{
    private WebSocket client;
    public string clientId;
    public HCManager manager;

    public bool doWsReconnect = false;

    public static HCConnector instance
    {
        get;
        private set;
    }

    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogError($"Duplicate HCConnector created!");
            Destroy(this);
            return;
        }

        instance = this;
    }

    public void Start()
    {
        ConfigureWS();
    }

    private async void ConfigureWS()
    {
        if (client != null)
        {
            await client.Close();
        }

        Debug.Log($"Setting up HC WS");
        client = new WebSocket("ws://localhost:8010");

        client.OnOpen += () =>
        {
            Debug.Log($"WS Client opened!");
            client.SendText("autosub");
        };

        client.OnClose += (closeCode) =>
        {
            Debug.Log($"WS Client closed: {closeCode}");
            if (doWsReconnect) StartCoroutine(ConfigureWSAfterDelay());
        };

        client.OnMessage += (message) =>
        {
            HandlerMessage(message);
        };

        await client.Connect();
    }

    private IEnumerator ConfigureWSAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        ConfigureWS();
    }

    private void HandlerMessage(byte[] bytes)
    {
        var isJsonMessage = bytes[0] == 123; // Check for initial '{'

        if (isJsonMessage)
        {
            string message = Encoding.UTF8.GetString(bytes);
            var jobj = JObject.Parse(message);
            // Debug.Log(jobj.ContainsKey("type"));
            // Debug.Log(jobj["type"].ToString());
            if (jobj.ContainsKey("type") && jobj["type"].ToString() == "assignId")
            {
                var cid = JsonConvert.DeserializeObject<ClientId>(message);
                // Debug.Log($"Parsing CID: {cid}");
                if (cid == null || cid.type != "assignId")
                {
                    Debug.Log($"Invalid client ID message: {message}");
                    return;
                }

                clientId = cid.id;
                Debug.Log($"Received clientId: {clientId}");

                SendCommandPacket(new ConfigureAIP());

                // var handlers = gameObject.GetComponentsInChildrenImplementing<IVehicleReadyNotificationHandler>();
            }
            else
            {
                var rpc = JsonConvert.DeserializeObject<RPC>(message);
                if (rpc == null)
                {
                    Debug.Log($"Unable to parse RPC: {message}");
                    return;
                }

                manager.HandleRPC(rpc);
            }
        }
        else
        {
            var decompressor = new DecompressorV5(bytes);
            var rpcs = decompressor.DecompressRPCPackets();
            foreach (var rpc in rpcs)
            {
                manager.HandleRPC(rpc);
            }
        }
    }

    public void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        client.DispatchMessageQueue();
#endif
    }

    public void SendRPC(RPC rpc)
    {
#if DO_NETWORK
        Send(new WrappedOutboundPacket { type = WrappedOutboundPacket.OutType.RPC, packet = rpc });
#endif
    }

    public void SendCommandPacket(CommandPacket commandPacket)
    {
#if DO_NETWORK
        commandPacket.type = commandPacket.GetType().Name;
        Send(new WrappedOutboundPacket { type = WrappedOutboundPacket.OutType.CommandPacket, packet = commandPacket });
#endif
    }

    private void Send(WrappedOutboundPacket wop)
    {
        if (clientId == null || clientId.Length == 0)
        {
            // Debug.Log($"Not sending packet because clientId is empty. Message: {message}");
            return;
        }

        var message = JsonConvert.SerializeObject(wop);
        client.SendText(message);
    }
}
