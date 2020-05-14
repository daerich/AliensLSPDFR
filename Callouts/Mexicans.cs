using Rage;
using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
//using LSPD_First_Response.Engine.Scripting.Entities;
using System;

namespace Aliens.Callouts
{
    [CalloutInfo("MexicansOnBorder", CalloutProbability.High)]
    public class MexicansOnBorder : Callout
    {
        private Ped Suspect;
        private Vector3 SpawnPoint;
        //private Vector3 ZoneSpawn;
        private Blip SuspectBlip;
        private LHandle Pursuit;
        private bool PursuitCreated = false;
        private static Vector3 ZoneSpawn = new Vector3(1821.44653f, 3289.97534f, 43.2852364f);
      
        public override bool OnBeforeCalloutDisplayed()
        {
            //ZoneSpawn = new Vector3(1821.44653f, 3289.97534f, 43.2852364f); I tried so hard
       

           
                SpawnPoint = World.GetNextPositionOnStreet(ZoneSpawn.Around(1000f));

                ShowCalloutAreaBlipBeforeAccepting(SpawnPoint, 30f);
                AddMaximumDistanceCheck(300f, Game.LocalPlayer.Character.Position);

                CalloutMessage = "Mexicans entering Country";
                CalloutPosition = SpawnPoint;

                return base.OnBeforeCalloutDisplayed();
            

            
        }

        public override bool OnCalloutAccepted()
        {
            Random rnd = new Random();
            int rsl = rnd.Next(0, 5);
            string[] pedlist = { "s_m_m_lathandy_01", "a_m_y_business_01", "ig_claypain", "s_f_m_fembarber", "a_f_y_genhot_01", "a_f_y_indian_01" };
            Suspect = new Ped(pedlist[rsl], SpawnPoint, 30f);
            //Debug
            Game.LogTrivial($"Sucessfully selected {pedlist[rsl]}!");
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
                StartElude(); 
            }

            if (PursuitCreated && !Functions.IsPursuitStillRunning(Pursuit))
            {
                End();
            }
        }

        private void StartElude()
        {
            GameFiber.StartNew(delegate
            {
                Game.DisplaySubtitle("Ayayay!");
                GameFiber.Sleep(3000);


                Pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(Pursuit, Suspect);
                Functions.SetPursuitIsActiveForPlayer(Pursuit, true);
                PursuitCreated = true;
            });
           
        }
        public override void End()
        {
            base.End();
            if (SuspectBlip.Exists()) { SuspectBlip.Delete();
            if (Suspect.Exists()) { Suspect.Dismiss(); }
            }

        }
    } }

