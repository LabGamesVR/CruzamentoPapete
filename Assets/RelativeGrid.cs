using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RelativeGrid : MonoBehaviour
{
    public Vector2 cellSize = new Vector2(0.5f, 0.5f);

    RectTransform rt;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }
    private void Update()
    {
        Vector2 tamaho = new Vector2(rt.rect.width, rt.rect.height);

        Vector2 ocup = new Vector2();
        RectTransform[] rts = transform.GetComponentsInChildren<RectTransform>();
        
        foreach (var item in rts)
        {
            if(item != rt)
            {
                item.sizeDelta = rt.sizeDelta.Abs() * cellSize;
                ocup.x += cellSize.x;
                if(ocup.x > 1f)
                {
                    ocup.x = 0f;
                    ocup.y += cellSize.y;
                }
                item.anchoredPosition = ocup * tamaho - tamaho*0.5f;

            }
        }
    }
}
