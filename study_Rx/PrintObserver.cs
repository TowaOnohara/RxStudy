using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace study_Rx
{
    /// <summary>
    /// 監視クラス
    /// </summary>
    class PrintObserver : IObserver<int>
    {
        /// <summary>
        /// 完了通知が来た時の処理
        /// </summary>
        public void OnCompleted()
        {
            Console.WriteLine("OnCompleted() called.");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("OnError() called. Msg:{0}", error.Message);
        }

        /// <summary>
        /// 監視対象から通知が来た時の処理
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(int value)
        {
            Console.WriteLine("OnNext({0}) Called", value);
        }
    }
}
