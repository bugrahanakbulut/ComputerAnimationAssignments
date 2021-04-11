using System.Collections.Generic;
using UnityEngine;

namespace LCA
{
    public class Node
    {
        public int Value { get; }

        public Node[] ChildNodes { get; }

        public Node(int value, Node[] childNodes)
        {
            Value = value;
            ChildNodes = childNodes;
        }
    }

    public class Tree : MonoBehaviour
    {
        [SerializeField] private TreeVisualizer _treeVisualizer = null;

        [SerializeField] private int _value1 = 4, _value2 = 7;

        [SerializeField] private bool _secTree = false;
        
        private Node _rootNode;

        private void Awake()
        {
            
            if (!_secTree)
                InitRootNode();
            else
                InitRootNode_1();
            
            _treeVisualizer.VisualizeTreeRoot(_rootNode);

            _treeVisualizer.HighlightNode(_value1, Color.green);
            _treeVisualizer.HighlightNode(_value2, Color.green);


            Node lca = FindLowestCommonAncestor(_rootNode, _value1, _value2);
            
            if (lca != null)
                _treeVisualizer.HighlightNode(lca.Value, Color.red);
        }

        private void InitRootNode()
        {
            Node firstChild = new Node(
                2,
                new Node[]
                {
                    new Node(4, null),
                    new Node(5, null),
                    new Node(6, null),
                });

            Node secondChild = new Node(
                3,
                new Node[]
                {
                    new Node(7, null),
                    new Node(8, null)
                });

            _rootNode = new Node(1, new Node[] {firstChild, secondChild});
        }

        private void InitRootNode_1()
        {
            Node firstChild = new Node(
                2,
                new Node[]
                {
                    new Node(4, null),
                    new Node(5, new Node[]
                    {
                        new Node(6, null),
                        new Node(7, null),
                    }),
                    new Node(8, null),
                });

            Node secondChild = new Node(
                3,
                new Node[]
                {
                    new Node(9, null),
                    new Node(10, null),
                    new Node(11, null)
                });

            _rootNode = new Node(1, new Node[] {firstChild, secondChild});
        }

        private Node FindLowestCommonAncestor(Node rootNode, int value1, int value2)
        {
            List<Node> path1 = new List<Node>();
            List<Node> path2 = new List<Node>();

            FindPathRootToNode(rootNode, value1, path1);
            FindPathRootToNode(rootNode, value2, path2);

            while (path1.Count > path2.Count)
                path2.Add(null);
            while (path2.Count > path1.Count)
                path1.Add(null);

            for (int i = 0; i < path1.Count && i < path2.Count; i++)
            {
                if (path1[i] != path2[i])
                {
                    Debug.Log(path1[i - 1].Value);
                    
                    return path1[i - 1];
                }
            }

            return null;
        }

        private bool FindPathRootToNode(Node root, int val, List<Node> path)
        {
            path.Add(root);

            if (root.Value == val)
            {
                return true;
            }

            if (root.ChildNodes != null)
            {
                foreach (Node node in root.ChildNodes)
                {
                    bool search = FindPathRootToNode(node, val, path);

                    if (search)
                        return true;
                }
            }

            path.Remove(root);

            return false;
        }
    }

}