using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 系統集合
    @note 使用者要自行尋找合適的地方接上所有函式
    */
    public class SystemsSet : ISetMgrSystem, IInitSystem, IUpdateSystem, IFreeSystem, IReloadSystem
    {
        /*
        @brief 實例
        */
        public static SystemsSet Inst { get; } = new SystemsSet();

        /*
        @brief 各種系統
        */
        private readonly List<ISetMgrSystem> setMgrSystems = new List<ISetMgrSystem>();
        private readonly List<IInitSystem> initSystems = new List<IInitSystem>();
        private readonly List<IUpdateSystem> updateSystems = new List<IUpdateSystem>();
        private readonly List<IFreeSystem> freeSystems = new List<IFreeSystem>();
        private readonly List<IReloadSystem> reloadSystems = new List<IReloadSystem>();

        /*
        @brief 新增系統
        */
        public void AddSystem(ISystem system)
        {
            var setMgrSystem = system as ISetMgrSystem;

            if (setMgrSystem != null)
                setMgrSystems.Add(setMgrSystem);

            var initSystem = system as IInitSystem;

            if (initSystem != null)
                initSystems.Add(initSystem);

            var updateSystem = system as IUpdateSystem;

            if (updateSystem != null)
                updateSystems.Add(updateSystem);

            var freeSystem = system as IFreeSystem;

            if (freeSystem != null)
                freeSystems.Add(freeSystem);

            var reloadSystem = system as IReloadSystem;

            if (reloadSystem != null)
                reloadSystems.Add(reloadSystem);
        }

        /*
        @brief 設定管理器
        */
        public void SetMgr(IManager mgr)
        {
            foreach (var system in setMgrSystems)
                system.SetMgr(mgr);
        }

        /*
        @brief 初始化
        */
        public void Init()
        {
            foreach (var system in initSystems)
                system.Init();
        }

        /*
        @brief 持續更新
        */
        public void Update()
        {
            foreach (var system in updateSystems)
                system.Update();
        }

        /*
        @brief 釋放
        */
        public void Free()
        {
            foreach (var system in freeSystems)
                system.Free();
        }

        /*
        @brief 重載
        */
        public void Reload()
        {
            foreach (var system in reloadSystems)
                system.Reload();
        }
    }
}
