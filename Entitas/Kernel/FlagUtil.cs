using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;

namespace Kent.Entitas
{
    /*
    @brief 旗標工具
    @note 只可容納32個開關(0~31)
    */
    public class FlagUtil
    {
        /*
        @brief 
        */
        private const int MaxSize = 32;

        /*
        @brief 遮罩
        @note 用來取單一個bit開關
        */
        private static readonly int[] masks = new int[MaxSize];

        /*
        @brief 
        */
        private BitVector32 depend;

        /*
        @brief 
        */
        public int Id { get { return depend.Data; } }

        /*
        @brief 
        */
        public bool this[int idx]
        {
            get
            {
                return MaxSize > idx ? depend[masks[idx]] : false;
            }
            set
            {
                if (MaxSize > idx)
                    depend[masks[idx]] = value;
            }
        }

        /*
        @brief 
        */
        static FlagUtil()
        {
            int temp = 0;

            for (int i = 0; i < MaxSize; i++)
            {
                masks[i] = BitVector32.CreateMask(temp);
                temp = masks[i];
            }
        }

        /*
        @brief 
        @note 會複製開關狀態
        */
        public FlagUtil(FlagUtil src)
        {
            depend = new BitVector32(src.depend);
        }

        /*
        @brief 
        @note 會複製開關狀態
        */
        public FlagUtil(BitVector32 src)
        {
            depend = new BitVector32(src);
        }

        /*
        @brief 
        @param value [in] 用二進位轉十進位方式設定初始開關狀態
        */
        public FlagUtil(int value = 0)
        {
            depend = new BitVector32(value);
        }

        /*
        @brief 清除所有開關
        @param isOpen [in] 會將所有開關都設定成此狀態
        */
        public void Clear(bool isOpen = true)
        {
            depend[int.MaxValue] = isOpen;
        }

        /*
        @brief 同時取多個開關
        @param value [in] 用二進位轉十進位方式傳入要取得的開關
        @return 要所有開關皆為true才會回傳true, 不然就是回傳false
        */
        public bool Get(int value)
        {
            return depend[value];
        }

        /*
        @brief 同時設定多個開關
        @param value [in] 用二進位轉十進位方式傳入要設定的開關
        */
        public void Set(int value, bool isOpen)
        {
            depend[value] = isOpen;
        }

        /*
        @brief 
        */
        public override int GetHashCode()
        {
            return Id;
        }

        /*
        @brief 
        */
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;

            return obj.GetHashCode() == GetHashCode();
        }

        /*
        @brief 
        */
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
