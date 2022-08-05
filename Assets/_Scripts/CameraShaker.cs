using DG.Tweening;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public Camera MainCamera;

    private void Start()
    {
        
    }

    public void ShakeCamera()
    {
        MainCamera.DOShakePosition(0.3f, 0.25f, 2, 45f, true);
    }
}
