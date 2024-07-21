using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private CameraSetting cameraSetting;

    private Camera mainCamera;
    public Camera MainCamera { get => mainCamera; }
    public void Init()
    {
        mainCamera = Camera.main;
        cameraSetting.Init(mainCamera);
        startButton.gameObject.SetActive(true);
    }

    public void CameraZoomOn(Vector3 targetPosition)
    {
        cameraSetting.CameraZoomOn(targetPosition);
    }

    public void CameraZoomOff()
    {
        cameraSetting.CameraZoomOff();
    }

    public bool CheckOutArea(Vector3 pos)
    {
        return cameraSetting.CheckOutArea(pos);
    }
}
