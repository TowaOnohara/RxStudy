using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace study_Rx
{
    class Program
    {
        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (false) { 
            // Ovserberパターン
            Main0();

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

            // HOT and COLD
            Main8_COLD();
            Main8_HOT();
        }


        static void Main0()
        {
            // 監視されるオブジェクトを生成
            var source = new NumberObservable();
            // 監視者を生成
            var sbscriber1 = source.Subscribe(new PrintObserver());
            var sbscriber2 = source.Subscribe(new PrintObserver());

            // 監視される人の処理を実行 
            Console.WriteLine("## Execute(1)");
            source.Execute(1);

            // 片方を監視する人から解雇 
            Console.WriteLine("## Dispose");
            sbscriber2.Dispose();
            // 再度処理を実行 
            Console.WriteLine("## Execute(2)");
            source.Execute(2);

            // エラーを起こしてみる 
            Console.WriteLine("## Execute(0)");
            source.Execute(0);

            // 完了通知 
            // もう1つ監視役を追加して完了通知を行う 
            var sbscriber3 = source.Subscribe(new PrintObserver());
            Console.WriteLine("## Completed"); source.Completed();
        }
        static void Main0_1()
        {
            // 監視されるオブジェクトを生成
            var source = new NumberObservable();
            source.Subscribe(
                (value) => { Console.WriteLine("OnNext({0}) called", value);            },
                (e)     => { Console.WriteLine("OnError(Msg:{0}) called", e.Message);   },
                ()      => { Console.WriteLine("OnCompleted() called");                 }
            );

        }

        static void Main1()
        {
            // ソースの生成
            var source = Observable.Return<int>(10);

            // 購読の開始
            IDisposable subscription = source.Delay(new TimeSpan(0,0,3)).Subscribe(
                (int i)       => { Console.WriteLine("OnNext():" + i.ToString()); },
                (Exception e) => { Console.WriteLine("OnError():" + e.Message);   },
                ()            => { Console.WriteLine("OnCompleted()");            }
            );
            Thread.Sleep(4000);

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

        /// <summary>
        /// HOT COLD
        /// </summary>
        public static void Main8_COLD()
        {
            // 1秒間隔で値を発行するIObservable<long>を生成
            var source = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            // 購読
            var subscription1 = source.Subscribe(
                i  => Console.WriteLine("{0:yyyy/MM/dd HH:mm:ss.FFF} 1##OnNext({1})", DateTime.Now, i),
                ex => Console.WriteLine("1##OnError({0})", ex.Message),
                () => Console.WriteLine("1##Completed()"));


            Thread.Sleep(3000);

            // 購読
            var subscription2 = source.Subscribe(
                i => Console.WriteLine("{0:yyyy/MM/dd HH:mm:ss.FFF} 2##OnNext({1})", DateTime.Now, i),
                ex => Console.WriteLine("2##OnError({0})", ex.Message),
                () => Console.WriteLine("2##Completed()"));


            Console.ReadLine();
            subscription1.Dispose();
            subscription2.Dispose();            
        }

        public static void Main8_HOT()
        {
            var timer = new System.Timers.Timer(1000);
            var source = Observable.FromEvent<ElapsedEventHandler, ElapsedEventArgs>(
                //h => (s, e) => h(e),
                (Action<ElapsedEventArgs> h) => 
                {
                    return ((s, e) => h(e));
                },
                h => timer.Elapsed += h,
                h => timer.Elapsed -= h);
         
            // タイマー開始 
            timer.Start();

            // 購読

            // 購読解除

        }


    }
}
