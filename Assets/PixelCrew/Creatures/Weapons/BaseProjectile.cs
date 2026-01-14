using System.Collections;
using UnityEngine;

namespace Assets.PixelCrew.Creatures.Weapons
{
    public class BaseProjectile : MonoBehaviour
    {
        [SerializeField] protected float speed;
        [SerializeField] protected bool invert;

        protected Rigidbody2D Rigidbody;
        protected int direction;
        protected virtual void Start()
        {
            var mod = invert ? -1 : 1;
            direction = mod * transform.lossyScale.x > 0 ? 1 : -1;
            Rigidbody = GetComponent<Rigidbody2D>();
        }
    }
}