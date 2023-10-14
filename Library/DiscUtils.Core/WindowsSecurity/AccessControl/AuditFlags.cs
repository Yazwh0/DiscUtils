using System;

namespace BitMagic.DiscUtils.Core.WindowsSecurity.AccessControl;

[Flags]
public enum AuditFlags
{
    None = 0,
    Success = 1,
    Failure = 2,
}