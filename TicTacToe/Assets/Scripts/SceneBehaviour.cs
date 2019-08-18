using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneBehaviour : MonoBehaviour{
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Quit();
        }
    }
    public void LoadScene(string name) {
        SceneManager.LoadScene(name);
    }

    public void Quit() {
        Application.Quit();

    }

    public void CallSaveAndLogOut() {
        StartCoroutine(SaveAndLogOut());
    }

    IEnumerator SaveAndLogOut() {
        if (DataBaseManager.LoggedIn) {
            WWWForm form = new WWWForm();
            form.AddField("username", DataBaseManager.username);
            form.AddField("amountofwins", DataBaseManager.amountOfWins);
            form.AddField("amountoflosses", DataBaseManager.amountOfLosses);

            WWW www = new WWW("https://studenthome.hku.nl/~mick.gerritsen/database/sql/savedata.php", form);
            yield return www;
            if (www.text == "0") {
                Debug.Log("Game Saved");
            } else {
                Debug.Log(www.text);
            }

            DataBaseManager.LogOut();
        }
        Quit();
    }
}
