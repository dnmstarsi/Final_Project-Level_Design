#ifndef LIGHTING_CEL_SHADED_INCLUDED
#define LIGHTING_CEL_SHADED_INCLUDED

void LIGHTINGCELSHADED_float(out float3 Color)
{
    #if defined(SHADERGRAPH_PREVIEW)
    ...
    #else
    Color = float3(0.5f, 0.5f, 0.5f);
    #endif
}

#endif