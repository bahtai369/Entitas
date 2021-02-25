using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 管理器實體變化
    */
    public delegate void MgrEntityChange(IManager mgr, IEntity entity);

    /*
    @brief 管理器群組變化
    */
    public delegate void MgrGroupChange(IManager mgr, IGroup group);
    
    /*
    @brief 管理器
    */
    public interface IManager
    {
        /*
        @brief 各種委派
        */
        event MgrEntityChange OnEntityCreate;        // 新創或復用實體
        event MgrEntityChange OnEntityReadyDestroy;  // 實體準備好銷毀
        event MgrEntityChange OnEntityDestroy;       // 銷毀實體
        event MgrGroupChange OnGroupCreate;          // 創建群組 

        /*
        @brief 使用中實體總數
        */
        int EntitiesCount { get; }

        /*
        @brief 實體池中可用數量
        */
        int EntitiesPoolCount { get; }

        /*
        @brief 回收時出錯的實體數量
        */
        int ErrEntitiesCount { get; }

        /*
        @brief 銷毀所有實體
        */
        void DestroyAllEntities();

        /*
        @brief 清除單一組件池
        */
        void ClearComponentsPool(int componentId);

        /*
        @brief 清除所有組件池
        */
        void ClearAllComponentsPools();

        /*
        @brief 重置
        @note 會銷毀所有使用中實體, 並將流水號還原成初始值
        */
        void Reset();

        /*
        @brief 將流水號還原成初始值
        */
        void ResetSerialId();

        /*
        @brief 清除所有委派
        */
        void ClearDelegates();
    }

    /*
    @brief 管理器
    */
    public interface IManager<T> : IManager where T : IEntity
    {
        /*
        @brief 創建實體
        */
        T CreateEntity();

        /*
        @brief 有無此實體
        */
        bool HasEntity(T entity);

        /*
        @brief 取得所有使用中的實體
        */
        T[] GetAllEntities();

        /*
        @brief 取得群組
        */
        IGroup<T> GetGroup(IMatcher<T> matcher);
    }
}
