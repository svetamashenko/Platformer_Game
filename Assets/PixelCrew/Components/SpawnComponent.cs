using TMPro;
using UnityEngine;

namespace PixelCrew.Components
{
    public class SpawnComponent : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private GameObject _prefab;

        [ContextMenu("Spawn")]
        public GameObject Spawn()
        {
            var instantiate = Instantiate(_prefab, _target.position, Quaternion.identity);
            instantiate.transform.localScale = _target.lossyScale;
            return instantiate;
        }
        public void SpawnObject()
        {
            var instantiate = Instantiate(_prefab, _target.position, Quaternion.identity);
            instantiate.transform.localScale = _target.lossyScale;
        }
    }
}
