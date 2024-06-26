using System.Runtime.InteropServices;
using System.Windows;

namespace EdgeBrowser
{
    /// <summary>
    /// JS调用函数
    /// </summary>
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class JsInvoke
    {
        /// <summary>
        /// 退出应用程序
        /// </summary>
        public void ExitApp()
        {
            // 关闭程序
            Application.Current.Shutdown();
        }
    }
}