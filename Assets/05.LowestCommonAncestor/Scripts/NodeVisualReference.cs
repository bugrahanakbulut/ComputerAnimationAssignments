using TMPro;
using UnityEngine;

namespace LCA
{
    public class NodeVisualReference : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _valueText = null;

        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        
        public Node StandingNode { get; private set; }
        
        public void InitVisual(Node node, int val)
        {
            StandingNode = node;
            
            _valueText.SetText(val.ToString());
        }

        public void UpdateBgColor(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}
