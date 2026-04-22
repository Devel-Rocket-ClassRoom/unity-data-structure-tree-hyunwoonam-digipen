using TMPro;
using UnityEngine;

public class NodeDisplay : MonoBehaviour
{
    public TextMeshPro keyText;
    public TextMeshPro valueText;
    public TextMeshPro heightText;

    public void SetData(string k, string v, int h)
    {
        keyText.text = $"K: {k}";
        valueText.text = $"V: {v}";
        heightText.text = $"H: {h}";
    }
}
