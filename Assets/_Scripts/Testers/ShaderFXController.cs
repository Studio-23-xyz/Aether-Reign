using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShaderFXController : MonoBehaviour
{
    public List<MeshRenderer> TargetMeshes;
    public float TimeToComplete = 1.8f;
    public float RefreshRate = 0.02f;
    [Range(0, 1)] public float MinGrow = 0.2f;
    [Range(0, 1)] public float MaxGrow = 0.97f;
    public float DissipateDelay = 1f;

    private List<Material> TargetMaterials = new List<Material>();
    [SerializeField] private bool IsCompleted;
    
    void Start()
    {
        for (int i = 0; i < TargetMeshes.Count; i++)
        {
            for (int j = 0; j < TargetMeshes[i].materials.Length; j++)
            {
                if (TargetMeshes[i].materials[j].HasProperty("_Grow"))
                {
                    TargetMeshes[i].materials[j].SetFloat("_Grow", MinGrow);
                    TargetMaterials.Add(TargetMeshes[i].materials[j]);
                }
            }
        }
        TriggerFX();
        Invoke("TriggerFX", TimeToComplete + DissipateDelay);
    }

    private void TriggerFX()
    {
        for (int i=0; i<TargetMaterials.Count; i++)
            GrowMesh(TargetMaterials[i]);
    }
    
    private async void GrowMesh(Material mat)
    {
        float growValue = mat.GetFloat("_Grow");

        if (!IsCompleted)
        {
            while (growValue < MaxGrow)
            {
                growValue += 1 / (TimeToComplete / RefreshRate);
                mat.SetFloat("_Grow", growValue);

                await UniTask.Delay(TimeSpan.FromSeconds(RefreshRate));
            }
        }
        else
        {
            while (growValue > MinGrow)
            {
                growValue -= 1 / (TimeToComplete / RefreshRate);
                mat.SetFloat("_Grow", growValue);

                await UniTask.Delay(TimeSpan.FromSeconds(RefreshRate));
            }
        }

        IsCompleted = growValue >= MaxGrow;
    }
}
