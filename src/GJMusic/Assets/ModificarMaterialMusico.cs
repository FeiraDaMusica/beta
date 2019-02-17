using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModificarMaterialMusico : MonoBehaviour
{
    public List<Material> materiaisMusicos = new List<Material>();
    public List<SkinnedMeshRenderer> musicosRoupas = new List<SkinnedMeshRenderer>();


    void Start()
    {
        foreach(SkinnedMeshRenderer musicos in musicosRoupas)
        {
            musicos.material = materiaisMusicos[Random.Range(0, materiaisMusicos.Count)];
        } 
    }
}
