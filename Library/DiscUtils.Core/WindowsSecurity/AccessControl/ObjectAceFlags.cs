using System;

namespace BitMagic.DiscUtils.Core.WindowsSecurity.AccessControl;

[Flags]
public enum ObjectAceFlags
{
    None = 0,
    ObjectAceTypePresent = 1,
    InheritedObjectAceTypePresent = 2,
}