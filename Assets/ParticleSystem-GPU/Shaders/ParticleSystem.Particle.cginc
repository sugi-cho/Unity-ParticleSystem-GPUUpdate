#ifndef PARTICLE_SYSTEM_PARTICLE
#define PARTICLE_SYSTEM_PARTICLE
struct Particle
{
    float3 position;
    float3 velocity;
    float3 animatedVelocity;
    float3 unused_totalVelocity; //velocityとanimatedの足し算。自動的に計算される
    float3 axisOfRotation;
    float3 rotation3D;
    float3 angularVelocity3D;
    float3 startSize3D;
    uint startColor; //Color32の代わり 0xFFFFFFFFで設定する(ABRGの順)
    uint randomSeed;
    float remainingLifetime;
    float startLifetime;
    float unused_rotation; //rotation3Dから、自動的に設定されるぽい
    float unused_angularVelocity; //angularVelociry3Dから、自動設定
    //float startSize;
    // sizeof(ParticleSystem.Particle) = 120なので、floatかint、1個分多いので、とりあえずstartSizeをコメントアウトしてみた
    // この並び順は、ParticleUpdate.compute bridge-kernelで調査した
};


uint Particle_ColorToUint(half4 col)
{
    uint col32 = ((uint) (col.r * 0xFF))
        + ((uint) (col.g * 0xFF) << 8)
        + ((uint) (col.b * 0xFF) << 16)
        + ((uint) (col.a * 0xFF) << 24);
    return col32;
}

#endif