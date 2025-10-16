using System.Collections;
using Mirror;
using UnityEngine;

public class IngameGUI : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text cointextbox;
    [SerializeField] TMPro.TMP_Text statustextbox;
    [SerializeField] GameObject[] icons;

    public static IngameGUI Singleton { get { return FindFirstObjectByType<IngameGUI>(); } }
    public string CoinText { set { cointextbox.text = value; } get { return cointextbox.text; } }
    public string StatusText { set { statustextbox.text = value; } get { return statustextbox.text; } }
    bool showIcons = false;
    public bool ShowIcons { get { return showIcons; } set { showIcons = value; foreach (var i in icons) { i.SetActive(value); } } }

    void Start() { ShowIcons = false; }
}
