using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GatheredParticle : MonoBehaviour
{

    public ComputeShader updater;
    public ParticleSystem pSystem;
    public string kernelName = "gathered";
    public Mesh targetMesh;
    public Transform targetTransform;
    public Material vertBufferWriter;

    ParticleSystem.Particle[] particles;
    ComputeBuffer particlesDataBuffer;
    ComputeBuffer targetVertexDataBuffer;

    struct vertexData
    {
        public Vector3 pos;
        public Vector3 normal;
        public Vector2 uv;
    }

    // Use this for initialization
    void Start()
    {
        var maxParticles = pSystem.main.maxParticles;
        particles = new ParticleSystem.Particle[maxParticles];

        var structSize = Marshal.SizeOf(typeof(ParticleSystem.Particle));
        particlesDataBuffer = new ComputeBuffer(maxParticles, structSize);

        var vertCount = targetMesh.vertexCount;
        structSize = Marshal.SizeOf(typeof(vertexData));
        targetVertexDataBuffer = new ComputeBuffer(vertCount, structSize);
    }

    private void OnDestroy()
    {
        new List<ComputeBuffer>() { particlesDataBuffer, targetVertexDataBuffer }.ForEach(cb => cb.Dispose());
    }

    private void FixedUpdate()
    {
        UpdateTargetVertBuffer();
        UpdateParticles();
    }

    void UpdateTargetVertBuffer()
    {
        Graphics.SetRandomWriteTarget(1, targetVertexDataBuffer);
        vertBufferWriter.SetPass(0);
        Graphics.DrawMeshNow(targetMesh, targetTransform.localToWorldMatrix);
        Graphics.ClearRandomWriteTargets();
    }

    void UpdateParticles()
    {
        var numPerticles = pSystem.GetParticles(particles);
        var kernel = updater.FindKernel(kernelName);
        var threadsX = Mathf.CeilToInt(numPerticles / 8f);

        particlesDataBuffer.SetData(particles, 0, 0, numPerticles);
        updater.SetBuffer(kernel, "_Particles", particlesDataBuffer);
        updater.SetBuffer(kernel, "_TargetData", targetVertexDataBuffer);
        updater.SetFloat("dt", Time.fixedDeltaTime);
        updater.SetInt("numVerts", targetMesh.vertexCount);

        updater.Dispatch(kernel, threadsX, 1, 1);
        particlesDataBuffer.GetData(particles, 0, 0, numPerticles);
        pSystem.SetParticles(particles, numPerticles);
    }
}
