using System;
using System.Collections.Generic;
using System.Text;

namespace LighTake.Infrastructure.Common
{

    public class BidirectionalMapping<T1,T2> : IBidirectionalMapping<T1,T2>
    {
        private IDictionary<T1, T2> dictionary = new Dictionary<T1, T2>();
        private IDictionary<T2, T1> reversedDictionary = new Dictionary<T2, T1>();
        private object locker = new object();

        #region Count
        public int Count
        {
            get
            {
                return this.dictionary.Count;
            }
        } 
        #endregion

        #region Add
        public void Add(T1 t1, T2 t2)
        {
            lock (this.locker)
            {
                if (!this.dictionary.ContainsKey(t1))
                {
                    this.dictionary.Remove(t1);
                }

                this.dictionary.Add(t1, t2);

                if (!this.reversedDictionary.ContainsKey(t2))
                {
                    this.reversedDictionary.Remove(t2);
                }

                this.reversedDictionary.Add(t2, t1);
            }
        } 
        #endregion

        #region RemoveByT1
        public void RemoveByT1(T1 t1)
        {
           

            lock (this.locker)
            {
                if (!this.dictionary.ContainsKey(t1))
                {
                    return;
                }

                T2 t2 = this.dictionary[t1];
                this.dictionary.Remove(t1);
                this.reversedDictionary.Remove(t2);
            }
        } 
        #endregion

        #region RemoveByT2
        public void RemoveByT2(T2 t2)
        {
            lock (this.locker)
            {
                if (!this.reversedDictionary.ContainsKey(t2))
                {
                    return;
                }

                T1 t1 = this.reversedDictionary[t2];
                this.reversedDictionary.Remove(t2);
                this.dictionary.Remove(t1);
            }
        }
        #endregion

        #region GetT2
        public T2 GetT2(T1 t1)
        {
            lock (this.locker)
            {
                if (!this.dictionary.ContainsKey(t1))
                {
                    return default(T2);
                }

                return this.dictionary[t1];
            }
        } 
        #endregion

        #region GetT1
        public T1 GetT1(T2 t2)
        {
            lock (this.locker)
            {
                if (!this.reversedDictionary.ContainsKey(t2))
                {
                    return default(T1);
                }

                return this.reversedDictionary[t2];
            }
        }
        #endregion

        #region ContainsT1
        public bool ContainsT1(T1 t1)
        {
            return this.dictionary.ContainsKey(t1);
        } 
        #endregion

        #region ContainsT2
        public bool ContainsT2(T2 t2)
        {
            return this.reversedDictionary.ContainsKey(t2);
        } 
        #endregion

        #region GetAllT1ListCopy
        public IList<T1> GetAllT1ListCopy()
        {
            lock (this.locker)
            {
                return CollectionConverter.CopyAllToList<T1>(this.dictionary.Keys);
            }
        } 
        #endregion

        #region GetAllT2ListCopy
        public IList<T2> GetAllT2ListCopy()
        {
            lock (this.locker)
            {
                return CollectionConverter.CopyAllToList<T2>(this.reversedDictionary.Keys);
            }
        } 
        #endregion
        
        public void Clear()
        {
            lock (this.locker)
            {
                dictionary.Clear();
                reversedDictionary.Clear();
            }
        }

    }
}
