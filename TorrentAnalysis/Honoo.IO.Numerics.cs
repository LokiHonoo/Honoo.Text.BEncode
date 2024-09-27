/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

using System;

namespace Honoo.IO
{
    /// <summary>
    /// 字节类型的容量单位。单位进位是 1024 字节。单位是 B， KiB， MiB 等。
    /// </summary>
    public enum SizeKilo
    {
        /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
        Auto,

        /// <summary>单位是 B。</summary>
        B,

        /// <summary>单位是 KiB。</summary>
        KiB,

        /// <summary>单位是 MiB。</summary>
        MiB,

        /// <summary>单位是 GiB。</summary>
        GiB,

        /// <summary>单位是 TiB。</summary>
        TiB,

        /// <summary>单位是 PiB。</summary>
        PiB,

        /// <summary>单位是 EiB。</summary>
        EiB,

        /// <summary>单位是 BiB。</summary>
        BiB,
    }

    /// <summary>
    /// 字节类型的容量单位。单位进位是 1000 字节。单位是 B， KB， MB 等。
    /// </summary>
    public enum SizeThousands
    {
        /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
        Auto,

        /// <summary>单位是 B。</summary>
        B,

        /// <summary>单位是 KB。</summary>
        KB,

        /// <summary>单位是 MB。</summary>
        MB,

        /// <summary>单位是 GB。</summary>
        GB,

        /// <summary>单位是 TB。</summary>
        TB,

        /// <summary>单位是 PB。</summary>
        PB,

        /// <summary>单位是 EB。</summary>
        EB,

        /// <summary>单位是 BB。</summary>
        BB,
    }

    /// <summary>
    /// 位类型的每秒速度单位。单位进位是 1000 位。单位是 bps， Kbps， Mbps 等。
    /// </summary>
    public enum SpeedBits
    {
        /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
        Auto,

        /// <summary>单位是 bps。</summary>
        bps,

        /// <summary>单位是 Kbps。</summary>
        Kbps,

        /// <summary>单位是 Mbps。</summary>
        Mbps,

        /// <summary>单位是 Gbps。</summary>
        Gbps,

        /// <summary>单位是 Tbps。</summary>
        Tbps,

        /// <summary>单位是 Pbps。</summary>
        Pbps,

        /// <summary>单位是 Ebps。</summary>
        Ebps,

        /// <summary>单位是 Bbps。</summary>
        Bbps,
    }

    /// <summary>
    /// 字节类型的每秒速度单位。单位进位是 1024 字节。单位是 B/s， KiB/s， MiB/s 等。
    /// </summary>
    public enum SpeedKilo
    {
        /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
        Auto,

        /// <summary>单位是 B/s。</summary>
        Bps,

        /// <summary>单位是 KiB/s。</summary>
        KiBps,

        /// <summary>单位是 MiB/s。</summary>
        MiBps,

        /// <summary>单位是 GiB/s。</summary>
        GiBps,

        /// <summary>单位是 TiB/s。</summary>
        TiBps,

        /// <summary>单位是 PiB/s。</summary>
        PiBps,

        /// <summary>单位是 EiB/s。</summary>
        EiBps,

        /// <summary>单位是 BiB/s。</summary>
        BiBps,
    }

    /// <summary>
    /// 字节类型的每秒速度单位。单位进位是 1000 字节。单位是 B/s， KB/s， MB/s 等。
    /// </summary>
    public enum SpeedThousands
    {
        /// <summary>保持转换数值大于 1，选择可能的最大单位。</summary>
        Auto,

        /// <summary>单位是 B/s。</summary>
        Bps,

        /// <summary>单位是 KB/s。</summary>
        KBps,

        /// <summary>单位是 MB/s。</summary>
        MBps,

        /// <summary>单位是 GB/s。</summary>
        GBps,

        /// <summary>单位是 TB/s。</summary>
        TBps,

        /// <summary>单位是 PB/s。</summary>
        PBps,

        /// <summary>单位是 EB/s。</summary>
        EBps,

        /// <summary>单位是 BB/s。</summary>
        BBps,
    }

    /// <summary>
    /// I/O 数值对象辅助。
    /// </summary>
    public static class Numerics
    {
        #region 转换

        /// <summary>
        /// 将字节容量数值转换为指定单位。
        /// </summary>
        /// <param name="byteLength">字节数值。</param>
        /// <param name="radix">字节类型的容量单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        /// <returns></returns>
        public static double GetSize(long byteLength, SizeKilo radix, int places, out string unit)
        {
            double value = byteLength;
            int unitIndex = 0;
            if (radix == SizeKilo.Auto)
            {
                while (value >= 1024 && unitIndex < 7)
                {
                    value /= 1024;
                    unitIndex++;
                }
            }
            else
            {
                int unitLimit = (int)radix - 1;
                while (unitIndex < unitLimit)
                {
                    value /= 1024;
                    unitIndex++;
                }
            }
            unit = unitIndex switch
            {
                7 => "BiB",
                6 => "EiB",
                5 => "PiB",
                4 => "TiB",
                3 => "GiB",
                2 => "MiB",
                1 => "KiB",
                _ => "B",
            };
            return Math.Round(value, places);
        }

