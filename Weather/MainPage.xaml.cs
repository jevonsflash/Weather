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
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Weather.App
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _timer;
        private int CurrentTimeSection = 1;

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

        private UserService userService;
        private WeatherService weatherService;
        private SettingService settingService;
        private ColorService colorService = null;

        private GetUserRespose userRespose;
        private GetUserCityRespose userCityRespose;
        private GetWeatherRespose weatherRespose;
        private GetWeatherTypeRespose weatherTypeRespose;
        private GetSettingAutoUpdateTimeRepose settingAutoUpdateTimeRepose;
        private string cityId = null;
        private HomePageModel homePageModel;
        private GetColorRespose colorResponse = null;


        public MainPage()
        {
            this.navigationHelper = new NavigationHelper(this);
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;

            userService = UserService.GetInstance();
            weatherService = WeatherService.GetInstance();
            settingService = SettingService.GetInstance();
            colorService = ColorService.GetInstance();

            userRespose = new GetUserRespose();
            userCityRespose = new GetUserCityRespose();
            weatherRespose = new GetWeatherRespose();
            weatherTypeRespose = new GetWeatherTypeRespose();
            settingAutoUpdateTimeRepose = new GetSettingAutoUpdateTimeRepose();
            colorResponse = new GetColorRespose();
            homePageModel = new HomePageModel();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(5);
            _timer.Tick += _timer_Tick;
            _timer.Start();

            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void _timer_Tick(object sender, object e)
        {
            int timeSection = TimeHelper.GetNowSection();
            if (timeSection != CurrentTimeSection)
            {
                ChangeBgColor(timeSection);

            }

        }


        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            colorResponse = await colorService.GetColorAsync();

            int timeSection = TimeHelper.GetNowSection();
            if (timeSection != CurrentTimeSection)
            {
                ChangeBgColor(timeSection);

            }

            this.TBClock.DataContext = new ClockViewModel();
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
            StatusBar sb = StatusBar.GetForCurrentView();
            await sb.HideAsync();

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
                homePageModel.Hum = "湿度:  " + now.hum + " %";
                homePageModel.Pres = now.pres + " hPa";
                homePageModel.Tmp = now.tmp + "°";
                homePageModel.Txt = now.cond.txt;
                homePageModel.Update = basic.update.loc + " 发布";
                homePageModel.Vis = "能见度:  " + now.vis + " km";
                homePageModel.Wind = now.wind.dir + " " + now.wind.sc + " 级";
                homePageModel.WeatherType = weatherTypeRespose.WeatherTypes.FirstOrDefault(x => x.Code == now.cond.code);

                List<ViewModel.DailyItem> dailyList = new List<ViewModel.DailyItem>();
                foreach (var item in daily_forecast)
                {
                    DailyItem daily = new DailyItem()
                    {
                        Date = GetDateStr(item.date),
                        Image = weatherTypeRespose.WeatherTypes.FirstOrDefault(x => x.Code == item.cond.code_d).TileWidePic,
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
                LayoutRoot.DataContext = null;
                LayoutRoot.DataContext = homePageModel;
            }
            else
            {
                NotifyUser("天气数据获取失败");
            }
        }

        private string GetDateStr(string date)
        {
            string result = string.Empty;
            DateTime DateOrg = DateTime.Parse(date);
            TimeSpan span = DateOrg.Date - DateTime.Now.Date;
            if (span == TimeSpan.Zero)
            {
                result = "今天";
            }
            else if (span == TimeSpan.FromDays(1.0))
            {
                result = "明天";

            }
            else if (span == TimeSpan.FromDays(2.0))
            {
                result = "后天";

            }
            else
            {
                result = DateOrg.ToString("MM 月 dd 日");
            }
            return result;
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

        /// <summary>
        /// 更改主页背景颜色
        /// </summary>
        /// <param name="timeSection">当前时间段</param>
        private void ChangeBgColor(int timeSection)
        {
            string colorStr = colorResponse.UserColors.FirstOrDefault(c => c.isSelected == "1").SingleColors[timeSection - 1].colorStr;
            Color bgColor = Utils.ColorHelper.GetColorFromHEX(colorStr);
            this.bgColorAnimation.To = Colors.Brown;
            bgStoryboard.Begin();
        }

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

        private void BTNFeature_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FeatureWeatherPage), homePageModel);

        }

        private void BTNToday_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TodayWeatherPage), homePageModel);

        }

        private void BTNSetting_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingPage), homePageModel);

        }
    }
}
