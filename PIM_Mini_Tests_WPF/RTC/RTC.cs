using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalPhase;

namespace PIM_Mini_Tests_WPF.RTC
{
    internal struct ClockData
    {
        internal int Year, Month, DayOfMonth, DayOfWeek, Hours, Minutes, Seconds;
    }

    public class RTC : HardwareTest
    {
        private int handle;
        private ushort pageSize;
        private int numPages;
        private ushort slaveAddress;
        private ushort port;
        private int bitrate;
        private int yearAddr, monthAddr, dayOfWeekAddr, dayOfMonthAddr, hoursAddr, minutesAddr, secondsAddr;
        private HardwareTest caller;

        public RTC() :
            base("RTC", new HardwareTest[]
        {
            new TestSetSecond(),
            new TestSetSecondIncr(),
            new TestSetMinute(),
            new TestSetMinuteIncr(),
            new TestSetHour(),
             new TestSetHourIncr(),
            new TestSetDay30(),
            new TestSetDay31(),
            new TestSetDay28(),
            new TestLeapYearDiv4(),
            new TestLeapYearDiv100(),
            new TestLeapYearDiv400(),
            new TestSetDayIncr(),
            new TestSetMonth(),
            new TestSetMonthIncr(),
            new TestSetYear(),
            new TestSetYearIncr(),
            new TestPeriod()
        })
        {
            this.pageSize = Properties.Settings.Default.eepromPageSize;
            this.numPages = Properties.Settings.Default.eepromNumPages;
            this.slaveAddress = Properties.Settings.Default.eepromSlaveAddress;
            this.port = Properties.Settings.Default.eepromPortNumber;
            this.bitrate = Properties.Settings.Default.eepromBitrate;
            this.yearAddr = Properties.Settings.Default.rtcYearAddr;
            this.monthAddr = Properties.Settings.Default.rtcMonthAddr;
            this.dayOfWeekAddr = Properties.Settings.Default.rtcDayOfWeekAddr;
            this.dayOfMonthAddr = Properties.Settings.Default.rtcDayOfMonthAddr;
            this.hoursAddr = Properties.Settings.Default.rtcHoursAddr;
            this.minutesAddr = Properties.Settings.Default.rtcMinutesAddr;
            this.secondsAddr = Properties.Settings.Default.rtcSecondsAddr;
        }

        public override void Test()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets up the tests inside the FRAM namespace
        /// </summary>
        /// <returns></returns>
        internal void SetUp(HardwareTest caller)
        {
            this.caller = caller;
            this.handle = AardvarkApi.aa_open(Properties.Settings.Default.rtcPortNumber);
            if (!this.caller.AssertNotEqual(this.handle, (int)AardvarkStatus.AA_UNABLE_TO_OPEN, "The specified port is not connected to an Aardvark device or the port is already in use.")) return;
            if (!this.caller.AssertNotEqual(this.handle, (int)AardvarkStatus.AA_INCOMPATIBLE_DEVICE, "There is a version mismatch between the DLL and the firmware. The DLL is not of a sufficient version for interoperability with the firmware version or vice versa.")) return;

            int handleConfig = AardvarkApi.aa_configure(handle, AardvarkConfig.AA_CONFIG_GPIO_I2C);
            if (!this.caller.AssertEqual(handleConfig, (int)AardvarkConfig.AA_CONFIG_GPIO_I2C, "The Aardvark adapter could not be set so that the SPI pins are configured as GPIO pins, and enable I2C.")) return;
            if (!this.caller.AssertNotEqual(handleConfig, (int)AardvarkStatus.AA_CONFIG_ERROR, "The I2C or SPI subsystem is currently active and the new configuration requires the subsystem to be deactivated.")) return;

            int pullup = AardvarkApi.aa_i2c_pullup(handle, AardvarkApi.AA_I2C_PULLUP_BOTH);
            if (!this.caller.AssertNotEqual(pullup, (int)AardvarkStatus.AA_INCOMPATIBLE_DEVICE, "The hardware version is not compatible with this feature. Only hardware versions 2.00 or greater support switchable pull-up resistors pins.")) return;
            if (!this.caller.AssertEqual(pullup, (int)AardvarkApi.AA_I2C_PULLUP_BOTH, "The Aardvark adapter could not be set so that the SCL/SDA pull-resistors are enabled.")) return;

            var powerStatus = AardvarkApi.aa_target_power(this.handle, AardvarkApi.AA_TARGET_POWER_BOTH);
            if (!this.caller.AssertNotEqual(powerStatus, (int)AardvarkStatus.AA_INCOMPATIBLE_DEVICE, "The hardware version is not compatible with this feature. Only hardware versions 2.00 or greater support switchable target power pins.")) return;
            if (!this.caller.AssertEqual(AardvarkApi.AA_TARGET_POWER_BOTH, powerStatus, "The Aardvark adapter could not be set to enable the target power pins.")) return;

            var bitrate = AardvarkApi.aa_i2c_bitrate(handle, 400); //  bitrate = 400
            if (!this.caller.AssertEqual(this.bitrate, this.bitrate, "The bitrate for the Aardvark adapter could not be set.")) return;

            var busTimeOut = AardvarkApi.aa_i2c_bus_timeout(this.handle, 10);
            if (!this.caller.AssertEqual(busTimeOut, 10, "The bus timeout for the Aardvark adapter could not be set.")) return;
        }

        internal void TearDown(HardwareTest calle)
        {
            var numClosed = AardvarkApi.aa_close(this.handle);
            if (!this.caller.AssertEqual(numClosed, 1, "An incorrect number of Aardvark adapters was closed.")) return;
        }

