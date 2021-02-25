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
        @brief 群組
        */
        private IGroup<DemoEntity> group;

        /*
        @brief 組件a
        */
        private ComponentA component;

        /*
        @brief 處理群組相關
        */
        private void HandleGroup()
        {
            // 設定此系統關心那些組件
            var flags = new FlagsUtil();
            flags[ComponentA.Id] = true;
            flags[ComponentB.Id] = true;

            // 用組件混和編號取配對器
            var matcher = Matcher<DemoEntity>.GetAllMatcher(flags.Id);

            // 用配對器取群組
            group = mgr.GetGroup(matcher);
        }

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

            // 處理群組相關
            HandleGroup();

            // 設定群組的委派
            group.OnEntityAdd += OnEntityAddHandler;
            group.OnEntityRemove += OnEntityRemoveHandler;

            // 新增實體並增加組件
            var entity = mgr.CreateEntity();
            entity.CreateComponent<ComponentB>(ComponentB.Id);

            // 增加組件, 做完後會觸發加入群組
            entity.CreateComponent<ComponentA>(ComponentA.Id);            

            // 移除組件, 做完後會觸發退出群組
            entity.RemoveComponent(ComponentB.Id);

            // 設定組件的訊息
            SetMsg("entitas demo end");

            // 顯示組件的訊息
            ShowMsg();
        }

        /*
        @brief 設定組件的訊息
        */
        private void SetMsg(string msg)
        {
            if (msg != string.Empty)
                component.Msg = msg;
        }

        /*
        @brief 顯示組件的訊息
        */
        private void ShowMsg()
        {
            if (component.Msg != string.Empty)
                Console.WriteLine(component.Msg);
        }

        /*
        @brief 當實體加入到群組中
        */
        private void OnEntityAddHandler(IGroup<DemoEntity> group, DemoEntity entity)
        {
            component = (ComponentA)entity.GetComponent(ComponentA.Id);

            Console.WriteLine($"demo entity: {entity} add to group");                       
        }

        /*
        @brief 當實體從群組中移除
        */
        private void OnEntityRemoveHandler(IGroup<DemoEntity> group, DemoEntity entity)
        {
            component = null;

            Console.WriteLine($"demo entity: {entity} remove from group");
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
