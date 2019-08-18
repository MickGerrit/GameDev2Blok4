﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {
    public InputField nameField;
    public InputField passwordField;

    public Button submitButton;

    public void CallLogin() {
        StartCoroutine(PlayerLogin());
    }

    IEnumerator PlayerLogin() {
        WWWForm form = new WWWForm();
        form.AddField("username", nameField.text);
        form.AddField("password", passwordField.text);
        WWW www = new WWW("https://studenthome.hku.nl/~mick.gerritsen/database/sql/login.php", form);
        yield return www;
        if (www.text[0] == '0') {
            DataBaseManager.username = nameField.text;
            DataBaseManager.amountOfWins = int.Parse(www.text.Split('\t')[1]);
            DataBaseManager.amountOfLosses = int.Parse(www.text.Split('\t')[2]);
            UnityEngine.SceneManagement.SceneManager.LoadScene("UserMenu");
        }
        else {
            Debug.Log("User login failed. Error: " + www.text);
        }
    }

    public void VerifyInputs() {
        submitButton.interactable = (nameField.text.Length >= 8 && passwordField.text.Length >= 8);
    }
}