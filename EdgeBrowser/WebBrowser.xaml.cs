using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Web.WebView2.Core;
using Path = System.IO.Path;

namespace EdgeBrowser
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WebBrowser : Window
    {
        #region Define

        /// <summary>
        /// 重新加载标志
        /// </summary>
        private static bool _reloadFlag;

        /// <summary>
        /// 访问URL地址,使用HTML静态文件或Web地址都可;
        /// </summary>
        private static readonly String _urlPath = Path.Combine(Directory.GetCurrentDirectory(), "WebView.Html");
        //private static readonly String _urlPath = "https://www.baidu.com/";
        #endregion

        public WebBrowser()
        {
            InitializeComponent();
            InitializeWebBrowser();
        }
        /// <summary>
        /// 初始化WebBrowser
        /// </summary>
        private async void InitializeWebBrowser()
        {
            // 初始化实例
            await Browser.EnsureCoreWebView2Async(null);
            // Js调用C#函数
            Browser.CoreWebView2.AddHostObjectToScript("webBrowser", new JsInvoke());
            // 注册全局变量webBrowser
            await Browser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
                "const webBrowser= window.chrome.webview.hostObjects.sync.webBrowser;");
            // 加载URL地址,Web地址或HTML静态地址都可
            Browser.CoreWebView2.Navigate(_urlPath);
            //监听窗口大小变化事件
            SizeChanged += (sender, args) =>
            {
                //更新WebView2控件大小适应新窗口尺寸
                Browser.Width = args.NewSize.Width;
                Browser.Height = args.NewSize.Height;
            };
        }
        private async void Browser_OnCoreWebView2InitializationCompleted(object sender, 
            CoreWebView2InitializationCompletedEventArgs eventArgs)
        {
            if (eventArgs.IsSuccess)
            {
                //设置LoadError事件
                Browser.CoreWebView2.NavigationCompleted += Browser_NavigationComplted;
                //监听网络可用性改变
                NetworkChange.NetworkAvailabilityChanged += Network_AcailabilityChanged;
                //初始化WebView2实例
                await Browser.EnsureCoreWebView2Async();
            }
        }
        /// <summary>
        /// WebView2导航功能
        /// </summary>
        /// <param name="serder"></param>
        /// <param name="eventArgs"></param>
        private void Browser_NavigationComplted(object serder, CoreWebView2NavigationCompletedEventArgs eventArgs)
        {
            //加载失败
            if (!eventArgs.IsSuccess)
            {
                //重新加载页面
                Browser.Reload();
            }
        }
        /// <summary>
        /// 网络状态改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Network_AcailabilityChanged(object sender, NetworkAvailabilityEventArgs args)
        {
            try
            {
                if (args.IsAvailable)
                {
                    if (!_reloadFlag)
                    {
                        /*
                         * 记录网络恢复日志
                         */
                        Console.WriteLine($"计算机网络已恢复",null);
                        Browser.CoreWebView2.Navigate(_urlPath);
                        //定义重新加载标志为True
                        _reloadFlag = true;
                    }
                   
                }
                else
                {
                    //定义重新加载标志为False
                    _reloadFlag = false;
                    /*
                     * 记录日志，提醒用户网络断开
                     */
                    Console.WriteLine($"计算机网络断开",null);  
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}