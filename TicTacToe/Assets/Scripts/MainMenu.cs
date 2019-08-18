using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public Button playButton;
    public Text userDisplay;
    public Text amountOfWinsText;
    public Text amountOfLossesText;
    private void Update() {
        if (DataBaseManager.LoggedIn) {
            playButton.interactable = true;
            userDisplay.text = "Hello " + DataBaseManager.username;
            amountOfWinsText.text = "Wins: " + DataBaseManager.amountOfWins;
            amountOfLossesText.text = "Losses: " + DataBaseManager.amountOfLosses;
        } else {
            playButton.interactable = false;
            amountOfWinsText.text = "";
            amountOfLossesText.text = "";
            userDisplay.text = "Please, log in.";
        }
    }
}
