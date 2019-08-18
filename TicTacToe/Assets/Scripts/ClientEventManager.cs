using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
public class ClientEventManager : MonoBehaviour
{
    public delegate void Function(object caller, DataStreamReader stream, ref DataStreamReader.Context context);

    public static readonly Dictionary<ClientEvent, Function> ClientEvents = new Dictionary<ClientEvent, Function>(){
        {ClientEvent.CLIENT_TURN_END, ClientTurnEnded},
        {ClientEvent.CLIENT_TURN_START, ClientTurnStarts},
        {ClientEvent.CLIENT_WIN, ClientWin},
        {ClientEvent.CLIENT_LOSE, ClientLose},
        {ClientEvent.DRAW, ClientDraw}
    };
    public static void ClientTurnEnded(object caller, DataStreamReader stream, ref DataStreamReader.Context context) {
        Client client = caller as Client;

        int positionXStringLength = stream.ReadInt(ref context);
        byte[] positionX = stream.ReadBytesAsArray(ref context, positionXStringLength);

        int positionYStringLength = stream.ReadInt(ref context);
        byte[] positionY = stream.ReadBytesAsArray(ref context, positionYStringLength);

        Vector2 pos = DataUtility.BytesToVector2(positionX, positionY);
        //client.gameManagerClient.occupiedByClient.Add(pos);sadasdas
        client.myTurn = false;
        client.gameManager.UpdateGrid(pos, false);
    }
    public static void ClientTurnStarts(object caller, DataStreamReader stream, ref DataStreamReader.Context context) {
        Client client = caller as Client;

        int positionXStringLength = stream.ReadInt(ref context);
        byte[] positionX = stream.ReadBytesAsArray(ref context, positionXStringLength);

        int positionYStringLength = stream.ReadInt(ref context);
        byte[] positionY = stream.ReadBytesAsArray(ref context, positionYStringLength);

        Vector2 pos = DataUtility.BytesToVector2(positionX, positionY);
        //client.gameManagerClient.occupiedByServer.Add(pos);
        client.myTurn = true;
        client.gameManager.UpdateGrid(pos, true);
    }

    public static void ClientWin(object caller, DataStreamReader stream, ref DataStreamReader.Context context) {
        Client client = caller as Client;
        Debug.Log("Win");
        client.gameManager.Win();
    }
    public static void ClientLose(object caller, DataStreamReader stream, ref DataStreamReader.Context context) {
        Client client = caller as Client;
        Debug.Log("Lose");
        client.gameManager.Lose();
    }

    public static void ClientDraw(object caller, DataStreamReader stream, ref DataStreamReader.Context context) {
        Client client = caller as Client;
        Debug.Log("Draw");
        client.gameManager.Draw();
    }

}
