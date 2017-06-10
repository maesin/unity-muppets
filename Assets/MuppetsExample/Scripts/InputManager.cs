using Muppets;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Puppeteer Puppeteer;

    bool Down;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Down = true;
            Puppeteer.OnDown(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Down = false;
            Puppeteer.OnUp();
        }
        else if (Down && Input.GetMouseButton(0))
        {
            Puppeteer.OnDrag(Input.mousePosition);
        }
    }
}
