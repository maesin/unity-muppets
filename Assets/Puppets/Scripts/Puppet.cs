using UnityEngine;
using UnityEngine.EventSystems;

namespace Puppets
{
    public interface Puppet : IEventSystemHandler
    {
        void OnClick();

        void OnLongDown();

        void OnMove(Vector3 direction);
    }
}
