using UnityEngine;

public class SafeArea : MonoBehaviour
{
    [SerializeField] private RectTransform rectTfmTopPanel;

    private void Awake()
    {
        Rect safeArea = Screen.safeArea;
        Vector2 minAnchor = rectTfmTopPanel.anchorMin;
        Vector2 maxAnchor = rectTfmTopPanel.anchorMax;

        minAnchor.x = 1 - safeArea.width / Screen.width;
        minAnchor.y = safeArea.height / Screen.height;
        maxAnchor.x = safeArea.width / Screen.width;
        maxAnchor.y = safeArea.height / Screen.height;

        rectTfmTopPanel.anchorMax = maxAnchor;
        rectTfmTopPanel.anchorMin = minAnchor;
    }
}
