using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StructPolymorphismGenerator.Sample;


[AttributeUsage(AttributeTargets.Interface)]
public class AbstractStructGeneratorAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Struct)]
public class AbstractStructAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Struct)]
public class SubStructAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class HeaderAttribute : Attribute
{
    public string EnumName { get; }

    public HeaderAttribute(string enumName)
    {
        EnumName = enumName;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class HeaderElementAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
public class StructVirtualAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class SwitchableAttribute : Attribute
{
}
public struct MissionHeader
{
    [Switchable] public GeneratedEnum Type;
    public int MissionId;
}

[AbstractStructGenerator]
public interface IMissionable
{
    [Header("MissionType")] MissionHeader Header { get; }
    [HeaderElement] GeneratedEnum Type { get; }

    [StructVirtual]
    bool IsMissionComplete();
}
[AbstractStruct]
public partial struct Mission : IMissionable
{
}

//#region UserCode

[SubStruct]
public partial struct DefenseMission : IMissionable
{
    public MissionHeader Header { get; }
    public GeneratedEnum Type { get; }

    public bool IsMissionComplete()
    {
        //時間経過とか
        return true;
    }
}

[SubStruct]
public partial struct AnnihilationMission : IMissionable
{
    public MissionHeader Header { get; }
    public GeneratedEnum Type { get; }

    public bool IsMissionComplete()
    {
        //残ってる敵の数とか
        return true;
    }
}


public class StructPolymorphismSExample
{
}