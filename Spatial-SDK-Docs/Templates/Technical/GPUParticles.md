# GPU Particles Template

## Overview
The GPU Particles template provides a high-performance particle system that leverages GPU computation for rendering large numbers of particles with minimal impact on frame rate. This template demonstrates how to implement compute shader-based particle systems optimized for WebGL deployment in Spatial experiences. It includes various effect presets, a customizable particle editor, and integration with Spatial's event system.

## Features
- **GPU-Accelerated Particles**: Compute shader-based particle system for handling thousands of particles
- **WebGL Optimization**: Specially optimized for web deployment with fallbacks for different capability levels
- **Interactive Editor**: In-editor particle system designer for easy customization
- **Effect Presets**: Various pre-configured particle effects including fire, smoke, water, magic, and explosions
- **Event Integration**: Trigger particle effects based on Spatial events
- **Performance Monitoring**: Built-in performance tracking with dynamic quality adjustment
- **LOD System**: Automatic level of detail scaling based on distance and performance
- **Collision Support**: Optional particle collision with simplified physics
- **Lifespan Management**: Automatic cleanup and recycling of particle systems

## Included Components

### 1. GPU Particle System
The core component for high-performance particle rendering:
- Compute shader implementation for particle simulation
- Instanced rendering for efficient drawing
- Buffer management for particle data
- Multiple emission shapes (point, sphere, cone, etc.)
- Customizable particle properties (size, color, lifetime, etc.)
- Runtime parameter adjustment
- WebGL compatibility layer

### 2. Particle Effect Editor
In-editor tools for designing and tweaking particle effects:
- Visual parameter adjustment
- Real-time preview
- Preset management
- Performance impact analysis
- Batch editing capability
- Export/import of effect definitions
- Mobile preview mode

### 3. Effect Trigger System
System for activating particle effects at runtime:
- Event-based triggering
- Programmatic control API
- Time-based sequencing
- Animation curve support
- Collision-based activation
- Distance-based activation/deactivation
- Actor interaction triggers

### 4. Performance Management
Tools for maintaining performance across different devices:
- Automatic LOD system
- Quality tier detection
- Dynamic particle count adjustment
- Frame rate monitoring
- Fallback effects for low-performance devices
- Particle budget management
- Culling optimization

## Integration with SDK Components
The template integrates with these key SDK components:

| SDK Component | Usage in Template |
|---------------|-------------------|
| SpatialBridge | Core connectivity to Spatial's systems |
| SpatialTriggerEvent | Triggering particle effects based on collisions |
| IActorService | Tracking actor proximity for particle effects |
| ICameraService | Distance-based LOD for particle systems |
| IPerformanceService | Monitoring and adjusting particle quality based on performance |
| SpatialNetworkObject | Synchronizing particle effect triggers across clients |

## When to Use
Use this template when:
- Creating visually rich environments with atmospheric effects
- Implementing special effects for games (explosions, magic, etc.)
- Building interactive experiences with visual feedback
- Developing experiences that require large numbers of particles
- Creating weather or environmental systems
- Designing immersive visual experiences
- Implementing gameplay mechanics that involve particle effects
- Developing multi-platform experiences that need optimized particles

## Implementation Details

### GPU Particle System Implementation
The core shader-based particle system:

