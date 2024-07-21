using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public static class Utils
{
    private static void CalculateCameraExtents(float zPosition, out float halfWidth, out float halfHeight)
    {
        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;
    }
    public static Vector3 GetBottomLeftPosition(float zPosition)
    {
        CalculateCameraExtents(zPosition, out float halfWidth, out float halfHeight);
        Vector3 bottomLeft = Camera.main.transform.position - new Vector3(halfWidth, halfHeight, 0f);
        bottomLeft.z = zPosition;
        return bottomLeft;
    }
    public static Vector3 GetBottomRightPosition(float zPosition)
    {
        CalculateCameraExtents(zPosition, out float halfWidth, out float halfHeight);
        Vector3 bottomRight = Camera.main.transform.position + new Vector3(halfWidth, -halfHeight, 0f);
        bottomRight.z = zPosition;
        return bottomRight;
    }
    public static Vector3 GetTopLeftPosition(float zPosition)
    {
        CalculateCameraExtents(zPosition, out float halfWidth, out float halfHeight);
        Vector3 topLeft = Camera.main.transform.position + new Vector3(-halfWidth, halfHeight, 0f);
        topLeft.z = zPosition;
        return topLeft;
    }
    public static Vector3 GetTopRightPosition(float zPosition)
    {
        CalculateCameraExtents(zPosition, out float halfWidth, out float halfHeight);
        Vector3 topRight = Camera.main.transform.position + new Vector3(halfWidth, halfHeight, 0f);
        topRight.z = zPosition;
        return topRight;
    }

    public static Vector3 GetRandomPosition(Vector3 size, float zPosition = 0f)
    {
        Vector3 bottomLeft = GetBottomLeftPosition(zPosition);
        Vector3 topRight = GetTopRightPosition(zPosition);

        float randomX = Random.Range(bottomLeft.x + size.x, topRight.x - size.x);
        float randomY = Random.Range(bottomLeft.y + size.y, topRight.y - size.y);

        return new Vector3(randomX, randomY, zPosition);
    }
}