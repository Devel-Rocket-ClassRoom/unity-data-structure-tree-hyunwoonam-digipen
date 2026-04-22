using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreeVisualize : MonoBehaviour
{
    public enum LayoutMode 
    { 
        Pow,
        LevelOrder,
        InOrder 
    }

    public LayoutMode currentMode = LayoutMode.Pow;
    public float xSpacing = 2f;
    public float ySpacing = 2f;

    public GameObject nodePrefab;
    public GameObject edgePrefab;

    private List<GameObject> spawnedObjects = new List<GameObject>();


    public void RenderTree<TKey, TValue>(BinarySearchTree<TKey, TValue> bst) where TKey : System.IComparable<TKey>
    {
        ClearTree();

        if (bst.root == null) 
            return;

        Dictionary<TreeNode<TKey, TValue>, Vector2> positions;

        switch (currentMode)
        {
            case LayoutMode.Pow:
                float initialXOffset = Mathf.Pow(2, bst.root.Height - 2) * xSpacing;


                break;
            case LayoutMode.LevelOrder:

                break;
            case LayoutMode.InOrder:

                break;
        }

        //DrawNodesAndEdges(positions);
    }

    //private void DrawNodesAndEdges<TKey, TValue>(Dictionary<TreeNode<TKey, TValue>, Vector2> positions)
    //{
    //    foreach (var kvp in positions)
    //    {
    //        var node = kvp.Key;
    //        var pos = kvp.Value;
    //
    //        GameObject nodeObj = Instantiate(nodePrefab, pos, Quaternion.identity, transform);
    //        spawnedObjects.Add(nodeObj);
    //
    //        var display = nodeObj.GetComponent<NodeDisplay>();
    //
    //        if (display != null)
    //        {
    //            display.SetData(node.Key.ToString(), node.Value.ToString(), node.Height);
    //        }
    //
    //        if (node.Left != null && positions.TryGetValue(node.Left, out Vector2 leftPos))
    //        {
    //            CreateEdge(pos, leftPos);
    //        }
    //        if (node.Right != null && positions.TryGetValue(node.Right, out Vector2 rightPos))
    //        {
    //            CreateEdge(pos, rightPos);
    //        }
    //            
    //    }
    //}

    private void CreateEdge(Vector2 start, Vector2 end)
    {
        GameObject edgeObj = Instantiate(edgePrefab, transform);
        spawnedObjects.Add(edgeObj);
        LineRenderer lr = edgeObj.GetComponent<LineRenderer>();
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    private void ClearTree()
    {
        foreach (var obj in spawnedObjects)
        {
            Destroy(obj);
        }

        spawnedObjects.Clear();
    }
}