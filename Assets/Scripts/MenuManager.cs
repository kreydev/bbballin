using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Transform menulight;
    readonly float sens = .1f;
    
    void FixedUpdate()
    {
        Vector2 mouse = Input.mousePosition;
        menulight.localEulerAngles = new((mouse.y + 1500) * sens / 10, -(mouse.x + 1500) * sens / 10);
    }
}
