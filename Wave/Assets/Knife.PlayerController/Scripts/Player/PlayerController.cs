﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using UnityEngine.Experimental.Rendering.HDPipeline;

namespace KnifePlayerController
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        public MouseLook Look;
        public Headbob HeadBob;

        public AnimationCurve HeadBobBlendCurve;
        public AnimationCurve HeadBobPeriodBlendCurve;

        public string ForwardAxis = "Vertical";
        public string StrafeAxis = "Horizontal";
        public Transform DirectionReference;
        public float CrouchSpeedMultiplier = 0.75f;
        public float RunSpeedMultiplier = 1.5f;
        public float RunIncreaseSpeedTime = 1f;
        public float RunSpeedThreshold = 1f;
        public AnimationCurve RunIncreaseSpeedCurve;
        public MeshRenderer NearBlurSphere;

        public float Speed = 1f;
        public LayerMask GroundLayer;
        public float Threshold = 0.1f;
        public float Gravity = 9.81f;
        public float StickToGround = 9.81f;

        public PlayerStandState StandState;
        public PlayerStandState CrouchState;
        public float StateChangeSpeed = 3.666f;
        public AnimationCurve StateChangeCurve;
        public float MaxSpeed = 1f;
        public float WeightSmooth = 3f;
        public float JumpSpeed = 5f;
        public TransformNoise CameraNoise;
        public float IdleNoise = 1f;
        public float RunNoise = 2f;
        public Camera PlayerCamera;
        public Transform ControlCamera;
        public Transform HandsHeadBobTarget;
        public float CameraHeadbobWeight = 1f;
        public float HandsHeadbobWeight = 0.3f;
        public float HandsHeadbobMultiplier = 1;
        public PlayerHands Hands;
        public PostProcessingController PPController;
        public PostProcessingController.DepthOfFieldSettings DefaultDofSettings;

        public PlayerFreezeChangedEvent PlayerFreezeChanged = new PlayerFreezeChangedEvent();

        [System.Serializable]
        public class PlayerStandState
        {
            public float ControlCameraHeight;
            public float ColliderHeight;
            public float ColliderCenterHeight;
        }

        public Vector3 PlayerVelocity
        {
            get
            {
                return controller.velocity;
            }
        }

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
        }
        public bool IsCrouching
        {
            get
            {
                return isCrouching;
            }
        }

        public bool IsGrounded
        {
            get
            {
                return controller.isGrounded;
            }
        }
        public float DefaultHandsHeadbobWeight
        {
            get
            {
                return defaultHandsHeadbobWeight;
            }
        }
        public PlayerDamageHandler DamageHandler
        {
            get
            {
                if (damageHandler == null)
                    damageHandler = GetComponent<PlayerDamageHandler>();

                return damageHandler;
            }
        }

        public UnityAction RunStartEvent;
        public UnityAction JumpStartEvent;
        public UnityAction JumpFallEvent;
        public UnityAction JumpEndEvent;
        public UnityAction CrouchEvent;
        public UnityAction StandUpEvent;

        CapsuleCollider charactarCollider;
        CharacterController controller;
        PlayerDamageHandler damageHandler;

        Vector3 playerVelocity = Vector3.zero;
        Vector3 oldPlayerVelocity = Vector3.zero;

        Vector3 oldPosition;

        Vector3 oldHandHeadBobPos;
        Vector3 oldCameraHeadBobPos;

        Vector3 controlCameraPosition;
        float standStateBlend;

        float runTime = 0f;
        float standChangeTime;

        float defaultHandsHeadbobWeight;
        bool freezeControl = false;
        bool isRunning;
        bool oldIsGrounded = false;
        bool isCrouching;

        public bool IsFreezed
        {
            get
            {
                return freezeControl;
            }
        }

        void Start()
        {
            damageHandler = GetComponent<PlayerDamageHandler>();
            charactarCollider = GetComponent<CapsuleCollider>();
            controller = GetComponent<CharacterController>();

            Look.Init(transform, ControlCamera);

            defaultHandsHeadbobWeight = HandsHeadbobWeight;

            controlCameraPosition = ControlCamera.localPosition;
        }

        public void UpdateDefaultDeath()
        {
            Look.RotateCameraSmoothlyTo(0, Time.deltaTime);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        void FixedUpdate()
        {
            Look.LookRotation(Time.fixedDeltaTime);

            if (controller.isGrounded)
            {
                move();
            }
            else
            {
                applyGravity();
            }

            if (oldPlayerVelocity.y > 0 && playerVelocity.y < 0)
            {
                if (JumpFallEvent != null)
                {
                    JumpFallEvent();
                }
            }

            if (controller.isGrounded && !oldIsGrounded)
            {
                if (JumpEndEvent != null)
                {
                    JumpEndEvent();
                }
            }

            oldPlayerVelocity = playerVelocity;
            oldIsGrounded = controller.isGrounded;
            oldPosition = transform.position;
        }

        public void Freeze(bool value)
        {
            if (!value && !damageHandler.Health.RealIsAlive)
                return;

            Look.Enabled = !value;
            freezeControl = value;

            PlayerFreezeChanged.Invoke(freezeControl);
        }

        public void SetNoiseEnabled(bool isEnabled)
        {
            CameraNoise.enabled = isEnabled;
        }

        void move()
        {
            float h = Input.GetAxis(StrafeAxis);
            float v = Input.GetAxis(ForwardAxis);

            if (freezeControl)
            {
                h = 0;
                v = 0;
            }

            HeadBob.CalcHeadbob(Time.time);

            HandsHeadBobTarget.localPosition -= oldHandHeadBobPos;

            // you can do any HandsHeadBobTarget position set

            HandsHeadBobTarget.localPosition += HeadBob.HeadBobPos * HandsHeadbobWeight * HandsHeadbobMultiplier;

            ControlCamera.localPosition -= oldCameraHeadBobPos;

            ControlCamera.localPosition = controlCameraPosition;
            // you can do any ControlCamera position set

            ControlCamera.localPosition += HeadBob.HeadBobPos * CameraHeadbobWeight;

            oldHandHeadBobPos = HeadBob.HeadBobPos * HandsHeadbobWeight * HandsHeadbobMultiplier;
            oldCameraHeadBobPos = HeadBob.HeadBobPos * CameraHeadbobWeight;

            Vector3 moveVector = DirectionReference.forward * v + DirectionReference.right * h;
            Vector3 playerXZVelocity = Vector3.Scale(playerVelocity, new Vector3(1, 0, 1));

            float speed = Speed;
            if (Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(1) && !Input.GetMouseButton(0) && playerXZVelocity.magnitude >= RunSpeedThreshold && !isCrouching)
            {
                //speed *= RunSpeedMultiplier;
                runTime += Time.fixedDeltaTime;
                if (!isRunning)
                {
                    isRunning = true;
                    if (RunStartEvent != null)
                    {
                        RunStartEvent();
                    }
                }

                if (!Hands.IsAiming)
                    CameraNoise.NoiseAmount = Mathf.MoveTowards(CameraNoise.NoiseAmount, RunNoise, Time.fixedDeltaTime * 5f);
            }
            else
            {
                runTime -= Time.fixedDeltaTime;

                if (!Hands.IsAiming)
                    CameraNoise.NoiseAmount = Mathf.MoveTowards(CameraNoise.NoiseAmount, IdleNoise, Time.fixedDeltaTime * 5f);
                isRunning = false;
            }

            if(Input.GetKeyDown(KeyCode.LeftControl) && !freezeControl)
            {
                isCrouching = true;
                
                if(CrouchEvent != null)
                {
                    CrouchEvent();
                }
            }

            if(isCrouching)
            {

            }

            if ((Input.GetKeyUp(KeyCode.LeftControl) && !freezeControl) || (Input.GetKey(KeyCode.LeftControl) && freezeControl && isCrouching))
            {
                isCrouching = false;

                if (damageHandler.Health.RealIsAlive)
                {
                    if (StandUpEvent != null)
                    {
                        StandUpEvent();
                    }
                }

            }

            standStateChange();

            runTime = Mathf.Clamp(runTime, 0, RunIncreaseSpeedTime);

            float runTimeFraction = runTime / RunIncreaseSpeedTime;
            Hands.SetRun(runTimeFraction);
            float runMultiplier = Mathf.Lerp(1, RunSpeedMultiplier, RunIncreaseSpeedCurve.Evaluate(runTimeFraction));
            speed *= runMultiplier;
            if (isCrouching)
                speed *= CrouchSpeedMultiplier;

            Ray r = new Ray(transform.position, Vector3.down);
            RaycastHit hitInfo;

            Physics.SphereCast(r, charactarCollider.radius, out hitInfo, charactarCollider.height / 2f, GroundLayer);

            Vector3 desiredVelocity = Vector3.ProjectOnPlane(moveVector, hitInfo.normal) * speed;
            playerVelocity.x = desiredVelocity.x;
            playerVelocity.z = desiredVelocity.z;
            playerVelocity.y = -StickToGround;

            Vector3 calculatedVelocity = playerVelocity;
            calculatedVelocity.y = 0;

            float speedFraction = calculatedVelocity.magnitude / MaxSpeed;
            HeadBob.HeadBobWeight = Mathf.Lerp(HeadBob.HeadBobWeight, HeadBobBlendCurve.Evaluate(speedFraction), WeightSmooth * Time.fixedDeltaTime);
            HeadBob.HeadBobPeriod = HeadBobPeriodBlendCurve.Evaluate(speedFraction);

            if (controller.isGrounded)
            {
                if (Input.GetKeyDown(KeyCode.Space) && !isCrouching && !freezeControl)
                {
                    playerVelocity.y = JumpSpeed;
                    if (JumpStartEvent != null)
                        JumpStartEvent();
                }
                controller.Move(playerVelocity * Time.fixedDeltaTime);
            }
        }

        void standStateChange()
        {
            standStateBlend = Mathf.MoveTowards(standStateBlend, isCrouching ? 1f : 0f, Time.deltaTime * StateChangeSpeed);

            charactarCollider.height = Mathf.Lerp(
                StandState.ColliderHeight,
                CrouchState.ColliderHeight,
                StateChangeCurve.Evaluate(standStateBlend)
                );


            Vector3 colliderCenter = charactarCollider.center;

            colliderCenter.y = Mathf.Lerp(
                StandState.ColliderCenterHeight,
                CrouchState.ColliderCenterHeight,
                StateChangeCurve.Evaluate(standStateBlend)
                );
            charactarCollider.center = colliderCenter;

            controller.height = charactarCollider.height;
            controller.center = charactarCollider.center;

            controlCameraPosition.y = Mathf.Lerp(
                StandState.ControlCameraHeight,
                CrouchState.ControlCameraHeight,
                StateChangeCurve.Evaluate(standStateBlend)
                );
        }

        public void SetSensivityMultiplier(float multiplier)
        {
            Look.SensivityMultiplier = multiplier;
        }

        void applyGravity()
        {
            playerVelocity += Vector3.down * Gravity * Time.fixedDeltaTime;
            controller.Move(playerVelocity * Time.fixedDeltaTime);
        }

        [System.Serializable]
        public class Headbob
        {
            public bool Enabled = true;
            public float HeadBobWeight = 1f;
            public Vector2 HeadBobAmount = new Vector2(0.11f, 0.08f);
            public float HeadBobPeriod = 1f;
            public AnimationCurve HeadBobCurveX;
            public AnimationCurve HeadBobCurveY;

            public Vector3 HeadBobPos
            {
                get
                {
                    return resultHeadbob;
                }
            }

            Vector3 resultHeadbob;

            public void CalcHeadbob(float currentTime)
            {
                float headBob = Mathf.PingPong(currentTime, HeadBobPeriod) / HeadBobPeriod;

                Vector3 headBobVector = new Vector3();

                headBobVector.x = HeadBobCurveX.Evaluate(headBob) * HeadBobAmount.x;
                headBobVector.y = HeadBobCurveY.Evaluate(headBob) * HeadBobAmount.y;

                headBobVector = Vector3.LerpUnclamped(Vector3.zero, headBobVector, HeadBobWeight);

                if (!Application.isPlaying)
                {
                    headBobVector = Vector2.zero;
                }

                if (Enabled)
                {
                    resultHeadbob = headBobVector;
                }
            }
        }

        [System.Serializable]
        public class MouseLook
        {
            public bool Enabled;
            public float XSensitivity = 2f;
            public float YSensitivity = 2f;
            public float SensivityMultiplier = 1f;
            public float MinimumX = -90F;
            public float MaximumX = 90F;
            public float SmoothTime = 15f;
            public bool ClampVerticalRotation = true;

            public string AxisXName = "Mouse X";
            public string AxisYName = "Mouse Y";

            private Quaternion characterTargetRot;
            private Quaternion cameraTargetRot;

            private Transform character;
            private Transform camera;

            public void Init(Transform character, Transform camera)
            {
                characterTargetRot = character.localRotation;
                cameraTargetRot = camera.localRotation;

                this.character = character;
                this.camera = camera;
            }

            public void LookRotation(float deltaTime)
            {
                if (!Enabled)
                    return;

                LookRotation(Input.GetAxis(AxisXName) * XSensitivity * SensivityMultiplier, Input.GetAxis(AxisYName) * YSensitivity * SensivityMultiplier, deltaTime);
            }

            public void LookRotation(float yRot, float xRot, float deltaTime)
            {
                characterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
                cameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

                if (ClampVerticalRotation)
                    cameraTargetRot = ClampRotationAroundXAxis(cameraTargetRot);

                character.localRotation = Quaternion.Slerp(character.localRotation, characterTargetRot, SmoothTime * deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, cameraTargetRot, SmoothTime * deltaTime);
            }

            public void RotateCameraSmoothlyTo(float xRot, float deltaTime)
            {
                cameraTargetRot = Quaternion.Euler(xRot, 0f, 0f);

                if (ClampVerticalRotation)
                    cameraTargetRot = ClampRotationAroundXAxis(cameraTargetRot);

                camera.localRotation = Quaternion.Slerp(camera.localRotation, cameraTargetRot, SmoothTime * deltaTime);
            }

            Quaternion ClampRotationAroundXAxis(Quaternion q)
            {
                q.x /= q.w;
                q.y /= q.w;
                q.z /= q.w;
                q.w = 1.0f;

                float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

                angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

                q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

                return q;
            }

        }
    }

    public class PlayerFreezeChangedEvent : UnityEvent<bool>
    { }

}