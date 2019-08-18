using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public GameObject gridParent;
    public List<Text> fields;
    private Client client;
    private Server server;
    public GameObject noConnectionObject;
    public GameObject winObject;
    public GameObject loseObject;
    public GameObject drawObject;
    private int currentWins;
    private int currentLosses;
    public Text myScoreText;
    public Text otherScoreText;
    public GameObject yourTurnObject;
    public GameObject opponentsTurnObject;
    public bool betweenRounds;

    private static GameManager instance;

    public static GameManager Instance { get { return instance; } }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < gridParent.transform.childCount; i++) {
            fields.Add(gridParent.transform.GetChild(i).GetChild(0).GetComponent<Text>());
        }
        server = Server.Instance;
        client = Client.Instance;
        myScoreText.text = "0";
        otherScoreText.text = "0";
        betweenRounds = false;
        currentLosses = 0;
        currentWins = 0;
    }

    // Update is called once per frame
    void Update() {
        Connection();

        if (betweenRounds) {
            opponentsTurnObject.SetActive(false);
            yourTurnObject.SetActive(false);
            return;
        }

        if (server != null) {
            MyTurn(server.myTurn);
        }else if (client != null) {
            MyTurn(client.myTurn);
        }

    }

    public void UpdateGrid(Vector2 field, bool occupiedByServer) {
        int fieldIndex;
        fieldIndex = (int)(field.y * 3 + field.x);
        if (occupiedByServer) {
            fields[fieldIndex].text = "O";
        } else {
            fields[fieldIndex].text = "X";
        }
    }

    public void Win() {
        StartCoroutine(NewRoundCountDown());
        winObject.SetActive(true);
        DataBaseManager.amountOfWins++;
        currentWins++;
        myScoreText.text = currentWins.ToString();
    }

    public void Lose() {
        StartCoroutine(NewRoundCountDown());
        loseObject.SetActive(true);
        DataBaseManager.amountOfLosses++;
        currentLosses++;
        otherScoreText.text = currentLosses.ToString();
    }

    public void Draw() {
        StartCoroutine(NewRoundCountDown());
        drawObject.SetActive(true);
    }

    public void NewRound() {
        for (int i = 0; i < fields.Count; i++) {
            fields[i].text = "";
        }
        winObject.SetActive(false);
        loseObject.SetActive(false);
        drawObject.SetActive(false);
        betweenRounds = false;
    }

    private void Connection() {
        if (client != null) {
            if (!client.isConnected) {
                if (!noConnectionObject.activeSelf) {
                    noConnectionObject.SetActive(true);
                }
            } else {
                if (noConnectionObject.activeSelf) {
                    noConnectionObject.SetActive(false);
                }
            }
        }
        if (server != null) {
            if (!server.isConnected) {
                if (!noConnectionObject.activeSelf) {
                    noConnectionObject.SetActive(true);
                }
            } else {
                if (noConnectionObject.activeSelf) {
                    noConnectionObject.SetActive(false);
                }
            }
        }

    }

    private void MyTurn(bool isIt) {
        if (yourTurnObject.activeSelf != isIt) {
            yourTurnObject.SetActive(isIt);
        }
        if (opponentsTurnObject.activeSelf == isIt) {
            opponentsTurnObject.SetActive(!isIt);
        }
    }

    public IEnumerator NewRoundCountDown() {
        betweenRounds = true;
        float duration = 3f; // 3 seconds you can change this 
                             //to whatever you want
        float normalizedTime = 0;
        while (normalizedTime <= 1f) {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        NewRound();
        if (server != null) {
            server.gridManager.EmptyFields();
        }

    }
}
