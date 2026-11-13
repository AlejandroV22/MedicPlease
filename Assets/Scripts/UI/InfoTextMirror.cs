using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Copia el texto de los TMP del panel que funciona a los TMP de la sección nueva.
public class InfoTextMirror : MonoBehaviour
{
    [Header("Origen (InformationPanel, el que funciona)")]
    public TextMeshProUGUI sourceInterrogation;   // ContentInterrogation
    public TextMeshProUGUI sourceExamination;     // ContentExamination

    [Header("Destino (EnfermedadActualSection)")]
    public TextMeshProUGUI targetInterrogation;   // InterrogationInfoText
    public TextMeshProUGUI targetExamination;     // ExaminationInfoText

    [Header("Scroll (opcional)")]
    public ScrollRect scrollRect;
    public RectTransform contentRect;

    private void LateUpdate()
    {
        bool changed = false;
        changed |= CopyText(sourceInterrogation, targetInterrogation);
        changed |= CopyText(sourceExamination, targetExamination);

        if (changed)
            RefreshLayout();
    }

    private bool CopyText(TextMeshProUGUI source, TextMeshProUGUI target)
    {
        if (source == null || target == null) return false;
        if (target.text == source.text) return false;

        target.text = source.text;
        return true;
    }

    private void RefreshLayout()
    {
        Canvas.ForceUpdateCanvases();

        if (contentRect != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 1f;
    }
}