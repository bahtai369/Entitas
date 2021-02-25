using System;
using System.Collections.Generic;
using System.Text;

namespace Kent.Entitas
{
    /*
    @brief 系統
    */
    public interface ISystem { }

    /*
    @brief 設定管理器的系統
    */
    public interface ISetMgrSystem : ISystem
    {
        /*
        @brief 設定管理器
        */
        void SetMgr(IManager mgr);
    }

    /*
    @brief 做初始化的系統
    */
    public interface IInitSystem : ISystem
    {
        /*
        @brief 初始化
        */
        void Init();
    }

    /*
    @brief 做持續更新的系統
    */
    public interface IUpdateSystem : ISystem
    {
        /*
        @brief 持續更新
        */
        void Update();
    }

    /*
    @brief 做釋放的系統
    */
    public interface IFreeSystem : ISystem
    {
        /*
        @brief 釋放
        */
        void Free();
    }

    /*
    @brief 做重載的系統
    */
    public interface IReloadSystem : ISystem
    {
        /*
        @brief 重載
        */
        void Reload();
    }
}
