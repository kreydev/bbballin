using System.Collections;
using Mirror;
using UnityEngine;

public class IngameGUI : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text cointextbox;
    [SerializeField] TMPro.TMP_Text statustextbox;
    [SerializeField] GameObject[] icons;

    public static IngameGUI Singleton { get {return FindFirstObjectByType<IngameGUI>();} }
    public string CoinText { set { cointextbox.text = value; } get { return cointextbox.text; } }
    public string StatusText { set { statustextbox.text = value; } get { return statustextbox.text; } }

    void Start()
    {
        StartCoroutine(WaitForNetwork());
    }
    
    IEnumerator WaitForNetwork()
    {
        foreach (var i in icons) { i.SetActive(false); }
        var wfs = new WaitForSeconds(0.25f);
        while (NetworkManager.singleton.numPlayers < 1) {
            yield return wfs;
        }
        foreach (var i in icons) { i.SetActive(true); }
    }
}
