using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace study_Rx
{
    class Program
    {
        static void Main(string[] args)
        {
            // ファクトリメソッド
            //---------------------------------
            // Return:特定の値
            Main1();

            // Repeat:繰り返し
            Main2();

            // Range:指定範囲の値を生成
            Main3();

            // Generate:For文
            Main4();
            Main4_1();

            // Defer
            Main5();

            // Create:自分で発行されるイベントを定義できる。
            Main6();

            // Throw:OnErrorを意図的に実行できる。
            Main7();
        }


        static void Main1()
        {
            // ソースの生成
            var source = Observable.Return<int>(10);

            // 購読の開始
            IDisposable subscription = source.Subscribe(
                (int i)       => { Console.WriteLine("OnNext():" + i.ToString()); },
                (Exception e) => { Console.WriteLine("OnError():" + e.Message);   },
                ()            => { Console.WriteLine("OnCompleted()");            }
            );

            // 購読の停止
            subscription.Dispose();
        }

        static void Main2()
        {
            // ソースの生成
            var source = Observable.Repeat<int>(2, 5);

            // 購読の開始
            IDisposable subscription = source.Subscribe(
                (int i)       => { Console.WriteLine("OnNext():" + i.ToString()); },
                (Exception e) => { Console.WriteLine("OnError():" + e.Message);   },
                ()            => { Console.WriteLine("OnCompleted()");            }
            );
            IDisposable subscription2 = source.Subscribe(
                (int i)       => { Console.WriteLine("OnNext():" + i.ToString()); },
                (Exception e) => { Console.WriteLine("OnError():" + e.Message);   },
                ()            => { Console.WriteLine("OnCompleted()");            }
            );

            // 購読の停止
            subscription.Dispose(); 
        }

        static void Main3()
        {
            IObservable<int> source = Observable.Range(1, 10);

            IDisposable subscription = source.Subscribe(
                (int i) => { Console.WriteLine("OnNext():" + i.ToString()); },
                (Exception e) => { Console.WriteLine("OnError():" + e.Message); },
                () => { Console.WriteLine("OnCompleted()"); }
                );

            subscription.Dispose();
        }

        static void Main4()
        {
            var source = Observable.Generate<int, int>(
                0,
                (int i) => { return (i < 10); },
                (int i) => { return ++i; },
                (int i) => { return (i * i); }
            );

            //var source2 = Observable.Generate<int, int>(
            //    0,
            //    i => i < 10,
            //    i => i++,
            //    i => i * i
            //);

            source.Subscribe(
                (int i) => { Console.WriteLine("OnNext():" + i.ToString()); },
                (Exception e) => { Console.WriteLine("OnError():" + e.Message); },
                () => { Console.WriteLine("OnException()"); });
        }

        static void Main4_1()
        {
            //Console.WriteLine("*** Start1 ***");
            //var source = Observable.Generate<int, int>(
            //    0, i => i < 10, i => AsyncFunc(i), i => i, Scheduler.Default)
            //    .Subscribe(x  => Console.WriteLine(x.ToString()),
            //               () => Console.WriteLine("--- END#1 ---"));


            Console.WriteLine("*** Start2 ***");
            var source2 = Observable.Generate<int, int>(
                0, i => i < 2, i => ++i, i => AsyncFunc2(i), Scheduler.Default)
                .Subscribe(x => Console.WriteLine(x.ToString()),
                          () => Console.WriteLine("--- END#2 ---"));


            Console.WriteLine("*** End main ***");
            Console.ReadKey();

        }

        private static int AsyncFunc(int i)
        {
            Console.WriteLine("Start....");
            //三秒待つ
            Thread.Sleep(3000);
            Console.WriteLine("{0} executed", i);
            //インクリメントする
            return ++i;
        }

        private static int AsyncFunc2(int i)
        {
            Console.WriteLine("Start....");
            //三秒待つ
            Thread.Sleep(3000);
            Console.WriteLine("{0} executed", i);
            return i;
        }

        public static void Main5()
        {
            // Defer,,,わからない。
        }

        public static void Main6()
        {
            var source = Observable.Create<int>((IObserver<int> observer) =>
            {
                Console.WriteLine("Start Create method");
                observer.OnNext(1);
                observer.OnNext(2);
                observer.OnNext(7);
                observer.OnCompleted();

                Console.WriteLine("Start Create method");
                return new Action(() => { Console.WriteLine("Disposable action"); });
            });

            var subscription = source.Subscribe(
                (int i)       => { Console.WriteLine("OnNext():" + i.ToString()); },
                (Exception e) => { Console.WriteLine(e.Message);                  },
                ()            => { Console.WriteLine("OnCompleted()");            });

            subscription.Dispose();
        }

        public static void Main7()
        {
            var source = Observable.Throw<double>(new Exception("Throw test"));

            var subscription = source.Subscribe(
                i => Console.WriteLine(i),
                e => Console.WriteLine(e.Message),
                () => Console.WriteLine("OnCompleted()"));

            subscription.Dispose();
        }

    }
}
