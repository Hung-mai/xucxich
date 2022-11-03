

#include "WaterLighting.hlsl"
#include "WaterColor.hlsl"
#include "WaterFoam.hlsl"
#include "WaterWaves.hlsl"
#include "WaterCaustics.hlsl"

void ViewDirectionParallax_half(half3 tangentViewDir, out half3 Out)
{
    tangentViewDir = normalize(tangentViewDir);
    tangentViewDir.xy /= (tangentViewDir.z + 0.42);
    Out = tangentViewDir;
}