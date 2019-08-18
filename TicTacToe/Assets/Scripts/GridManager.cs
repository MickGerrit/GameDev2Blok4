using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager instance;

    public static GridManager Instance { get { return instance; } }

    public bool isServer;
    public Server server;

    public List<Vector2> markedByClient;
    public List<Vector2> markedByServer;

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    private void Start() {
        server = Server.Instance;
        isServer = false;
        if (server != null)
            isServer = true;

    }


    public bool IsFieldValid(Vector2 tile) {
        if (!markedByClient.Contains(tile) && !markedByServer.Contains(tile)) {
            if (tile.x >= 0 && tile.x < 4 && tile.y >= 0 && tile.y < 4) {
                return true;
            }
        }
        Debug.Log("Checked Tile");
        return false;
    }

    public bool UpdateGridServer(Vector2 tile, bool serverTurn) {
        if (!IsFieldValid(tile)) {
            return false;
        }
        if (serverTurn) {
            markedByServer.Add(tile);
        } else if (!serverTurn) {
            markedByClient.Add(tile);
        }

        if (serverTurn) {
            Debug.Log("Server: " + tile);
        } else {
            Debug.Log("Client: " + tile);
        }
        return true;
    }

    public void HasWin() {
        if (WinnerCheck(markedByServer)) {
            server.RoundEnded(ClientEvent.CLIENT_LOSE);
            server.gameManager.Win();
            return;
        }
        if (WinnerCheck(markedByClient)) {
            server.RoundEnded(ClientEvent.CLIENT_WIN);
            server.gameManager.Lose();
            return;
        }
        if ((markedByClient.Count + markedByServer.Count   ) >= 9) {
            server.RoundEnded(ClientEvent.DRAW);
            server.gameManager.Draw();
        }
    }

    public void EmptyFields() {
        markedByClient.Clear();
        markedByServer.Clear();
    }

    public bool WinnerCheck(List<Vector2> markedFields) {
        bool hasWon = false;
        if (markedFields.Contains(new Vector2(1, 1))) {
            if ((markedFields.Contains(new Vector2(0, 2)) && markedFields.Contains(new Vector2(2, 0))) ||
               (markedFields.Contains(new Vector2(2, 2)) && markedFields.Contains(new Vector2(0, 0))) ||
               (markedFields.Contains(new Vector2(1, 2)) && markedFields.Contains(new Vector2(1, 0))) ||
               (markedFields.Contains(new Vector2(0, 1)) && markedFields.Contains(new Vector2(2, 1)))) {
                    hasWon = true;
            }
        }
        if (markedFields.Contains(new Vector2(0, 2))) {
            if (markedFields.Contains(new Vector2(0, 1)) && markedFields.Contains(new Vector2(0, 0))||
                markedFields.Contains(new Vector2(1, 2)) && markedFields.Contains(new Vector2(2, 2))) {
                    hasWon = true;
            }
        }
        if (markedFields.Contains(new Vector2(2, 0))) {
            if (markedFields.Contains(new Vector2(2, 1)) && markedFields.Contains(new Vector2(2, 2)) ||
                markedFields.Contains(new Vector2(1, 0)) && markedFields.Contains(new Vector2(0, 0))) {
                    hasWon = true;
            }
        }
        return hasWon;
    }
}
