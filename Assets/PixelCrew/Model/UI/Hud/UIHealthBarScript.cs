using UnityEngine;
using UnityEngine.UI;
using PixelCrew.Components.Health;

namespace PixelCrew.Model.UI.Hud
{
    public class UIHealthBarScript : MonoBehaviour
    {
        [SerializeField] private GameObject uiPrefab;
        [SerializeField, Tooltip("Смещение UI относительно позиции монстра")]
        private float verticalOffset = 0.5f;

        private GameObject instanceUi;
        private Canvas canvas;
        private HealthComponent _healthComponent;
        private EnemyHealthBarController _barController;

        private void Start()
        {
            _healthComponent = GetComponent<HealthComponent>();
            if (_healthComponent == null)
            {
                Debug.LogError("HealthComponent не найден на объекте!");
                return;
            }

            InitializeCanvas();
            InstanceHealthBar();
        }

        private void LateUpdate()
        {
            if (instanceUi != null && canvas != null)
            {
                PositionHealthBarAboveMonster();
            }
        }

        private void InitializeCanvas()
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas foundCanvas in canvases)
            {
                if (foundCanvas.renderMode == RenderMode.WorldSpace)
                {
                    canvas = foundCanvas;
                    return;
                }
            }

            GameObject canvasGO = new GameObject("World Space Canvas");
            canvasGO.AddComponent<Canvas>();
            canvasGO.AddComponent<GraphicRaycaster>();

            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            canvas.sortingLayerName = "GameLayer";
            canvas.sortingOrder = 20;

            RectTransform rt = canvasGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(100, 100);
            canvasGO.transform.position = Vector3.zero;
        }

        private void InstanceHealthBar()
        {
            if (uiPrefab == null)
            {
                Debug.LogError("UI Prefab не назначен для Health Bar!");
                return;
            }

            instanceUi = Instantiate(uiPrefab, canvas.transform);

            RectTransform uiRectTransform = instanceUi.GetComponent<RectTransform>();
            if (uiRectTransform == null)
            {
                uiRectTransform = instanceUi.AddComponent<RectTransform>();
            }

            uiRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            uiRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            uiRectTransform.pivot = new Vector2(0.5f, 0.5f);
            uiRectTransform.anchoredPosition = Vector2.zero;
            uiRectTransform.sizeDelta = new Vector2(100, 20);

            _barController = instanceUi.AddComponent<EnemyHealthBarController>();
            _barController.SetHealthComponent(_healthComponent);

            instanceUi.SetActive(true);
        }

        private void PositionHealthBarAboveMonster()
        {
            Vector3 monsterPosition = transform.position;
            float monsterHeight = GetMonsterHeight();

            Vector3 uiPosition = monsterPosition + new Vector3(0, monsterHeight + verticalOffset, 0);
            instanceUi.transform.position = uiPosition;
        }

        private float GetMonsterHeight()
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                return collider.bounds.extents.y;
            }

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                return renderer.bounds.extents.y;
            }

            return 1.0f;
        }

        private void OnDestroy()
        {
            if (instanceUi != null)
            {
                Destroy(instanceUi);
            }
        }
    }
}