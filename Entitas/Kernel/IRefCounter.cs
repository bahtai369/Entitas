using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 參考計數器
    */
    public interface IRefCounter
    {
        /*
        @brief 參考總數
        */
        int RefCount { get; }

        /*
        @brief 新增參考
        @param owner [in] 對象
        */
        void AddRef(object owner);

        /*
        @brief 移除參考
        @param owner [in] 對象
        */
        void RemoveRef(object owner);
    }

    /*
    @brief 簡易參考計數器
    */
    public class UnsafeRefCounter : IRefCounter
    {
        /*
        @brief 參考總數
        */
        public int RefCount { get; private set; }

        /*
        @brief 新增參考
        @param owner [in] 對象
        */
        public void AddRef(object owner)
        {
            RefCount += 1;
        }

        /*
        @brief 移除參考
        @param owner [in] 對象
        */
        public void RemoveRef(object owner)
        {
            RefCount -= 1;
        }
    }

    /*
    @brief 參考計數器
    */
    public class SafeRefCounter : IRefCounter
    {
        /*
        @brief 宿主
        */
        private readonly object host;

        /*
        @brief 參考物件列表
        */
        private readonly HashSet<object> owners = new HashSet<object>();

        /*
        @brief 參考總數
        */
        public int RefCount { get { return owners.Count; } }

        /*
        @brief 
        */
        public SafeRefCounter(object host)
        {
            this.host = host;
        }

        /*
        @brief 新增參考
        @param owner [in] 對象
        */
        public void AddRef(object owner)
        {
            if (owners.Add(owner))
                throw new Exception($"obj: {owner} already ref obj: {host}");
        }

        /*
        @brief 移除參考
        @param owner [in] 對象
        */
        public void RemoveRef(object owner)
        {
            if (owners.Remove(owner))
                throw new Exception($"obj: {owner} doesn't ref obj: {host}");
        }
    }
}
