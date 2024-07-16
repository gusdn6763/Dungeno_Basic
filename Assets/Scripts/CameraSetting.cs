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
    private Vector2 mapSize;
    private Vector2 combatMovableBounds;

    public void Init(Camera camera)
    {
        mainCamera = camera;
        Managers.Game.onStateChange += OnGameStateChange;
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

    private void Update()
    {
        //근사값 비교
        if (Mathf.Approximately(mainCamera.orthographicSize, combatOrthographicSize))
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

    Coroutine zoomCoroutine;
    private void OnGameStateChange(E_AreaState newState)
    {
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        if (newState == E_AreaState.Battle_Start)
            zoomCoroutine = StartCoroutine(CameraZoom(combatOrthographicSize, zoomChangeDuration));
        else
            zoomCoroutine = StartCoroutine(CameraZoom(normalOrthographicSize, zoomChangeDuration));
    }

    private IEnumerator CameraZoom(float targetOrthographicSize, float duration)
    {
        float startOrthographicSize = mainCamera.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            mainCamera.orthographicSize = Mathf.Lerp(startOrthographicSize, targetOrthographicSize, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.orthographicSize = targetOrthographicSize;
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