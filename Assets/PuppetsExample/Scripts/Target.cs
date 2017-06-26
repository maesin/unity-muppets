using Puppets;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Target : MonoBehaviour, Puppet
{
    public float Speed = 4;

    public Material Clicked;

    public Material LongDowned;

    Renderer Renderer;

    Material Default;

    Vector3 Direction;

    void Start()
    {
        Renderer = GetComponent<Renderer>();
        Default = Renderer.material;
    }

    void Update()
    {
        transform.Translate(Direction * Speed * Time.deltaTime);
    }

    public void OnMove(Vector3 direction)
    {
        Direction = direction;
    }

    public void OnClick()
    {
        if (Renderer.material == Default)
        {
            Renderer.material = Clicked;
        }
        else
        {
            Renderer.material = Default;
        }
    }

    public void OnLongDown()
    {
        Renderer.material = LongDowned;
    }
}
