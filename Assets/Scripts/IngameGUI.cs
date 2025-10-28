using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class IngameGUI : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text cointextbox;
    [SerializeField] TMPro.TMP_Text statustextbox;
    [SerializeField] GameObject[] icons;
    public GameObject[] statusIcons;

    public void SetIconState(PType type, bool enabled, float time)
    {
        statusIcons[(int)type - 1].SetActive(enabled);
        if (enabled) { statusIcons[(int)type - 1].GetComponentInChildren<Image>().fillAmount = time / 20; }
    }

    public static IngameGUI Singleton { get { return FindFirstObjectByType<IngameGUI>(); } }
    public string CoinText { set { cointextbox.text = value; } get { return cointextbox.text; } }
    public string StatusText { set { statustextbox.text = value; } get { return statustextbox.text; } }
    bool showIcons = false;
    public bool ShowIcons { get { return showIcons; } set { showIcons = value; foreach (var i in icons) { i.SetActive(value); } } }

    void Start() {
        ShowIcons = false;
        for (int i = 0; i < statusIcons.Length; ++i)
        {
            SetIconState((PType)(i + 1), false, 0);
        }
    }
}