```csharp
using UnityEngine;
using System.Collections.Generic;

public class GPUParticleSystem : MonoBehaviour
{
    // Particle settings
    [Header("Particle Settings")]
    public int maxParticles = 1000;
    public float particleLifetime = 5f;
    public float emissionRate = 100f; // Particles per second
    
    [Header("Appearance")]
    public Gradient colorOverLifetime;
    public AnimationCurve sizeOverLifetime = AnimationCurve.Linear(0, 0.5f, 1, 0);
    public Vector2 sizeRange = new Vector2(0.1f, 0.5f);
    
    [Header("Movement")]
    public Vector3 initialVelocity = Vector3.up;
    public Vector3 velocityVariation = new Vector3(1f, 1f, 1f);
    public Vector3 gravity = new Vector3(0, -0.1f, 0);
    
    [Header("Emission Shape")]
    public EmissionShape shape = EmissionShape.Sphere;
    public float radius = 1f;
    
    [Header("Rendering")]
    public Material particleMaterial;
    public Mesh particleMesh;
    
    // Compute shader resources
    [Header("Compute Shader")]
    public ComputeShader computeShader;
    
    // Internal data
    private int simulateKernelId;
    private ComputeBuffer particleBuffer;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    private ParticleData[] particles;
    private float emissionAccumulator = 0f;
    private bool initialized = false;
    
    // LOD settings
    [Header("LOD Settings")]
    public float maxLODDistance = 50f;
    public float minLODParticleFactor = 0.1f; // At max distance, use this percentage of particles
    
    public enum EmissionShape
    {
        Point,
        Sphere,
        Cone,
        Box,
        Circle
    }
    
    // Particle data structure - must match the one in the compute shader
    struct ParticleData
    {
        public Vector3 position;
        public Vector3 velocity;
        public float lifetime;
        public float maxLifetime;
        public float size;
        public float maxSize;
        public Vector4 color;
        public uint state; // 0 = inactive, 1 = active
    }
    
    private void OnEnable()
    {
        InitializeParticleSystem();
    }
    
    private void OnDisable()
    {
        ReleaseBuffers();
    }
    
    private void InitializeParticleSystem()
    {
        if (initialized) return;
        
        // Check if compute shaders are supported
        if (!SystemInfo.supportsComputeShaders)
        {
            Debug.LogWarning("Compute shaders not supported on this device. Falling back to CPU particles.");
            gameObject.AddComponent<CPUFallbackParticleSystem>();
            enabled = false;
            return;
        }
        
        // Initialize particles array
        particles = new ParticleData[maxParticles];
        for (int i = 0; i < maxParticles; i++)
        {
            particles[i] = new ParticleData
            {
                position = Vector3.zero,
                velocity = Vector3.zero,
                lifetime = 0,
                maxLifetime = 0,
                size = 0,
                maxSize = 0,
                color = Vector4.zero,
                state = 0
            };
        }
        
        // Create compute buffers
        particleBuffer = new ComputeBuffer(maxParticles, System.Runtime.InteropServices.Marshal.SizeOf<ParticleData>());
        particleBuffer.SetData(particles);
        
        // Set up args buffer for instanced drawing
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        
        if (particleMesh != null)
        {
            args[0] = particleMesh.GetIndexCount(0);
            args[1] = (uint)maxParticles;
            args[2] = particleMesh.GetIndexStart(0);
            args[3] = particleMesh.GetBaseVertex(0);
        }
        argsBuffer.SetData(args);
        
        // Get kernel ID from compute shader
        simulateKernelId = computeShader.FindKernel("CSSimulate");
        
        // Assign buffer to compute shader
        computeShader.SetBuffer(simulateKernelId, "particles", particleBuffer);
        
        // Apply initial shader parameters
        UpdateShaderParameters();
        
        initialized = true;
    }
    
    private void UpdateShaderParameters()
    {
        if (computeShader != null)
        {
            computeShader.SetFloat("deltaTime", Time.deltaTime);
            computeShader.SetVector("gravity", gravity);
            computeShader.SetMatrix("localToWorld", transform.localToWorldMatrix);
        }
    }
    
    private void Update()
    {
        if (!initialized)
        {
            InitializeParticleSystem();
            return;
        }
        
        // Update LOD based on camera distance
        UpdateLOD();
        
        // Update shader parameters
        UpdateShaderParameters();
        
        // Emit new particles
        EmitParticles();
        
        // Dispatch compute shader to update particles
        int groups = Mathf.CeilToInt(maxParticles / 64.0f);
        computeShader.Dispatch(simulateKernelId, groups, 1, 1);
        
        // Render particles
        RenderParticles();
    }
    
    private void UpdateLOD()
    {
        // Get distance to camera
        float distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
        
        // Calculate LOD factor (1.0 at distance 0, minLODParticleFactor at maxLODDistance)
        float lodFactor = Mathf.Lerp(1.0f, minLODParticleFactor, Mathf.Clamp01(distanceToCamera / maxLODDistance));
        
        // Update emission rate based on LOD
        computeShader.SetFloat("lodFactor", lodFactor);
    }
    
    private void EmitParticles()
    {
        // Calculate how many particles to emit this frame
        emissionAccumulator += emissionRate * Time.deltaTime;
        int emitCount = Mathf.FloorToInt(emissionAccumulator);
        emissionAccumulator -= emitCount;
        
        if (emitCount <= 0) return;
        
        // Set emit count in shader
        computeShader.SetInt("emitCount", emitCount);
        
        // Set emission shape parameters
        computeShader.SetInt("emissionShape", (int)shape);
        computeShader.SetFloat("emissionRadius", radius);
        computeShader.SetVector("emissionVelocity", initialVelocity);
        computeShader.SetVector("velocityVariation", velocityVariation);
        computeShader.SetFloat("lifetime", particleLifetime);
        
        // Set appearance parameters
        // Note: In a real implementation, you would need to convert the gradient to a texture or color array
        computeShader.SetVector("startColor", colorOverLifetime.Evaluate(0));
        computeShader.SetVector("endColor", colorOverLifetime.Evaluate(1));
        
        float startSize = sizeOverLifetime.Evaluate(0) * Random.Range(sizeRange.x, sizeRange.y);
        float endSize = sizeOverLifetime.Evaluate(1) * Random.Range(sizeRange.x, sizeRange.y);
        computeShader.SetFloat("startSize", startSize);
        computeShader.SetFloat("endSize", endSize);
    }
    
    private void RenderParticles()
    {
        if (particleMaterial == null || particleMesh == null) return;
        
        // Set material properties
        particleMaterial.SetBuffer("particleBuffer", particleBuffer);
        
        // Draw instanced particles
        Graphics.DrawMeshInstancedIndirect(
            particleMesh,
            0,
            particleMaterial,
            new Bounds(transform.position, Vector3.one * 100),
            argsBuffer,
            0,
            null,
            UnityEngine.Rendering.ShadowCastingMode.Off,
            false
        );
    }
    
    private void ReleaseBuffers()
    {
        if (particleBuffer != null)
        {
            particleBuffer.Release();
            particleBuffer = null;
        }
        
        if (argsBuffer != null)
        {
            argsBuffer.Release();
            argsBuffer = null;
        }
        
        initialized = false;
    }
    
    private void OnDestroy()
    {
        ReleaseBuffers();
    }
}
```

