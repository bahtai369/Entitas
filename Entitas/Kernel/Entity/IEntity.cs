using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 實體變化
    @param id [in] 組件編號
    */
    public delegate void EntityChange(IEntity entity, int id, IComponent Component);

    /*
    @brief 實體更新
    @param id [in] 組件編號
    */
    public delegate void EntityUpdate(IEntity entity, int id, IComponent oldComponent, IComponent newComponent);

    /*
    @brief 實體事件
    */
    public delegate void EntityEvent(IEntity entity);

    /*
    @brief 實體
    */
    public interface IEntity : IRefCounter
    {
        /*
        @brief 各種委派
        */
        event EntityChange OnComponentAdd;     // 新增組件
        event EntityChange OnComponentRemove;  // 移除組件
        event EntityUpdate OnComponentUpdate;  // 組件更新
        event EntityEvent OnEntityRelease;     // 實體參考歸零
        event EntityEvent OnEntityDestroy;     // 實體銷毀

        /*
        @brief 是否啟用
        */
        bool IsEnabled { get; }

        /*
        @brief 流水號
        */
        int SerialId { get; }

        /*
        @brief 組件混和編號
        */
        int ComplexId { get; }

        /*
        @brief 初始化
        @param serialId [in] 流水號
        @param maxComponents [in] 組件數量上限, 不可超過32
        @param componentsPools [in] 組件池
        @param debugInfo [in] 除錯資訊
        @note 新創時使用
        */
        void Init(int serialId, int maxComponents, Stack<IComponent>[] componentsPools, DebugInfo debugInfo);

        /*
        @brief 初始化
        @param serialId [in] 流水號
        @note 復用時使用
        */
        void Init(int serialId);

        /*
        @brief 銷毀
        */
        void Destroy();

        /*
        @brief 銷毀
        @note 限定管理器使用
        */
        void DestroyInner();

        /*
        @brief 當參考歸零時做的清除
        @note 限定管理器使用
        */
        void ReleaseClear();

        /*
        @brief 新增組件
        @param id [in] 組件編號
        */
        void AddComponent(int id, IComponent component);

        /*
        @brief 移除組件
        @param id [in] 組件編號
        */
        void RemoveComponent(int id);

        /*
        @brief 移除所有組件
        */
        void RemoveAllComponents();

        /*
        @brief 更新組件
        @param id [in] 組件編號
        */
        void UpdateComponent(int id, IComponent component);

        /*
        @brief 取得組件
        @param id [in] 組件編號
        */
        IComponent GetComponent(int id);

        /*
        @brief 取得所有組件
        */
        IComponent[] GetAllComponents();

        /*
        @brief 有無此組件
        @param id [in] 組件編號
        */
        bool HasComponent(int id);

        /*
        @brief 有無所有組件
        @param complexId [in] 組件混和編號
        */
        bool HasAllComponents(int complexId);

        /*
        @brief 有無任一組件
        @param complexId [in] 組件混和編號
        */
        bool HasAnyComponents(int complexId);

        /*
        @brief 創建組件
        @param id [in] 組件編號
        */
        IComponent CreateComponent(int id, Type type);

        /*
        @brief 創建組件
        @param id [in] 組件編號
        */
        T CreateComponent<T>(int id) where T : IComponent, new();
    }
}
