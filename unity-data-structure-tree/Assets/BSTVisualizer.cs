using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 사용을 위해 필요

public class BSTVisualizer : MonoBehaviour
{
    public float horizontalSpacing = 2.0f;
    public float verticalSpacing = 2.0f;

    public GameObject nodePrefab;
    public GameObject edgePrefab;

    public TMP_InputField nodeInputField;

    private enum DrawMode 
    { 
        Pow,
        LevelOrder,
        InOrder
    }

    private DrawMode currentDrawMode = DrawMode.Pow;

    private BinarySearchTree<int, int> bst;

    private readonly Dictionary<object, Vector3> nodePositions = new();

    private List<GameObject> instantiatedObjects = new();

    private void Start()
    {
        InitializeTree();

        DrawPow();
    }

    private void InitializeTree()
    {
        bst = new BinarySearchTree<int, int>();
        int[] insertKeys = { 50, 30, 70, 20, 40, 60, 80, 10, 35, 65, 90 };
        foreach (int key in insertKeys)
        {
            bst.Add(key, key); 
        }
    }

    public void AddNodeFromUI()
    {
        int maxAttempts = 10; 

        for (int i = 0; i < maxAttempts; i++)
        {

            int randomKey = UnityEngine.Random.Range(1, 100);

            try
            {

                bst.Add(randomKey, randomKey);

                RedrawCurrentTree();

                if (nodeInputField != null)
                {
                    nodeInputField.text = randomKey.ToString();
                }

                Debug.Log($"[추가 성공] 랜덤 노드 {randomKey} 생성 완료.");
                return; 
            }
            catch (System.ArgumentException)
            {
                Debug.Log($"[중복 발생] 키 {randomKey}는 이미 존재합니다. 다시 뽑습니다.");
            }
        }

        Debug.LogWarning("[경고] 랜덤 노드 추가 실패: 중복되지 않는 난수를 찾는 데 실패했습니다.");
    }


    public void RemoveNodeFromUI()
    {
        if (nodeInputField != null && int.TryParse(nodeInputField.text, out int key))
        {
            bool isRemoved = bst.Remove(key);

            if (isRemoved)
            {
                RedrawCurrentTree();
                nodeInputField.text = "";
                Debug.Log($"[삭제 성공] 노드 {key} 삭제 완료.");
            }
            else
            {
                Debug.LogWarning($"[경고] 삭제하려는 키 {key}가 트리에 없습니다.");
            }
        }
    }

    private void RedrawCurrentTree()
    {
        switch (currentDrawMode)
        {
            case DrawMode.Pow: DrawPow(); break;
            case DrawMode.LevelOrder: DrawLevelOrder(); break;
            case DrawMode.InOrder: DrawInOrder(); break;
        }
    }

    public void DrawPow()
    {
        currentDrawMode = DrawMode.Pow; 
        ClearVisualization();
        if (bst.root != null)
        {
            AssignPositionsPow(bst.root, Vector3.zero, bst.root.Height);
            InstantiateSubtree();
        }
    }

    private void AssignPositionsPow<TKey, TValue>(TreeNode<TKey, TValue> node, Vector3 position, int height)
    {
        if (node == null) return;

        nodePositions[node] = position;

        float offset = horizontalSpacing * 0.5f * Mathf.Pow(2, height - 1);

        Vector3 childBase = position + Vector3.down * verticalSpacing;

        AssignPositionsPow(node.Left, childBase + Vector3.left * offset, height - 1);
        AssignPositionsPow(node.Right, childBase + Vector3.right * offset, height - 1);
    }


    public void DrawLevelOrder()
    {
        currentDrawMode = DrawMode.LevelOrder; 
        ClearVisualization();
        if (bst.root != null)
        {
            AssignPositionsLevelOrder(bst.root);
            InstantiateSubtree();
        }
    }

    private void AssignPositionsLevelOrder<TKey, TValue>(TreeNode<TKey, TValue> root)
    {
        var levels = new List<List<TreeNode<TKey, TValue>>>();
        var queue = new Queue<(TreeNode<TKey, TValue> node, int depth)>();

        queue.Enqueue((root, 0));

        while (queue.Count > 0)
        {
            var (node, depth) = queue.Dequeue();

            while (levels.Count <= depth)
            {
                levels.Add(new List<TreeNode<TKey, TValue>>());
            }

            levels[depth].Add(node);

            if (node.Left != null)
            {
                queue.Enqueue((node.Left, depth + 1));
            }
            if (node.Right != null)
            {
                queue.Enqueue((node.Right, depth + 1));
            }
        }

        for (int depth = 0; depth < levels.Count; depth++)
        {
            float y = -depth * verticalSpacing;
            var row = levels[depth];

            for (int i = 0; i < row.Count; i++)
            {
                // [TODO 해결] BFS 탐색 큐에 들어간 순서(i)대로 x좌표를 단순히 증가시키며 할당
                nodePositions[row[i]] = new Vector3(i * horizontalSpacing, y, 0f);
            }
        }
    }

    public void DrawInOrder()
    {
        currentDrawMode = DrawMode.InOrder; // [수정] 상태 저장 추가
        ClearVisualization();
        if (bst.root != null)
        {
            int xIndex = 0;
            AssignPositionsInOrder(bst.root, 0, ref xIndex);
            InstantiateSubtree();
        }
    }

    private void AssignPositionsInOrder<TKey, TValue>(TreeNode<TKey, TValue> node, int depth, ref int xIndex)
    {
        if (node == null)
        {
            return;
        }

        AssignPositionsInOrder(node.Left, depth + 1, ref xIndex);

        nodePositions[node] = new Vector3(xIndex * horizontalSpacing, -depth * verticalSpacing, 0f);
        xIndex++;

        AssignPositionsInOrder(node.Right, depth + 1, ref xIndex);
    }



    private void ClearVisualization()
    {
        foreach (var obj in instantiatedObjects)
        {
            Destroy(obj);
        }
        instantiatedObjects.Clear();
        nodePositions.Clear();
    }

    private void InstantiateSubtree()
    {
        foreach (var kvp in nodePositions)
        {
            var node = kvp.Key as TreeNode<int, int>;
            if (node == null) continue;

            Vector3 pos = kvp.Value;
            GameObject nodeObj = Instantiate(nodePrefab, pos, Quaternion.identity, transform);

            TextMeshPro[] textMeshes = nodeObj.GetComponentsInChildren<TextMeshPro>();

            textMeshes[0].text = node.Key.ToString();
            textMeshes[1].text = node.Value.ToString();
            textMeshes[2].text = node.Height.ToString();

            instantiatedObjects.Add(nodeObj);

            DrawEdge(pos, node.Left);
            DrawEdge(pos, node.Right);
        }
    }

    private void DrawEdge<TKey, TValue>(Vector3 parentPos, TreeNode<TKey, TValue> childNode)
    {
        if (childNode != null && nodePositions.TryGetValue(childNode, out Vector3 childPos))
        {
            GameObject edgeObj = Instantiate(edgePrefab, Vector3.zero, Quaternion.identity, transform);
            LineRenderer lr = edgeObj.GetComponent<LineRenderer>();

            if (lr != null)
            {
                lr.positionCount = 2;
                lr.SetPosition(0, parentPos);
                lr.SetPosition(1, childPos);
            }
            instantiatedObjects.Add(edgeObj);
        }
    }

}