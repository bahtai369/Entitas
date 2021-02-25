using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Kent.Entitas
{
    /*
    @brief 管理器
    */
    public class Manager<T> : IManager<T> where T : class, IEntity
    {
        /*
        @brief 各種委派
        */
        public event MgrEntityChange OnEntityCreate;        // 新創或復用實體
        public event MgrEntityChange OnEntityReadyDestroy;  // 實體準備好銷毀
        public event MgrEntityChange OnEntityDestroy;       // 銷毀實體
        public event MgrGroupChange OnGroupCreate;          // 創建群組

        /*
        @brief 除錯資訊
        */
        private readonly DebugInfo debugInfo;

        /*
        @brief 
        @param entityFactory [in] 實體工廠
        @param entityMaxComponents [in] 每個實體的組件數量上限, 不可超過32
        */
        public Manager(Func<T> entityFactory, int entityMaxComponents) : this(entityFactory, entityMaxComponents, 0, null) { }

        /*
        @brief 
        @param entityFactory [in] 實體工廠
        @param entityMaxComponents [in] 每個實體的組件數量上限, 不可超過32
        @param initEntitySerialId [in] 初始實體流水號
        @param debugInfo [in] 除錯資訊
        */
        public Manager(Func<T> entityFactory, int entityMaxComponents, int initEntitySerialId, DebugInfo debugInfo)
        {
            //
            this.entityFactory = entityFactory;
            this.entityMaxComponents = entityMaxComponents;
            this.initEntitySerialId = initEntitySerialId;
            this.entitySerialId = initEntitySerialId;

            // 除錯資訊設定
            if (debugInfo != null)
            {
                if (debugInfo.ComponentNames.Length != entityMaxComponents)
                    throw new Exception($"debug info's component names size != entity max components: {entityMaxComponents}");

                this.debugInfo = debugInfo;
            }
            else
            {
                debugInfo = CreateDefaultDebugInfo();
            }

            //
            componentsPools = new Stack<IComponent>[entityMaxComponents];
            componentIdToGroups = new List<IGroup<T>>[entityMaxComponents];
        }

        /*
        @brief 創建預設的除錯資訊
        */
        private DebugInfo CreateDefaultDebugInfo()
        {
            var names = new string[entityMaxComponents];

            for (int i = 0; i < entityMaxComponents; i++)
                names[i] = i.ToString();

            return new DebugInfo("default", names);
        }

        /*
        @brief 重置
        @note 會銷毀所有使用中實體, 並將流水號還原成初始值
        */
        public void Reset()
        {
            DestroyAllEntities();
            ResetEntitySerialId();
        }

        /*
        @brief 將實體流水號還原成初始值
        */
        public void ResetEntitySerialId()
        {
            entitySerialId = initEntitySerialId;
        }

        /*
        @brief 清除所有委派
        */
        public void ClearDelegates()
        {
            OnEntityCreate = null;
            OnEntityReadyDestroy = null;
            OnEntityDestroy = null;
            OnGroupCreate = null;
        }
        
        #region 實體操作
        /*
        @brief 使用中實體總數
        */
        public int EntitiesCount { get { return entities.Count; } }

        /*
        @brief 使用中實體
        */
        private readonly HashSet<T> entities = new HashSet<T>();

        /*
        @brief 使用中實體快取
        */
        private T[] entitiesCache;

        /*
        @brief 實體池中可用數量
        */
        public int EntitiesPoolCount { get { return entitiesPool.Count; } }

        /*
        @brief 實體池
        */
        private readonly Stack<T> entitiesPool = new Stack<T>();

        /*
        @brief 回收時出錯的實體數量
        */
        public int ErrEntitiesCount { get { return errEntities.Count; } }

        /*
        @brief 出錯的實體
        */
        private readonly HashSet<T> errEntities = new HashSet<T>(EntityComparer<T>.Comparer);

        /*
        @brief 實體工廠
        */
        private readonly Func<T> entityFactory;

        /*
        @brief 實體流水號
        */
        private int entitySerialId;

        /*
        @brief 初始實體流水號
        */
        private int initEntitySerialId;

        /*
        @brief 每個實體的組件數量上限
        */
        private readonly int entityMaxComponents;        

        /*
        @brief 銷毀所有實體
        */
        public void DestroyAllEntities()
        {
            foreach (var entity in errEntities)
                entity.Destroy();

            entities.Clear();

            // 還有出錯的實體
            if (ErrEntitiesCount != 0)
            {
                throw new Exception($"mgr: {this} has err entities size: {ErrEntitiesCount}:\n" +
                    string.Join("\n", errEntities.Select(iter => iter.ToString()).ToArray()));
            }
        }

        /*
        @brief 創建實體
        */
        public T CreateEntity()
        {
            T entity;

            if (EntitiesPoolCount > 0)
            {
                // 復用
                entity = entitiesPool.Pop();
                entity.Init(entitySerialId++);
            }
            else
            {
                // 新創
                entity = entityFactory();
                entity.Init(entitySerialId++, entityMaxComponents, componentsPools, debugInfo);
            }

            //
            entities.Add(entity);
            entity.AddRef(this);
            entitiesCache = null;

            // 
            entity.OnComponentAdd += OnEntityComponentAddOrRemoveHandler;
            entity.OnComponentRemove += OnEntityComponentAddOrRemoveHandler;
            entity.OnComponentUpdate += OnEntityComponentUpdateHandler;
            entity.OnEntityRelease += OnEntityReleaseHandler;
            entity.OnEntityDestroy += OnEntityDestroyHandler;

            //
            OnEntityCreate?.Invoke(this, entity);

            return entity;
        }        

        /*
        @brief 有無此實體
        */
        public bool HasEntity(T entity)
        {
            return entities.Contains(entity);
        }

        /*
        @brief 取得所有使用中的實體
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
        #endregion

        #region 組件操作
        /*
        @brief 組件池
        */
        private readonly Stack<IComponent>[] componentsPools;

        /*
        @brief 清除單一組件池
        @param id [in] 組件編號
        */
        public void ClearComponentsPool(int id)
        {
            var pool = componentsPools[id];

            if (pool != null)
                pool.Clear();
        }

        /*
        @brief 清除所有組件池
        */
        public void ClearAllComponentsPools()
        {
            for (int i = 0; i < entityMaxComponents; i++)
                ClearComponentsPool(i);
        }
        #endregion

        #region 群組操作
        /*
        @brief 用比對器找群組
        */
        private readonly Dictionary<IMatcher<T>, IGroup<T>> matcherToGroup = new Dictionary<IMatcher<T>, IGroup<T>>();

        /*
        @brief 組件編號找所在群組
        */
        private readonly List<IGroup<T>>[] componentIdToGroups;

        /*
        @brief 取得群組
        */
        public IGroup<T> GetGroup(IMatcher<T> matcher)
        {
            IGroup<T> group;

            // 取不到現成的就新創
            if (matcherToGroup.TryGetValue(matcher, out group) == false)
            {
                group = new Group<T>(matcher);

                // 將所有現存實體做分類
                foreach (var entity in entities)
                    group.HandleEntityMute(entity);

                // 加入查找
                matcherToGroup.Add(matcher, group);

                int temp = 1;

                // 加入查找
                for (int i = 0; i < entityMaxComponents; i++)
                {
                    // 此配對器有關注此組件
                    if ((matcher.MixComplexId & temp) > 0)
                    {
                        if (componentIdToGroups[i] != null)
                            componentIdToGroups[i] = new List<IGroup<T>>();

                        componentIdToGroups[i].Add(group);
                    }

                    temp = temp << 1;
                }

                //
                OnGroupCreate?.Invoke(this, group);
            }

            return group;
        }
        #endregion

        /*
        @brief 當實體組件新增或移除
        @param id [in] 組件編號
        */
        private void OnEntityComponentAddOrRemoveHandler(IEntity entity, int id, IComponent component)
        {
            var groups = componentIdToGroups[id];

            if (groups == null)
                return;

            var temp = (T)entity;

            foreach (var group in groups)
                group.HandleEntity(temp, id, component);
        }

        /*
        @brief 當實體組件更新
        @param id [in] 組件編號
        */
        private void OnEntityComponentUpdateHandler(IEntity entity, int id, IComponent oldComponent, IComponent newComponent)
        {
            var groups = componentIdToGroups[id];

            if (groups == null)
                return;

            var temp = (T)entity;

            foreach (var group in groups)
                group.UpdateEntity(temp, id, oldComponent, newComponent);
        }

        /*
        @brief 當實體參考歸零
        */
        private void OnEntityReleaseHandler(IEntity entity)
        {
            if (entity.IsEnabled == false)
                throw new Exception($"can't release entity: {entity}, entity doesn't enabled");
            
            //
            entity.ReleaseClear();

            // 回收
            var temp = (T)entity;
            errEntities.Remove(temp);
            entitiesPool.Push(temp);
        }

        /*
        @brief 當時體銷毀
        */
        private void OnEntityDestroyHandler(IEntity entity)
        {
            var temp = (T)entity;

            if (entities.Remove(temp) == false)
                throw new Exception($"can't destroy entity: {entity}, ???");

            entitiesCache = null;

            OnEntityReadyDestroy?.Invoke(this, temp);

            temp.DestroyInner();

            OnEntityDestroy?.Invoke(this, entity);

            if (entity.RefCount <= 1)
            {
                // 除了管理器以外無參考對象
                temp.OnEntityRelease -= OnEntityReleaseHandler;
                temp.RemoveRef(this);
                temp.ReleaseClear();
            }
            else
            {
                // 尚有其他參考對象, 要等到參考對象都消失才可回收
                errEntities.Add(temp);
                temp.RemoveRef(this);
            }
        }
    }
}
