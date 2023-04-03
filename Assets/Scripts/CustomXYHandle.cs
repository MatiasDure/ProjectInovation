using UnityEngine;
using UnityEditor;

public static class CustomXYHandle
{
    public static Vector3 XYPlaneMoveHandle(Vector3 position, float size)
    {
        size *= 2;
        // Draw a 2D rectangle for the X-Y plane move handle
        Handles.color = new Color(1,1,1, 1f);
        Handles.DrawSolidRectangleWithOutline(
            new Rect(position - new Vector3(size / 2, size / 2, 0), new Vector2(size, size)),
            new Color(1f, 1f, 0, 1),
            Color.black);

        // Draw the X-Y plane move handle
        Handles.color = Color.white;
        return Handles.FreeMoveHandle(position, Quaternion.identity, size, Vector3.zero, Handles.RectangleHandleCap);
    }


    public static void SolidCircleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
        switch (eventType)
        {
            case EventType.Layout:
            case EventType.MouseMove:
                Handles.CircleHandleCap(controlID, position, rotation, size, eventType);
                break;
            case EventType.Repaint:
                Handles.DrawSolidDisc(position, rotation * Vector3.forward, size);
                break;
        }
    }
}