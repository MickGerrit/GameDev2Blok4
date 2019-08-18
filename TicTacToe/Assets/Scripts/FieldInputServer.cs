using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldInputServer : MonoBehaviour {
    public Server server;
    public GameManager gameManager;
    public GridManager gridManager;
    private Text field;
    private void Start() {
        gridManager = GridManager.Instance;
        server = Server.Instance;
        gameManager = GameManager.Instance;
        field = transform.GetChild(0).GetComponent<Text>();
        field.text = "";
    }

    public void IndexToPosition() {
        if (!server.myTurn || gameManager.betweenRounds || !server.isConnected) {
            return;
        }
        int siblingIndex = transform.GetSiblingIndex();
        int xPos = siblingIndex % 3;
        int yPos = Mathf.FloorToInt(siblingIndex / 3);
        Vector2 position = new Vector2(xPos, yPos);
        if (!GridManager.Instance.UpdateGridServer(position, true)) {
            return;
        } else {
            server.gameManager.UpdateGrid(position, true);
            server.gridManager.HasWin();
            server.ClientTurnStarts(position);
        }
    }
}
