using UnityEngine;

namespace ZoremGame.Player
{
    public class PlayerMotor : MonoBehaviour
    {
        #region Inspector Variables

        [Header("- Movement")]
        [Tooltip("Use Root Motion from Animator")]
        public bool useRootMotion = false;
        [Tooltip("Rotate using World axis (true) or Camera axis (false)")]
        public bool rotateByWorld = false;
        [Tooltip("Sprint toggle mode: on/off per press")]
        public bool useContinuousSprint = true;
        [Tooltip("Can only sprint in free movement")]
        public bool sprintOnlyFree = true;

        public enum LocomotionType
        {
            FreeWithStrafe,
            OnlyStrafe,
            OnlyFree,
        }
        public LocomotionType locomotionType = LocomotionType.FreeWithStrafe;

        public MovementSpeed freeSpeed = new MovementSpeed();
        public MovementSpeed strafeSpeed = new MovementSpeed();

        [Header("- Airborne")]
        public bool jumpWithRigidbodyForce = false;
        public bool jumpAndRotate = true;
        public float jumpTimer = 0.3f;
        public float jumpHeight = 4f;
        public float airSpeed = 5f;
        public float airSmooth = 6f;
        public float extraGravity = -10f;
        [HideInInspector]
        public float limitFallVelocity = -15f;

        [Header("- Ground")]
        public LayerMask groundLayer = 1 << 0;
        public float groundMinDistance = 0.25f;
        public float groundMaxDistance = 0.5f;
        [Range(30, 80)] public float slopeLimit = 75f;

        [Header("- Slide")]
        public float slideDuration = 1f;
        public float slideSpeed = 8f;
        [Range(0.2f, 1f)] public float slideColliderHeightMultiplier = 0.5f;

        [Header("- Crouch")]
        [Range(0.1f, 1f)] public float crouchSpeedMultiplier = 0.5f;
        [Range(0.3f, 1f)] public float crouchColliderHeightMultiplier = 0.6f;
        #endregion

        #region Components
        internal Animator animator;
        internal Rigidbody _rigidbody;
        internal PhysicsMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;
        internal CapsuleCollider _capsuleCollider;
        #endregion

        #region Internal Variables
        internal bool isJumping;
        internal bool isStrafing
        {
            get { return _isStrafing; }
            set { _isStrafing = value; }
        }
        internal bool isGrounded { get; set; }
        internal bool isSprinting { get; set; }
        internal bool isSliding { get; private set; }
        internal bool isCrouching { get; private set; }
        public bool stopMove { get; protected set; }

        internal float inputMagnitude;
        internal float verticalSpeed;
        internal float horizontalSpeed;
        internal float moveSpeed;
        internal float verticalVelocity;
        internal float colliderRadius, colliderHeight;
        internal float heightReached;
        internal float jumpCounter;
        internal float groundDistance;
        internal float slideTimer;
        internal Vector3 slideDirection;
        internal bool wantsToStandUp;
        internal RaycastHit groundHit;
        internal bool lockMovement = false;
        internal bool lockRotation = false;
        internal bool _isStrafing;
        internal Transform rotateTarget;
        internal Vector3 input;
        internal Vector3 colliderCenter;
        internal Vector3 inputSmooth;
        internal Vector3 moveDirection;
        #endregion

