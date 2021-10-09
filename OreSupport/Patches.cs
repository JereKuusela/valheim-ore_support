using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace OreSupport {
  [HarmonyPatch]
  public class Patch {
    private static T Get<T>(object obj, string field) => Traverse.Create(obj).Field<T>(field).Value;
    public static ZNetView Nview(MonoBehaviour obj) => Get<ZNetView>(obj, "m_nview");
    public static float Health(object obj) => Get<float>(obj, "m_health");
    public static object Bound(object obj) => Get<object>(obj, "m_bound");
    public static Vector3 Pos(object obj) => Get<Vector3>(obj, "m_pos");
    public static Vector3 Size(object obj) => Get<Vector3>(obj, "m_size");
    public static Quaternion Rot(object obj) => Get<Quaternion>(obj, "m_rot");
    public static Collider Collider(object obj) => Get<Collider>(obj, "m_collider");
    public static List<Collider> HitAreas(MineRock obj) => Get<List<Collider>>(obj, "m_hitAreas");
    public static int GroundLayer(MineRock5 obj) => Get<int>(obj, "m_groundLayer");
    public static IEnumerable<object> HitAreas(MineRock5 obj) => Get<IEnumerable<object>>(obj, "m_hitAreas");
    public static int RayMask(MineRock5 obj) => Get<int>(obj, "m_rayMask");
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(MineRock5), "GetSupport")]
    public static bool MineRock5_GetSupport(MineRock5 instance, Collider c) {
      throw new NotImplementedException("Dummy");
    }
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(MineRock5), "SetupColliders")]
    public static void MineRock5_SetupColliders(MineRock5 instance) {
      throw new NotImplementedException("Dummy");
    }
  }
}