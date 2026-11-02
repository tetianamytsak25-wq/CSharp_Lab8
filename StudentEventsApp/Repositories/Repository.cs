using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentRegistryApp.Repositories
{
    public class Repository<T>
    {
        private readonly List<T> items = new List<T>();
        public void Add(T item)
        {
            items.Add(item);
        }
        public void Update(T oldItem, T newItem)
        {
            int index = items.IndexOf(oldItem);

            if (index == -1)
            {
                throw new ArgumentException("Елемент не знайдено.");
            }

            items[index] = newItem;
        }
        public void Remove(T item)
        {
            items.Remove(item);
        }
        public T? Find(Func<T, bool> condition)
        {
            return items.FirstOrDefault(condition);
        }
        public IEnumerable<T> GetAll()
        {
            return items;
        }
    }
}