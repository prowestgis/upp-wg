using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Configuration
{
    using System.Configuration;
    using System.Collections.Generic;

    public abstract class EnumerableConfigurationElementCollection<T> :
        ConfigurationElementCollection, IEnumerable<T> where T : ConfigurationElement, new()
    {
        public T this[object key]
        {
            get
            {
                return base.BaseGet(key) as T;
            }
        }

        public T this[int index]
        {
            get
            {
                return base.BaseGet(index) as T;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        #region IEnumerable<T> Members

        public new IEnumerator<T> GetEnumerator()
        {
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                yield return base.BaseGet(i) as T;
            }
        }

        #endregion

        #region Editable Support

        public void Add(T thing)
        {
            base.BaseAdd(thing);
        }

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public void Remove(T thing)
        {
            base.BaseRemove(GetElementKey(thing));
        }

        public void Clear()
        {
            base.BaseClear();
        }

        public void RemoveAt(int index)
        {
            base.BaseRemoveAt(index);
        }

        public string GetKey(int index)
        {
            return (string)base.BaseGetKey(index);
        }

        #endregion
    }
}