        public void Init()
        {
            animator = GetComponent<Animator>();
            animator.updateMode = AnimatorUpdateMode.Normal;

            frictionPhysics = new PhysicsMaterial();
            frictionPhysics.name = "frictionPhysics";
            frictionPhysics.staticFriction = .25f;
            frictionPhysics.dynamicFriction = .25f;
            frictionPhysics.frictionCombine = PhysicsMaterialCombine.Multiply;

            maxFrictionPhysics = new PhysicsMaterial();
            maxFrictionPhysics.name = "maxFrictionPhysics";
            maxFrictionPhysics.staticFriction = 1f;
            maxFrictionPhysics.dynamicFriction = 1f;
            maxFrictionPhysics.frictionCombine = PhysicsMaterialCombine.Maximum;

            slippyPhysics = new PhysicsMaterial();
            slippyPhysics.name = "slippyPhysics";
            slippyPhysics.staticFriction = 0f;
            slippyPhysics.dynamicFriction = 0f;
            slippyPhysics.frictionCombine = PhysicsMaterialCombine.Minimum;

            _rigidbody = GetComponent<Rigidbody>();
            _capsuleCollider = GetComponent<CapsuleCollider>();

            colliderCenter = _capsuleCollider.center;
            colliderRadius = _capsuleCollider.radius;
            colliderHeight = _capsuleCollider.height;

            isGrounded = true;
        }

        public virtual void UpdateMotor()
        {
            CheckGround();
            ControlSlideBehaviour();
            ControlCrouchBehaviour();
            CheckSlopeLimit();
            ControlJumpBehaviour();
            AirControl();
        }

        #region Locomotion
        public virtual void SetControllerMoveSpeed(MovementSpeed speed)
        {
            float target = speed.walkByDefault
                ? (isSprinting ? speed.runningSpeed : speed.walkSpeed)
                : (isSprinting ? speed.sprintSpeed : speed.runningSpeed);

            if (isCrouching) target *= crouchSpeedMultiplier;

            moveSpeed = Mathf.Lerp(moveSpeed, target, speed.movementSmooth * Time.deltaTime);
        }

        public virtual void MoveCharacter(Vector3 _direction)
        {
            inputSmooth = Vector3.Lerp(inputSmooth, input, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);

            if (!isGrounded || isJumping) return;

            _direction.y = 0;
            _direction.x = Mathf.Clamp(_direction.x, -1f, 1f);
            _direction.z = Mathf.Clamp(_direction.z, -1f, 1f);

            if (_direction.magnitude > 1f)
                _direction.Normalize();

            Vector3 targetPosition = (useRootMotion ? animator.rootPosition : _rigidbody.position) + _direction * (stopMove ? 0 : moveSpeed) * Time.deltaTime;
            Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

            targetVelocity.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = targetVelocity;
        }

        public virtual void CheckSlopeLimit()
        {
            if (input.sqrMagnitude < 0.1) return;

            RaycastHit hitinfo;
            var hitAngle = 0f;

            if (Physics.Linecast(transform.position + Vector3.up * (_capsuleCollider.height * 0.5f), transform.position + moveDirection.normalized * (_capsuleCollider.radius + 0.2f), out hitinfo, groundLayer))
            {
                hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

                var targetPoint = hitinfo.point + moveDirection.normalized * _capsuleCollider.radius;
                if ((hitAngle > slopeLimit) && Physics.Linecast(transform.position + Vector3.up * (_capsuleCollider.height * 0.5f), targetPoint, out hitinfo, groundLayer))
                {
                    hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

                    if (hitAngle > slopeLimit && hitAngle < 85f)
                    {
                        stopMove = true;
                        return;
                    }
                }
            }
            stopMove = false;
        }

        public virtual void RotateToPosition(Vector3 position)
        {
            Vector3 desiredDirection = position - transform.position;
            RotateToDirection(desiredDirection.normalized);
        }

        public virtual void RotateToDirection(Vector3 direction)
        {
            RotateToDirection(direction, isStrafing ? strafeSpeed.rotationSpeed : freeSpeed.rotationSpeed);
        }

        public virtual void RotateToDirection(Vector3 direction, float rotationSpeed)
        {
            if (!jumpAndRotate && !isGrounded) return;
            direction.y = 0f;
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, direction.normalized, rotationSpeed * Time.deltaTime, .1f);
            Quaternion _newRotation = Quaternion.LookRotation(desiredForward);
            transform.rotation = _newRotation;
        }
        #endregion

