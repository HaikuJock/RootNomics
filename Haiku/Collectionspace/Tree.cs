using System.Collections.Generic;

namespace Haiku.Collectionspace
{
    // Each node in the tree is a Tree<K, V> object.
    // Each node has a sorted list of children, which are also Tree<K, V> objects.
    public class Tree<K, V> : SortedDictionary<K, Tree<K, V>>
    {
        // Each node has a Body property, which is of type V.
        public V Body { get; set; }

        // Children are sorted by key.
        public Tree(IComparer<K> comparer) 
            : base(comparer)
        {
        }

        public Tree()
        {
        }
    }
}
