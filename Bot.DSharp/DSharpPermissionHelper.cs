
using Bot.Api;
using DSharpPlus.Entities;
using System;

namespace Bot.DSharp
{
    public static class DSharpPermissionHelper
    {
        public static IBaseChannel.Permissions BasePermissionsFromDSharpPermissions(DiscordPermissions dSharpPermissions)
        {
            ulong value = 0;

            foreach (DiscordPermission permission in Enum.GetValues<DiscordPermission>())
            {
                if (dSharpPermissions.HasFlag(permission))
                {
                    value |= 1UL << permission.AsUnderlyingType();
                }
            }

            return (IBaseChannel.Permissions)value;
        }

        public static DiscordPermissions DSharpPermissionsFromBasePermissions(IBaseChannel.Permissions basePermissions)
        {
            ulong value = Convert.ToUInt64(basePermissions);
            DiscordPermissions permissions = DiscordPermissions.None;

            foreach (DiscordPermission permission in Enum.GetValues<DiscordPermission>())
            {
                ulong flag = 1UL << permission.AsUnderlyingType();

                if ((value & flag) != 0)
                {
                    permissions.Add(permission);
                }
            }

            return permissions;
        }
    }
}
