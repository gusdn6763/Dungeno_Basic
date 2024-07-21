using UnityEngine;
using System.Collections;
using static Define;

public class CameraSetting : MonoBehaviour
{
    [Header("카메라 설정")]
    [SerializeField] private float combatOrthographicSize = 3.5f; // 전투 시 orthographicSize
    [SerializeField] private float normalOrthographicSize = 5f; // 일반 상황 orthographicSize
    [SerializeField] private float zoomChangeDuration = 1f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float edgeScrollThreshold = 10f;

    private Camera mainCamera;
    private Creature currentCreature;
    private Vector2 mapSize;
    private Vector2 combatMovableBounds;

    #region 세팅
    public void Init(Camera camera)
    {
        mainCamera = camera;
        CalculateMapSize();
        CalculateMovableBounds();
    }
    private void CalculateMapSize()
    {
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;
        mapSize = new Vector2(width, height);
    }
    private void CalculateMovableBounds()
    {
        float verticalSize = combatOrthographicSize;
        float horizontalSize = combatOrthographicSize * mainCamera.aspect;

        float horizontalMovableRange = (mapSize.x / 2) - horizontalSize;
        float verticalMovableRange = (mapSize.y / 2) - verticalSize;

        combatMovableBounds = new Vector2(horizontalMovableRange, verticalMovableRange);
    }
    #endregion

    private void Update()
    {
        //근사값 비교
        if (Mathf.Approximately(mainCamera.orthographicSize, combatOrthographicSize) && zoomCoroutine == null)
            MoveCamera();
    }
    private void MoveCamera()
    {
        Vector3 movement = Vector3.zero;

        // 마우스 위치 확인
        Vector3 mousePosition = Input.mousePosition;

        if (mousePosition.x <= edgeScrollThreshold)
            movement.x = -1;
        else if (mousePosition.x >= Screen.width - edgeScrollThreshold)
            movement.x = 1;

        if (mousePosition.y <= edgeScrollThreshold)
            movement.y = -1;
        else if (mousePosition.y >= Screen.height - edgeScrollThreshold)
            movement.y = 1;

        movement = movement.normalized * moveSpeed * Time.deltaTime;

        // 카메라 위치 업데이트 및 경계 제한
        Vector3 newPosition = mainCamera.transform.position + movement;
        newPosition.x = Mathf.Clamp(newPosition.x, -combatMovableBounds.x, combatMovableBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, -combatMovableBounds.y, combatMovableBounds.y);

        mainCamera.transform.position = newPosition;
    }

    public bool CheckOutArea(Vector3 position)
    {
        if (position.x < -mapSize.x / 2 || position.x > mapSize.x / 2 ||
            position.y < -mapSize.y / 2 || position.y > mapSize.y / 2)
        {
            return true;
        }
        return false;
    }

    Coroutine zoomCoroutine;
    public void CameraZoomOn(Vector3 targetPosition)
    {
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(CameraZoomOn(combatOrthographicSize, targetPosition));
    }
    public void CameraZoomOff()
    {
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(CameraZoomOff(normalOrthographicSize));
    }

    private IEnumerator CameraZoomOn(float targetOrthographicSize, Vector3 targetPosition)
    {
        float startOrthographicSize = mainCamera.orthographicSize;
        Vector3 startPosition = mainCamera.transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < zoomChangeDuration)
        {
            // 카메라 크기 조정
            mainCamera.orthographicSize = Mathf.Lerp(startOrthographicSize, targetOrthographicSize, elapsedTime / zoomChangeDuration);

            // 새로운 카메라 위치 계산
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / zoomChangeDuration);

            // 카메라 위치를 맵 경계 내로 제한
            float halfHeight = mainCamera.orthographicSize;
            float halfWidth = halfHeight * mainCamera.aspect;

            newPosition.x = Mathf.Clamp(newPosition.x, -mapSize.x / 2 + halfWidth, mapSize.x / 2 - halfWidth);
            newPosition.y = Mathf.Clamp(newPosition.y, -mapSize.y / 2 + halfHeight, mapSize.y / 2 - halfHeight);
            newPosition.z = -10; // 카메라의 z 위치 유지

            mainCamera.transform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 위치 및 크기 설정
        mainCamera.orthographicSize = targetOrthographicSize;
        Vector3 finalPosition = targetPosition;
        finalPosition.x = Mathf.Clamp(finalPosition.x, -mapSize.x / 2 + mainCamera.orthographicSize * mainCamera.aspect, mapSize.x / 2 - mainCamera.orthographicSize * mainCamera.aspect);
        finalPosition.y = Mathf.Clamp(finalPosition.y, -mapSize.y / 2 + mainCamera.orthographicSize, mapSize.y / 2 - mainCamera.orthographicSize);
        finalPosition.z = -10;
        mainCamera.transform.position = finalPosition;
        zoomCoroutine = null;
    }

    private IEnumerator CameraZoomOff(float targetOrthographicSize)
    {
        float startOrthographicSize = mainCamera.orthographicSize;
        Vector3 startPosition = mainCamera.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < zoomChangeDuration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(startOrthographicSize, targetOrthographicSize, elapsedTime / zoomChangeDuration);

            mainCamera.transform.position = Vector3.Lerp(startPosition, new Vector3(0, 0, -10), elapsedTime / zoomChangeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.orthographicSize = targetOrthographicSize;
        zoomCoroutine = null;
    }

    #region 기즈모
    [SerializeField] private Color normalBoundsColor = Color.green; // 일반 상황 경계 색상
    [SerializeField] private Color combatBoundsColor = Color.red; // 전투 상황 경계 색상

    private void OnDrawGizmos()
    {
        if (mainCamera != null)
        {
            // 맵 전체 영역 그리기
            Gizmos.color = Color.white;
            DrawMapBounds();

            // 일반 상황의 카메라 영역
            Gizmos.color = normalBoundsColor;
            DrawCameraBoundsGizmo(normalOrthographicSize, Vector3.zero);

            // 전투 상황의 카메라 이동 가능 영역
            Gizmos.color = combatBoundsColor;
            DrawCameraBoundsGizmo(combatOrthographicSize, mainCamera.transform.position);

            // 현재 카메라 위치 표시
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(mainCamera.transform.position, 0.5f);
        }
    }

    private void DrawMapBounds()
    {
        Vector3 topLeft = new Vector3(-mapSize.x / 2, mapSize.y / 2, 0);
        Vector3 topRight = new Vector3(mapSize.x / 2, mapSize.y / 2, 0);
        Vector3 bottomLeft = new Vector3(-mapSize.x / 2, -mapSize.y / 2, 0);
        Vector3 bottomRight = new Vector3(mapSize.x / 2, -mapSize.y / 2, 0);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    private void DrawCameraBoundsGizmo(float orthographicSize, Vector3 offset)
    {
        float height = 2f * orthographicSize;
        float width = height * mainCamera.aspect;

        Vector3 center = new Vector3(offset.x, offset.y, 0); // 오브젝트 평면의 중심
        Vector3 topLeft = center + new Vector3(-width / 2, height / 2, 0);
        Vector3 topRight = center + new Vector3(width / 2, height / 2, 0);
        Vector3 bottomLeft = center + new Vector3(-width / 2, -height / 2, 0);
        Vector3 bottomRight = center + new Vector3(width / 2, -height / 2, 0);

        // 경계 상자 그리기
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    #endregion
}