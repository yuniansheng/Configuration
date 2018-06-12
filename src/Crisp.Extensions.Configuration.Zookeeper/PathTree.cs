using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crisp.Extensions.Configuration.Zookeeper
{
    internal class PathTree
    {
        public TreeNode Root { get; set; }

        public PathTree()
        {
            Root = new TreeNode(string.Empty);
        }

        public TreeNode AddNode(string key, TreeNode parent)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            var node = new TreeNode(key) { Parent = parent };
            parent.Children.Add(node);
            return node;
        }

        public bool RemoveNode(string path)
        {
            var node = FindNode(path);
            if (node == null)
            {
                return false;
            }

            node.Parent.Children.Remove(node);
            return true;
        }

        public TreeNode FindNode(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (!path.StartsWith("/"))
            {
                throw new ArgumentException("path must starts with /", nameof(path));
            }

            if (path == "/") return Root;

            var segments = path.Trim('/').Split('/');
            var layerIndex = 0;

            var current = Root;
            while (current != null && layerIndex < segments.Length)
            {
                current = current.Children.FirstOrDefault(item => item.Key == segments[layerIndex]);
                layerIndex++;
            }

            return current;
        }

        public class TreeNode
        {
            public string Key { get; set; }

            public TreeNode Parent { get; set; }

            public List<TreeNode> Children { get; set; }

            public TreeNode(string key)
            {
                Children = new List<TreeNode>();
            }
        }
    }
}
