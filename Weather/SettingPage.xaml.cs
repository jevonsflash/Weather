using Weather.App.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Weather.Utils;
using Weather.Service.Message;
using Weather.Service.Implementations;
using Windows.Phone.UI.Input;
using System.Threading.Tasks;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Weather.App
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private SettingService settingService = null;
        private UserService userService = null;
        private ColorService colorService = null;

        private GetSettingSwitchesRespose switchesRespose = null;
        private GetSettingAutoUpdateTimeRepose autoUpdateTimeRepose = null;
        private GetUserRespose userRespose = null;
        private GetColorRespose colorResponse = null;
        public SettingPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            settingService = SettingService.GetInstance();
            userService = UserService.GetInstance();
            colorService = ColorService.GetInstance();
            switchesRespose = new GetSettingSwitchesRespose();
            autoUpdateTimeRepose = new GetSettingAutoUpdateTimeRepose();
            userRespose = new GetUserRespose();
            colorResponse = new GetColorRespose();
        }

        /// <summary>
        /// 获取与此 <see cref="Page"/> 关联的 <see cref="NavigationHelper"/>。
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// 获取此 <see cref="Page"/> 的视图模型。
        /// 可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }


        #region NavigationHelper 注册

        /// <summary>
        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// <para>
        /// 应将页面特有的逻辑放入用于
        /// <see cref="NavigationHelper.LoadState"/>
        /// 和 <see cref="NavigationHelper.SaveState"/> 的事件处理程序中。
        /// 除了在会话期间保留的页面状态之外
        /// LoadState 方法中还提供导航参数。
        /// </para>
        /// </summary>
        /// <param name="e">提供导航方法数据和
        /// 无法取消导航请求的事件处理程序。</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            await LoadData();

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async Task LoadData()
        {
            switchesRespose = settingService.GetSettingSwitches();
            autoUpdateTimeRepose = settingService.GetSettingAutoUpdateTime();
            userRespose = await userService.GetUserAsync();

            ViewModel.AutoUpdateSettingPage autoUpdateSettingPage = new ViewModel.AutoUpdateSettingPage()
            {
                Switches = switchesRespose.Switches,
                AutoUpdateTimes = autoUpdateTimeRepose.AutoUpdateTimes
            };
            ViewModel.GeneralSettingPage generalSettingPage = new ViewModel.GeneralSettingPage()
            {
                Switches = switchesRespose.Switches
            };
            ViewModel.SettingPage settingPage = new ViewModel.SettingPage()
            {
                AutoUpdateSettingPage = autoUpdateSettingPage,
                GeneralSettingPage = generalSettingPage,
                UserConfig = userRespose.UserConfig

            };
            colorResponse = await colorService.GetColorAsync();
            this.LVMood.DataContext = colorResponse.UserColors;
            
            this.LayoutRoot.DataContext = null;
            this.LayoutRoot.DataContext = settingPage;
        }


        /// <summary>
        /// 仅在WI-FI建立后更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cbbIsWifiUpdate_DropDownClosed(object sender, object e)
        {
            userRespose.UserConfig.IsWifiUpdate = int.Parse(cbbIsWifiUpdate.SelectedValue.ToString());
            await userService.SaveUser(userRespose);
        }


        /// <summary>
        /// 是否允许自动更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cbbIsAutoUpdateForCity_DropDownClosed(object sender, object e)
        {
            userRespose.UserConfig.IsAutoUpdateForCity = int.Parse(cbbIsAutoUpdateForCity.SelectedValue.ToString());
            await userService.SaveUser(userRespose);
        }

        /// <summary>
        /// 自动更新频率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void cbbAutoUpdateTime_DropDownClosed(object sender, object e)
        {
            userRespose.UserConfig.AutoUpdateTime = int.Parse(cbbAutoUpdateTime.SelectedValue.ToString());
            await userService.SaveUser(userRespose);
        }

        private void BTNCityChoose_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddCityPage));
        }

        private void LVMood_ItemClick(object sender, ItemClickEventArgs e)
        {
        }
    }
}
