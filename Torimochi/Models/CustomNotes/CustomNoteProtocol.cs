using System;
using System.ComponentModel;

namespace Torimochi.Models.CustomNotes
{
    public static class EnumExtention
    {
        /// <summary>
        /// <see cref="Enum"/>の<see cref="DescriptionAttribute"/>属性に指定された文字列を取得する拡張メソッドです。
        /// </summary>
        /// <param name="value">文字列を取得したい<see cref="Enum"/></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute) {
                return attribute.Description;
            }
            else {
                return value.ToString();
            }
        }
    }
    [Flags]
    public enum CustomNoteProtocol
    {
        None = 0,

        Right = 1 << 1,
        Left = 1 << 2,

        Normal = 1 << 3,
        Bomb = 1 << 4,
        BurstSliderHead = 1 << 5,
        BurstSliderElement = 1 << 6,
        BurstSliderElementFill = 1 << 7,

        Arrow = 1 << 8,
        Any = 1 << 9,

        [Description("cn.left.arrow")]
        LeftArrowPool = (Left | Normal | Arrow),
        [Description("cn.right.arrow")]
        RightArrowPool = (Right | Normal | Arrow),
        [Description("cn.left.dot")]
        LeftDotPool = (Left | Normal | Any),
        [Description("cn.right.dot")]
        RightDotPool = (Right | Normal | Any),
        [Description("cn.bomb")]
        BombPool = (Bomb),
        [Description("cn.left.burstslider")]
        LeftBurstSliderPool = (Left | BurstSliderElement | Arrow),
        [Description("cn.right.burstslider")]
        RightBurstSliderPool = (Right | BurstSliderElement | Arrow),
        [Description("cn.left.burstslider.head")]
        LeftBurstSliderHeadPool = (Left | BurstSliderHead | Arrow),
        [Description("cn.right.burstslider.head")]
        RightBurstSliderHeadPool = (Right | BurstSliderHead | Arrow),
        [Description("cn.left.burstslider.head.dot")]
        LeftBurstSliderHeadDotPool = (Left | BurstSliderHead | Any),
        [Description("cn.right.burstslider.head.dot")]
        RightBurstSliderHeadDotPool = (Right | BurstSliderHead | Any)
    }
}
