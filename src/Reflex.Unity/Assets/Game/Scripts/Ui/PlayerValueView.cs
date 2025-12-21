using TMPro;
using UnityEngine;

public sealed class PlayerValueView : MonoBehaviour
{
    public TMP_Text Text;
    public Vector3 LocalOffset = new(0f, 1f, 0f);

    private void Awake()
    {
        if (Text == null)
            Text = GetComponent<TMP_Text>();
    }

    private void LateUpdate()
    {
        // Because this object is a child of Player, localPosition is what you want.
        transform.SetLocalPositionAndRotation(LocalOffset, Quaternion.identity);
    }

    public void Render(int value)
    {
        if (Text != null)
            Text.text = value.ToString();
    }
}
