using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Networking.Transport;

public class ServerEventManager : MonoBehaviour
{
    public delegate void Function(object caller, DataStreamReader stream, ref DataStreamReader.Context context);

    public static readonly Dictionary<ServerEvent, Function> ServerEvents = new Dictionary<ServerEvent, Function>(){
        {ServerEvent.CLIENT_CLICKED, ClientClicked}
    };
    public static void ClientClicked(object caller, DataStreamReader stream, ref DataStreamReader.Context context) {
        Debug.Log("Played");
        Server server = caller as Server;
        if (server.myTurn) {
            return;
        }
        int positionXStringLength = stream.ReadInt(ref context);
        byte[] positionX = stream.ReadBytesAsArray(ref context, positionXStringLength);

        int positionYStringLength = stream.ReadInt(ref context);
        byte[] positionY = stream.ReadBytesAsArray(ref context, positionYStringLength);
        Vector2 position = DataUtility.BytesToVector2(positionX, positionY);
        if (!server.gridManager.UpdateGridServer(position, false)) {
            return;
        } else {

            server.ClientTurnEnded(position);
            server.gameManager.UpdateGrid(position, false);

            //check for win condition
            server.gridManager.HasWin();
        }
    }
}
