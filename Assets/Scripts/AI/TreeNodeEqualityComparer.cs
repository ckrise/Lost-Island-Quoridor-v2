using System.Collections.Generic;

//Equality comparer used to compare tree nodes for storage in hashset.
class TreeNodeEqualityComparer : IEqualityComparer<TreeNode>
{
    public bool Equals(TreeNode x, TreeNode y)
    {
        return x.Equals(y);
    }

    public int GetHashCode(TreeNode obj)
    {
        return obj.GetHashCode();
    }
}

