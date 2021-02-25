using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 配對器
    @note 比對部分
    */
    public partial class Matcher<T>
    {
        /*
        @brief hash是否產生變化
        */
        private bool isHashChanged;

        /*
        @brief hash值
        */
        private int hash;        

        /*
        @brief 
        */
        public override int GetHashCode()
        {
            if (isHashChanged)
                hash = AllComplexId | AnyComplexId | NoneComplexId;

            return hash;
        }

        /*
        @brief 
        */
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            if (obj.GetHashCode() != GetHashCode())
                return false;

            var temp = (Matcher<T>)obj;

            if (temp.AllComplexId != AllComplexId)
                return false;
            if (temp.AnyComplexId != AnyComplexId)
                return false;
            if (temp.NoneComplexId != NoneComplexId)
                return false;

            return true;
        }
    }
}
