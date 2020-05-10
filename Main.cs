using Rage;
using LSPD_First_Response;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Aliens.Callouts;


namespace Aliens
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            Functions.OnOnDutyStateChanged += OnDutyStateHandler;
            Game.LogTrivial("Aliens intialized");
        }

        public override void Finally()
        {
            Game.LogTrivial("Aliens vanished");
        }

        private static void OnDutyStateHandler(bool OnDuty)
        {
            if (OnDuty)
            {
                RegisterCallouts();
            }
        }

        private static void RegisterCallouts()
        {
            Functions.RegisterCallout(typeof(MexicansOnBorder));
        }
    }
}