        internal static int DecToHex(int value) => ((value / 10) << 4) + value % 10;

        internal static int HexToDec(int value) => ((value) & 0x0f) + (value >> 4) * 10;

        /// <summary>
        /// Checks that the clock can accurately keep the time
        /// </summary>
        /// <param name="this.caller">Caller class</param>
        /// <param name="data">The data which will be initially written to the RTC</param>
        /// <param name="delta">The time delta (seconds) after which the RTC will be checked</param>
        internal void Check(ClockData data, int delta = 10)
        {
            DateTime formattedTime = ConvertClockDataToDateTime(data);
            if (!this.caller.AssertEqual(data.DayOfWeek, ((int)formattedTime.DayOfWeek) + 1, "Internal software test error - specified day of week does not match the calculated day of week")) return;
            formattedTime.AddSeconds(delta);

            this.Write(data);
            System.Threading.Thread.Sleep(delta * 1000);
            var dataIn = this.Read();

            if (!this.caller.AssertEqual(dataIn, formattedTime, "The time expected does not match the time of the RTC")) return;
        }

        /// <summary>
        /// Writes data to the RTC
        /// </summary>
        /// <param name=""></param>
        internal void _write(int address, int value)
        {
            byte[] dataOut = new byte[] { (byte)(address & 0xff), (byte)value };
            var count = AardvarkApi.aa_i2c_write(aardvark: this.handle, slave_addr: (ushort)address, flags: AardvarkI2cFlags.AA_I2C_NO_FLAGS, num_bytes: 1, data_out: dataOut);
            if (!this.caller.AssertNotEqual(count, (int)AardvarkStatus.AA_I2C_WRITE_ERROR, "There was an error reading the acknowledgment from the Aardvark adapter. This is most likely a result of a communication error.")) return;
            if (!this.caller.AssertEqual(count, 1, "The number of bytes was not the expected amount")) return;
        }

        private void Write(ClockData data)
        {
            var dataOut = WriteConvert(data);
            this._write(this.yearAddr, dataOut.Year);
            this._write(this.monthAddr, dataOut.Month);
            this._write(this.dayOfMonthAddr, dataOut.DayOfMonth);
            this._write(this.dayOfWeekAddr, dataOut.DayOfWeek);
            this._write(this.hoursAddr, dataOut.Hours);
            this._write(this.minutesAddr, dataOut.Minutes);
            this._write(this.secondsAddr, dataOut.Seconds);
        }

        /// <summary>
        /// Convers data for writing to the RTC
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static ClockData WriteConvert(ClockData data)
        {
            int centuryBit = (data.Year - 1900) > 199 ? (1 << 7) : 0;
            ClockData dataOut = new ClockData
            {
                Year = DecToHex(data.Year % 100),
                Month = DecToHex(data.Month) | centuryBit,
                DayOfMonth = DecToHex(data.DayOfMonth),
                DayOfWeek = DecToHex(data.DayOfWeek),
                Hours = DecToHex(data.Hours),
                Minutes = DecToHex(data.Minutes),
                Seconds = DecToHex(data.Seconds)
            };
            return dataOut;
        }

        /// <summary>
        /// Converts ClockData to DateTime
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static DateTime ConvertClockDataToDateTime(ClockData data) => new DateTime(year: data.Year, month: data.Month, day: data.DayOfMonth, hour: data.Hours, minute: data.Minutes, second: data.Seconds);

        private DateTime Read()
        {
            var regMonth = this._read(this.monthAddr);
            ClockData dataIn = new ClockData()
            {
                Year = HexToDec(this._read(this.yearAddr)) + 100,
                Month = HexToDec(regMonth & 0x1f),
                DayOfMonth = HexToDec(this._read(this.dayOfMonthAddr)),
                DayOfWeek = HexToDec(this._read(this.dayOfWeekAddr)),
                Hours = HexToDec(this._read(this.hoursAddr) & 0x3f),
                Minutes = HexToDec(this._read(this.minutesAddr)),
                Seconds = HexToDec(this._read(this.secondsAddr)),
            };

            if ((regMonth & (1 << 7)) != 0)
            {
                dataIn.Year += 1;
            }
            DateTime formattedData = ConvertClockDataToDateTime(dataIn);
            return formattedData;
        }

        /// <summary>
        /// Reads an address and returns the data inside it
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private int _read(int address)
        {
            var count = AardvarkApi.aa_i2c_write(aardvark: this.handle, slave_addr: this.slaveAddress, flags: AardvarkI2cFlags.AA_I2C_NO_FLAGS, num_bytes: 1, data_out: new byte[] { (byte)(address & 0xff) });
            if (!this.caller.AssertNotEqual(count, (int)AardvarkStatus.AA_I2C_WRITE_ERROR, "There was an error reading the acknowledgment from the Aardvark adapter. This is most likely a result of a communication error.")) return -1;

            byte[] dataIn = new byte[1];
            count = AardvarkApi.aa_i2c_read(aardvark: this.handle, slave_addr: (ushort)address, flags: AardvarkI2cFlags.AA_I2C_NO_FLAGS, num_bytes: 1, data_in: dataIn);
            if (!this.caller.AssertNotEqual(count, (int)AardvarkStatus.AA_I2C_WRITE_ERROR, "There was an error reading the acknowledgment from the Aardvark adapter. This is most likely a result of a communication error.")) return -1;
            if (!this.caller.AssertNotEqual(count, 1, "The amount of data read was not the expected amount")) return -1;

            return dataIn[0];
        }
    }
}
