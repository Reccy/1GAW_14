using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    [Header("Cursor Setup")]
    [SerializeField] List<Texture2D> m_cursorImages;
    [SerializeField] Vector2 m_hotSpot;

    private int Max => m_cursorImages.Count;
    private int idx = 0;

    private void Awake()
    {
        SetCursorImage(m_cursorImages[idx]);
    }

    private void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        if (++idx >= Max)
            idx = 0;

        //SetCursorImage(m_cursorImages[idx]);
    }

    private void SetCursorImage(Texture2D texture)
    {
        Cursor.SetCursor(texture, m_hotSpot, CursorMode.Auto);
    }
}
