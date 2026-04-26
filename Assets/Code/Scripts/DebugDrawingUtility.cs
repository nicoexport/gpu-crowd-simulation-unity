using UnityEngine;

namespace Utility
{
    public static class DebugDrawingUtility
    {
        public static void DrawRect(Rect rect, Color color)
        {
            DrawRect(rect.min, rect.max, color);
        }

        public static void DrawRect(Rect rect, Color color, float duration)
        {
            DrawRect(rect.min, rect.max, color, duration);
        }

        public static void DrawRect(Vector3 min, Vector3 max, Color color)
        {
            Debug.DrawLine(min, new Vector3(min.x, max.y), color);
            Debug.DrawLine(new Vector3(min.x, max.y), max, color);
            Debug.DrawLine(max, new Vector3(max.x, min.y), color);
            Debug.DrawLine(min, new Vector3(max.x, min.y), color);
        }

        public static void DrawRect(Vector3 min, Vector3 max, Color color, float duration)
        {
            Debug.DrawLine(min, new Vector3(min.x, max.y), color, duration);
            Debug.DrawLine(new Vector3(min.x, max.y), max, color, duration);
            Debug.DrawLine(max, new Vector3(max.x, min.y), color, duration);
            Debug.DrawLine(min, new Vector3(max.x, min.y), color, duration);
        }

        // Arrow drawing taken from: https://discussions.unity.com/t/debug-drawarrow/442586
        public static void DrawArrowGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void DrawArrowGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void DrawArrowDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Debug.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Debug.DrawRay(pos + direction, right * arrowHeadLength);
            Debug.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void DrawArrowDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Debug.DrawRay(pos, direction, color);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
            Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
        }
    }
}