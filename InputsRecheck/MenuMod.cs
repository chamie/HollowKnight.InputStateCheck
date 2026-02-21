using Modding;
using System.Collections.Generic;
using IB = InputsRecheck.InputsRecheck;

namespace InputsRecheck
{
    internal static class MenuMod
    {
        internal static List<IMenuMod.MenuEntry> Menu = new()
        {
            new()
            {
                Name = "Re-check Jumps",
                Description = "",
                Values = new string[] { "Off", "On" },
                Saver = opt =>
                {
                    IB.GS.ReCheckJump = !IB.GS.ReCheckJump;
                },
                Loader = () => IB.GS.ReCheckJump ? 1 : 0
            },

            new()
            {
                Name = "Re-check Dash",
                Description = "",
                Values = new string[] { "Off", "On" },
                Saver = opt =>
                {
                    IB.GS.ReCheckDash = !IB.GS.ReCheckDash;
                },
                Loader = () => IB.GS.ReCheckDash ? 1 : 0
            },

            new()
            {
                Name = "Re-check Attacks",
                Description = "",
                Values = new string[] { "Off", "On" },
                Saver = opt =>
                {
                    IB.GS.ReCheckAttack = !IB.GS.ReCheckAttack;
                },
                Loader = () => IB.GS.ReCheckAttack ? 1 : 0
            },

            new()
            {
                Name = "Re-check Casts",
                Description = "",
                Values = new string[] { "Off", "On" },
                Saver = opt =>
                {
                    IB.GS.ReCheckCast = !IB.GS.ReCheckCast;
                },
                Loader = () => IB.GS.ReCheckCast ? 1 : 0
            },

            new()
            {
                Name = "Re-check Quick Casts",
                Description = "",
                Values = new string[] { "Off", "On" },
                Saver = opt =>
                {
                    IB.GS.ReCheckQuickCast = !IB.GS.ReCheckQuickCast;
                },
                Loader = () => IB.GS.ReCheckQuickCast ? 1 : 0
            },

            new()
            {
                Name = "Re-check Dream Nail",
                Description = "",
                Values = new string[] { "Off", "On" },
                Saver = opt =>
                {
                    IB.GS.ReCheckDreamNail = !IB.GS.ReCheckDreamNail;
                },
                Loader = () => IB.GS.ReCheckDreamNail ? 1 : 0
            },

            new()
            {
                Name = "Re-check Superdash",
                Description = "",
                Values = new string[] { "Off", "On" },
                Saver = opt =>
                {
                    IB.GS.ReCheckSuperdash = !IB.GS.ReCheckSuperdash;
                },
                Loader = () => IB.GS.ReCheckSuperdash ? 1 : 0
            },

            new()
            {
                Name = "Superdash Release",
                Description = "Hold the direction you are facing to automatically release superdash",
                Values = new string[] { "Off", "On" },
                Saver = opt =>
                {
                    IB.GS.SuperdashRelease = !IB.GS.SuperdashRelease;
                },
                Loader = () => IB.GS.SuperdashRelease ? 1 : 0
            },
        };
    }
}
