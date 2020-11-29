using System;
using System.Threading;

namespace MinecraftClient
{
    /// <summary>
    /// Allow easy timeout on pieces of code
    /// </summary>
    /// <remarks>
    /// By ORelio - (c) 2014 - CDDL 1.0
    /// </remarks>
    public class AutoTimeout
    {
        /// <summary>
        /// Perform the specified action with specified timeout
        /// </summary>
        /// <param name="action">操作运行</param>
        /// <param name="timeout">最大超时(毫秒)</param>
        /// <returns>如果动作在没有超时的情况下完成，则为 True</returns>
        public static bool Perform(Action action, int timeout)
        {
            return Perform(action, TimeSpan.FromMilliseconds(timeout));
        }

        /// <summary>
        /// Perform the specified action with specified timeout
        /// </summary>
        /// <param name="action">操作运行</param>
        /// <param name="timeout">最大超时</param>
        /// <returns>如果动作在没有超时的情况下完成，则为 True</returns>
        public static bool Perform(Action action, TimeSpan timeout)
        {
            Thread thread = new Thread(new ThreadStart(action));
            thread.Start();
            bool success = thread.Join(timeout);
            if (!success)
                thread.Abort();
            return success;
        }
    }
}