using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Weather.App.Common;
using Weather.App.ViewModel;
using Weather.Service.Implementations;
using Weather.Service.Message;
using Weather.Utils;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Weather.App
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        private UserService userService;
        private WeatherService weatherService;
        private SettingService settingService;
        private GetUserRespose userRespose;
        private GetUserCityRespose userCityRespose;
        private GetWeatherRespose weatherRespose;
        private GetWeatherTypeRespose weatherTypeRespose;
        private GetSettingAutoUpdateTimeRepose settingAutoUpdateTimeRepose;
        private string cityId = null;

        public MainPage()
        {
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;

            userService = UserService.GetInstance();
            weatherService = WeatherService.GetInstance();
            settingService = SettingService.GetInstance();
            userRespose = new GetUserRespose();
            userCityRespose = new GetUserCityRespose();
            weatherRespose = new GetWeatherRespose();
            weatherTypeRespose = new GetWeatherTypeRespose();
            settingAutoUpdateTimeRepose = new GetSettingAutoUpdateTimeRepose();

            this.InitializeComponent();
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

        #region 状态
        /// <summary>
        /// 使用在导航过程中传递的内容填充页。在从以前的会话
        /// 重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        /// 事件的来源；通常为 <see cref="NavigationHelper"/>。
        /// </param>
        /// <param name="e">事件数据，其中既提供在最初请求此页时传递给
        /// <see cref="Frame.Navigate(Type, Object)"/> 的导航参数，又提供
        /// 此页在以前会话期间保留的状态的
        /// 的字典。首次访问页面时，该状态将为 null。</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //通知
            //string toastText = "明天冷空气南下，请注意保暖";
            //ToastHelper.CreateToast(toastText);

        }



        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合序列化
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="sender">事件的来源；通常为 <see cref="NavigationHelper"/>。</param>
        ///<param name="e">提供要使用可序列化状态填充的空字典
        ///的事件数据。</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: 在此处保存页面的唯一状态。
        }
        #endregion

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
            VisualStateManager.GoToState(this, "MoreHide", false);
            cityId = e.Parameter == null ? "" : e.Parameter.ToString();
            this.navigationHelper.OnNavigatedTo(e);

            userCityRespose = await userService.GetUserCityAsync();

            if (userCityRespose == null)
            {
                Frame.Navigate(typeof(AddCityPage), 1);
                return;
            }
            else
            {
                if (cityId == "")
                {
                    cityId = userCityRespose.UserCities.FirstOrDefault().CityId;
                }
                userRespose = await userService.GetUserAsync();
                weatherTypeRespose = await weatherService.GetWeatherTypeAsync();
                await GetWeather(cityId, 0);
            }


        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
        #endregion


        #region 获取天气数据

        /// <summary>
        /// 获取天气数据
        /// </summary>
        /// <param name="cityId"></param>
        private async Task GetWeather(string cityId, int isRefresh)
        {

            Model.UserCity userCity = string.IsNullOrEmpty(cityId) ? userCityRespose.UserCities.FirstOrDefault(x => x.IsDefault == 1) : userCityRespose.UserCities.FirstOrDefault(x => x.CityId == cityId);

            IGetWeatherRequest weatherRequest = GetWeatherRequestFactory.CreateGetWeatherRequest(GetWeatherMode.CityId, userCity.CityId);

            //有网络
            if (NetHelper.IsNetworkAvailable())
            {
                if (userRespose.UserConfig.IsWifiUpdate == 0)
                {
                    weatherRespose = await GetWeatherAsync(isRefresh, userCity, weatherRequest);
                }
                else
                {
                    if (NetHelper.IsWifiConnection())
                    {
                        weatherRespose = await GetWeatherAsync(isRefresh, userCity, weatherRequest);
                    }
                    else
                    {
                        weatherRespose.result = null;
                        NotifyUser("Wifi未启动");
                    }
                }
            }
            else
            {
                weatherRespose = await weatherService.GetWeatherByClientAsync(userCity.CityId.ToString());
                NotifyUser("请开启网络，以更新最新天气数据");
            }

            if (weatherRespose != null)
            {
                var respose = weatherRespose.result.FirstOrDefault();

                ViewModel.HomePageModel homePageModel = new ViewModel.HomePageModel();

                var aqi = respose.aqi;
                var now = respose.now;
                var basic = respose.basic;
                var daily_forecast = respose.daily_forecast;
                var hourly_forecast = respose.hourly_forecast;
                if (aqi != null)
                {
                    homePageModel.Aqi = "空气质量: " + aqi.city.qlty;
                }
                homePageModel.City = basic.city;
                homePageModel.Daytmp = daily_forecast.FirstOrDefault().tmp.min + "° / " + daily_forecast.FirstOrDefault().tmp.max + "°";
                homePageModel.Hum = now.hum + " %";
                homePageModel.Pres = now.pres + " hPa";
                homePageModel.Tmp = now.tmp + "°";
                homePageModel.Txt = now.cond.txt;
                homePageModel.Update = basic.update.loc + " 发布";
                homePageModel.Vis = now.vis + " km";
                homePageModel.Wind = now.wind.dir + " " + now.wind.sc + " 级";
                homePageModel.WeatherType = weatherTypeRespose.WeatherTypes.FirstOrDefault(x => x.Code == now.cond.code);

                List<ViewModel.DailyItem> dailyList = new List<ViewModel.DailyItem>();
                foreach (var item in daily_forecast)
                {
                    DailyItem daily = new DailyItem()
                    {
                        Date = item.date,
                        Image = weatherTypeRespose.WeatherTypes.FirstOrDefault(x => x.Code == item.cond.code_d).TomorrowPic,
                        Tmp = item.tmp.min + "° / " + item.tmp.max + "°",
                        Txt = item.cond.txt_d
                    };
                    dailyList.Add(daily);
                }

                homePageModel.DailyList = dailyList;

                List<ViewModel.HourlyItem> hourlyList = new List<ViewModel.HourlyItem>();
                foreach (var item in hourly_forecast)
                {
                    HourlyItem hourly = new HourlyItem()
                    {
                        Date = DateTime.Parse(item.date).ToString("HH:mm"),
                        Hum = item.hum + " %",
                        Tmp = item.tmp + "°",
                        Wind = item.wind.dir + " " + item.wind.sc
                    };
                    hourlyList.Add(hourly);
                }
                homePageModel.HourlyList = hourlyList;

                LayoutRoot.DataContext = homePageModel;
            }
            else
            {
                NotifyUser("天气数据获取失败");
            }
        }

        private void NotifyUser(string v)
        {
            JMessbox.JMessBox jb = new JMessbox.JMessBox(v);
            jb.Show();
        }

        private async Task<GetWeatherRespose> GetWeatherAsync(int isRefresh, Model.UserCity userCity, IGetWeatherRequest weatherRequest)
        {
            GetWeatherRespose weatherRespose = new GetWeatherRespose();

            if (isRefresh == 1)
            {
                weatherRespose = await weatherService.GetWeatherAsync(weatherRequest);
                await weatherService.SaveWeather(weatherRespose, userCity.CityId.ToString());
            }
            else
            {
                string filePath = StringHelper.GetTodayFilePath(userCity.CityId);

                if (!await FileHelper.IsExistFileAsync(filePath))
                {
                    //不存在当天的天气数据，就从网络获取数据
                    weatherRespose = await weatherService.GetWeatherAsync(weatherRequest);
                    await DeleteFile(userCity.CityId);
                    await weatherService.SaveWeather(weatherRespose, userCity.CityId.ToString());
                }
                else
                {
                    weatherRespose = await weatherService.GetWeatherByClientAsync(userCity.CityId.ToString());
                }
            }
            return weatherRespose;
        }


        #endregion
        #region 删除过期文件

        /// <summary>
        /// 删除过期文件
        /// </summary>
        /// <param name="cityId"></param>
        /// <returns></returns>
        private async Task DeleteFile(string cityId)
        {
            string fileName = cityId + "_" + DateTime.Now.AddDays(-1).ToString("yyyyMMdd") + ".json";
            string filePath = "Temp\\" + fileName;
            bool x = await FileHelper.IsExistFileAsync(filePath);
            if (x)
            {
                await FileHelper.DeleteFileAsync(filePath);

            }
        }
        #endregion

        private void BTNShowMore_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as ToggleButton).IsChecked)
            {
                VisualStateManager.GoToState(this, "MoreShow", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "MoreHide", false);
            }
        }
    }
}
