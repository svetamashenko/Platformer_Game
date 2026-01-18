using UnityEngine;

namespace PixelCrew.Components
{
    [RequireComponent(typeof(Transform))]
    [ExecuteInEditMode]
    public class OrbitalControllerComponent : MonoBehaviour
    {
        [Header("Параметры орбиты")]
        [SerializeField] private float radius = 5f;
        [SerializeField] private float speed = 2f;

        private Transform[] children;
        private float angleStep;

        private void Awake()
        {
            InitializeChildren();
        }

        private void Update()
        {
            if (Application.isPlaying)
                RotateChildren();
            else
                CheckForEditorChanges();
        }

        private void InitializeChildren()
        {
            int childCount = transform.childCount;
            children = new Transform[childCount];
            angleStep = 360f / childCount;

            for (int i = 0; i < childCount; i++)
                children[i] = transform.GetChild(i);

            PositionChildrenOnCircle();
        }

        private void CheckForEditorChanges()
        {
            if (children == null || children.Length != transform.childCount)
                InitializeChildren();
            else
                PositionChildrenOnCircle();
        }

        private void PositionChildrenOnCircle()
        {
            for (int i = 0; i < children.Length; i++)
            {
                float angleRadians = (i * angleStep) * Mathf.Deg2Rad;
                float x = Mathf.Cos(angleRadians) * radius;
                float y = Mathf.Sin(angleRadians) * radius;

                children[i].localPosition = new Vector3(x, y, 0f);
            }
        }

        private void RotateChildren()
        {
            float circumference = 2f * Mathf.PI * radius;
            float anglePerSecond = (speed / circumference) * 360f;
            float deltaAngle = anglePerSecond * Time.deltaTime;

            Quaternion rotation = Quaternion.AngleAxis(deltaAngle, Vector3.forward);

            for (int i = 0; i < children.Length; i++)
            {
                Vector3 direction = children[i].localPosition;
                direction = rotation * direction;
                children[i].localPosition = direction;
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
                CheckForEditorChanges();
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
                CheckForEditorChanges();
        }
    }
}
