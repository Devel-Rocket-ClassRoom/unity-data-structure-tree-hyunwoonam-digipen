using System;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<TElement, TPriority> 
{
    public int Count => heap.Count;

    List<(TElement Element, TPriority Priority)> heap;

    Comparer<TPriority> defComp;

    public PriorityQueue()
    {
        heap = new List<(TElement Element, TPriority Priority)>();
        defComp = Comparer<TPriority>.Default;
    }

    public void Enqueue(TElement element, TPriority priority)
    {
        heap.Add((element, priority));

        HeapifyUp(heap.Count -1);

        for (int i = heap.Count - 1; i > 1; i--)
        {
            if (defComp.Compare(heap[i].Priority, heap[(i - 1) / 2].Priority) < 0)
            {
                var temp = heap[i];
                heap[i] = heap[(i - 1) / 2];
                heap[(i - 1) / 2] = temp;
            }
            else
            {
                break;
            }
        }
    }

    private void HeapifyUp(int index)
    {
        while ( index > 0)
        {
            int parentIndex = (index - 1) / 2;

            if (defComp.Compare(heap[index].Priority, heap[parentIndex].Priority) >= 0)
            {
                break;
            }

            Swap(index, parentIndex);

            index = parentIndex;
        }
    }
    public TElement Dequeue()
    {
        if(heap.Count == 0)
        {
            throw new InvalidOperationException("큐가 비어있습니다.");
        }

        var root = heap[0].Element;

        int lastIndex = heap.Count - 1;

        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);

        if (heap.Count > 0)
        {
            HeapifyDown(0);
        }

        return root;
    }

    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;

        while(true)
        {
            int leftchildindex = 2 * index + 1;
            int rightchildindex = 2 * index + 2;
            int smallestindex = index;

            
            if (leftchildindex <= lastIndex && defComp.Compare(heap[leftchildindex].Priority, heap[smallestindex].Priority) < 0)
            {
                smallestindex = leftchildindex;
            }

            if (rightchildindex <= lastIndex && defComp.Compare(heap[rightchildindex].Priority, heap[smallestindex].Priority) < 0)
            {
                smallestindex = rightchildindex;
            }

            if (smallestindex == index)
            {
                break;
            }

            Swap(index, smallestindex);

            index = smallestindex;
        }
    }

    private void Swap(int index1, int index2)
    {
        var temp = heap[index1];
        heap[index1] = heap[index2];
        heap[index2] = temp;
    }
    public TElement Peek()
    {
        if (heap.Count == 0)
        {
            throw new InvalidOperationException("큐가 비어있습니다.");
        }
        return heap[0].Element;
    }
    public void Clear()
    {
        heap.Clear();
    }

}
