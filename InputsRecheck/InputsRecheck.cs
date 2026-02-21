using Modding;
using System.Collections.Generic;
using Vasi;
using HC = HeroController;
using IH = InputHandler;
using RH = Modding.ReflectionHelper;

namespace InputsRecheck
{
    public class InputsRecheck : Mod, IMenuMod, IGlobalSettings<Settings>
    {
        public static InputsRecheck Instance;
        public override string GetVersion() => "1.0.0";

        public static Settings GS = new();
        public void OnLoadGlobal(Settings gs) => GS = gs;
        public Settings OnSaveGlobal() => GS;

        public bool ToggleButtonInsideMenu => false;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry) => MenuMod.Menu;

        // Snapshot of what was possible last frame
        private bool _couldJump;
        private bool _couldWallJump;
        private bool _couldDoubleJump;
        private bool _couldDash;
        private bool _couldAttack;
        private bool _couldCast;
        private bool _couldFocus;
        private bool _couldDreamNail;
        private bool _couldSuperDash;

        private static void TakeSnapshot(HC self,
            out bool jump, out bool wallJump, out bool doubleJump,
            out bool dash, out bool attack, out bool cast, out bool focus,
            out bool dreamNail, out bool superDash)
        {
            jump = RH.CallMethod<HC, bool>(self, "CanJump");
            wallJump = RH.CallMethod<HC, bool>(self, "CanWallJump");
            doubleJump = RH.CallMethod<HC, bool>(self, "CanDoubleJump");
            dash = RH.CallMethod<HC, bool>(self, "CanDash");
            attack = RH.CallMethod<HC, bool>(self, "CanAttack");
            cast = self.CanCast();
            focus = self.CanFocus();
            dreamNail = self.CanDreamNail();
            superDash = self.CanSuperDash();
        }

        public override void Initialize()
        {
            Instance = this;
            On.HeroController.FixedUpdate += HeroController_FixedUpdate;
        }

        private void UpdateSnapshot(HC self)
        {
            TakeSnapshot(self, out _couldJump, out _couldWallJump, out _couldDoubleJump,
                    out _couldDash, out _couldAttack, out _couldCast, out _couldFocus,
                    out _couldDreamNail, out _couldSuperDash);
        }

        private void HeroController_FixedUpdate(On.HeroController.orig_FixedUpdate orig, HC self)
        {
            orig(self);

            if (GameManager.instance.isPaused
                || !GameManager.instance.IsGameplayScene()
                || !self.acceptingInput
                || GameManager.instance.inventoryFSM.ActiveStateName != "Closed")
            {
                // Reset snapshots so we don't fire on re-entry
                UpdateSnapshot(self);
                return;
            }

            TakeSnapshot(self, out var canJump, out var canWallJump, out var canDoubleJump,
                    out var canDash, out var canAttack, out var canCast, out var canFocus,
                    out var canDreamNail, out var canSuperDash);

            var actions = IH.Instance.inputActions;

            // --- JUMP ---
            if (GS.ReCheckJump && actions.jump.IsPressed)
            {
                if (!_couldWallJump && canWallJump)
                {
                    RH.CallMethod(self, "DoWallJump");
                    RH.SetField(self, "doubleJumpQueuing", false);
                }
                else if (!_couldJump && canJump)
                {
                    RH.CallMethod(self, "HeroJump");
                }
                else if (!_couldDoubleJump && canDoubleJump)
                {
                    RH.CallMethod(self, "DoDoubleJump");
                }
            }

            // --- DASH ---
            if (GS.ReCheckDash && actions.dash.IsPressed)
            {
                if (!_couldDash && canDash)
                    RH.CallMethod(self, "HeroDash");
            }

            // --- ATTACK ---
            if (GS.ReCheckAttack && actions.attack.IsPressed && IsFacing(self, actions))
            {
                if (!_couldAttack && canAttack)
                    RH.CallMethod(self, "DoAttack");
            }

            // --- CAST (hold = focus, tap handled by quick cast below) ---
            if (GS.ReCheckCast && actions.cast.IsPressed && IsFacing(self, actions))
            {
                if (!_couldFocus && canFocus
                    && self.spellControl.ActiveStateName != "Button Down")
                {
                    self.spellControl.SendEvent("BUTTON DOWN");
                }
            }

            // --- QUICK CAST ---
            if (GS.ReCheckQuickCast && actions.quickCast.IsPressed
                && HasEnoughMP()
                && self.spellControl.ActiveStateName != "Fireball Antic"
                && self.spellControl.ActiveStateName != "Quake Antic"
                && self.spellControl.ActiveStateName != "Scream Antic1"
                && self.spellControl.ActiveStateName != "Scream Antic2")
            {
                if (!_couldCast && canCast)
                    self.spellControl.SendEvent("QUICK CAST");
            }

            // --- DREAM NAIL ---
            if (GS.ReCheckDreamNail && actions.dreamNail.IsPressed
                && IsFacing(self, actions)
                && self.gameObject.LocateMyFSM("Dream Nail").ActiveStateName != "Start")
            {
                if (!_couldDreamNail && canDreamNail)
                    self.gameObject.LocateMyFSM("Dream Nail").SendEvent("BUTTON DOWN");
            }

            // --- SUPERDASH ---
            if (GS.ReCheckSuperdash && actions.superDash.IsPressed
                && self.superDash.ActiveStateName != "Ground Charge"
                && self.superDash.ActiveStateName != "Wall Charge")
            {
                // On the ground, also enforce facing direction
                bool directionOk = self.hero_state == GlobalEnums.ActorStates.airborne
                    || IsFacing(self, actions);
                if (!_couldSuperDash && canSuperDash && directionOk)
                    self.superDash.SendEvent("BUTTON DOWN");
            }

            // --- SUPERDASH AUTO-RELEASE ---
            if (GS.SuperdashRelease)
            {
                bool groundCharged = self.superDash.ActiveStateName == "Ground Charged"
                    && ((actions.left.IsPressed && !self.cState.facingRight)
                        || (actions.right.IsPressed && self.cState.facingRight));
                bool wallCharged = self.superDash.ActiveStateName == "Wall Charged"
                    && ((actions.left.IsPressed && self.cState.facingRight)
                        || (actions.right.IsPressed && !self.cState.facingRight));

                if (groundCharged || wallCharged)
                    self.superDash.SendEvent("BUTTON UP");
            }

            UpdateSnapshot(self);
        }

        // Returns true if the player is not pressing against the knight's back
        // (avoids firing attacks/spells in the wrong direction after a turn)
        private static bool IsFacing(HC self, HeroActions actions)
        {
            bool pushingBack = (actions.left.IsPressed && self.cState.facingRight)
                            || (actions.right.IsPressed && !self.cState.facingRight);
            return !pushingBack;
        }

        private static bool HasEnoughMP()
        {
            return PlayerData.instance.GetInt("MPCharge")
                >= HC.instance.spellControl.GetOrCreateInt("MP Cost").Value;
        }
    }
}