        /// <summary>
        /// 将字节容量数值转换为指定单位。
        /// </summary>
        /// <param name="byteLength">字节数值。</param>
        /// <param name="radix">字节类型的容量单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        /// <returns></returns>
        public static double GetSize(long byteLength, SizeThousands radix, int places, out string unit)
        {
            double value = byteLength;
            int unitIndex = 0;
            if (radix == SizeThousands.Auto)
            {
                while (value >= 1000 && unitIndex < 7)
                {
                    value /= 1000;
                    unitIndex++;
                }
            }
            else
            {
                int unitLimit = (int)radix - 1;
                while (unitIndex < unitLimit)
                {
                    value /= 1000;
                    unitIndex++;
                }
            }
            unit = unitIndex switch
            {
                7 => "BB",
                6 => "EB",
                5 => "PB",
                4 => "TB",
                3 => "GB",
                2 => "MB",
                1 => "KB",
                _ => "B",
            };
            return Math.Round(value, places);
        }

        /// <summary>
        /// 将字节速度数值转换为指定单位。
        /// </summary>
        /// <param name="bytesPerSecond">每秒字节数值。</param>
        /// <param name="radix">位类型的速度单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        public static double GetSpeed(long bytesPerSecond, SpeedBits radix, int places, out string unit)
        {
            double value = bytesPerSecond * 8;
            int unitIndex = 0;
            if (radix == SpeedBits.Auto)
            {
                while (value >= 1000 && unitIndex < 7)
                {
                    value /= 1000;
                    unitIndex++;
                }
            }
            else
            {
                int unitLimit = (int)radix - 1;
                while (unitIndex < unitLimit)
                {
                    value /= 1000;
                    unitIndex++;
                }
            }
            unit = unitIndex switch
            {
                7 => "Bbps",
                6 => "Ebps",
                5 => "Pbps",
                4 => "Tbps",
                3 => "Gbps",
                2 => "Mbps",
                1 => "Kbps",
                _ => "bps",
            };
            return Math.Round(value, places);
        }

        /// <summary>
        /// 将字节速度数值转换为指定单位。
        /// </summary>
        /// <param name="bytesPerSecond">每秒字节数值。</param>
        /// <param name="radix">字节类型的速度单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        public static double GetSpeed(long bytesPerSecond, SpeedKilo radix, int places, out string unit)
        {
            double value = bytesPerSecond;
            int unitIndex = 0;
            if (radix == SpeedKilo.Auto)
            {
                while (value >= 1024 && unitIndex < 7)
                {
                    value /= 1024;
                    unitIndex++;
                }
            }
            else
            {
                int unitLimit = (int)radix - 1;
                while (unitIndex < unitLimit)
                {
                    value /= 1024;
                    unitIndex++;
                }
            }
            unit = unitIndex switch
            {
                7 => "BiB/s",
                6 => "EiB/s",
                5 => "PiB/s",
                4 => "TiB/s",
                3 => "GiB/s",
                2 => "MiB/s",
                1 => "KiB/s",
                _ => "B/s",
            };
            return Math.Round(value, places);
        }

        /// <summary>
        /// 将字节速度数值转换为指定单位。
        /// </summary>
        /// <param name="bytesPerSecond">每秒字节数值。</param>
        /// <param name="radix">字节类型的速度单位。</param>
        /// <param name="places">保留小数位数。</param>
        /// <param name="unit">字节数值的容量单位的字符串表示。</param>
        public static double GetSpeed(long bytesPerSecond, SpeedThousands radix, int places, out string unit)
        {
            double value = bytesPerSecond;
            int unitIndex = 0;
            if (radix == SpeedThousands.Auto)
            {
                while (value >= 1000 && unitIndex < 7)
                {
                    value /= 1000;
                    unitIndex++;
                }
            }
            else
            {
                int unitLimit = (int)radix - 1;
                while (unitIndex < unitLimit)
                {
                    value /= 1000;
                    unitIndex++;
                }
            }
            unit = unitIndex switch
            {
                7 => "BB/s",
                6 => "EB/s",
                5 => "PB/s",
                4 => "TB/s",
                3 => "GB/s",
                2 => "MB/s",
                1 => "KB/s",
                _ => "B/s",
            };
            return Math.Round(value, places);
        }

        #endregion 转换
    }
}