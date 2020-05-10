using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using LSPD_First_Response.Engine.Scripting.Entities;
using System;

namespace Aliens.Callouts
{
    [CalloutInfo("MexicansOnBorder", CalloutProbability.High)]
    public class MexicansOnBorder : Callout
    {
        private Ped Suspect;
        private Vector3 SpawnPoint;
        private Blip SuspectBlip;
        private LHandle Pursuit;
        private bool PursuitCreated = false;


        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(1000f));

            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
            AddMinimumDistanceCheck(40f, SpawnPoint);

            CalloutMessage = "Mexicans entering Country";
            CalloutPosition = SpawnPoint;


            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {

            Suspect = new Ped("s_m_m_lathandy_01", SpawnPoint, 30f);
            Suspect.Tasks.Wander();
            SuspectBlip = Suspect.AttachBlip();
            Suspect.IsPersistent = true;


            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();
            if (!PursuitCreated && Game.LocalPlayer.Character.DistanceTo(Suspect.Position) <= 10f)
            {
                Pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(Pursuit, Suspect);
                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                PursuitCreated = true;
            }

            if (Suspect.IsCuffed && !Functions.IsPursuitStillRunning(Pursuit))
            {
                End();
            }
        }

        public override void End()
        {
            base.End();
            if (SuspectBlip.Exists()) { SuspectBlip.Delete();
            if (Suspect.Exists()) { Suspect.Dismiss(); }
            }

        }
    } }