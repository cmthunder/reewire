using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

// Thanks, json2csharp.com!

// This file is horrible, but the speedup...

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
[Serializable]
public class AlphaOverValue
{
    public int interpolationType;
    public List<ControlPoint> controlPoints;
}
[Serializable]
public class BladeMappings
{
    public ColorOverValue colorOverValue;
    public AlphaOverValue alphaOverValue;
    public ScaleOverValue scaleOverValue;
    public float valueFrom;
    public float valueTo;
}
[Serializable]
public class ColorOverride
{
    public int type;
    public float hue;
    public float saturation;
    public float value;
    public float hueShiftPerSecond;
    public float fakeGlowMultiplier;
}
[Serializable]
public class ColorOverValue
{
    public int interpolationType;
    public List<ColorControlPoint> controlPoints;
}
[Serializable]
public class Config
{
    public SaberSettings SaberSettings;
    public bool Enabled;
    public string Name;
    public LocalTransform LocalTransform;
    public bool ForceColorOverride;
    public ColorOverride ColorOverride;
}
[Serializable]
public class ControlPoint
{
    public float time;
    public float value;
}
[Serializable]
public class ColorControlPoint
{
    public float time;
    public HandleColor value;
}
[Serializable]
public class HandleColor
{
    public float r;
    public float g;
    public float b;
    public float a;
}
[Serializable]
public class HandleMask
{
    public int interpolationType;
    public List<ControlPoint> controlPoints;
}
[Serializable]
public class LocalTransform
{
    public Position Position;
    public Rotation Rotation;
    public Scale Scale;
}
[Serializable]
public class MaskSettings
{
    public int bladeMaskResolution;
    public int driversMaskResolution;
    public HandleMask handleMask;
    public BladeMappings bladeMappings;
    public int driversSampleMode;
    public ViewingAngleMappings viewingAngleMappings;
    public SurfaceAngleMappings surfaceAngleMappings;
    public List<object> drivers;
}
[Serializable]
public class Module
{
    public string ModuleId;
    public int Version;
    public Config Config;
}
[Serializable]
public class Position
{
    public float x;
    public float y;
    public float z;
}
[Serializable]
public class Root
{
    public string ModVersion;
    public int Version;
    public LocalTransform LocalTransform;
    public List<Module> Modules;
}
[Serializable]
public class Rotation
{
    public float x;
    public float y;
    public float z;
}
[Serializable]
public class SaberProfile
{
    public int interpolationType;
    public List<ControlPoint> controlPoints;
}
[Serializable]
public class SaberSettings
{
    public float zOffsetFrom;
    public float zOffsetTo;
    public float thickness;
    public SaberProfile saberProfile;
    public bool startCap;
    public bool endCap;
    public float verticalResolution;
    public float horizontalResolution;
    public int renderQueue;
    public int cullMode;
    public bool depthWrite;
    public int blurFrames;
    public float glowMultiplier;
    public float handleRoughness;
    public HandleColor handleColor;
    public MaskSettings maskSettings;
}
[Serializable]
public class Scale
{
    public float x;
    public float y;
    public float z;
}
[Serializable]
public class ScaleOverValue
{
    public int interpolationType;
    public List<ControlPoint> controlPoints;
}
[Serializable]
public class SurfaceAngleMappings
{
    public ColorOverValue colorOverValue;
    public AlphaOverValue alphaOverValue;
    public ScaleOverValue scaleOverValue;
    public float valueFrom;
    public float valueTo;
}
[Serializable]
public class Value
{
    public int r;
    public int g;
    public int b;
    public int a;
}
[Serializable]
public class ViewingAngleMappings
{
    public ColorOverValue colorOverValue;
    public AlphaOverValue alphaOverValue;
    public ScaleOverValue scaleOverValue;
    public float valueFrom;
    public float valueTo;
}