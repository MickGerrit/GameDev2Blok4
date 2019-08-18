using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataBaseManager {
    public static string username;
    public static int amountOfWins;
    public static int amountOfLosses;

    public static bool LoggedIn { get { return username != null; } }

    public static void LogOut() {
        username = null;
    }

    public static void IncreaseWins() {
        amountOfWins++;
    }
    public static void IncreaseLosses() {
        amountOfLosses++;
    }
}
