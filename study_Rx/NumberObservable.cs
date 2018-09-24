using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace study_Rx
{
    /// <summary>
    /// 監視対象
    /// </summary>
    class NumberObservable : IObservable<int>
    {
        /// <summary>
        /// 自身の監視者リスト
        /// </summary>
        private List<IObserver<int>> observers = new List<IObserver<int>>();

        /// <summary>
        /// 監視者に対して値を通知する。
        /// </summary>
        /// <param name="value"></param>
        public void Execute(int value)
        {
            // エラー判定処理
            if (value == 0)
            {
                // 値0の場合はエラーとする。
                foreach (var obs in observers)
                {
                    obs.OnError(new Exception("value is 0"));
                }
                // エラーが起きたので処理は終了 
                this.observers.Clear();
                return;
            }

            // 監視者に対して通知
            foreach (var obs in observers)
            {
                obs.OnNext(value);
            }
        }

        /// <summary>
        /// 完了通知 
        /// </summary>
        public void Completed()
        {
            foreach (var obs in observers)
            {
                obs.OnCompleted();
            }
            // 完了したので監視してる人たちをクリア 
            this.observers.Clear();
        }


        public IDisposable Subscribe(IObserver<int> observer)
        {
            this.observers.Add(observer);
            return new RemoveListDisposable(observers, observer);
        }

        // Disposeが呼ばれたらobserverを監視対象から削除する 
        private class RemoveListDisposable : IDisposable
        {
            private List<IObserver<int>> observers = new List<IObserver<int>>();
            private IObserver<int> observer;

            public RemoveListDisposable(List<IObserver<int>> observers, IObserver<int> observer)
            {
                this.observers = observers;
                this.observer = observer;
            }
            public void Dispose()
            {
                if (this.observers == null)
                {
                    return;
                }
                if (observers.IndexOf(observer) != -1)
                {
                    this.observers.Remove(observer);
                }
                this.observers = null;
                this.observer = null;
            }
        }
    }
}
