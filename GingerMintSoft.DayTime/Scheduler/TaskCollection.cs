using System.Collections;

namespace GingerMintSoft.DayTime.Scheduler
{
    public class TaskCollection : IEnumerable<ITask>
    {
        private readonly List<ITask?> _tasks = new();

        public void Add(ITask? item)
        {
            var index = IndexOf(item);

            if (index > -1)
                _tasks.Insert(index + 1, item);
            else
                _tasks.Insert(~index, item);
        }

        public bool Remove(ITask? item)
        {
            var index = IndexOf(item);

            if (index >= 0)
            {
                _tasks.RemoveAt(index);
            }

            return index >= 0;

        }

        public ITask? First()
        {
            return _tasks.Count > 0 
                ? _tasks[0] 
                : null;
        }

        /// <summary>
        /// Returns the index of <paramref name="item"/> using Binary Search
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(ITask? item)
        {
            return _tasks.BinarySearch(item, new TaskComparer()!);
        }

        public void RemoveAt(int index)
        {
            _tasks.RemoveAt(index);
        }

        public void Clear()
        {
            _tasks.Clear();
        }

        public bool Contains(ITask? item)
        {
            return IndexOf(item) > 0;
        }

        public void CopyTo(ITask?[] array, int arrayIndex)
        {
            _tasks.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ITask> GetEnumerator()
        {
            return ((IEnumerable<ITask>)_tasks).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ITask>)_tasks).GetEnumerator();
        }

        public int Count => _tasks.Count;

        public ITask? this[int index] => _tasks[index];
    }
}
