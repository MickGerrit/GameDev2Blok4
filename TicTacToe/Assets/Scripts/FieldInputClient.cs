using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldInputClient : MonoBehaviour {
    public Client client;
    public GameManager gameManager;
    private Text field;

    private void Start() {
        client = Client.Instance;
        gameManager = GameManager.Instance;
        field = transform.GetChild(0).GetComponent<Text>();
        field.text = "";
    }
    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Pressed");
        }
    }

    public void IndexToPosition() {
        if (!client.isConnected || gameManager.betweenRounds) {
            return;
        }
        int siblingIndex = transform.GetSiblingIndex();
        int xPos = siblingIndex % 3;
        int yPos = Mathf.FloorToInt(siblingIndex / 3);
        client.ClickField(new Vector2(xPos, yPos));
    }
}