        #region Jump Methods
        protected virtual void ControlJumpBehaviour()
        {
            if (!isJumping) return;

            jumpCounter -= Time.deltaTime;
            if (jumpCounter <= 0)
            {
                jumpCounter = 0;
                isJumping = false;
            }

            var vel = _rigidbody.linearVelocity;
            vel.y = jumpHeight;
            _rigidbody.linearVelocity = vel;
        }

        public virtual void AirControl()
        {
            if ((isGrounded && !isJumping)) return;
            if (transform.position.y > heightReached) heightReached = transform.position.y;
            inputSmooth = Vector3.Lerp(inputSmooth, input, airSmooth * Time.deltaTime);

            if (jumpWithRigidbodyForce && !isGrounded)
            {
                _rigidbody.AddForce(moveDirection * airSpeed * Time.deltaTime, ForceMode.VelocityChange);
                return;
            }

            moveDirection.y = 0;
            moveDirection.x = Mathf.Clamp(moveDirection.x, -1f, 1f);
            moveDirection.z = Mathf.Clamp(moveDirection.z, -1f, 1f);

            Vector3 targetPosition = _rigidbody.position + (moveDirection * airSpeed) * Time.deltaTime;
            Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

            targetVelocity.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = Vector3.Lerp(_rigidbody.linearVelocity, targetVelocity, airSmooth * Time.deltaTime);
        }

        protected virtual bool jumpFwdCondition
        {
            get
            {
                Vector3 p1 = transform.position + _capsuleCollider.center + Vector3.up * -_capsuleCollider.height * 0.5F;
                Vector3 p2 = p1 + Vector3.up * _capsuleCollider.height;
                return Physics.CapsuleCastAll(p1, p2, _capsuleCollider.radius * 0.5f, transform.forward, 0.6f, groundLayer).Length == 0;
            }
        }
        #endregion

        #region Slide Methods
        protected virtual void StartSlide()
        {
            if (isSliding) return;

            isSliding = true;
            slideTimer = slideDuration;
            slideDirection = moveDirection.sqrMagnitude > 0.01f ? moveDirection.normalized : transform.forward;
            lockMovement = true;
            lockRotation = true;
            SetColliderHeight(slideColliderHeightMultiplier);
        }

        protected virtual void EndSlide()
        {
            if (!isSliding) return;

            isSliding = false;
            lockMovement = false;
            lockRotation = false;
            RestoreColliderHeight();
        }

        protected virtual void ControlSlideBehaviour()
        {
            if (!isSliding) return;

            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f || !isGrounded)
            {
                EndSlide();
                return;
            }

