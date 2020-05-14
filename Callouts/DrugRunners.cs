using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;

namespace Aliens.Callouts
{
    [CalloutInfo("DrugRunners", CalloutProbability.High)]
    public class DrugRunners : Callout
    {
        private Ped Suspect;
        private Vehicle SusVehicle;
        private Vector3 SpawnPoint;
        private Blip SuspectBlip;
       // private Weapon weapon = new Weapon(WeaponAsset weapon_microsmg,)
        private bool isSuspect = false;

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
            Suspect.Inventory.GiveNewWeapon("WEAPON_PISTOL", 10000, true);

            SuspectBlip = Suspect.AttachBlip();
            SuspectBlip.IsFriendly = false;
            Suspect.RelationshipGroup = "HATES_PLAYER";

            //setup relationship for combat situations

            Game.LocalPlayer.Character.RelationshipGroup = "COP";
            Game.SetRelationshipBetweenRelationshipGroups("HATES_PLAYER", "COP", Relationship.Hate);
            Game.SetRelationshipBetweenRelationshipGroups("COP", "HATES_PLAYER", Relationship.Hate);

            // Suspect.Tasks.CruiseWithVehicle(SusVehicle, 20f, VehicleDrivingFlags.FollowTraffic);
            Suspect.Tasks.DriveToPosition(Game.LocalPlayer.Character.Position, 10f, VehicleDrivingFlags.FollowTraffic);

            Events.OnPulloverStarted += PulloverStartedHandler;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

          /*  if (isSuspect)
            {
                StartShooting();
                
            } */

            if(!Suspect.IsAlive && isSuspect || Suspect.IsCuffed && isSuspect)
            {
                End();
            }
        }

        private void PulloverStartedHandler(LHandle Pullover)
        {
           if(Suspect == Functions.GetPulloverSuspect(Pullover))
            {
                isSuspect = true;
                StartScenario();
            }
        }

        private void StartScenario()
        {
            GameFiber.StartNew(delegate
            {

                GameFiber.Sleep(5000);

                Suspect.Tasks.LeaveVehicle(SusVehicle, LeaveVehicleFlags.LeaveDoorOpen);

                GameFiber.Sleep(5000); //crude async emulation
                Suspect.Tasks.FireWeaponAt(Game.LocalPlayer.Character.Position, 10000, FiringPattern.SingleShot);



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
      


