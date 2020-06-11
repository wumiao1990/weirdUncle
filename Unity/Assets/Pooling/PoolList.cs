using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using UnityEngine;

public class PoolList<T> : IEnumerable, ICollection<T>, IEnumerable<T>, IList<T>
{
    #region static 

    private static readonly ObjectPool<PoolList<T>> s_ListPool = new ObjectPool<PoolList<T>>(null, l => l.Clear());

    public static PoolList<T> Get()
    {
        return s_ListPool.Get();
    }

    public static void Release(PoolList<T> toRelease)
    {
        s_ListPool.Release(toRelease);
    }

    #endregion

    private List<T> m_listData;

    public void Release()
    {
        Release(this);
    }

    /// <summary>
    /// 不要直接创建，使用Get()
    /// </summary>
    public PoolList()
    {
        m_listData = new List<T>();
    }

    /// <summary>
    /// 不要直接创建，使用Get()
    /// </summary>
    public PoolList(IEnumerable<T> collection)
    {
        m_listData = new List<T>(collection);
    }

    /// <summary>
    /// 不要直接创建，使用Get()
    /// </summary>
    public PoolList(int capacity)
    {
        m_listData = new List<T>(capacity);
    }

    public T this[int index]
    {
        get { return m_listData[index]; }
        set { m_listData[index] = value; }
    }

    public int Count { get { return m_listData.Count; } }
    public int Capacity
    {
        get { return m_listData.Capacity; }
        set { m_listData.Capacity = value; }
    }

    public bool IsSynchronized
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public bool IsFixedSize
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public bool IsReadOnly { get { return false; } }

    public void Add(T item)
    {
        m_listData.Add(item);
    }

    public void AddRange(IEnumerable<T> collection)
    {
        m_listData.AddRange(collection);
    }

    /*
     * 可回收数组不实现此方法
    public ReadOnlyCollection<T> AsReadOnly()
    {
        return m_listData.AsReadOnly();
    }*/

    public int BinarySearch(T item)
    {
        return m_listData.BinarySearch(item);
    }

    public int BinarySearch(T item, IComparer<T> comparer)
    {
        return m_listData.BinarySearch(item, comparer);
    }

    public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
    {
        return m_listData.BinarySearch(index, count, item, comparer);
    }

    public void Clear()
    {
        m_listData.Clear();
    }

    public bool Contains(T item)
    {
        return m_listData.Contains(item);
    }

    /*
     * 可回收数组不实现此方法
    public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
    {
        return m_listData.ConvertAll(converter);
    }*/

    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
        m_listData.CopyTo(index, array, arrayIndex, count);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        m_listData.CopyTo(array, arrayIndex);
    }

    public void CopyTo(T[] array)
    {
        m_listData.CopyTo(array);
    }

    public bool Exists(Predicate<T> match)
    {
        return m_listData.Exists(match);
    }

    public T Find(Predicate<T> match)
    {
        return m_listData.Find(match);
    }

    public List<T> FindAll(Predicate<T> match)
    {
        return m_listData.FindAll(match);
    }

    public int FindIndex(Predicate<T> match)
    {
        return m_listData.FindIndex(match);
    }

    public int FindIndex(int startIndex, Predicate<T> match)
    {
        return m_listData.FindIndex(startIndex, match);
    }

    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
        return m_listData.FindIndex(startIndex, count, match);
    }

    public T FindLast(Predicate<T> match)
    {
        return m_listData.FindLast(match);
    }

    public int FindLastIndex(Predicate<T> match)
    {
        return m_listData.FindLastIndex(match);
    }
    public int FindLastIndex(int startIndex, Predicate<T> match)
    {
        return m_listData.FindLastIndex(startIndex, match);
    }
    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
        return m_listData.FindLastIndex(startIndex, count, match);
    }
    public void ForEach(Action<T> action)
    {
        m_listData.ForEach(action);
    }

    /*
     * 可回收数组不实现此方法
    public List<T> GetRange(int index, int count)
    {
        return m_listData.GetRange(index, count);
    }*/

    public int IndexOf(T item, int index, int count)
    {
        return m_listData.IndexOf(item, index, count);
    }
    public int IndexOf(T item, int index)
    {
        return m_listData.IndexOf(item, index);
    }
    public int IndexOf(T item)
    {
        return m_listData.IndexOf(item);
    }
    public void Insert(int index, T item)
    {
        m_listData.Insert(index, item);
    }
    public void InsertRange(int index, IEnumerable<T> collection)
    {
        m_listData.InsertRange(index, collection);
    }
    public int LastIndexOf(T item)
    {
        return m_listData.LastIndexOf(item);
    }
    public int LastIndexOf(T item, int index)
    {
        return m_listData.LastIndexOf(item, index);
    }
    public int LastIndexOf(T item, int index, int count)
    {
        return m_listData.LastIndexOf(item, index, count);
    }
    public bool Remove(T item)
    {
        return m_listData.Remove(item);
    }
    public int RemoveAll(Predicate<T> match)
    {
        return m_listData.RemoveAll(match);
    }
    public void RemoveAt(int index)
    {
        m_listData.RemoveAt(index);
    }
    public void RemoveRange(int index, int count)
    {
        m_listData.RemoveRange(index, count);
    }
    public void Reverse(int index, int count)
    {
        m_listData.Reverse(index, count);
    }
    public void Reverse()
    {
        m_listData.Reverse();
    }
    public void Sort(Comparison<T> comparison)
    {
        m_listData.Sort(comparison);
    }
    public void Sort(int index, int count, IComparer<T> comparer)
    {
        m_listData.Sort(index, count, comparer);
    }
    public void Sort()
    {
        m_listData.Sort();
    }
    public void Sort(IComparer<T> comparer)
    {
        m_listData.Sort(comparer);
    }

    /*
     * 可回收数组不实现此方法
    public T[] ToArray()
    {
        return m_listData.ToArray();
    }*/

    public void TrimExcess()
    {
        m_listData.TrimExcess();
    }

    public bool TrueForAll(Predicate<T> match)
    {
        return m_listData.TrueForAll(match);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return new Enumerator(this);
    }

    public IEnumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    public struct Enumerator : IEnumerator, IDisposable, IEnumerator<T>
    {
        private PoolList<T> m_list;
        private int m_index;
        private T m_current;

        internal Enumerator(PoolList<T> list)
        {
            m_list = list;
            m_index = 0;
            m_current = default(T);
        }

        public T Current { get { return m_current; } }

        object IEnumerator.Current
        {
            get
            {
                if (m_index <= 0 || m_index >= m_list.Count + 1)
                {
                    throw new IndexOutOfRangeException();
                }
                return Current;
            }
        }

        public void Dispose()
        {
            m_list = null;
            m_index = 0;
            m_current = default(T);
        }

        public bool MoveNext()
        {
            if (m_index >= 0 && m_index < m_list.Count)
            {
                m_current = m_list[m_index];
                m_index++;
                return true;
            }
            else
            {
                m_index = m_list.Count + 1;
                m_current = default(T);
                return false;
            }
        }

        public void Reset()
        {
            m_index = 0;
            m_current = default(T);
        }
    }
}