### Compute Shader Implementation
The compute shader that drives the GPU particle simulation:

```csharp
// GPUParticlesCompute.compute
#pragma kernel CSSimulate

// Particle structure
struct Particle
{
    float3 position;
    float3 velocity;
    float lifetime;
    float maxLifetime;
    float size;
    float maxSize;
    float4 color;
    uint state; // 0 = inactive, 1 = active
};

// Buffers
RWStructuredBuffer<Particle> particles;

// Parameters
float deltaTime;
float3 gravity;
float4x4 localToWorld;
int emitCount;
int emissionShape;
float emissionRadius;
float3 emissionVelocity;
float3 velocityVariation;
float lifetime;
float4 startColor;
float4 endColor;
float startSize;
float endSize;
float lodFactor;

// Random number generation based on hash
float rand(float2 co)
{
    return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
}

// Random direction in a sphere
float3 randomDirection(float3 seed)
{
    float u = rand(seed.xy) * 2 - 1;
    float theta = rand(seed.yz) * 3.14159265 * 2;
    float r = sqrt(1 - u * u);
    return float3(r * cos(theta), r * sin(theta), u);
}

// Random point in a sphere
float3 randomPointInSphere(float3 center, float radius, float3 seed)
{
    float3 dir = randomDirection(seed);
    float distance = rand(seed.xz) * radius;
    return center + dir * distance;
}

[numthreads(64, 1, 1)]
void CSSimulate(uint3 id : SV_DispatchThreadID)
{
    uint index = id.x;
    if (index >= particles.Length)
        return;
        
    Particle particle = particles[index];
    
    // Update active particles
    if (particle.state == 1)
    {
        // Update lifetime
        particle.lifetime -= deltaTime;
        
        // Check if particle is still alive
        if (particle.lifetime <= 0)
        {
            particle.state = 0;
        }
        else
        {
            // Update velocity (apply gravity)
            particle.velocity += gravity * deltaTime;
            
            // Update position
            particle.position += particle.velocity * deltaTime;
            
            // Calculate life factor (0 to 1)
            float lifeFactor = 1.0 - (particle.lifetime / particle.maxLifetime);
            
            // Update color based on lifetime
            particle.color = lerp(startColor, endColor, lifeFactor);
            
            // Update size based on lifetime
            particle.size = lerp(startSize, endSize, lifeFactor);
        }
    }
    
    // Emit new particles
    if (particle.state == 0 && emitCount > 0)
    {
        // Use interlocked to decrement emitCount safely
        int originalValue;
        InterlockedAdd(emitCount, -1, originalValue);
        
        if (originalValue > 0)
        {
            // Calculate random seed
            float3 seed = float3(index, index * 0.5731, index * 0.1127) + deltaTime;
            
            // Initialize new particle
            particle.state = 1;
            particle.maxLifetime = lifetime * (0.8 + rand(seed.xy) * 0.4); // Add some variation
            particle.lifetime = particle.maxLifetime;
            
            // Apply LOD factor to lifetime
            particle.lifetime *= lodFactor;
            
            // Set size
            particle.maxSize = startSize * (0.8 + rand(seed.yz) * 0.4); // Add some variation
            particle.size = particle.maxSize;
            
            // Set color
            particle.color = startColor;
            
            // Set position based on emission shape
            float3 localPosition = float3(0, 0, 0);
            switch (emissionShape)
            {
                case 0: // Point
                    localPosition = float3(0, 0, 0);
                    break;
                    
                case 1: // Sphere
                    localPosition = randomPointInSphere(float3(0, 0, 0), emissionRadius, seed);
                    break;
                    
                case 2: // Cone
                    float angle = rand(seed.xy) * 3.14159265 * 2;
                    float distance = rand(seed.yz) * emissionRadius;
                    float height = rand(seed.zx) * emissionRadius;
                    localPosition = float3(cos(angle) * distance, height, sin(angle) * distance);
                    break;
                    
                case 3: // Box
                    localPosition = float3(
                        (rand(seed.xy) * 2 - 1) * emissionRadius,
                        (rand(seed.yz) * 2 - 1) * emissionRadius,
                        (rand(seed.zx) * 2 - 1) * emissionRadius
                    );
                    break;
                    
                case 4: // Circle
                    float circleAngle = rand(seed.xy) * 3.14159265 * 2;
                    float circleDistance = rand(seed.yz) * emissionRadius;
                    localPosition = float3(cos(circleAngle) * circleDistance, 0, sin(circleAngle) * circleDistance);
                    break;
            }
            
            // Transform local position to world space
            float4 worldPosition = mul(localToWorld, float4(localPosition, 1));
            particle.position = worldPosition.xyz;
            
            // Set velocity with variation
            particle.velocity = emissionVelocity + float3(
                (rand(seed.xy) * 2 - 1) * velocityVariation.x,
                (rand(seed.yz) * 2 - 1) * velocityVariation.y,
                (rand(seed.zx) * 2 - 1) * velocityVariation.z
            );
        }
    }
    
    // Write back to buffer
    particles[index] = particle;
}
```

