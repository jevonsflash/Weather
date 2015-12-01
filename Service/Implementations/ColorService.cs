using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.Service.Message;

namespace Weather.Service.Implementations
{
    public class ColorService
    {
        private static readonly ColorService instance = new ColorService();

        private ColorService()
        {
        }

        public static ColorService GetInstance()
        {
            return instance;
        }

        public async Task<GetColorRespose> GetColorAsync()
        {
            GetColorRespose respose = new GetColorRespose();
            respose = await Weather.Utils.JsonSerializeHelper.JsonDeSerializeForFileAsync<GetColorRespose>("Color\\ColorConfig.json").ConfigureAwait(false);
            return respose;
        }

        public async Task SaveColor(GetColorRespose getColorRespose)
        {
            await Weather.Utils.JsonSerializeHelper.JsonSerializeForFileAsync<GetColorRespose>(getColorRespose, "Color\\ColorConfig.json").ConfigureAwait(false);
        }
        /// <summary>
        /// 获取初始化颜色Json格式
        /// </summary>
        /// <returns></returns>
        public string GetColorJson()
        {
            GetColorRespose respose = new GetColorRespose();
            respose = new GetColorRespose()
            {
                UserColors = new List<Model.UserColor>()
                {
                    new Model.UserColor()
                    {
                        SingleColors =new List<Model.SingleColor>()
                        {
                            new Model.SingleColor() {colorStr="0x324D5C" },
                            new Model.SingleColor() {colorStr="0x46B29D" },
                            new Model.SingleColor() {colorStr="0xF0CA4D" },
                            new Model.SingleColor() {colorStr="0xE37B40" },
                            new Model.SingleColor() {colorStr="0xDE5B49" },
                            new Model.SingleColor() {colorStr="0x293841" },

                        },
                        isSelected ="1",
                        Title="标准心情"
                    },
                    new Model.UserColor()
                    {
                        SingleColors =new List<Model.SingleColor>()
                        {
                            new Model.SingleColor() {colorStr="0xFF91BD" },
                            new Model.SingleColor() {colorStr="0x8793E8" },
                            new Model.SingleColor() {colorStr="0x99FFC5" },
                            new Model.SingleColor() {colorStr="0xE8DE7F" },
                            new Model.SingleColor() {colorStr="0xFFAB8C" },
                            new Model.SingleColor() {colorStr="0x293841" },

                        },
                        isSelected ="0",
                        Title="清新花季"
                    },
                    new Model.UserColor()
                    {
                        SingleColors =new List<Model.SingleColor>()
                        {
                            new Model.SingleColor() {colorStr="0x442D65" },
                            new Model.SingleColor() {colorStr="0x775BA3" },
                            new Model.SingleColor() {colorStr="0x91C5A9" },
                            new Model.SingleColor() {colorStr="0xF8E1B4" },
                            new Model.SingleColor() {colorStr="0xF98A5F" },
                            new Model.SingleColor() {colorStr="0x293841" },

                        },
                        isSelected ="0",
                        Title="哥德情怀"
                    },
                    new Model.UserColor()
                    {
                        SingleColors =new List<Model.SingleColor>()
                        {
                            new Model.SingleColor() {colorStr="0x169CF4" },
                            new Model.SingleColor() {colorStr="0x23A60A" },
                            new Model.SingleColor() {colorStr="0xD88F09" },
                            new Model.SingleColor() {colorStr="0xF0620B" },
                            new Model.SingleColor() {colorStr="0xF3302F" },
                            new Model.SingleColor() {colorStr="0x293841" },

                        },
                        isSelected ="0",
                        Title="浓郁夏午"
                    },
                }
            };
            return Utils.JsonSerializeHelper.JsonSerialize<GetColorRespose>(respose);
        }
    }
}
