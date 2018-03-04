using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class ParticleGPUUpdate : MonoBehaviour
{

    public ComputeShader updater;
    public ParticleSystem pSystem;
    public string kernelName = "update";
    public UnityEvent GPUSetup;

    ParticleSystem.Particle[] particles;

    ComputeBuffer particlesDataBuffer;
    new Camera camera;

    Vector3 mousePos;
    float attraction;

    // Use this for initialization
    void Start()
    {
        var maxParticles = pSystem.main.maxParticles;
        particles = new ParticleSystem.Particle[maxParticles];

        var structSize = Marshal.SizeOf(typeof(ParticleSystem.Particle));
        particlesDataBuffer = new ComputeBuffer(maxParticles, structSize);
        camera = Camera.main;
    }
    private void OnDestroy()
    {
        if (particlesDataBuffer != null)
            particlesDataBuffer.Release();
    }

    private void FixedUpdate()
    {
        mousePos = Input.mousePosition;
        mousePos.z = 15f;
        mousePos = camera.ScreenToWorldPoint(mousePos);
        attraction = Input.GetMouseButton(0) ? 0.5f : 0f;

        GPUUpdate();
    }

    void GPUUpdate()
    {
        GPUSetup.Invoke();

        var numParticles = pSystem.GetParticles(particles);
        var kernel = updater.FindKernel(kernelName);
        var threadsX = Mathf.CeilToInt(numParticles / 8f);

        particlesDataBuffer.SetData(particles, 0, 0, numParticles);
        updater.SetBuffer(kernel, "_Particles", particlesDataBuffer);

        updater.SetVector("mousePos", mousePos);
        updater.SetFloat("attraction", attraction);
        updater.SetFloat("dt", Time.fixedDeltaTime);

        updater.Dispatch(kernel, threadsX, 1, 1);
        particlesDataBuffer.GetData(particles, 0, 0, numParticles);

        pSystem.SetParticles(particles, numParticles);
    }
}
