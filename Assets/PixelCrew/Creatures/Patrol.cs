using System.Collections;
using UnityEngine;

namespace Assets.PixelCrew.Creatures
{
    public abstract class Patrol : MonoBehaviour
    {
        public abstract IEnumerator DoPatrol();
    }
}