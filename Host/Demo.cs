using System;
using System.Collections.Generic;
using System.Text;
using Kent.Entitas;

namespace Host
{
    /*
    @brief 演示組件a
    */
    public class ComponentA : IComponent
    {
        /*
        @brief 演示組件a索引
        */
        public const int Id = 0;

        /*
        @brief 訊息
        */
        public string Msg { get; set; }
    }

    /*
    @brief 演示組件b
    */
    public class ComponentB : IComponent
    {
        /*
        @brief 演示組件b索引
        */
        public const int Id = 1;
    }

    /*
    @brief 演示實體
    */
    public class DemoEntity : Entity { }

    /*
    @brief 演示系統
    */
    public class DemoSystem : ISetMgrSystem, IInitSystem
    {      
        /*
        @brief 實例
        */
        public static DemoSystem Inst { get; } = new DemoSystem();

        /*
        @brief 管理器
        */
        private DemoMgr mgr;

        /*
        @brief 組件a
        */
        private ComponentA componentA;

        /*
        @brief 組件b
        */
        private ComponentB componentB;

        /*
        @brief 設定管理器
        */
        public void SetMgr(IManager mgr)
        {
            this.mgr = (DemoMgr)mgr;
        }

        /*
        @brief 初始
        */
        public void Init()
        {
            Console.WriteLine("entitas demo start");

            // 
            var flags = new FlagsUtil();
            flags[ComponentA.Id] = true;
            flags[ComponentB.Id] = true;

            // 取得關心的群組 
            var matcher = Matcher<DemoEntity>.GetAllMatcher(flags.Id);
            var group = mgr.GetGroup(matcher);

            // 當群組實體新增
            group.OnEntityAdd += (IGroup<DemoEntity> temp_group, DemoEntity temp_entity, int id, IComponent component) =>
            {
                Console.WriteLine($"demo entity: {id} add to group");
            };

            // 當群組實體移除
            group.OnEntityRemove += (IGroup<DemoEntity> temp_group, DemoEntity temp_entity, int id, IComponent component) =>
            {
                Console.WriteLine($"demo entity: {id} remove from group");
            };

            // 新增實體並增加組件
            var entity = mgr.CreateEntity();
            componentA = entity.CreateComponent<ComponentA>(ComponentA.Id);
            componentB = entity.CreateComponent<ComponentB>(ComponentB.Id);

            // 移除實體
            entity.RemoveComponent(ComponentB.Id);
            componentB = null;

            // 
            SetMsg("entitas demo end");

            //
            ShowMsg();
        }

        /*
        @brief 設定訊息
        */
        public void SetMsg(string msg)
        {
            componentA.Msg = msg;
        }

        /*
        @brief 顯示訊息
        */
        public void ShowMsg()
        {
            Console.WriteLine(componentA.Msg);
        }
    }

    /*
    @brief 演示管理器
    */
    public class DemoMgr : Manager<DemoEntity>
    {
        /*
        @brief 
        @note 宣告實體創建方法
        */
        public DemoMgr() : base(() => new DemoEntity(), 32) { }
    }
}
