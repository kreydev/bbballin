using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using NUnit.Framework.Constraints;
using System.Threading.Tasks;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Transform menulight;
    [SerializeField] RawImage loadingIcon;
    readonly float sens = .1f;
    Rect loc = new Rect(0f,0f,1f,0.0384615399f);

    void FixedUpdate()
    {
        Vector2 mouse = Input.mousePosition;
        menulight.localEulerAngles = new((mouse.y + 1500) * sens / 10, -(mouse.x + 1500) * sens / 10);
        loc.y += 0.0384615399f;
        loadingIcon.uvRect = loc;
    }


    public void StartGame()
    {
        loadingIcon.gameObject.SetActive(true);
        SceneManager.LoadScene("MainScene");
    }
}
