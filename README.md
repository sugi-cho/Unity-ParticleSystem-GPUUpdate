# Unity-ParticleSystem-GPUUpdate
Update built-in ParticleSystem with ComputeShader

## ParticleSystem.Particle for shader

[struct ParticleSystem.Particle](https://docs.unity3d.com/ja/2017.3/ScriptReference/ParticleSystem.Particle.html)

[cginc](https://github.com/sugi-cho/Unity-ParticleSystem-GPUUpdate/blob/master/Assets/ParticleSystem-GPU/Shaders/ParticleSystem.Particle.cginc)

## GPUで処理するのとCPUで処理するので、比べてみた

`ParticleSystem.GetParticles(particles)`, `ParticleSystem.SetParticles(particles)`,`ComputeBuffer.SetData(particles)`,`ComputeBuffer.GetData(particles)`あたりの処理が、パーティクル数が増えると重くなってくるっぽい。
現状は、そんなに使えない感じがする

[GPU処理](https://github.com/sugi-cho/Unity-ParticleSystem-GPUUpdate/blob/master/Assets/ParticleSystem-GPU/Scripts/ParticleGPUUpdate.cs#L57-L74)

ComputeBufferから直接パーティクル情報をSetできる何かあると良いな
