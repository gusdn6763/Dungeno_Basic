using UnityEngine;
using System.Collections;
using static Define;

public class CameraSetting : MonoBehaviour
{
    [Header("ī�޶� ����")]
    [SerializeField] private float combatFOV = 50;
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float fovChangeDuration = 3f;
    [SerializeField] private float moveSpeed = 5f;


    private Camera mainCamera;

    public void Init()
    {
        mainCamera = Camera.main;
        Managers.Game.onStateChange += OnGameStateChange;
    }

    private void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0) * moveSpeed * Time.deltaTime;

        // ī�޶� ��ġ ������Ʈ
        mainCamera.transform.position += movement;
    }

    private void OnGameStateChange(E_GameState newState)
    {
        switch (newState)
        {
            case E_GameState.Battle_Start:
                StartCoroutine(CameraChange(combatFOV, fovChangeDuration));
                break;
            case E_GameState.Exploring:
                StartCoroutine(CameraChange(normalFOV, fovChangeDuration));
                break;
        }
    }

    private IEnumerator CameraChange(float targetFOV, float duration)
    {
        float startFOV = mainCamera.fieldOfView;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        mainCamera.fieldOfView = targetFOV;
    }

    [SerializeField] private Color normalFOVColor = Color.green; // �Ϲ� FOV ����
    [SerializeField] private Color combatFOVColor = Color.red; // ���� FOV ����
    private void OnDrawGizmos()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
        {
            // ���� FOV �׸���
            Gizmos.color = Color.yellow;
            DrawFOVGizmo(mainCamera.fieldOfView, -transform.position.z);

            // �Ϲ� FOV �׸���
            Gizmos.color = normalFOVColor;
            DrawFOVGizmo(normalFOV, -transform.position.z);

            // ���� FOV �׸���
            Gizmos.color = combatFOVColor;
            DrawFOVGizmo(combatFOV, -transform.position.z);
        }
    }

    private void DrawFOVGizmo(float fov, float distance)
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = mainCamera.transform.localToWorldMatrix;

        float halfFOV = fov * 0.5f;
        float aspect = mainCamera.aspect;

        Vector3 forward = Vector3.forward * distance;
        Vector3 top = Mathf.Tan(halfFOV * Mathf.Deg2Rad) * Vector3.up * distance;
        Vector3 right = Mathf.Tan(halfFOV * Mathf.Deg2Rad) * Vector3.right * distance * aspect;

        // ����
        Gizmos.DrawLine(Vector3.zero, forward + top + right);
        Gizmos.DrawLine(Vector3.zero, forward + top - right);
        Gizmos.DrawLine(Vector3.zero, forward - top + right);
        Gizmos.DrawLine(Vector3.zero, forward - top - right);

        // �ĸ�
        Gizmos.DrawLine(forward + top + right, forward + top - right);
        Gizmos.DrawLine(forward + top - right, forward - top - right);
        Gizmos.DrawLine(forward - top - right, forward - top + right);
        Gizmos.DrawLine(forward - top + right, forward + top + right);

        Gizmos.matrix = oldMatrix;
    }
}