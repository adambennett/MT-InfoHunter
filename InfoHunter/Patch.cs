using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace InfoHunter
{
    [HarmonyPatch(typeof(AssetLoadingManager), "LoadAllCoroutine")]
    static class AssetLoadingManager_LoadAdditionalCards_Patch
    {
        private static bool triggered;
        
        static void Postfix(ref AssetLoadingData ____assetLoadingData)
        {
            if (!triggered)
            {
                Debug.Log("Info Hunter :: Looking for info...");
                List<CardData> cardData = ____assetLoadingData.AllGameData.GetAllCardData();
                List<string> consumeNames = new List<string>();
                List<string> healNames = new List<string>();
                int consumeAmt = 0;
                int healAmt = 0;
                for (int i = 0; i < cardData.Count; i++)
                {
                    try
                    {
                        CardData check = cardData[i];
                        foreach (CardTraitData trait in check.GetTraits())
                        {
                            if (trait.GetTraitStateName() == "CardTraitExhaustState")
                            {
                                consumeAmt++;
                                consumeNames.Add(check.GetName());
                                break;
                            }
                        }
    
                        foreach (CardEffectData effect in check.GetEffects())
                        {
                            if (effect.GetEffectStateName() == "CardEffectHeal")
                            {
                                healAmt++;
                                healNames.Add(check.GetName());
                                break;
                            }
                        }
                        
                    } catch (Exception ignored) {}
                }
                Debug.Log("all consume cards: ");
                foreach (string card in consumeNames)
                {
                    Debug.Log(card);
                }
                Debug.Log("--------------------------");
                Debug.Log("all healing cards: ");
                foreach (string card in healNames)
                {
                    Debug.Log(card);
                }
                Debug.Log("Amount of Consume Cards: " + consumeAmt);
                Debug.Log("Amount of Healing Cards: " + healAmt);
                triggered = true;
            }
            else
            {
                Debug.Log("Already ran Info Hunter once!");
            }
        }
    }
}