using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField] private GameObject indicatorPrefab;

    private Camera mainCamera;
    private RectTransform indicatorRect;
    private bool isVisible = true;

    [SerializeField] float sideEdge = 0.06f;
    [SerializeField] float topDownEdge = 0.07f;
    [SerializeField] float minScale = 0.5f;
    [SerializeField] float maxScale = 1.5f;
    [SerializeField] float minDistance = 5f;
    [SerializeField] float maxDistance = 50f;

    private void Start()
    {
        mainCamera = Camera.main;
        SetVisibility(false);
    }

    private void Update()
    {
        if (!isVisible)
        {
            UpdateIndicatorPosition();
        }
    }

    private void OnDestroy()
    {
        DestroyIndicator();
    }

    private void OnBecameVisible()
    {
        SetVisibility(true);
    }
    private void OnBecameInvisible()
    {
        SetVisibility(false);
    }

    private void SetVisibility(bool isOn)
    {
        if (isOn != isVisible)
        {
            isVisible = isOn;
            if (isVisible)
            {
                DestroyIndicator();
            }
            else
            {
                CreateIndicator();
            }
        }
    }
    private void CreateIndicator()
    {
        if (indicatorRect == null)
        {
            GameObject indicator = Instantiate(indicatorPrefab, Managers.UI.IndicatorCanvas.transform);
            indicatorRect = indicator.GetComponent<RectTransform>();
        }
    }
    private void DestroyIndicator()
    {
        if (indicatorRect != null)
        {
            Destroy(indicatorRect.gameObject);
            indicatorRect = null;
        }
    }

    private void UpdateIndicatorPosition()
    {
        if (indicatorRect == null)
            return;

        Vector3 objectScreenPosition = mainCamera.WorldToScreenPoint(transform.position);
        Vector2 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
        Vector2 normalizedPosition = new Vector2(viewportPosition.x - 0.5f, viewportPosition.y - 0.5f);
        float angle = Mathf.Atan2(normalizedPosition.y, normalizedPosition.x) * Mathf.Rad2Deg;
        Vector2 anchorPosition = CalculateAnchorPosition(angle, normalizedPosition);

        indicatorRect.anchorMin = anchorPosition;
        indicatorRect.anchorMax = anchorPosition;
        indicatorRect.anchoredPosition = Vector2.zero;

        // 거리에 따른 스케일 조절
        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        float scale = Mathf.Lerp(maxScale, minScale, Mathf.InverseLerp(minDistance, maxDistance, distance));
        indicatorRect.localScale = Vector3.one * scale;

        // 화살표 회전 적용
        indicatorRect.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    private Vector2 CalculateAnchorPosition(float angle, Vector2 normalizedPosition)
    {
        if (angle > -45 && angle <= 45) // Right
        {
            return new Vector2(1 - sideEdge, Mathf.Clamp(0.5f + normalizedPosition.y / normalizedPosition.x * 0.5f, topDownEdge, 1 - topDownEdge));
        }
        else if (angle > 45 && angle <= 135) // Top
        {
            return new Vector2(Mathf.Clamp(0.5f + normalizedPosition.x / normalizedPosition.y * 0.5f, sideEdge, 1 - sideEdge), 1- topDownEdge);
        }
        else if (angle > 135 || angle <= -135) // Left
        {
            return new Vector2(sideEdge, Mathf.Clamp(0.5f - normalizedPosition.y / normalizedPosition.x * 0.5f, topDownEdge, 1 - topDownEdge));
        }
        else // Bottom
        {
            return new Vector2(Mathf.Clamp(0.5f - normalizedPosition.x / normalizedPosition.y * 0.5f, sideEdge, 1 - sideEdge), topDownEdge);
        }
    }
}