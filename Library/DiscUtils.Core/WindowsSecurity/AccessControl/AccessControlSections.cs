using System;

namespace BitMagic.DiscUtils.Core.WindowsSecurity.AccessControl;

[Flags]
public enum AccessControlSections
{
    None,
    Audit = 1,
    Access = 2,
    Owner = 4,
    Group = 8,
    All = Audit | Access | Owner | Group
}