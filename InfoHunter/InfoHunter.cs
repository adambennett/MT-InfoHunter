using BepInEx;
using HarmonyLib;

namespace InfoHunter
{
    [BepInPlugin("nyoxide.monstertrain.info-hunter", "Info Hunter", "1.0.0.0")]
    [BepInProcess("MonsterTrain.exe")]
    [BepInProcess("MtLinkHandler.exe")]
    public class Hunter : BaseUnityPlugin
    {
        void Patch()
        {
            var harmony = new Harmony("nyoxide.monstertrain.harmony");
            harmony.PatchAll();
        }

        void Awake()
        {
            Patch();
        }
    }
}