using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Networking.Transport; 
using Unity.Collections;
using UdpCNetworkDriver = Unity.Networking.Transport.UdpNetworkDriver;

public class Server : MonoBehaviour
{
    public UdpCNetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;

    private static Server instance;

    public static Server Instance { get { return instance; } }

    public bool isConnected;
    public bool myTurn;
    public GridManager gridManager;
    public GameManager gameManager;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
        gridManager = GridManager.Instance;
        myTurn = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_Driver = new UdpCNetworkDriver(new INetworkParameter[0]);
        if (m_Driver.Bind( NetworkEndPoint.Parse("0.0.0.0", 9000 ) ) != 0)
            Debug.Log("Failed to bind to port ...");
        else
            m_Driver.Listen();
        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        gameManager = GameManager.Instance;
    }

    public void OnDestroy()
    {
        m_Driver.Dispose();
        m_Connections.Dispose();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_Driver.ScheduleUpdate().Complete();

        // Clean up connections
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        // Accept new connections
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            m_Connections.Add(c);
            isConnected = true;
            Debug.Log("Accepted a connection");
        }

            
        UpdateConnections();
        UpdateServer();
    }

    private void UpdateServer() {
        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++) {
            if (!m_Connections[i].IsCreated)
                continue;

            NetworkEvent.Type cmd;
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty) {
                switch (cmd) {
                    case NetworkEvent.Type.Data:
                        HandleData(stream, i);
                        break;

                    case NetworkEvent.Type.Disconnect:
                        PlayerDisconnect(i);
                        break;
                }
            }
        }
    }

    private void UpdateConnections() {
        // Clean up connections
        for (int i = 0; i < m_Connections.Length; i++) {
            if (!m_Connections[i].IsCreated) {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        // Accept new connections
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection)) {
            m_Connections.Add(c);
            Debug.Log("Accepted a connection");
        }
    }

    private void HandleData(DataStreamReader stream, int connectionIndex) {
        var readerCtx = default(DataStreamReader.Context);
        ServerEvent eventName = (ServerEvent)stream.ReadUInt(ref readerCtx);
        ServerEventManager.ServerEvents[eventName](this, stream, ref readerCtx);
    }

    private void PlayerDisconnect(int playerIndex) {
        Debug.Log("Client disconnected from server");
        m_Connections[playerIndex] = default(NetworkConnection);
    }

    public void ClientTurnEnded(Vector2 pos) {
        byte[] positionXInBytes = DataUtility.VectorToBytes(pos.x);
        byte[] positionZInBytes = DataUtility.VectorToBytes(pos.y);
        for (int i = 0; i < m_Connections.Length; i++) {
            if (!m_Connections[i].IsCreated)
                continue;

            using (var writer = new DataStreamWriter(64, Allocator.Temp)) {
                writer.Write((uint)ClientEvent.CLIENT_TURN_END);
                writer.Write(positionXInBytes.Length);                           //Position.X lenght of Array
                writer.Write(positionXInBytes, positionXInBytes.Length);         //PositionArray

                writer.Write(positionZInBytes.Length);                           //Position.Z lenght of Array
                writer.Write(positionZInBytes, positionZInBytes.Length);

                m_Connections[i].Send(m_Driver, writer);
            }
        }
        myTurn = true;
    }

    public void ClientTurnStarts(Vector2 position) {
        byte[] positionXInBytes = DataUtility.VectorToBytes(position.x);
        byte[] positionZInBytes = DataUtility.VectorToBytes(position.y);
        for (int i = 0; i < m_Connections.Length; i++) {
            if (!m_Connections[i].IsCreated)
                continue;

            using (var writer = new DataStreamWriter(64, Allocator.Temp)) {
                writer.Write((uint)ClientEvent.CLIENT_TURN_START);
                writer.Write(positionXInBytes.Length);                           //Position.X lenght of Array
                writer.Write(positionXInBytes, positionXInBytes.Length);         //PositionArray

                writer.Write(positionZInBytes.Length);                           //Position.Z lenght of Array
                writer.Write(positionZInBytes, positionZInBytes.Length);

                m_Connections[i].Send(m_Driver, writer);
            }
        }
        myTurn = false;
    }

    public void RoundEnded(ClientEvent state) {
        for (int i = 0; i < m_Connections.Length; i++) {
            if (!m_Connections[i].IsCreated)
                continue;
            using (var writer = new DataStreamWriter(64, Allocator.Temp)) {
                writer.Write((uint)state);
                m_Connections[i].Send(m_Driver, writer);
            }
        }
    }
}
