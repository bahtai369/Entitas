using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 群組變化
    @param id [in] 組件編號
    */
    public delegate void GroupChange<T>(IGroup<T> group, T entity) where T : IEntity;

    /*
    @brief 群組更新
    @param id [in] 組件編號
    */
    public delegate void GroupUpdate<T>(IGroup<T> group, T entity, int id, IComponent oldComponent, IComponent newComponent) where T : IEntity;

    /*
    @brief 實體群組
    */
    public interface IGroup
    {
        /*
        @brief 實體總數
        */
        int EntitiesCount { get; }

        /*
        @brief 清除委派
        */
        void ClearDelegates();
    }

    /*
    @brief 實體群組
    */
    public interface IGroup<T> : IGroup where T : IEntity
    {
        /*
        @brief 配對器
        */
        IMatcher<T> Matcher { get; }

        /*
        @brief 各種委派
        */
        event GroupChange<T> OnEntityAdd;     // 新增實體
        event GroupChange<T> OnEntityRemove;  // 移除實體
        event GroupUpdate<T> OnEntityUpdate;  // 實體更新

        /*
        @brief 處理實體進出群組
        @note 不會觸發委派
        */
        void HandleEntityMute(T entity);

        /*
        @brief 處理實體進出群組
        @param id [in] 組件編號
        */
        void HandleEntity(T entity, int id, IComponent component);

        /*
        @brief 實體更新
        @note 限定管理器使用
        @param id [in] 組件編號
        */
        void UpdateEntity(T entity, int id, IComponent oldComponent, IComponent newComponent);

        /*
        @brief 有無此實體
        */
        bool HasEntity(T entity);

        /*
        @brief 取得所有實體
        */
        T[] GetAllEntities();

        /*
        @brief 取得所有實體
        @param dest [out] 使用前會清空
        */
        void GetAllEntities(ref List<T> dest);

        /*
        @brief 
        */
        HashSet<T>.Enumerator GetEnumerator();
    }
}
