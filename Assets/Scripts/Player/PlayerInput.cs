using UnityEngine;

namespace ZoremGame.Player
{
    public class PlayerInput : MonoBehaviour
    {
        [Header("Controller Input")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode strafeInput = KeyCode.Tab;
        public KeyCode sprintInput = KeyCode.LeftShift;

        [Header("Camera Input")]
        public string rotateCameraXInput = "Mouse X";
        public string rotateCameraYInput = "Mouse Y";

        [HideInInspector] public PlayerController playerController;
        [HideInInspector] public ThirdPersonCamera tpCamera;
        [HideInInspector] public Camera cameraMain;

        protected virtual void Awake()
        {
            // Auto-add PlayerController if missing (before Start)
            if (GetComponent<PlayerController>() == null)
            {
                gameObject.AddComponent<PlayerController>();
            }
        }

        protected virtual void Start()
        {
            InitializeController();
            InitializeCamera();
        }

        protected virtual void FixedUpdate()
        {
            if (playerController == null) return;
            playerController.UpdateMotor();
            playerController.ControlLocomotionType();
            playerController.ControlRotationType();
        }

        protected virtual void Update()
        {
            if (playerController == null) return;
            HandleInput();
            playerController.UpdateAnimator();
        }

        public virtual void OnAnimatorMove()
        {
            if (playerController == null) return;
            playerController.ControlAnimatorRootMotion();
        }

        #region Initialization
        protected virtual void InitializeController()
        {
            if (playerController == null)
            {
                playerController = GetComponent<PlayerController>();
            }

            // Fallback: if still not found, try to add it
            if (playerController == null)
            {
                Debug.LogWarning("PlayerController not found, creating one...");
                playerController = gameObject.AddComponent<PlayerController>();
            }

            if (playerController != null)
            {
                playerController.Init();
            }
        }

        protected virtual void InitializeCamera()
        {
            if (tpCamera == null)
            {
                tpCamera = FindFirstObjectByType<ThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }
        #endregion

        #region Input Handling
        protected virtual void HandleInput()
        {
            MoveInput();
            CameraInput();
            SprintInput();
            StrafeInput();
            JumpInput();
        }

        public virtual void MoveInput()
        {
            playerController.input.x = Input.GetAxis(horizontalInput);
            playerController.input.z = Input.GetAxis(verticallInput);
        }

        protected virtual void CameraInput()
        {
            if (!cameraMain)
            {
                if (!Camera.main) Debug.Log("Missing MainCamera tag. Add one to your main camera.");
                else
                {
                    cameraMain = Camera.main;
                    playerController.rotateTarget = cameraMain.transform;
                }
            }

            if (cameraMain)
            {
                playerController.UpdateMoveDirection(cameraMain.transform);
            }

            if (tpCamera == null)
                return;

            var Y = Input.GetAxis(rotateCameraYInput);
            var X = Input.GetAxis(rotateCameraXInput);

            tpCamera.RotateCamera(X, Y);
        }

        protected virtual void StrafeInput()
        {
            if (Input.GetKeyDown(strafeInput))
                playerController.Strafe();
        }

        protected virtual void SprintInput()
        {
            if (Input.GetKeyDown(sprintInput))
                playerController.Sprint(true);
            else if (Input.GetKeyUp(sprintInput))
                playerController.Sprint(false);
        }

        protected virtual bool JumpConditions()
        {
            return playerController.isGrounded && playerController.GroundAngle() < playerController.slopeLimit && !playerController.isJumping && !playerController.stopMove;
        }

        protected virtual void JumpInput()
        {
            if (Input.GetKeyDown(jumpInput) && JumpConditions())
                playerController.Jump();
        }
        #endregion
    }
}
