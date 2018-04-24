using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ThreadPool
{
    /// <summary>
    /// 共享的线程控制器
    /// made by C.S
    /// 2018.03.19
    /// </summary>
    public static class ThreadPoolMgrSimple
    {
        private static ThreadPoolMgr poolMgr;

        public static ThreadPoolMgr PoolMgr
        {
            get
            {
                if (poolMgr == null)
                    poolMgr = new ThreadPoolMgr(100, 10);
                return poolMgr;
            }

            set
            {
                poolMgr = value;
            }
        }



        /// <summary>
        /// 需要在提前初始化的的时候调用
        /// 如果已经被初始化过，则无法再次初始化
        /// </summary>
        /// <param name="maxThreadCount">最大线程池支持数</param>
        /// <param name="idleCount">待机线程数</param>
        /// <param name="weightQueueMaxWeight">权重队列的权重最多值</param>
        public static void Init(int maxThreadCount = 100, int idleCount = 10, int weightQueueMaxWeight = 5)
        {
            if(PoolMgr == null)
                PoolMgr = new ThreadPoolMgr(idleCount, maxThreadCount, weightQueueMaxWeight);
        }

        /// <summary>
        /// 开启一个优先级线程
        /// </summary>
        /// <param name="handler">线程方法</param>
        /// <param name="weight">线程优先级，越小越优先</param>
        /// <param name="errorHandler">线程崩溃回调方法</param>
        /// <param name="idHandler">线程id回传方法</param>
        public static void Start(ThreadPoolMgr.ThreadMainHandleWithOutObj handler, int weight = 2, ThreadPoolMgr.ErrorMessageHandle errorHandler = null, ThreadPoolMgr.GetThreadIdHandle idHandler = null)
        {
            PoolMgr.Start(handler, weight, errorHandler, idHandler);
        }
        /// <summary>
        /// 开启一个优先级线程
        /// </summary>
        /// <param name="handler">线程方法</param>
        /// <param name="obj">线程所带参数</param>
        /// <param name="weight">线程优先级，越小越优先</param>
        /// <param name="errorHandler">线程崩溃回调方法</param>
        /// <param name="idHandler">线程id回传方法</param>
        public static void Start(ThreadPoolMgr.ThreadMainHandleWithObj handler, object obj, int weight = 2, ThreadPoolMgr.ErrorMessageHandle errorHandler = null, ThreadPoolMgr.GetThreadIdHandle idHandler = null)
        {
            PoolMgr.Start(handler, obj, weight, errorHandler, idHandler);
        }

        /// <summary>
        /// 销毁线程池
        /// </summary>
        public static void Destory()
        {
            PoolMgr.Destory();
        }

        /// <summary>
        /// 获取线程池中等待队列的数量
        /// </summary>
        /// <returns></returns>
        public static int GetIdleCount()
        {
            return PoolMgr.GetIdleCount();
        }

        /// <summary>
        /// 获取正在执行的线程池中线程数量
        /// </summary>
        /// <returns></returns>
        public static int GetAliveCount()
        {
            return PoolMgr.GetAliveCount();
        }

        /// <summary>
        /// 获取已销毁的线程id列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetDestoryThreadNameList()
        {
            return PoolMgr.GetDestoryThreadNameList();
        }
        /// <summary>
        /// 获取所有的报错信息
        /// </summary>
        /// <returns>报错信息用数组的方式 id+报错信息</returns>
        public static string[,] GetErrorArr()
        {
            return PoolMgr.GetErrorArr();
        }
    }
}
