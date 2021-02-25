using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 
    */
    public class EntityComparer<T> : IEqualityComparer<T> where T : class, IEntity
    {
        /*
        @brief 
        */
        public static readonly IEqualityComparer<T> Comparer = new EntityComparer<T>();

        /*
        @brief 
        */
        public bool Equals(T left, T right)
        {
            return left == right;
        }

        /*
        @brief 
        */
        public int GetHashCode(T obj)
        {
            return obj.SerialId;
        }
    }
}
