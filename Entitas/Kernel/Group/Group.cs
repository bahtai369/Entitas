
using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 實體群組
    */
    public class Group<T> : IGroup<T> where T : class, IEntity
    {        
        /*
        @brief 配對器
        */
        public IMatcher<T> Matcher { get; private set; }

        /*
        @brief 各種委派
        */
        public event GroupChange<T> OnEntityAdd;     // 新增實體
        public event GroupChange<T> OnEntityRemove;  // 移除實體
        public event GroupUpdate<T> OnEntityUpdate;  // 實體更新

        /*
        @brief 實體列表
        */
        private readonly HashSet<T> entities = new HashSet<T>(EntityComparer<T>.Comparer);

        /*
        @brief 實體列表快取
        */
        private T[] entitiesCache;

        /*
        @brief 實體總數
        */
        public int EntitiesCount { get { return entities.Count; } }

        /*
        @brief 輸出字串快取
        */
        private string toStrCache;

        /*
        @brief 
        */
        public Group(IMatcher<T> matcher)
        {
            Matcher = matcher;
        }

        /*
        @brief 處理實體進出群組
        @note 不會觸發委派
        */
        public void HandleEntityMute(T entity)
        {
            if (Matcher.IsEntityMatch(entity))
                AddEntityMute(entity);
            else
                RemoveEntityMute(entity);
        }

        /*
        @brief 新增實體
        @note 不會觸發委派
        */
        private bool AddEntityMute(T entity)
        {
            if (entity.IsEnabled == false)
                return false;

            var res = entities.Add(entity);

            if (res)
            {
                entitiesCache = null;
                entity.AddRef(this);
            }

            return res;
        }

        /*
        @brief 移除實體
        @note 不會觸發委派
        */
        private bool RemoveEntityMute(T entity)
        {
            var res = entities.Remove(entity);

            if (res)
            {
                entitiesCache = null;
                entity.RemoveRef(this);
            }

            return res;
        }

        /*
        @brief 處理實體進出群組
        */
        public void HandleEntity(T entity, int componentId, IComponent component)
        {
            if (Matcher.IsEntityMatch(entity))
                AddEntity(entity, componentId, component);
            else
                RemoveEntity(entity, componentId, component);
        }

        /*
        @brief 新增實體
        */
        private void AddEntity(T entity, int componentId, IComponent component)
        {
            if (AddEntityMute(entity) && OnEntityAdd != null)
                OnEntityAdd(this, entity, componentId, component);
        }

        /*
        @brief 移除實體
        */
        private void RemoveEntity(T entity, int componentId, IComponent component)
        {
            var res = entities.Remove(entity);

            if (res)
            {
                entitiesCache = null;

                if (OnEntityRemove != null)
                    OnEntityRemove(this, entity, componentId, component);

                entity.RemoveRef(this);
            }
        }

        /*
        @brief 實體更新
        @note 限定管理器使用
        */
        public void UpdateEntity(T entity, int componentId, IComponent oldComponent, IComponent newComponent)
        {
            if (HasEntity(entity) == false)
                return;

            if (OnEntityRemove != null)
                OnEntityRemove(this, entity, componentId, oldComponent);

            if (OnEntityRemove != null)
                OnEntityAdd(this, entity, componentId, newComponent);

            if (OnEntityUpdate != null)
                OnEntityUpdate(this, entity, componentId, oldComponent, newComponent);
        }

        /*
        @brief 有無此實體
        */
        public bool HasEntity(T entity)
        {
            return entities.Contains(entity);
        }

        /*
        @brief 取得所有實體
        */
        public T[] GetAllEntities()
        {
            if (entitiesCache == null)
            {
                entitiesCache = new T[EntitiesCount];
                entities.CopyTo(entitiesCache);
            }

            return entitiesCache;
        }

        /*
        @brief 取得所有實體
        @param dest [out] 使用前會清空
        */
        public void GetAllEntities(ref List<T> dest)
        {
            dest.Clear();
            dest.AddRange(entities);
        }

        /*
        @brief 
        */
        public HashSet<T>.Enumerator GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        /*
        @brief 清除委派
        */
        public void ClearDelegates()
        {
            OnEntityAdd = null;
            OnEntityRemove = null;
            OnEntityUpdate = null;
        }

        /*
        @brief 
        */
        public override string ToString()
        {
            if (toStrCache == null)
                toStrCache = $"group: {Matcher}";

            return toStrCache;
        }
    }    
}
