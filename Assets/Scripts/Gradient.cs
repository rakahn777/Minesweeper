using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class Gradient : BaseMeshEffect
{
    public enum Dimension
    {
        X,
        Y,
        Both
    }

    [SerializeField]
    private Color32 topColor = Color.white;
    [SerializeField]
    private Color32 bottomColor = Color.black;
    [SerializeField]
    private Dimension dimension;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        List<UIVertex> list = new List<UIVertex>();
        vh.GetUIVertexStream(list);

        ModifyVertices(list);

        vh.Clear();
        vh.AddUIVertexTriangleStream(list);
    }

    private void ModifyVertices(List<UIVertex> vertexList)
    {
        if (!IsActive() || vertexList.Count == 0)
        {
            return;
        }
        
        int count = vertexList.Count;

        float bottomX = vertexList[0].position.x;
        float topX = vertexList[0].position.x;
		float bottomY = vertexList[0].position.y;
        float topY = vertexList[0].position.y;

        for (int i = 1; i < count; i++)
        {
            float x = vertexList[i].position.x;
			float y = vertexList[i].position.y;
			
            if (x > topX)
            {
                topX = x;
            }
            else if (x < bottomX)
            {
                bottomX = x;
            }

            if (y > topY)
            {
                topY = y;
            }
            else if (y < bottomY)
            {
                bottomY = y;
            }
        }

        float uiElementWidth = topX - bottomX;
        float uiElementHeight = topY - bottomY;
        
        for (int i = 0; i < count; i++)
        {
            UIVertex uiVertex = vertexList[i];
            float currentValue = 
                (dimension != Dimension.X ? (uiVertex.position.y - bottomY) : 0) +
                (dimension != Dimension.Y ? (uiVertex.position.x - bottomX) : 0);
            float maxValue = 
                (dimension != Dimension.X ? uiElementHeight : 0) +
                (dimension != Dimension.Y ? uiElementWidth : 0);
            uiVertex.color = Color32.Lerp(bottomColor, topColor, currentValue / maxValue);
			
            vertexList[i] = uiVertex;
        }
    }
}