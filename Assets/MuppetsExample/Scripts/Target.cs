using Muppets;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Target : MonoBehaviour, Puppet
{
    public float Speed = 4;

    Vector3 Direction;

    void Update()
    {
        transform.Translate(Direction * Speed * Time.deltaTime);
    }

    public void OnMove(Vector3 direction)
    {
        Direction = direction;
    }
}
