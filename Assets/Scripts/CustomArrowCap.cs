using UnityEngine;
using UnityEditor;

public static class CustomArrowCap
{
    public static void DrawCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
        // Set the scale for the arrow tip
        float arrowTipScale = 0.3f; // You can change this value to make the arrow tip bigger or smaller

        Handles.color = new Color(1, 1, 0, 1f);

        // Draw the arrow tip with the custom scale
        Handles.ConeHandleCap(controlID, position, rotation, size * arrowTipScale, eventType);
    }
}