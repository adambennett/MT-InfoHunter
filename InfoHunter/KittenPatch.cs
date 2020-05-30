using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace InfoHunter
{
    [HarmonyPatch(typeof(AssetLoadingManager), "LoadAllCoroutine")]
	static class AssetLoadingManager_Patch
	{
		private static bool triggered;
		
		static void Postfix(ref AssetLoadingData ____assetLoadingData)
		{
			if (!triggered)
			{
				// Reach into the game data and pull out the full card set
				List<CardData> cardData = ____assetLoadingData.AllGameData.GetAllCardData();

				// Set aside some values to fill in later
				CardTraitData piercingTraitData = null;
				CardTraitData offeringTraitData = null;
				CardEffectData frostbiteData = null;
				CardData lanceData = null;

				// Iterate through all cards
				for (int i = 0; i < cardData.Count; i++)
				{
					if (cardData[i].GetName() == "Horn Break")
					{ // Set aside Horn Break's piercing trait, so we can copy it onto Frozen Lance later
						piercingTraitData = cardData[i].GetTraits()[0].Copy();
					}
					if (cardData[i].GetName() == "Titanstooth")
					{ // Set aside Titanstooth's offering trait and frostbite effect for the same reason
						offeringTraitData  = cardData[i].GetTraits()[0].Copy();
						frostbiteData = cardData[i].GetEffects()[1];
					}
					if (cardData[i].GetName() == "Frozen Lance")
					{ // Set aside Frozen Lance's card data, so we have a place to copy the aforementioned onto
						lanceData = cardData[i];
					}
				}

				// Use reflection to set the paramCardData field of the traits to Frozen Lance's card data
				// We only need to do this for traits, not effects
				var prop = typeof(CardTraitData).GetField("paramCardData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				prop.SetValue(piercingTraitData, lanceData);
				prop.SetValue(offeringTraitData, lanceData);

				// Add the traits to Frozen Lance's trait list
				lanceData.GetTraits().Add(piercingTraitData);
				lanceData.GetTraits().Add(offeringTraitData);

				// Find Frozen Lance's damage effect and set its value to 3
				foreach (CardEffectData cardEffectData in lanceData.GetEffects())
				{
					prop = typeof(CardEffectData).GetField("paramInt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					prop.SetValue(cardEffectData, 3);
				}

				// Modify the frostbite effect's targeting mode to only the last targeted characters
				// This prevents it from targeting the entire room (what it would otherwise do, since we copied it from Titanstooth)
				// This probably modifies Titanstooth's behaviour, too (that's bad), but I haven't checked
				prop = typeof(CardEffectData).GetField("targetMode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				prop.SetValue(frostbiteData, TargetMode.LastTargetedCharacters);

				// Add the effect to Frozen Lance's effects list
				lanceData.GetEffects().Add(frostbiteData);
				
				// Prevent patch from running again
				triggered = true;
			}
			else
			{
				Debug.Log("KittenPatch has already run once!");
			}
			
		}
	}
}