using DG.Tweening;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public Camera MainCamera;
    public float ShakeDelay;
    [SerializeField] private float _shakeDuration;
    [SerializeField] private float _shakeStrength;

    private void Start()
    {
        Invoke("ShakeCamera", ShakeDelay);
    }

    [ContextMenu("ShakeCam")]
    public void ShakeCamera()
    {
        MainCamera.DOShakePosition(_shakeDuration, _shakeStrength, 2, 45f, true);
    }
}
