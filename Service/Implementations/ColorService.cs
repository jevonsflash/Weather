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
        public  string GetColorJson()
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
                        isSelected ="1"
                    },
                    new Model.UserColor()
                    {
                        SingleColors =new List<Model.SingleColor>()
                        {
                            new Model.SingleColor() {colorStr="0x98FF72" },
                            new Model.SingleColor() {colorStr="0x65D97D" },
                            new Model.SingleColor() {colorStr="0x42A881" },
                            new Model.SingleColor() {colorStr="0x1F8784" },
                            new Model.SingleColor() {colorStr="0x00697D" },
                            new Model.SingleColor() {colorStr="0x293841" },

                        },
                        isSelected ="0"
                    },
                    new Model.UserColor()
                    {
                        SingleColors =new List<Model.SingleColor>()
                        {
                            new Model.SingleColor() {colorStr="0x2E4497" },
                            new Model.SingleColor() {colorStr="0x865D98" },
                            new Model.SingleColor() {colorStr="0xDF4440" },
                            new Model.SingleColor() {colorStr="0xA42113" },
                            new Model.SingleColor() {colorStr="0x72170C" },
                            new Model.SingleColor() {colorStr="0x293841" },

                        },
                        isSelected ="0"
                    }

                }
            };
            return Utils.JsonSerializeHelper.JsonSerialize<GetColorRespose>(respose);
        }
    }
}
