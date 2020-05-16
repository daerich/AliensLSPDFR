using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using System;

namespace Aliens.Callouts
{
    [CalloutInfo("DrugRunners", CalloutProbability.High)]
    public class DrugRunners : Callout
    {
        private Ped Suspect;
        private Vehicle SusVehicle;
        private Vector3 SpawnPoint;
        private Blip SuspectBlip;

        public override bool OnBeforeCalloutDisplayed()
        {
            SpawnPoint = World.GetNextPositionOnStreet(Game.LocalPlayer.Character.Position.Around(300f));
            ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
            AddMinimumDistanceCheck(50f, SpawnPoint);

            CalloutMessage = "DrugRunners";
            CalloutPosition = SpawnPoint;

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            SusVehicle = new Vehicle("STANIER", SpawnPoint);
            SusVehicle.IsPersistent = true;
            Suspect = SusVehicle.CreateRandomDriver();

            Suspect.IsPersistent = true;
            Suspect.BlockPermanentEvents = true;
            Suspect.Inventory.GiveNewWeapon("WEAPON_PISTOL", -1, true);

            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.IsFriendly = false;
            

            // Suspect.Tasks.CruiseWithVehicle(SusVehicle, 20f, VehicleDrivingFlags.FollowTraffic);
            Suspect.Tasks.DriveToPosition(Game.LocalPlayer.Character.Position, 10f, VehicleDrivingFlags.Normal);

            Events.OnPulloverStarted += PulloverStartedHandler;

            Events.OnPulloverEnded += PulloverEndedEventHandler;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

          if (!Suspect.IsAlive || Suspect.IsCuffed)
            {
                End();
            } 


        }


        private int RandomDecision()
        {
            Random rnd = new Random();

            return rnd.Next(1000);

        }

        private void PulloverStartedHandler(LHandle Pullover)
        {
            if (Suspect == Functions.GetPulloverSuspect(Pullover) && RandomDecision() <= 500) //debug probablity
            {
                Game.LogTrivial("Unpleasant ending");
                StartScenario();
            }


        }

       private void PulloverEndedEventHandler(LHandle Pullover, bool normalEnding)
        {
            if (normalEnding)
            {
                End();

            }           
        } 

        private void StartScenario()
        {
            GameFiber.StartNew(delegate
            {

                GameFiber.Sleep(3000);

                Suspect.Tasks.LeaveVehicle(SusVehicle, LeaveVehicleFlags.LeaveDoorOpen);

                GameFiber.Sleep(5000); //crude async emulation
                //Suspect.Tasks.FireWeaponAt(Game.LocalPlayer.Character.Position, -1 , FiringPattern.SingleShot);
                Suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);



            });
        }

       public override void End()
        {
            base.End();

            if (SusVehicle.Exists()) { SusVehicle.Delete(); }
            if (Suspect.Exists()) { Suspect.Dismiss(); }
            if (SuspectBlip.Exists()) { SuspectBlip.Delete(); }
        }
    }

}
      


