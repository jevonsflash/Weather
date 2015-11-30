using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Service.Message
{
    /// <summary>
    /// 根据城市名称获取天气
    /// </summary>
    public class GetWeatherByCityNameRequest : IGetWeatherRequest
    {
        #region Field

        private string _key = "c506e27d063345d4800889da480323b4";

        /// <summary>
        /// 城市名称
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// key
        /// </summary>
        public string key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }
        #endregion

        #region Method

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityname">城市名称</param>
        public GetWeatherByCityNameRequest(string city)
        {
            this.city = city;
        }

        /// <summary>
        /// 获取请求Url
        /// </summary>
        /// <returns></returns>
        public string GetRequestUrl()
        {
            return "https://api.heweather.com/x3/weather?city=" + city + "&key=" + key;
        }
        #endregion
    }
}
