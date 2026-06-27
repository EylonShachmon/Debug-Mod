using BepInEx;
using HarmonyLib;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[BepInPlugin("com.EylonShachmon.DebugMod", "Eylonsh's Debug Mod", "0.6.0")]
public class MyDebugMod : BaseUnityPlugin
{
    private static bool alreadyActivated = false;
    private static readonly AccessTools.FieldRef<HeroController, Rigidbody2D> rb2dRef = AccessTools.FieldRefAccess<HeroController, Rigidbody2D>("rb2d");
    private static bool invincibility = false;
    private static Collider2D collider = null;
    private static bool item_Dreamnail;
    private static bool item_Wings;
    private static bool item_Cloak;
    private static bool item_ShadeCloak;
    private static bool item_Lantern;
    private static KeyCode invincibilityKey = KeyCode.P;
    private static KeyCode flightKey = KeyCode.O;
    private static KeyCode damageKey = KeyCode.Alpha0;
    private static KeyCode speedKey = KeyCode.CapsLock;

    private static bool damage = false;
    private static int storeDamage;
    private static bool speedUp = false;
    private static bool flight = false;
    private void Awake()
    {
        base.Logger.LogInfo("Plugin loaded and initialized");
        Harmony.CreateAndPatchAll(typeof(MyDebugMod));
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HeroController), "Update")]
    private static void Cheats(HeroController __instance)
    {
        if (Input.GetKeyDown(flightKey))
        {
            flight = !flight;
            if (collider == null)
            {
                collider = __instance.gameObject.GetComponent<Collider2D>();
            }
            CheatPhase(__instance, flight);
            Debug.Log("Turned flight " + (flight ? "on" : "off"));
        }
        if (Input.GetKeyDown(speedKey))
        {
            speedUp = !speedUp;
            Debug.Log("Turned speedUp " + (speedUp ? "on" : "off"));
        }
        if (Input.GetKeyDown(damageKey))
        {
            damage = !damage;
            Debug.Log("Turned damageBoost " + (damage ? "on" : "off"));
        }
        if (Input.GetKeyDown(invincibilityKey))
        {
            invincibility = !invincibility;
            if (invincibility)
            {
                __instance.damageMode = GlobalEnums.DamageMode.NO_DAMAGE;
            }
            else
            {
                __instance.damageMode = GlobalEnums.DamageMode.FULL_DAMAGE;
            }
            Debug.Log("Turned invincibility " + (invincibility ? "on" : "off"));
        }
        if (invincibility)
        {
            __instance.damageMode = GlobalEnums.DamageMode.NO_DAMAGE;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HeroController), "FixedUpdate")]
    private static void CheatMovePre(HeroController __instance)
    {
        if (flight)
        {
            CheatMove(__instance);
        }
        else if (speedUp)
        {
            CheatSpeed(__instance);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HeroController), "FixedUpdate")]
    private static void CheatMovePost(HeroController __instance)
    {
        if (flight)
        {
            CheatMove(__instance);
        }
        else if (speedUp)
        {
            CheatSpeed(__instance);
        }
    }

    private static void CheatMove(HeroController __instance)
    {

        float speed = 20f;
        float y = 0f;
        float x = 0f;
        if (speedUp)
        {
            speed = 60f;
        }
        Rigidbody2D rb2d = rb2dRef(__instance);
        if (Input.GetKey(KeyCode.S))
        {
            y = -speed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            y = speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            x = -speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x = speed;
        }
        rb2d.linearVelocityY = y;
        rb2d.linearVelocityX = x;

    }

    private static void CheatSpeed(HeroController __instance)
    {
        rb2dRef(__instance).linearVelocityX *= 3;
    }
    private static void CheatPhase(HeroController __instance, bool value)
    {
        __instance.AffectedByGravity(!value);
        collider.enabled = !value;
    }

    private static void CheatItems(HeroController __instance, bool value)
    {
        if (value)
        {
            item_Dreamnail = __instance.playerData.hasDreamNail;
            item_Cloak = __instance.playerData.hasDash;
            item_ShadeCloak = __instance.playerData.hasShadowDash;
            item_Wings = __instance.playerData.hasDoubleJump;
            item_Lantern = __instance.playerData.hasLantern;

            __instance.playerData.hasDreamNail = true;
            __instance.playerData.hasDash = true;
            __instance.playerData.hasShadowDash = true;
            __instance.playerData.hasDoubleJump = true;
            __instance.playerData.hasLantern = true;
        }
        else
        {
            __instance.playerData.hasDreamNail = item_Dreamnail;
            __instance.playerData.hasDash = item_Cloak;
            __instance.playerData.hasShadowDash = item_ShadeCloak;
            __instance.playerData.hasDoubleJump = item_Wings;
            __instance.playerData.hasLantern = item_Lantern;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HealthManager), "TakeDamage")]
    private static void TakeDamage(ref HitInstance hitInstance)
    {
        if (damage)
        {
            hitInstance.DamageDealt = 200;
        }
    }
}
