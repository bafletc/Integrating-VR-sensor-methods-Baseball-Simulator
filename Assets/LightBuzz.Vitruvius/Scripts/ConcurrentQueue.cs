using System;
using System.Collections.Generic;

//public class Tuple<T1, T2>
//{
//    public T1 Item1 { get; set; }
//    public T2 Item2 { get; set; }

//    public Tuple(T1 item1, T2 item2)
//    {
//        Item1 = item1;
//        Item2 = item2;
//    }
//}

internal class ConcurrentQueue<T>
{
    private readonly object syncLock = new object();
    private Queue<T> queue;

    public int Count
    {
        get
        {
            lock (syncLock)
            {
                return queue.Count;
            }
        }
    }

    public ConcurrentQueue()
    {
        this.queue = new Queue<T>();
    }

    public T Peek()
    {
        lock (syncLock)
        {
            return queue.Peek();
        }
    }

    public void Enqueue(T obj)
    {
        lock (syncLock)
        {
            queue.Enqueue(obj);
        }
    }

    public T Dequeue()
    {
        lock (syncLock)
        {
            return queue.Dequeue();
        }
    }

    public void Clear()
    {
        lock (syncLock)
        {
            queue.Clear();
        }
    }

    public T[] CopyToArray()
    {
        lock (syncLock)
        {
            if (queue.Count == 0)
            {
                return new T[0];
            }

            T[] values = new T[queue.Count];
            queue.CopyTo(values, 0);
            return values;
        }
    }

    public static ConcurrentQueue<T> InitFromArray(IEnumerable<T> initValues)
    {
        var queue = new ConcurrentQueue<T>();

        if (initValues == null)
        {
            return queue;
        }

        foreach (T val in initValues)
        {
            queue.Enqueue(val);
        }

        return queue;
    }
}