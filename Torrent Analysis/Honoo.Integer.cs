/*
 * Copyright
 *
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published under the terms of the MIT license.
 */

using System;

namespace Honoo
{
    /// <summary>
    /// 数值对象辅助。
    /// </summary>
    public static class Integer
    {
        #region 转换

        /// <summary>
        /// 字节数值的容量单位。进位是 1000。
        /// </summary>
        public enum SizeRadix10
        {
            /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
            Auto,

            /// <summary></summary>
            B,

            /// <summary></summary>
            KB,

            /// <summary></summary>
            MB,

            /// <summary></summary>
            GB,

            /// <summary></summary>
            TB,

            /// <summary></summary>
            PB,

            /// <summary></summary>
            EB,
        }

        /// <summary>
        /// 字节数值的容量单位。进位是 1024。
        /// </summary>
        public enum SizeRadix2
        {
            /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
            Auto,

            /// <summary></summary>
            B,

            /// <summary></summary>
            KiB,

            /// <summary></summary>
            MiB,

            /// <summary></summary>
            GiB,

            /// <summary></summary>
            TiB,

            /// <summary></summary>
            PiB,

            /// <summary></summary>
            EiB,
        }

        /// <summary>
        /// 字节数值的速度单位。进位是 1024，并转换为 Bit 值。
        /// </summary>
        public enum SpeedBits
        {
            /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
            Auto,

            /// <summary></summary>
            bps,

            /// <summary></summary>
            Kbps,

            /// <summary></summary>
            Mbps,

            /// <summary></summary>
            Gbps,

            /// <summary></summary>
            Tbps,

            /// <summary></summary>
            Pbps,

            /// <summary></summary>
            Ebps,
        }

        /// <summary>
        /// 字节数值的速度单位。进位是 1024。
        /// </summary>
        public enum SpeedRadix2
        {
            /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
            Auto,

            /// <summary></summary>
            Bps,

            /// <summary></summary>
            KiBps,

            /// <summary></summary>
            MiBps,

            /// <summary></summary>
            GiBps,

            /// <summary></summary>
            TiBps,

            /// <summary></summary>
            PiBps,

            /// <summary></summary>
            EiBps,
        }

        /// <summary>
        /// 将字节容量数值转换为指定单位。
        /// </summary>
        /// <param name="byteLength">字节数值。</param>
        /// <param name="radix">字节数值的容量单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        /// <returns></returns>
        public static double GetSize(long byteLength, SizeRadix2 radix, int places, out string unit)
        {
            double value = byteLength;
            int index = 0;
            if (radix == SizeRadix2.Auto)
            {
                while (value >= 1024 && index < 6)
                {
                    value /= 1024;
                    index++;
                }
            }
            else
            {
                int um = (int)radix - 1;
                while (index < um)
                {
                    value /= 1024;
                    index++;
                }
            }
            switch (index)
            {
                case 6: unit = "EiB"; break;
                case 5: unit = "PiB"; break;
                case 4: unit = "TiB"; break;
                case 3: unit = "GiB"; break;
                case 2: unit = "MiB"; break;
                case 1: unit = "KiB"; break;
                case 0: default: unit = "B"; break;
            }
            return Math.Round(value, places);
        }

        /// <summary>
        /// 将字节容量数值转换为指定单位。
        /// </summary>
        /// <param name="byteLength">字节数值。</param>
        /// <param name="radix">字节数值的容量单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        /// <returns></returns>
        public static double GetSize(long byteLength, SizeRadix10 radix, int places, out string unit)
        {
            double value = byteLength;
            int index = 0;
            if (radix == SizeRadix10.Auto)
            {
                while (value >= 1000 && index < 6)
                {
                    value /= 1000;
                    index++;
                }
            }
            else
            {
                int um = (int)radix - 1;
                while (index < um)
                {
                    value /= 1000;
                    index++;
                }
            }
            switch (index)
            {
                case 6: unit = "EB"; break;
                case 5: unit = "PB"; break;
                case 4: unit = "TB"; break;
                case 3: unit = "GB"; break;
                case 2: unit = "MB"; break;
                case 1: unit = "KB"; break;
                case 0: default: unit = "B"; break;
            }
            return Math.Round(value, places);
        }

        /// <summary>
        /// 将字节速度数值转换为指定单位。
        /// </summary>
        /// <param name="byteLength">字节数值。</param>
        /// <param name="radix">字节数值的速度单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        public static double GetSpeed(long byteLength, SpeedRadix2 radix, int places, out string unit)
        {
            double value = byteLength;
            int index = 0;
            if (radix == SpeedRadix2.Auto)
            {
                while (value >= 1024 && index < 6)
                {
                    value /= 1024;
                    index++;
                }
            }
            else
            {
                int um = (int)radix - 1;
                while (index < um)
                {
                    value /= 1024;
                    index++;
                }
            }
            switch (index)
            {
                case 6: unit = "EiB/s"; break;
                case 5: unit = "PiB/s"; break;
                case 4: unit = "TiB/s"; break;
                case 3: unit = "GiB/s"; break;
                case 2: unit = "MiB/s"; break;
                case 1: unit = "KiB/s"; break;
                case 0: default: unit = "B/s"; break;
            }
            return Math.Round(value, places);
        }

        /// <summary>
        /// 将字节速度数值转换为指定单位。
        /// </summary>
        /// <param name="byteLength">字节数值。</param>
        /// <param name="radix">字节数值的速度单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        public static double GetSpeed(long byteLength, SpeedBits radix, int places, out string unit)
        {
            double value = byteLength;
            int index = 0;
            if (radix == SpeedBits.Auto)
            {
                while (value >= 1024 && index < 6)
                {
                    value /= 1024;
                    index++;
                }
            }
            else
            {
                int um = (int)radix - 1;
                while (index < um)
                {
                    value /= 1024;
                    index++;
                }
            }
            switch (index)
            {
                case 6: unit = "Ebps"; break;
                case 5: unit = "Pbps"; break;
                case 4: unit = "Tbps"; break;
                case 3: unit = "Gbps"; break;
                case 2: unit = "Mbps"; break;
                case 1: unit = "Kbps"; break;
                case 0: default: unit = "bps"; break;
            }
            return Math.Round(value * 8, places);
        }

        #endregion 转换
    }
}