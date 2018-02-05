#ifndef PARTICLE_SYSTEM_PARTICLE
#define PARTICLE_SYSTEM_PARTICLE
struct Particle
{
    float3 position;
    float3 velocity;
    float3 animatedVelocity;
    float3 totalVelocity;
    float remainingLifetime;
    float startLifetime;
    float3 startSize3D;
    float3 axisOfRotation;
    float rotation;
    float3 rotation3D;
    float angularVelocity;
    float3 angularVelocity3D;
    uint startColor; //Color32の代わり
    uint randomSeed;
//  float startSize;
// sizeof(ParticleSystem.Particle) = 120なので、floatかint、1個分多いので、とりあえずstartSizeをコメントアウトしてみた
// この並び順は、UnityEngine.ParticleSystem.ParticleのメタデータのStructの逆順。
};

#endif