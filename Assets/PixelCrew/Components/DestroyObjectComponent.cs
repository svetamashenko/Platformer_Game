using UnityEngine;

namespace PixelCrew.Components
{
    public class DestroyObjectComponent : MonoBehaviour
    {
        //[SerializeField] private GameObject _objectToDestroy;
        public void DestroyObject(GameObject _objectToDestroy)
        {
            Destroy(_objectToDestroy);
        }
    }
}
