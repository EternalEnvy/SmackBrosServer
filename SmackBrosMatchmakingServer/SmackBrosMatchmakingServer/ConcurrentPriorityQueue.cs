using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmackBrosMatchmakingServer
{
    class ConcurrentPriorityQueue<T>
    {
        private const int MAX_THREADS = 5;
        public static ConcurrentPriorityQueue<T> Instance = new ConcurrentPriorityQueue<T>();
        private static Semaphore pool = new Semaphore(0, MAX_THREADS);
        private bool SemaphoreActive = true;
        private int count;
        private List<Tuple<int,T>> lst = new List<Tuple<int,T>>();
        public ConcurrentPriorityQueue(ConcurrentPriorityQueue<T> old)
        {
            lst = new List<Tuple<int,T>>(old.lst);
            count = old.Count;
        }

        public ConcurrentPriorityQueue()
        {
            return; 
        }
        public int Count
        {
            get { return count; }
        }
        public bool Empty()
        {
            return count == 0;
        }
        public void Enqueue(int priority, T item)
        {
            pool.WaitOne();     
            lst.Add(new Tuple<int,T>(priority, item));
            count++;
            int i = count;
            while(i > 0 && (lst[i].Item1 > lst[(i - 1) / 2].Item1))
            {
                Tuple<int, T> temp = lst[i];
                lst[i] = lst[(i - 1) / 2];
                lst[(i - 1) / 2] = temp;
            }
            pool.Release();
        }
        public void Clear()
        {
            while(lst.Count > 0)
            {
                Instance.Dequeue();
            }
        }
        public T Front()
        {
            if(!Empty())
            {
                return lst[0].Item2;
            }
            else
            {
                throw new KeyNotFoundException("Invalid access: List is empty");
            }
        }
        public T Dequeue()
        { 
            if (!Empty())
            {
                pool.WaitOne();
                T temp = lst[0].Item2;
                if (count > 1)
                {
                    lst[0] = lst[count];
                    lst.RemoveAt(count);
                    int i = 0;
                    while (true)
                    {
                        if (2 * i + 1 >= count)
                            break;
                        if (2 * i + 2 >= count)
                        {
                            var temp1 = lst[2 * i + 1];
                            lst[2 * i + 1] = lst[i];
                            lst[i] = temp1;
                            i = 2 * i + 1;
                            break;
                        }
                        else
                        {
                            if (lst[2 * i + 1].Item1 >= lst[2 * i + 2].Item1)
                            {
                                var temp1 = lst[2 * i + 1];
                                lst[2 * i + 1] = lst[i];
                                lst[i] = temp1;
                                i = 2 * i + 1;
                            }
                            else
                            {
                                var temp1 = lst[2 * i + 2];
                                lst[2 * i + 2] = lst[i];
                                lst[i] = temp1;
                                i = 2 * i + 2;
                            }
                        }
                    }
                }
                count--;
                pool.Release();
                return temp;
            }
            else
            {
                throw new KeyNotFoundException("Invalid access: List is empty");
            }
        }
    }
}
