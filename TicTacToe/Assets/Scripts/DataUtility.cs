using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataUtility {
    public static Vector2 BytesToVector2(byte[] bytesX, byte[] bytesY) {

        double x = System.BitConverter.ToDouble(bytesX, 0);
        double y = System.BitConverter.ToDouble(bytesY, 0);

        Vector2 vector = new Vector2((float)x, (float)y);
        return vector;
    }

    public static byte[] VectorToBytes(float v2) {
        double rounded = Round(v2, 2);
        byte[] bytes = System.BitConverter.GetBytes(rounded);
        return bytes;
    }

    public static double Round(float value, int digits) {
        float multpliedValue = Mathf.Pow(10.0f, (float)digits);
        return (double)Mathf.Round(value * multpliedValue) / multpliedValue;
    }
}
