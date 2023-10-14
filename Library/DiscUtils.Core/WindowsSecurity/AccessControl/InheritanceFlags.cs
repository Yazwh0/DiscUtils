using System;

namespace BitMagic.DiscUtils.Core.WindowsSecurity.AccessControl;

[Flags]
public enum InheritanceFlags
{
    None = 0,
    ContainerInherit = 1,
    ObjectInherit = 2,
}