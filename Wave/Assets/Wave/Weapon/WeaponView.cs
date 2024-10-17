using KINEMATION.FPSAnimationFramework.Runtime.Camera;
using KINEMATION.FPSAnimationFramework.Runtime.Core;
using KINEMATION.FPSAnimationFramework.Runtime.Layers.IkMotionLayer;
using KINEMATION.FPSAnimationFramework.Runtime.Playables;
using KINEMATION.FPSAnimationFramework.Runtime.Recoil;
using KINEMATION.KAnimationCore.Runtime.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Wave.Weapon
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] protected IkMotionLayerSettings equipMotion;
        [SerializeField] protected IkMotionLayerSettings unEquipMotion;

        [SerializeField] private FPSAnimationAsset _fireClip;
        [SerializeField] private RecoilAnimData _recoilData;

        private Animator _controllerAnimator;
        private UserInputController _userInputController;
        private IPlayablesController _playablesController;
        private FPSCameraController _fpsCameraController;

        private FPSAnimator _fpsAnimator;
        private FPSAnimatorEntity _fpsAnimatorEntity;

        private RecoilAnimation _recoilAnimation;
        private RecoilPattern _recoilPattern;

        private Animator _weaponAnimator;

        private FireMode _fireMode;
        private float _fireRate;
        
        private bool _isInitialized;

        public void Initialize(FireMode settings, float fireRate)
        {
            if (_isInitialized) return;
            
            _isInitialized = true;
            _fireMode = settings;
            _fireRate = fireRate;
        }

        public void OnFire()
        {
            if (_weaponAnimator != null)
            {
                _weaponAnimator.Play("Fire", 0, 0f);
            }

            if (_fireClip != null) _playablesController.PlayAnimation(_fireClip);

            if (_recoilAnimation != null && _recoilData != null)
            {
                _recoilAnimation.Play();
            }
        }

        /// <summary>
        /// 武器を装備した際の処理
        /// </summary>
        /// <param name="parent">取り出して装備する親オブジェクト</param>
        public void OnEquip(GameObject parent)
        {
            if (!_isInitialized)
            {
                Debug.LogError("WeaponView is not initialized.");
                return;
            }
            
            if (parent == null) return;
            _fpsAnimator = parent.GetComponent<FPSAnimator>();
            _fpsAnimatorEntity = GetComponent<FPSAnimatorEntity>();

            _weaponAnimator = GetComponentInChildren<Animator>();

            _controllerAnimator = parent.GetComponent<Animator>();
            _userInputController = parent.GetComponent<UserInputController>();
            _playablesController = parent.GetComponent<IPlayablesController>();
            _fpsCameraController = parent.GetComponentInChildren<FPSCameraController>();

            //InitializeAttachments();

            _recoilAnimation = parent.GetComponent<RecoilAnimation>();
            _recoilPattern = parent.GetComponent<RecoilPattern>();

            _fpsAnimator.LinkAnimatorProfile(gameObject);

            //barrelAttachments.Initialize(_fpsAnimator);
            //gripAttachments.Initialize(_fpsAnimator);

            _recoilAnimation.Init(_recoilData, _fireRate, _fireMode);

            if (_recoilPattern != null)
            {
                //_recoilPattern.Init(recoilPatternSettings);
            }

            _fpsAnimator.LinkAnimatorLayer(equipMotion);
        }
    }
}