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
using System.Collections.ObjectModel;
using Weather.Service.Message;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Weather.Service.Implementations;
using Weather.Utils;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkID=390556 上有介绍

namespace Weather.App
{
    /// <summary>
    /// 可独立使用或用于导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AddCityPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private ViewModel.SelectCityPage page = null;
        private CityService cityService = null;
        private UserService userService = null;
        private GetCityRespose resposeCities = null;
        private GetHotCityRespose resposeHotCities = null;
        private GetUserCityRespose resposeUserCity = null;



        public AddCityPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;


            cityService = CityService.GetInstance();
            userService = UserService.GetInstance();
            resposeCities = new GetCityRespose();
            resposeHotCities = new GetHotCityRespose();
            resposeUserCity = new GetUserCityRespose();

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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);

            // 获取用户城市数据
            resposeUserCity = await userService.GetUserCityAsync();

            // 获取城市
            resposeCities = await cityService.GetCityAsync();
            // 获取人们城市
            resposeHotCities = await cityService.GetHotCityAsync(); ;

            // 数据绑定
            page = new ViewModel.SelectCityPage();

            page.Cities = resposeCities.Cities;

            page.HotCities = resposeHotCities.Cities;

            LayoutRoot.DataContext = page;

        }

        #endregion


        private void asbCity_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            string userInput = sender.Text.Trim();
            if (args.Reason == AutoSuggestionBoxTextChangeReason.SuggestionChosen)
            {
                return;
            }
            if (userInput.Length == 0)
            {
                return;
            }
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                // 判断输入值是否是英文
                if (Weather.Utils.StringHelper.IsEN(userInput))
                {
                    sender.ItemsSource = from d in page.Cities
                                         where d.DistrictEN.Contains(userInput)
                                         select d.DistrictZH;
                }
                else
                {
                    sender.ItemsSource = from d in page.Cities
                                         where d.DistrictZH.Contains(userInput)
                                         select d.DistrictZH;
                }

            }
        }

        private async void asbCity_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            asbCity.Text = "";
            string cityname = args.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(cityname))
            {
                bool isSuccess = await UpdateUserCity(cityname);
                if (isSuccess)
                {
                    if (Frame.CanGoBack)
                    {
                        Frame.GoBack();
                        return;

                    }
                    else
                    {
                        Frame.Navigate(typeof(MainPage));
                        return;
                    }
                }
            }

        }

        private async void gvCity_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {

                Model.WeatherCity city = (Model.WeatherCity)e.ClickedItem;
                if (!string.IsNullOrEmpty(city.DistrictZH))
                {
                    bool isSuccess = await UpdateUserCity(city.DistrictZH);
                    if (isSuccess)
                    {
                        if (Frame.CanGoBack)
                        {
                            Frame.GoBack();
                            return;

                        }
                        else
                        {
                            Frame.Navigate(typeof(MainPage));
                            return;
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserCity(string cityName)
        {
            bool isAdd = false;

            Model.UserCity userCity = new Model.UserCity()
            {
                CityId = (from c in page.Cities
                          where c.DistrictZH == cityName
                          select c.ID).FirstOrDefault(),
                AddTime = DateTime.Now,
                CityName = cityName.Trim(),
                IsDefault = 1
            };
            GetUserCityRespose respose = new GetUserCityRespose();
            List<Model.UserCity> userCityList = new List<Model.UserCity>();
            userCityList.Add(userCity);
            respose.UserCities = userCityList;
            await userService.SaveUserCity(respose);
            isAdd = true;

            return isAdd;
        }
        private void NotifyUser(string v)
        {
            JMessbox.JMessBox jb = new JMessbox.JMessBox(v);
            jb.Show();
        }

    }
}
