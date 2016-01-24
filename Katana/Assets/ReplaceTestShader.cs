using UnityEngine;
using System.Collections;

public class ReplaceTestShader : MonoBehaviour {
    public Material mat;
    public void Replace()
    {
        Debug.Log("Collide Spike");
        var material = GetComponent<SkinnedMeshRenderer>();
        material.materials[0].shader = Shader.Find("Custom/surface_two_pass");
    }

}
