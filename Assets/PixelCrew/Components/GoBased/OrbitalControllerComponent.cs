using UnityEngine;

namespace PixelCrew.Components.GoBased
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
        private int initialChildCount;

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
            int currentChildCount = transform.childCount;

            if (initialChildCount == 0)
                initialChildCount = currentChildCount;

            children = new Transform[initialChildCount];
            angleStep = 360f / initialChildCount;

            for (int i = 0; i < currentChildCount; i++)
                children[i] = transform.GetChild(i);

            PositionChildrenOnCircle();
        }

        private void CheckForEditorChanges()
        {
            if (children == null)
                InitializeChildren();
            else
                PositionChildrenOnCircle();
        }

        private void PositionChildrenOnCircle()
        {
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i] == null) continue;

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
                // Пропускаем удалённые объекты
                if (children[i] == null) continue;

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
