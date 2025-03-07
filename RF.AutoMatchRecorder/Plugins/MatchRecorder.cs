using System.Text;
using HarmonyLib;
using RF.AutoMatchRecorder.Modules;
using Scripts.EnsoGame.Pause;
// ReSharper disable InconsistentNaming

namespace RF.AutoMatchRecorder.Plugins;

[HarmonyPatch]
public class MatchRecorder
{
    [HarmonyPatch(typeof(ResultPlayer))]
    [HarmonyPatch(nameof(ResultPlayer.ToWaitState))]
    [HarmonyPatch(MethodType.Normal)]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void ResultPlayer_ToWaitState_Prefix(ResultPlayer __instance)
    { 
        // Stop record
        Obs.Instance.StopRecord();
    }

    [HarmonyPatch(typeof(ResultPlayer))]
    [HarmonyPatch(nameof(ResultPlayer.waitResultDisp))]
    [HarmonyPatch(MethodType.Normal)]
    [HarmonyPrefix]
    public static void ResultPlayer_waitResultDisp_Prefix(ResultPlayer __instance)
    {
        // Stop record
        Obs.Instance.StopRecord();
    }
    
    [HarmonyPatch(typeof(SongInfoLayOut))]
    [HarmonyPatch(nameof(SongInfoPlayer.Update))]
    [HarmonyPatch(MethodType.Normal)]
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void SongInfoLayOut_Update_Postfix(SongInfoLayOut __instance)
    {
        var songName = Encoding.UTF8.GetString(
            Encoding.Latin1.GetBytes(__instance.txt.m_text.ToCharArray())
        );

        if (songName.Length != 0)
        {
            Logger.Log(songName);

            // Stop record
            Obs.Instance.StopRecord();
            
            // Start record
            Obs.Instance.StartRecord(songName);
        }
    }
    
    
    [HarmonyPatch(typeof(UiPauseMenu))]
    [HarmonyPatch(nameof(UiPauseMenu.Open))]
    [HarmonyPatch(MethodType.Normal)]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void UiPauseMenu_Open_Prefix(UiPauseMenu __instance)
    {
        // Pause record
        Obs.Instance.PauseRecord();
    }
    
    [HarmonyPatch(typeof(UiPauseMenu))]
    [HarmonyPatch(nameof(UiPauseMenu.Close))]
    [HarmonyPatch(MethodType.Normal)]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void UiPauseMenu_Close_Prefix(UiPauseMenu __instance)
    {
        // Resume record
        Obs.Instance.ResumeRecord();
    }
    
    [HarmonyPatch(typeof(UiPauseMenu))]
    [HarmonyPatch(nameof(UiPauseMenu.Unbind))]
    [HarmonyPatch(MethodType.Normal)]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void UiPauseMenu_Unbind_Prefix(UiPauseMenu __instance)
    {
        // Stop record
        Obs.Instance.StopRecord();
    }
}