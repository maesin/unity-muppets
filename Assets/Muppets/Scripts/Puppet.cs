using UnityEngine;
using UnityEngine.EventSystems;

namespace Muppets
{
    public interface Puppet : IEventSystemHandler
    {
        void OnMove(Vector3 direction);
    }
}
