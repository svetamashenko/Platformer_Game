using Assets.PixelCrew.Creatures.Weapons;
using UnityEngine;

namespace PixelCrew.Components.Weapon
{
    public class Projectile : BaseProjectile
    {
        protected override void Start()
        {
            base.Start();

            var force = new Vector2(direction * speed, 0);
            Rigidbody.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
