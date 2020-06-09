using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 *	
 *  Blend Color Using BaseVertexEffect
 *
 *	by Xuanyi
 *
 */


namespace UiEffect
{
    [AddComponentMenu ("UI/Effects/Blend Color")]
    [RequireComponent (typeof (Graphic))]
    public class BlendColor : BaseMeshEffect
    {
        public enum BLEND_MODE
        {
            Multiply,
            Additive,
            Subtractive,
            Override,
        }

        public BLEND_MODE blendMode = BLEND_MODE.Multiply;
        public Color color = Color.grey;

        Graphic graphic;

        [SerializeField]
        private Color32 topColor = Color.white;
        [SerializeField]
        private Color32 bottomColor = Color.black;

        public override void ModifyMesh(VertexHelper vh)
        {
            if(!this.IsActive())
                return;
            List<UIVertex> vertexList = new List<UIVertex> ();
            vh.GetUIVertexStream(vertexList);

            ModifyVertices (vertexList);

            vh.Clear ();
            vh.AddUIVertexTriangleStream(vertexList);

            int count = vertexList.Count;
            float bottomY = vertexList[0].position.y;
            float topY = vertexList[0].position.y;

            for( int i = 1; i < count; i++ )
            {
                float y = vertexList[i].position.y;
                if( y > topY )
                {
                    topY = y;
                }
                else if( y < bottomY )
                {
                    bottomY = y;
                }
            }

            float uiElementHeight = topY - bottomY;

            for( int i = 0; i < count; i++ )
            {
                UIVertex uiVertex = vertexList[i];
                uiVertex.color = Color32.Lerp( bottomColor, topColor, (uiVertex.position.y - bottomY ) / uiElementHeight );
                vertexList[i] = uiVertex;
            }
        }

        public override void ModifyMesh (Mesh mesh)
        {
            List<UIVertex> vertexList = new List<UIVertex>();
            using (VertexHelper vertexHelper = new VertexHelper(mesh))
            {
                // Move previous VH-related code that you need to keep here
            }
        }
       
        public void ModifyVertices (List<UIVertex> vList)
        {
            if (IsActive () == false || vList == null || vList.Count == 0) {
                return;
            }

            UIVertex tempVertex = vList[0];
            for (int i = 0; i < vList.Count; i++) {
                tempVertex = vList[i];
                byte orgAlpha = tempVertex.color.a;
                switch (blendMode) {
                    case BLEND_MODE.Multiply:
                        tempVertex.color *= color;
                        break;
                    case BLEND_MODE.Additive:
                        tempVertex.color += color;
                        break;
                    case BLEND_MODE.Subtractive:
                        tempVertex.color -= color;
                        break;
                    case BLEND_MODE.Override:
                        tempVertex.color = color;
                        break;
                }
                tempVertex.color.a = orgAlpha;
                vList[i] = tempVertex;
            }
        }

        /// <summary>
        /// Refresh Blend Color on playing.
        /// </summary>
        public void Refresh ()
        {
            if (graphic == null) {
                graphic = GetComponent<Graphic> ();
            }
            if (graphic != null) {
                graphic.SetVerticesDirty ();
            }
        }
    }
}
