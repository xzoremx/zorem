using UnityEngine;

namespace ZoremGame.Player
{
    public class PlayerAnimator : PlayerMotor
    {
        #region Animator Parameters
        private int hashInputHorizontal;
        private int hashInputVertical;
        private int hashInputMagnitude;
        private int hashIsGrounded;
        private int hashIsStrafing;
        private int hashIsSprinting;
        private int hashGroundDistance;
        #endregion

        #region Animation Speeds
        public const float walkSpeed = 0.5f;
        public const float runningSpeed = 1f;
        public const float sprintSpeed = 1.5f;
        #endregion

        protected virtual void OnEnable()
        {
            hashInputHorizontal = Animator.StringToHash("InputHorizontal");
            hashInputVertical = Animator.StringToHash("InputVertical");
            hashInputMagnitude = Animator.StringToHash("InputMagnitude");
            hashIsGrounded = Animator.StringToHash("IsGrounded");
            hashIsStrafing = Animator.StringToHash("IsStrafing");
            hashIsSprinting = Animator.StringToHash("IsSprinting");
            hashGroundDistance = Animator.StringToHash("GroundDistance");
        }

        public virtual void UpdateAnimator()
        {
            if (animator == null || !animator.enabled) return;

            animator.SetBool(hashIsStrafing, isStrafing);
            animator.SetBool(hashIsSprinting, isSprinting);
            animator.SetBool(hashIsGrounded, isGrounded);
            animator.SetFloat(hashGroundDistance, groundDistance);

            if (isStrafing)
            {
                animator.SetFloat(hashInputHorizontal, stopMove ? 0 : horizontalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
                animator.SetFloat(hashInputVertical, stopMove ? 0 : verticalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
            }
            else
            {
                animator.SetFloat(hashInputVertical, stopMove ? 0 : verticalSpeed, freeSpeed.animationSmooth, Time.deltaTime);
            }

            animator.SetFloat(hashInputMagnitude, stopMove ? 0f : inputMagnitude, isStrafing ? strafeSpeed.animationSmooth : freeSpeed.animationSmooth, Time.deltaTime);
        }

        public virtual void SetAnimatorMoveSpeed(MovementSpeed speed)
        {
            Vector3 relativeInput = transform.InverseTransformDirection(moveDirection);
            verticalSpeed = relativeInput.z;
            horizontalSpeed = relativeInput.x;

            var newInput = new Vector2(verticalSpeed, horizontalSpeed);

            if (speed.walkByDefault)
                inputMagnitude = Mathf.Clamp(newInput.magnitude, 0, isSprinting ? runningSpeed : walkSpeed);
            else
                inputMagnitude = Mathf.Clamp(isSprinting ? newInput.magnitude + 0.5f : newInput.magnitude, 0, isSprinting ? sprintSpeed : runningSpeed);
        }
    }
}