            Vector3 velocity = slideDirection * slideSpeed;
            velocity.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = velocity;
        }

        // Reduce el collider manteniendo la base fija para no clipear el suelo.
        protected virtual void SetColliderHeight(float heightMultiplier)
        {
            float newHeight = colliderHeight * heightMultiplier;
            float heightDelta = colliderHeight - newHeight;
            _capsuleCollider.height = newHeight;
            _capsuleCollider.center = colliderCenter - Vector3.up * (heightDelta * 0.5f);
        }

        protected virtual void RestoreColliderHeight()
        {
            _capsuleCollider.height = colliderHeight;
            _capsuleCollider.center = colliderCenter;
        }
        #endregion

        #region Crouch Methods
        protected virtual void StartCrouch()
        {
            if (isCrouching) return;

            isCrouching = true;
            wantsToStandUp = false;
            SetColliderHeight(crouchColliderHeightMultiplier);
        }

        // No se levanta de inmediato: espera a que haya espacio libre arriba.
        protected virtual void StopCrouch()
        {
            if (!isCrouching) return;
            wantsToStandUp = true;
        }

        protected virtual void ControlCrouchBehaviour()
        {
            if (!isCrouching || !wantsToStandUp) return;

            if (CanStandUp())
            {
                isCrouching = false;
                wantsToStandUp = false;
                RestoreColliderHeight();
            }
        }

        protected virtual bool CanStandUp()
        {
            float clearance = colliderHeight - _capsuleCollider.height;
            if (clearance <= 0.01f) return true;

            Vector3 origin = transform.position + Vector3.up * (_capsuleCollider.center.y + _capsuleCollider.height * 0.5f);
            return !Physics.SphereCast(origin, _capsuleCollider.radius * 0.9f, Vector3.up, out _, clearance, groundLayer);
        }
        #endregion

        #region Ground Check
        protected virtual void CheckGround()
        {
            CheckGroundDistance();
            ControlMaterialPhysics();

            if (groundDistance <= groundMinDistance)
            {
                isGrounded = true;
                if (!isJumping && groundDistance > 0.05f)
                    _rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);

                heightReached = transform.position.y;
            }
            else
            {
                if (groundDistance >= groundMaxDistance)
                {
                    isGrounded = false;
                    verticalVelocity = _rigidbody.linearVelocity.y;
                    if (!isJumping)
                    {
                        _rigidbody.AddForce(transform.up * extraGravity * Time.deltaTime, ForceMode.VelocityChange);
                    }
                }
                else if (!isJumping)
                {
                    _rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);
                }
            }
        }

        protected virtual void ControlMaterialPhysics()
        {
            _capsuleCollider.material = (isGrounded && GroundAngle() <= slopeLimit + 1) ? frictionPhysics : slippyPhysics;

            if (isGrounded && input == Vector3.zero)
                _capsuleCollider.material = maxFrictionPhysics;
            else if (isGrounded && input != Vector3.zero)
                _capsuleCollider.material = frictionPhysics;
            else
                _capsuleCollider.material = slippyPhysics;
        }

        protected virtual void CheckGroundDistance()
        {
            if (_capsuleCollider != null)
            {
                float radius = _capsuleCollider.radius * 0.9f;
                var dist = 10f;
                Ray ray2 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);
                if (Physics.Raycast(ray2, out groundHit, (colliderHeight / 2) + dist, groundLayer) && !groundHit.collider.isTrigger)
                    dist = transform.position.y - groundHit.point.y;

                if (dist >= groundMinDistance)
                {
                    Vector3 pos = transform.position + Vector3.up * (_capsuleCollider.radius);
                    Ray ray = new Ray(pos, -Vector3.up);
                    if (Physics.SphereCast(ray, radius, out groundHit, _capsuleCollider.radius + groundMaxDistance, groundLayer) && !groundHit.collider.isTrigger)
                    {
                        Physics.Linecast(groundHit.point + (Vector3.up * 0.1f), groundHit.point + Vector3.down * 0.15f, out groundHit, groundLayer);
                        float newDist = transform.position.y - groundHit.point.y;
                        if (dist > newDist) dist = newDist;
                    }
                }
                groundDistance = (float)System.Math.Round(dist, 2);
            }
        }

        public virtual float GroundAngle()
        {
            var groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
            return groundAngle;
        }

        public virtual float GroundAngleFromDirection()
        {
            var dir = isStrafing && input.magnitude > 0 ? (transform.right * input.x + transform.forward * input.z).normalized : transform.forward;
            var movementAngle = Vector3.Angle(dir, groundHit.normal) - 90;
            return movementAngle;
        }
        #endregion

        [System.Serializable]
        public class MovementSpeed
        {
            [Range(1f, 20f)]
            public float movementSmooth = 6f;
            [Range(0f, 1f)]
            public float animationSmooth = 0.2f;
            [Tooltip("Character rotation speed")]
            public float rotationSpeed = 16f;
            [Tooltip("Walk instead of run")]
            public bool walkByDefault = false;
            [Tooltip("Rotate with Camera when idle")]
            public bool rotateWithCamera = false;
            [Tooltip("Walk speed")]
            public float walkSpeed = 2f;
            [Tooltip("Run speed")]
            public float runningSpeed = 4f;
            [Tooltip("Sprint speed")]
            public float sprintSpeed = 6f;
        }
    }
}
