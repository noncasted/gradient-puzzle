using Internal;
using UnityEngine;

namespace GamePlay.Paints.GameObjects
{
    public class PaintGameObject : MonoBehaviour, IEntityComponent, IPaintGameObject
    {
        public void Register(IEntityBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IPaintGameObject>();
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}