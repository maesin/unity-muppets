using UnityEngine;
using UnityEngine.EventSystems;

namespace Puppets
{
    public interface Puppet : IEventSystemHandler
    {
        void OnClick(Vector3 position);

        void OnLongDown();

        void OnMove(Vector3 direction);
    }
}
