using HarmonyLib;
using UnityEngine;
using System;

namespace AnxietyMod
{
    internal class Patches
    {
        [HarmonyPatch(typeof(Anxiety), "Update")]
        internal class Anxiety_Update
        {
            private static bool affliction;
            //
            private static bool IsDay()//   Day is 7:00 to 19:00   //
            {
                return GameManager.GetTimeOfDayComponent().IsDay();
            }
            private static bool IsNight()//   Dusk is 19:00 to 20:00, Night is 20:00 to 6:00, Dawn is 6:00 to 7:00   //
            {
                if (GameManager.GetTimeOfDayComponent().IsNight())
                {
                    return true;
                }
                else if (GameManager.GetTimeOfDayComponent().IsDusk())
                {
                    return true;
                }
                else if (GameManager.GetTimeOfDayComponent().IsDawn())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            private static bool PlayerIsSheltered()//   Not all shelters are safe from hostile wildlife   //
            {
                if (GameManager.GetWeatherComponent().IsIndoorScene())
                {
                    return true;
                }
                else if (GameManager.GetWeatherComponent().IsIndoorEnvironment())
                {
                    return true;
                }
                else if (GameManager.GetPlayerInVehicle().IsInside())
                {
                    return true;
                }
                else if (GameManager.GetSnowShelterManager().PlayerInShelter())
                {
                    return true;
                }
                //else if (GameManager.GetLeanToManager().PlayerInLeanTo())
                //{
                    //return true;
                //}
                else
                {
                    return false;
                }
            }
            private static bool FireIsBurning()
            {
                Fire fireState = GameManager.GetFireManagerComponent().GetClosestFire(GameManager.GetPlayerTransform().position);
                if (fireState)
                {
                    if (fireState.GetFireState() == FireState.FullBurn)
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            private static float DistanceToClosestFire()
            {
                return GameManager.GetFireManagerComponent().GetDistanceToClosestFire(GameManager.GetPlayerTransform().position);
            }
            //private static void Play_AfflictionAnxietyTreated()
            //{
                //GameManager.GetPlayerVoiceComponent().Play("Play_VOCatchBreath", Voice.Priority.Normal);
            //}
            //
            private static void Prefix(Anxiety __instance)
            {
                void AnxietyTreatedPopup()
                {
                    PlayerDamageEvent.SpawnAfflictionEvent(__instance.m_AnxietyLocalizedString.m_LocalizationID, "Treated", __instance.m_AnxietyIcon, InterfaceManager.m_FirstAidBuffColor);
                }
                //
                if (IsNight())
                {
                    bool fireStatus = FireIsBurning();
                    bool distanceToFire = DistanceToClosestFire() < 4f;
                    bool nearBurningFire = fireStatus && distanceToFire;
                    bool sheltered = PlayerIsSheltered();
                    //
                    if (!affliction && !nearBurningFire && !sheltered)//   None of the conditions are met; anxiety added   //
                    {
                        __instance.StartAffliction();
                        //AnxietyVoiceStop() or AnxietyVoiceSwap();
                        //AnxietyPopupRemoval();
                        //CustomAnxietyPopup();
                        affliction = true;
                    }
                    else if (affliction && nearBurningFire || sheltered)//   Near a burning fire/embers, or sheltered; anxiety removed   //
                    {
                        __instance.StopAffliction(true);
                        //Play_AfflictionAnxietyTreated();
                        affliction = false;
                    }
                }
                else if (IsDay() && affliction)//   Day time; anxiety removed   //
                {
                    __instance.StopAffliction(true);
                    AnxietyTreatedPopup();
                    affliction = false;
                }
            }
        }
    }
}
// ///                      To Do                      ///
//
// Fix so embers do not treat affliction
// Remove voice/sound cue from affliction (character wont stop talking; "It's like something out of a *cough* *low budget script* horror movie!")
// Remove popup when affliction added (should be made removable for low/no HUD players)
// Change health menu localizations to not reference the darkwalker
//
//
// ///               Notes for Atlas-Lumi              ///
//
// private static void Play_AfflictionAnxiety() is redundant. The game already triggers this when the affliction is added
// private static void Play_AfflictionAnxietyTreated() I lost the functionality of this I dont know why
// I removed the torch from my version because I don't usually wait or sleep while holding a lit torch, as that would consume the torch
// 
//
// ////               v     UNUSED     v              ////
//
//public static bool HoldingLitTorch()
//{
    //return (GameManager.GetPlayerManagerComponent().m_ItemInHands && GameManager.GetPlayerManagerComponent().m_ItemInHands.m_TorchItem && GameManager.GetPlayerManagerComponent().m_ItemInHands.m_TorchItem.IsBurning());
//}
//
//private static void Play_AfflictionAnxiety()
//{
    //GameManager.GetPlayerVoiceComponent().Play("PLAY_ANXIETYAFFLICTION", Voice.Priority.Normal);
//}
//
// ////            v     NOT WORKING     v            ////
//
//private static void Play_AfflictionAnxietyCured()
//{
    //GameManager.GetPlayerVoiceComponent().Play("Play_VOCatchBreath", Voice.Priority.Normal);
//}
//