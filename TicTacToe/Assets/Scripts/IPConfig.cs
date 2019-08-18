using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IPConfig : MonoBehaviour {

    public InputField inputField;
    private string inputFieldIP;
    private bool ipChangeRequest;

    private void Awake() {
        ipChangeRequest = false;
    }

    private void Update() {

        if (SceneManager.GetActiveScene().name == "ClientScene") {
            DontDestroyOnLoad(this.gameObject);

            if (ipChangeRequest) {
                Client.Instance.RestartConnection(inputFieldIP);
            }

            Destroy(this.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public void SetIpAddress() {
        Debug.Log(inputField.text);
        inputFieldIP = inputField.text;
        ipChangeRequest = true;
    }

}
