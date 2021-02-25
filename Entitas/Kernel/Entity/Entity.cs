using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 實體
    */
    public class Entity : IEntity
    {
        /*
        @brief 各種委派
        */
        public event EntityChange OnComponentAdd;     // 新增組件
        public event EntityChange OnComponentRemove;  // 移除組件
        public event EntityUpdate OnComponentUpdate;  // 組件更新
        public event EntityEvent OnEntityRelease;     // 實體參考歸零
        public event EntityEvent OnEntityDestroy;     // 實體銷毀

        /*
        @brief 是否啟用
        */
        public bool IsEnabled { get; private set; }

        /*
        @brief 流水號
        */
        public int SerialId { get; private set; }

        /*
        @brief 組件混和編號
        */
        public int ComplexId { get { return componentsFlags.Id; } }

        /*
        @brief 組件旗標
        */
        private readonly FlagUtil componentsFlags = new FlagUtil();

        /*
        @brief 組件池
        */
        private Stack<IComponent>[] componentsPools;

        /*
        @brief 組件列表
        */
        private IComponent[] components;

        /*
        @brief 組件列表快取
        */
        private IComponent[] componentsCache;

        /*
        @brief 組件列表緩存
        */
        private List<IComponent> componentsBuf = new List<IComponent>();

        /*
        @brief 組件數量上限
        */
        private int maxComponents;

        /*
        @brief 參考總數
        */
        public int RefCount { get { return refCounter.RefCount; } }

        /*
        @brief 參考計數器
        */
        private IRefCounter refCounter;

        /*
        @brief 除錯資訊
        */
        private DebugInfo debugInfo;

        /*
        @brief 字串輸出快取
        */
        private string toStrCache;

        /*
        @brief 動態字串工具
        */
        private StringBuilder toStrBuilder;

        /*
        @brief 初始化
        @param serialId [in] 流水號
        @param maxComponents [in] 組件數量上限, 不可超過32
        @param componentsPools [in] 組件池
        @param debugInfo [in] 除錯資訊
        @note 新創時使用
        */
        public void Init(int serialId, int maxComponents, Stack<IComponent>[] componentsPools, DebugInfo debugInfo)
        {
            this.maxComponents = maxComponents;
            components = new IComponent[maxComponents];
            this.componentsPools = componentsPools;
            this.debugInfo = debugInfo;
            refCounter = new SafeRefCounter(this);

            Init(serialId);
        }

        /*
        @brief 初始化
        @param serialId [in] 流水號
        @note 復用時使用
        */
        public void Init(int serialId)
        {
            SerialId = serialId;
            IsEnabled = true;
        }

        /*
        @brief 銷毀
        */
        public void Destroy()
        {
            if (IsEnabled == false)
                throw new Exception($"can't destroy entity: {this}, entity doesn't enabled");

            if (OnEntityDestroy != null)
                OnEntityDestroy(this);
        }

        /*
        @brief 銷毀
        @note 限定管理器使用
        */
        public void DestroyInner()
        {
            RemoveAllComponents();

            OnComponentAdd = null;
            OnComponentRemove = null;
            OnComponentUpdate = null;
            OnEntityDestroy = null;

            IsEnabled = false;
        }

        /*
        @brief 當參考歸零時做的清除
        @note 限定管理器使用
        */
        public void ReleaseClear()
        {
            OnEntityRelease = null;
        }

        /*
        @brief 新增組件
        @param id [in] 組件編號
        */
        public void AddComponent(int id, IComponent component)
        {
            if (IsEnabled == false)
                throw new Exception($"can't add component: {GetComponentName(id)} to entity: {this}, entity doesn't enabled");

            if (HasComponent(id))
                throw new Exception($"can't add component: {GetComponentName(id)} to entity: {this}, component repeat");

            componentsCache = null;
            toStrCache = string.Empty;

            components[id] = component;
            componentsFlags[id] = true;

            if (OnComponentAdd != null)
                OnComponentAdd(this, id, component);
        }

        /*
        @brief 移除組件
        @param id [in] 組件編號
        */
        public void RemoveComponent(int id)
        {
            if (IsEnabled == false)
                throw new Exception($"can't remove component: {GetComponentName(id)} from entity: {this}, entity doesn't enabled");

            if (HasComponent(id) == false)
                //throw new Exception($"can't remove component: {GetComponentName(id)} from entity: {this}, component doesn't exists");
                return;

            UpdateComponent(id, null);
        }

        /*
        @brief 移除所有組件
        */
        public void RemoveAllComponents()
        {
            for (int i = 0; i < maxComponents; i++)
                RemoveComponent(i);
        }

        /*
        @brief 更新組件
        @param id [in] 組件編號
        */
        public void UpdateComponent(int id, IComponent component)
        {
            if (IsEnabled == false)
                throw new Exception($"can't update component: {GetComponentName(id)} on entity: {this}, entity doesn't enabled");

            if (HasComponent(id))
                UpdateComponentInner(id, component);
            else
                AddComponent(id, component);
        }

        /*
        @brief 更新組件
        @param id [in] 組件編號
        */
        private void UpdateComponentInner(int id, IComponent component)
        {
            var old = components[id];

            if (component != null)
            {
                components[id] = component;
                componentsCache = null;

                if (component != null)
                {
                    if (OnComponentUpdate != null)
                        OnComponentUpdate(this, id, old, component);
                }
                else
                {
                    toStrCache = null;
                    componentsFlags[id] = false;

                    if (OnComponentRemove != null)
                        OnComponentRemove(this, id, old);
                }

                GetComponentsPool(id).Push(old);
            }
            else
            {
                if (OnComponentUpdate != null)
                    OnComponentUpdate(this, id, old, component);
            }
        }

        /*
        @brief 取得組件池
        @param id [in] 組件編號
        */
        private Stack<IComponent> GetComponentsPool(int id)
        {
            var pool = componentsPools[id];

            if (pool == null)
            {
                pool = new Stack<IComponent>();
                componentsPools[id] = pool;
            }

            return pool;
        }

        /*
        @brief 取得組件
        @param id [in] 組件編號
        */
        public IComponent GetComponent(int id)
        {
            if (IsEnabled == false)
                throw new Exception($"can't get component: {GetComponentName(id)} from entity: {this}, entity doesn't enabled");

            return components[id];
        }

        /*
        @brief 取得所有組件
        */
        public IComponent[] GetAllComponents()
        {
            if (componentsCache == null)
            {
                foreach (var component in components)
                {
                    if (component != null)
                        componentsBuf.Add(component);
                }

                componentsCache = componentsBuf.ToArray();
                componentsBuf.Clear();
            }

            return componentsCache;
        }

        /*
        @brief 有無此組件
        @param id [in] 組件編號
        */
        public bool HasComponent(int id)
        {
            return components[id] != null;
        }

        /*
        @brief 有無所有組件
        @param complexId [in] 組件混和編號
        */
        public bool HasAllComponents(int complexId)
        {
            return ComplexId == complexId;
        }

        /*
        @brief 有無任一組件
        @param complexId [in] 組件混和編號
        */
        public bool HasAnyComponents(int complexId)
        {
            return (ComplexId & complexId) > 0;
        }

        /*
        @brief 創建組件
        @param id [in] 組件編號
        */
        public IComponent CreateComponent(int id, Type type)
        {
            var pool = GetComponentsPool(id);
            var component = pool.Count > 0 ? pool.Pop() : (IComponent)Activator.CreateInstance(type);

            AddComponent(id, component);

            return component;
        }

        /*
        @brief 創建組件
        @param id [in] 組件編號
        */
        public T CreateComponent<T>(int id) where T : IComponent, new()
        {
            var pool = GetComponentsPool(id);
            var component = pool.Count > 0 ? (T)pool.Pop() : new T();

            AddComponent(id, component);

            return component;
        }

        /*
        @brief 新增參考
        @param owner [in] 對象
        */
        public void AddRef(object owner)
        {
            refCounter.AddRef(owner);
        }

        /*
        @brief 移除參考
        @param owner [in] 對象
        */
        public void RemoveRef(object owner)
        {
            refCounter.RemoveRef(owner);

            if (RefCount <= 0 && OnEntityRelease != null)
                OnEntityRelease(this);
        }

        /*
        @brief 
        */
        public override string ToString()
        {
            if (toStrCache != null)
                return toStrCache;

            if (toStrBuilder == null)
                toStrBuilder = new StringBuilder();

            toStrBuilder.Length = 0;

            // 開頭
            toStrBuilder.Append("entity_").Append(SerialId).Append("(");

            var last = maxComponents - 1;

            // 內容
            for (int i = 0; i < maxComponents; i++)
            {
                if (components[i] == null)
                    continue;

                toStrBuilder.Append(GetComponentName(i));
                toStrBuilder.Append(".");
                toStrBuilder.Append(GetComponentType(i));

                // 分割符號
                if (i < last)
                    toStrBuilder.Append(", ");
            }

            // 結尾
            toStrBuilder.Append(")");

            toStrCache = toStrBuilder.ToString();

            return toStrCache;
        }

        /*
        @brief 取得組件名稱
        @param id [in] 組件編號
        */
        private string GetComponentName(int id)
        {
            return debugInfo.ComponentNames[id];
        }

        /*
        @brief 取得組件類型
        @param id [in] 組件編號
        */
        private Type GetComponentType(int id)
        {
            return debugInfo.ComponentTypes[id];
        }
    }
}
