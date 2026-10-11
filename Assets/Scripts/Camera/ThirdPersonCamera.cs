using UnityEngine;

namespace ZoremGame.Player
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        #region Inspector
        public Transform target;
        [Tooltip("Lerp speed for camera rotation")]
        public float smoothCameraRotation = 12f;
        [Tooltip("Layers to check collision against")]
        public LayerMask cullingLayer = 1 << 0;

        public float rightOffset = 0f;
        public float defaultDistance = 2.5f;
        public float height = 1.4f;
        public float smoothFollow = 10f;
        public float xMouseSensitivity = 3f;
        public float yMouseSensitivity = 3f;
        public float yMinLimit = -40f;
        public float yMaxLimit = 80f;
        #endregion

        #region Private Variables
        [HideInInspector] public Vector2 movementSpeed;

        private Vector3 currentTargetPos;
        private float distance = 5f;
        private float mouseY = 0f;
        private float mouseX = 0f;
        private float xMinLimit = -360f;
        private float xMaxLimit = 360f;
        #endregion

        void Start()
        {
            Init();
        }

        public void Init()
        {
            if (target == null)
                return;

            mouseY = 0f;
            mouseX = 0f;
            distance = defaultDistance;
        }

        void LateUpdate()
        {
            if (target == null) return;
            CameraMovement();
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void SetMainTarget(Transform newTarget)
        {
            target = newTarget;
            Init();
        }

        public Ray ScreenPointToRay(Vector3 point)
        {
            return GetComponent<Camera>().ScreenPointToRay(point);
        }

        public void RotateCamera(float x, float y)
        {
            mouseX += x * xMouseSensitivity;
            mouseY -= y * yMouseSensitivity;

            mouseY = ClampAngle(mouseY, yMinLimit, yMaxLimit);
            mouseX = ClampAngle(mouseX, xMinLimit, xMaxLimit);
        }

        void CameraMovement()
        {
            if (target == null) return;

            distance = Mathf.Lerp(distance, defaultDistance, smoothFollow * Time.deltaTime);

            // Calcula la posición deseada
            currentTargetPos = target.position + Vector3.up * height;
            Vector3 desiredPosition = currentTargetPos - (Quaternion.Euler(mouseY, mouseX, 0) * Vector3.forward) * distance;

            // Raycast para collision avoidance
            RaycastHit hit;
            float rayDistance = Vector3.Distance(transform.position, desiredPosition);

            if (Physics.Raycast(currentTargetPos, (desiredPosition - currentTargetPos).normalized, out hit, rayDistance, cullingLayer))
            {
                desiredPosition = hit.point + (currentTargetPos - desiredPosition).normalized * 0.5f;
            }

            // Interpola la cámara suavemente
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothCameraRotation * Time.deltaTime);

            // Rota la cámara para mirar al personaje
            transform.LookAt(currentTargetPos);
        }

        static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
