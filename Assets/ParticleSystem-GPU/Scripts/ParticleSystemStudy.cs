using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;
using UnityEngine;

public class ParticleSystemStudy : MonoBehaviour
{
    public Particle editable;
    public Particle[] particles;

    public ComputeShader computeBridge;
    ComputeBuffer particleDataBuffer;

    new ParticleSystem particleSystem;
    ParticleSystem.Particle[] particleData;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleData = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        particles = new Particle[particleData.Length];

        var size = Marshal.SizeOf(typeof(ParticleSystem.Particle));
        Debug.Log(size);
        size = Marshal.SizeOf(typeof(Particle));
        Debug.Log(size);

        size = 0;
        var particle = new ParticleSystem.Particle();
        Debug.Log(Marshal.SizeOf(particle.startSize));

        var ps = typeof(ParticleSystem.Particle).GetProperties();

        foreach (var p in ps)
        {
            if (p.GetCustomAttributes(false).Length < 1)
                if (p.CanWrite)
                {
                    //Debug.Log(p);
                    //Debug.Log(p.PropertyType);
                    size += Marshal.SizeOf(p.PropertyType);
                }
        }
        Debug.Log(size);

        particleDataBuffer = new ComputeBuffer(10, Marshal.SizeOf(typeof(ParticleSystem.Particle)));
    }

    private void OnDestroy()
    {
        particleDataBuffer.Release();
    }

    private void Update()
    {

        var num = particleSystem.GetParticles(particleData);
        particleDataBuffer.SetData(particleData, 0, 0, num);
        var kernel = computeBridge.FindKernel("bridge");
        computeBridge.SetBuffer(kernel, "_ParticlesOUT", particleDataBuffer);
        computeBridge.Dispatch(kernel, 1, 1, 1);
        particleDataBuffer.GetData(particleData, 0, 0, num);

        for (var i = 0; i < num; i++)
            particles[i].SetData(particleData[i]);

    }

    //for inspector
    [System.Serializable]
    public struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 animatedVelocity;
        public Vector3 totalVelocity;
        public float remainingLifetime;
        public float startLifetime;
        public Vector3 startSize3D;
        public Vector3 axisOfRotation;
        public float rotation;
        public Vector3 rotation3D;
        public float angularVelocity;
        public Vector3 angularVelocity3D;
        public Color32 startColor;
        public uint randomSeed;
        public float startSize;

        public void SetData(ParticleSystem.Particle source)
        {
            position = source.position;
            velocity = source.velocity;
            animatedVelocity = source.animatedVelocity;
            totalVelocity = source.totalVelocity;
            remainingLifetime = source.remainingLifetime;
            startLifetime = source.startLifetime;
            startSize3D = source.startSize3D;
            axisOfRotation = source.axisOfRotation;
            rotation = source.rotation;
            rotation3D = source.rotation3D;
            angularVelocity = source.angularVelocity;
            angularVelocity3D = source.angularVelocity3D;
            startColor = source.startColor;
            randomSeed = source.randomSeed;
            startSize = source.startSize;
        }

        public ParticleSystem.Particle GetUnityParticle()
        {
            var particle = new ParticleSystem.Particle();
            particle.position = position;
            particle.velocity = velocity;
            //particle.animatedVelocity = animatedVelocity;
            //particle.totalVelocity = totalVelocity;
            particle.remainingLifetime = remainingLifetime;
            particle.startLifetime = startLifetime;
            particle.startSize3D = startSize3D;
            particle.axisOfRotation = axisOfRotation;
            particle.rotation = rotation;
            particle.rotation3D = rotation3D;
            particle.angularVelocity = angularVelocity;
            particle.angularVelocity3D = angularVelocity3D;
            particle.startColor = startColor;
            particle.randomSeed = randomSeed;
            particle.startSize = startSize;
            return particle;
        }
    }
}
