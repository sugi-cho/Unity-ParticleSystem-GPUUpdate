using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ParticleGPUUpdate : MonoBehaviour
{

    public ComputeShader updater;
    public ParticleSystem pSystem;
    public bool useCPU;

    ParticleSystem.Particle[] particlesIN;
    ParticleSystem.Particle[] particlesOUT;

    ComputeBuffer particlesINBuffer;
    ComputeBuffer particlesOUTBuffer;
    new Camera camera;

    Vector3 mousePos;
    float attraction;

    // Use this for initialization
    void Start()
    {
        var maxParticles = pSystem.main.maxParticles;
        particlesIN = new ParticleSystem.Particle[maxParticles];
        particlesOUT = new ParticleSystem.Particle[maxParticles];

        var structSize = Marshal.SizeOf(typeof(ParticleSystem.Particle));
        particlesINBuffer = new ComputeBuffer(maxParticles, structSize);
        particlesOUTBuffer = new ComputeBuffer(maxParticles, structSize);
        camera = Camera.main;

        //Debug.Log(structSize); => 120
    }
    private void OnDestroy()
    {
        if (particlesINBuffer != null)
            particlesINBuffer.Release();
        if (particlesOUTBuffer != null)
            particlesOUTBuffer.Release();
    }

    private void Update()
    {
        mousePos = Input.mousePosition;
        mousePos.z = 5f;
        mousePos = camera.ScreenToWorldPoint(mousePos);
        attraction = Input.GetMouseButton(0) ? 0.5f : 0f;

        if (useCPU)
            CPUUpdate();
        else
            GPUUpdate();
    }

    void GPUUpdate()
    {
        var numParticles = pSystem.GetParticles(particlesIN);
        var kernel = updater.FindKernel("update");
        var threadsX = Mathf.CeilToInt(numParticles / 8f);

        particlesINBuffer.SetData(particlesIN, 0, 0, numParticles);
        updater.SetBuffer(kernel, "_ParticlesIN", particlesINBuffer);
        updater.SetBuffer(kernel, "_ParticlesOUT", particlesOUTBuffer);

        updater.SetVector("mousePos", mousePos);
        updater.SetFloat("attraction", attraction);

        updater.Dispatch(kernel, threadsX, 1, 1);
        particlesOUTBuffer.GetData(particlesOUT, 0, 0, numParticles);

        pSystem.SetParticles(particlesOUT, numParticles);
    }

    void CPUUpdate()
    {
        var numParticles = pSystem.GetParticles(particlesIN);
        for (var i = 0; i < numParticles; i++)
        {
            var p = particlesIN[i];

            var to = mousePos - p.position;
            var dst = Mathf.Max(0.0001f, to.magnitude);
            var force = to.normalized / dst;
            p.velocity += force * attraction;
            particlesOUT[i] = p;
        }
        pSystem.SetParticles(particlesOUT, numParticles);
    }
}
