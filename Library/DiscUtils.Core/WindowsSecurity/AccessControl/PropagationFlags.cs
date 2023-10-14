using System;

namespace BitMagic.DiscUtils.Core.WindowsSecurity.AccessControl;

[Flags]
public enum PropagationFlags
{
    None = 0,
    NoPropagateInherit = 1,
    InheritOnly = 2,
}