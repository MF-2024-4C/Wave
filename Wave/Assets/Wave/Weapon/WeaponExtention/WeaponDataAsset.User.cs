using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public partial class WeaponDataAsset
{
    [Tooltip("銃のモデル")] public GameObject WeaponPrefab;
    [Tooltip("射撃音")] public AudioResource FireSound;
    [Tooltip("リロード音")] public AudioResource ReloadSound;
    [Tooltip("取り出し音")] public AudioResource EquipSound;
}