### Particle Effect Trigger Implementation
Example of event-based particle effect triggering:

```csharp
using UnityEngine;
using System.Collections.Generic;

public class ParticleEffectTrigger : MonoBehaviour
{
    [System.Serializable]
    public class EffectMapping
    {
        public string triggerName;
        public GameObject particleEffectPrefab;
        public float duration = 3f;
        public bool followTarget = false;
    }
    
    [Header("Effect Settings")]
    public List<EffectMapping> effectMappings = new List<EffectMapping>();
    
    [Header("Trigger Settings")]
    [SerializeField] private float triggerRadius = 3f;
    [SerializeField] private bool triggerOnce = false;
    
    private Dictionary<string, GameObject> activeEffects = new Dictionary<string, GameObject>();
    private Dictionary<string, float> effectTimers = new Dictionary<string, float>();
    private bool hasTriggered = false;
    
    private void OnEnable()
    {
        SpatialBridge.eventService.onEventReceived += HandleEventReceived;
    }
    
    private void OnDisable()
    {
        SpatialBridge.eventService.onEventReceived -= HandleEventReceived;
    }
    
    private void HandleEventReceived(string eventName, object eventData)
    {
        // Check if we have an effect mapped to this event
        foreach (var mapping in effectMappings)
        {
            if (mapping.triggerName == eventName)
            {
                TriggerEffect(mapping, eventData);
                break;
            }
        }
    }
    
    private void Update()
    {
        // Update effect timers and remove expired effects
        List<string> keysToRemove = new List<string>();
        
        foreach (var kvp in effectTimers)
        {
            effectTimers[kvp.Key] -= Time.deltaTime;
            
            if (effectTimers[kvp.Key] <= 0)
            {
                keysToRemove.Add(kvp.Key);
            }
        }
        
        foreach (var key in keysToRemove)
        {
            if (activeEffects.ContainsKey(key) && activeEffects[key] != null)
            {
                Destroy(activeEffects[key]);
                activeEffects.Remove(key);
            }
            
            effectTimers.Remove(key);
        }
        
        // Check for player proximity trigger
        if (!hasTriggered || !triggerOnce)
        {
            CheckProximityTrigger();
        }
    }
    
    private void CheckProximityTrigger()
    {
        if (SpatialBridge.actorService.localActor == null || 
            SpatialBridge.actorService.localActor.avatar == null)
            return;
            
        float distance = Vector3.Distance(
            transform.position,
            SpatialBridge.actorService.localActor.avatar.position
        );
        
        if (distance <= triggerRadius)
        {
            // Find the effect mapping with triggerName "proximity" if it exists
            foreach (var mapping in effectMappings)
            {
                if (mapping.triggerName == "proximity")
                {
                    TriggerEffect(mapping, null);
                    hasTriggered = true;
                    break;
                }
            }
        }
    }
    
    private void TriggerEffect(EffectMapping mapping, object eventData)
    {
        string effectId = mapping.triggerName + "_" + System.Guid.NewGuid().ToString();
        
        // Remove previous instance if one exists with the same trigger name
        foreach (var key in new List<string>(activeEffects.Keys))
        {
            if (key.StartsWith(mapping.triggerName + "_"))
            {
                if (activeEffects[key] != null)
                {
                    Destroy(activeEffects[key]);
                }
                
                activeEffects.Remove(key);
                effectTimers.Remove(key);
                break;
            }
        }
        
        // Create new effect
        Vector3 position = transform.position;
        Transform target = null;
        
        // If the event data contains position information, use it
        if (eventData is Vector3 positionData)
        {
            position = positionData;
        }
        else if (eventData is GameObject gameObject)
        {
            position = gameObject.transform.position;
            target = gameObject.transform;
        }
        else if (eventData is SpatialTriggerEventArgs triggerArgs)
        {
            // This assumes SpatialTriggerEventArgs has a position or object reference
            position = triggerArgs.position;
            
            if (triggerArgs.actor != null && 
                SpatialBridge.actorService.actors.ContainsKey(triggerArgs.actor.actorNumber))
            {
                var actor = SpatialBridge.actorService.actors[triggerArgs.actor.actorNumber];
                if (actor.hasAvatar)
                {
                    target = actor.avatar.transform;
                }
            }
        }
        
        // Instantiate the effect
        GameObject effect = Instantiate(mapping.particleEffectPrefab, position, Quaternion.identity);
        
        // Store the effect
        activeEffects[effectId] = effect;
        effectTimers[effectId] = mapping.duration;
        
        // Set up target following if needed
        if (mapping.followTarget && target != null)
        {
            EffectFollowTarget follower = effect.AddComponent<EffectFollowTarget>();
            follower.target = target;
        }
    }
    
    // Helper component to make effects follow a target
    private class EffectFollowTarget : MonoBehaviour
    {
        public Transform target;
        
        private void Update()
        {
            if (target != null)
            {
                transform.position = target.position;
            }
        }
    }
}
```

