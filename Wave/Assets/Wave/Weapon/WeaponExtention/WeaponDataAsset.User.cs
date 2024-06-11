using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public partial class WeaponDataAsset
{
    [Tooltip("銃のモデル")] public GameObject WeaponPrefab;
    [Tooltip("射撃サウンド")] public AudioResource FireSound;
}