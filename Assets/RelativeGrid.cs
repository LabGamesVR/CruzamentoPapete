using System.Collections;
using System.Collections.Generic;
using TMPro;
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
                item.sizeDelta =  new Vector2(Mathf.Abs(rt.sizeDelta.x),Mathf.Abs(rt.sizeDelta.y)) *cellSize;
                item.anchoredPosition = ocup * tamaho;
                
                ocup.x += cellSize.x;
                if(ocup.x >= 1f)
                {
                    ocup.x = 0f;
                    ocup.y -= cellSize.y;
                }
            }
        }
    }
}
