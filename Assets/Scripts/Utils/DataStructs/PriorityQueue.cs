using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils.DataStructs
{
    public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority> 
    {
        private readonly LinkedList<(TElement element, TPriority priotity)> _queue = new();

        public int Count => _queue.Count;

        public void Enqueue(TElement element, ref TPriority priority)
        {
            if (_queue.Count == 0 || _queue.First.Value.priotity.CompareTo(priority) <= 0)
            {
                _queue.AddFirst((element, priority));
                return;
            }
            LinkedListNode<(TElement element, TPriority priotity)> current = _queue.First;

            while (current.Next != null && current.Next.Value.priotity.CompareTo(priority) > 0)
            {
                current = current.Next;
            }
            _queue.AddAfter(current, (element, priority));
        }

        public TElement Dequeue()
        {
            TElement element = _queue.First().element;
            _queue.RemoveFirst();

            return element;
        }

        public IReadOnlyList<TElement> GetOrder()
        {
            List<TElement> list = new();
         
            foreach (var unit in _queue)
            {
                list.Add(unit.element);
            }
            return list;
        }

        public void TryRemove(TElement element)
        {
            (TElement element, TPriority priotity) pairByElement =
                _queue.FirstOrDefault(searchingEltment => searchingEltment.element.Equals(element));

            _queue.Remove(pairByElement);
        }
    }
}