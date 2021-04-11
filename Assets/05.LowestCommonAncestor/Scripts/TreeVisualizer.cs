using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LCA
{
    public class TreeVisualizer : MonoBehaviour
    {
        [SerializeField] private NodeVisualReference _reference = null;

        [SerializeField] private GameObject _lineVis = null;
        
        [SerializeField] private Vector3 _rootPosition = Vector3.zero;

        [SerializeField] private float _horizontalOffset = 2.5f;

        [SerializeField] private float _verticalOffset = 2.5f;

        private List<NodeVisualReference> _visualReferences = new List<NodeVisualReference>();
        
        private int _levelOffset = 0;

        
        public void VisualizeTreeRoot(Node root)
        {
            List<Node> nodes = new List<Node>();
            nodes.Add(root);

            _levelOffset = 0;

            while (nodes.Count > 0)
            {
                int n = nodes.Count;

                _levelOffset++;
                
                for (int i = 0; i < n; i++)
                {
                    Node nodeTmp = nodes[0];

                    nodes.Remove(nodeTmp);
                    
                    VisualizeNodeChild(nodeTmp);

                    if (nodeTmp.ChildNodes != null)
                    {
                        foreach (Node childNode in nodeTmp.ChildNodes)
                        {
                            nodes.Add(childNode);
                        }
                    }
                }
            }
        }
        

        private void VisualizeNodeChild(Node node)
        {
            if (node == null)
                return;

            NodeVisualReference referenceNode = _visualReferences.SingleOrDefault(i => i.StandingNode == node);

            if (referenceNode == null)
            {
                GameObject rootInst = Instantiate(_reference.gameObject);
            
                referenceNode = rootInst.GetComponent<NodeVisualReference>();

                referenceNode.InitVisual(node, node.Value);
                
                rootInst.transform.position = _rootPosition;
                
                _visualReferences.Add(referenceNode);
            }

            if (node.ChildNodes != null)
            {
                float centerOffset = (node.ChildNodes.Length - 1) / 2f;

                Vector3 parentPos = referenceNode.transform.position;

                for (int i = 0; i < node.ChildNodes.Length; i++)
                {
                    NodeVisualReference childRef = _visualReferences.SingleOrDefault(refNode => refNode.StandingNode == node.ChildNodes[i]);

                    if (childRef == null)
                    {
                        GameObject inst = Instantiate(_reference.gameObject);
                    
                        childRef = inst.GetComponent<NodeVisualReference>();

                        childRef.InitVisual(node.ChildNodes[i], node.ChildNodes[i].Value);
                        
                        _visualReferences.Add(childRef);

                        inst.transform.position =
                            parentPos + new Vector3((i - centerOffset) * (_horizontalOffset /  (_levelOffset*_levelOffset) + 0.5f ), -_verticalOffset, 0);

                        inst = Instantiate(_lineVis);
                        inst.gameObject.SetActive(true);

                        LineRenderer lr = inst.GetComponent<LineRenderer>();
                        
                        lr.SetPosition(0, parentPos);
                        lr.SetPosition(1, parentPos + new Vector3((i - centerOffset) * (_horizontalOffset / (_levelOffset*_levelOffset)+ 0.5f), -_verticalOffset, 0));
                    }
                }
            }
        }

        public void HighlightNode(int value, Color color)
        {
            NodeVisualReference visref = _visualReferences.SingleOrDefault(i => i.StandingNode.Value == value);

            if (visref != null)
                visref.UpdateBgColor(color);

        }
    }
}
