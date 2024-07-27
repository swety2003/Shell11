/// <class>ColorKeyAlphaEffect</class>

/// <description>An effect that makes pixels of a particular color transparent.</description>

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------

/// <summary>The color that to be replaced.</summary>
/// <defaultValue>White</defaultValue>
float4 ColorSource : register(C0);

/// <summary>The tolerance in color differences.</summary>
/// <minValue>0</minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>0.3</defaultValue>
float Tolerance : register(C1);

/// <summary>The color that replace.</summary>
/// <defaultValue>Green</defaultValue>
float4 ColorTarget : register(C2);

//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including Texture1)
//--------------------------------------------------------------------------------------

sampler2D Texture1Sampler : register(S0);

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 main(float2 uv : TEXCOORD) : COLOR
{
   float4 color = tex2D( Texture1Sampler, uv );
   
   if (all(abs(color.rgb - ColorSource.rgb) < Tolerance)) {
      //color.rgba = 0;
      color.rgb = ColorTarget.rgb;
   }
   
   return color;
}