## Best Practices
- **Performance Monitoring**: Always test particle effects on lower-end devices to ensure good performance
- **Use LOD**: Implement distance-based particle count reduction for far-away effects
- **Optimize Particle Count**: Start with fewer particles and increase as needed
- **Buffer Management**: Always release compute buffers when they're no longer needed
- **Fallback Support**: Provide CPU-based fallbacks for devices without compute shader support
- **Memory Usage**: Be mindful of texture and buffer sizes, especially for mobile devices
- **Batching**: Group similar particle systems for more efficient rendering
- **Effect Lifetime**: Limit the duration of effects and ensure proper cleanup
- **Particle Recycling**: Reuse particle systems to reduce instantiation overhead
- **Emission Rate Control**: Dynamically adjust emission rates based on performance

## Related Templates
- [Top Down Shooter](../Games/TopDownShooter.md) - For implementing particle-based weapon effects
- [Obby (Obstacle Course) Game](../Games/ObbyGame.md) - For particle-based obstacle feedback
- [Simple Car Controller](../Vehicles/SimpleCarController.md) - For vehicle particle effects

## Additional Resources
- [Spatial Performance Guidelines](https://docs.spatial.io/performance-guidelines)
- [Unity Compute Shader Documentation](https://docs.unity3d.com/Manual/class-ComputeShader.html)
- [GitHub Repository](https://github.com/spatialsys/spatial-gpu-particles-template)
- [WebGL Particle Optimization](https://docs.unity3d.com/Manual/webgl-performance.html)
