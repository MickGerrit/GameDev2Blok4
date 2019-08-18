using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour
{
    private static Client instance;

    public static Client Instance { get { return instance; } }

    public GameManager gameManager;
    public bool myTurn;

    public UdpNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public bool isConnected;
    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
        myTurn = false;
    }
    
    void Start () {
        gameManager = GameManager.Instance;
        StartConnection("127.0.0.1");
    }

    public void OnDestroy() { 
        m_Driver.Dispose();
    }

    public void RestartConnection(string ipAddress) {
        m_Driver.Dispose();
        StartConnection(ipAddress);

    }
    private void StartConnection(string ipAddress) {
        m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);
        m_Connection = default(NetworkConnection);

        var endpoint = NetworkEndPoint.Parse(ipAddress, 9000);
        m_Connection = m_Driver.Connect(endpoint);
    }

    void FixedUpdate() {
        m_Driver.ScheduleUpdate().Complete();
        if (!m_Connection.IsCreated) {
            return;
        }
        UpdateClient();
    }

    //Get data from the server
    private void UpdateClient() {
        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty) {
            switch (cmd) {
                case NetworkEvent.Type.Connect:
                    ConnectClient();
                    break;

                case NetworkEvent.Type.Data:
                    HandleData(stream);
                    break;

                case NetworkEvent.Type.Disconnect:
                    DisconnectClient();
                    break;
            }
        }
    }

    private void ConnectClient() {
        isConnected = true;
    }
    private void HandleData(DataStreamReader stream) {
        var readerCtx = default(DataStreamReader.Context);
        ClientEvent eventName = (ClientEvent)stream.ReadUInt(ref readerCtx);
        ClientEventManager.ClientEvents[eventName](this, stream, ref readerCtx);
    }
    private void DisconnectClient() {         
        Debug.Log("Disconnect from server");
        isConnected = false;
        m_Connection = default(NetworkConnection); //make sure connection is true
    }

    public void ClickField(Vector2 tilePlacement) {
        byte[] xInBytes = DataUtility.VectorToBytes(tilePlacement.x);
        byte[] yInBytes = DataUtility.VectorToBytes(tilePlacement.y);

        using (var writer = new DataStreamWriter(64, Allocator.Temp)) {
            writer.Write((uint)ServerEvent.CLIENT_CLICKED);
            writer.Write(xInBytes.Length);                           
            writer.Write(xInBytes, xInBytes.Length);  
            writer.Write(yInBytes.Length);                           
            writer.Write(yInBytes, xInBytes.Length);   
            m_Connection.Send(m_Driver, writer);
        }
    }
}
