using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTraqPlugin
{
    internal class SkytraqController : IDisposable
    {
        // 対象ポート
        private SerialPort _com;
        // ポートオープン時に取得したバージョン
        private readonly SoftwareVersion _version;

        private const byte START_OF_SEQUENCE_1 = 0xa0;
        private const byte START_OF_SEQUENCE_2 = 0xa1;

        private const byte END_OF_SEQUENCE_1 = 0x0d;
        private const byte END_OF_SEQUENCE_2 = 0x0a;
        private const UInt16 MASK_LOBYTE = 0x00FF;

        private const int READ_TIMEOUT = (10 * 1000);
        private const int READ_TIMEOUT_INTERNAL = (300);

        public event ReadProgressEventHandler OnRead;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="portName">対象のポート名</param>
        public SkytraqController(string portName)
        {
            _com = new SerialPort(portName, 38400, Parity.None, 8, StopBits.One);

            _com.ReadTimeout = READ_TIMEOUT;
            _com.Open();

            // 同期しつつバージョンを取得してみる
            SyncBaudRate( out _version);
        }

        public void Dispose()
        {
            // 最後はCloseしておく
            if (_com.IsOpen)
            {
                _com.Close();
                _com = null;
            }
        }

        #region 内部処理
        /// <summary>
        /// ボーレートを合わせる
        /// </summary>
        /// <returns></returns>
        private void SyncBaudRate( out SoftwareVersion version)
        {
            int[] BaudRateList = { 0, 115200, 38400, 230400, 57600, 19200, 9600, 4800 };
            foreach (int baudRate in BaudRateList)
            {
                // 0は今の設定を使う
                if (0 != baudRate)
                {
                    _com.BaudRate = baudRate;
                    System.Threading.Thread.Sleep(50);
                }
                try
                {
                    // 読み出しのタイムアウトを短くする
                    _com.ReadTimeout = READ_TIMEOUT_INTERNAL;
                    // ソフトバージョンを取得してみる
                    version = GetSoftwareVersion();
                    // タイムアウト値をもどします。
                    _com.ReadTimeout = READ_TIMEOUT;

                    System.Diagnostics.Debug.Print("Soft Type=0x{0:X2}", version.SoftType);
                    System.Diagnostics.Debug.Print("Kernel Vresion=0x{0:X8}", version.KernelVersion);
                    System.Diagnostics.Debug.Print("ODM Vresion=0x{0:X8}", version.ODMVersion);
                    System.Diagnostics.Debug.Print("Revision=0x{0:X8}", version.Revision);

                    return;
                }
                catch
                {
                    continue;
                }
            }
            throw new InvalidOperationException("応答を受信できませんでした");
        }

        public Payload Read()
        {
            DateTime start = DateTime.Now;
            TimeSpan ts;

            bool findHeader1 = false;
            bool findHeader2 = false;

            // header check
            byte readData;
            while (true)
            {
                readData = (byte)(0x00FF & _com.ReadByte());

                if (findHeader1)
                {
                    if (START_OF_SEQUENCE_2 == readData)
                    {
                        findHeader2 = true;
                        break;
                    }
                    findHeader1 = false;
                }
                else
                {
                    if (START_OF_SEQUENCE_1 == readData)
                    {
                        findHeader2 = false;
                        findHeader1 = true;
                    }
                }
                // データは読み出せたけど、目的のデータが読み出せないので、タイムアウトとして処理します。
                ts = DateTime.Now - start;
                if (_com.ReadTimeout < ts.TotalMilliseconds)
                    throw new TimeoutException();
            }

            // payload length
            UInt16 payloadLength = (UInt16)(0x00FF & _com.ReadByte());
            payloadLength <<= 8;
            payloadLength |= (UInt16)(0x00FF & _com.ReadByte());

            // read payload
            System.Threading.Thread.Sleep(10);
            byte[] rawPayload = new byte[payloadLength];
            start = DateTime.Now;
            for (int readCount = 0; readCount < payloadLength;)
            {
                readCount += _com.Read(rawPayload, readCount, payloadLength - readCount);
                // 時間が経過しても読み出しきらない
                ts = DateTime.Now - start;
                if (_com.ReadTimeout < ts.TotalMilliseconds)
                    throw new TimeoutException();
            }

            Payload result = new Payload(rawPayload, 0, rawPayload.Length);

            // CS
            byte checkSum = (byte)(0x00FF & _com.ReadByte());

            byte calcChecSum = 0;
            for (int index = 0; index < rawPayload.Length; ++index)
            {
                calcChecSum ^= rawPayload[index];
            }

            if (checkSum != calcChecSum)
            {
                throw new Exception("check sum error");
            }

            // footer check
            bool findFooter1 = false;
            bool findFooter2 = false;

            start = DateTime.Now;
            while (true)
            {
                readData = (byte)(0x00FF & _com.ReadByte());

                if (findFooter1)
                {
                    if (END_OF_SEQUENCE_2 == readData)
                    {
                        findFooter2 = true;
                        break;
                    }
                    findFooter1 = false;
                }
                else
                {
                    if (END_OF_SEQUENCE_1 == readData)
                    {
                        findFooter2 = false;
                        findFooter1 = true;
                    }
                    else
                    {
                        throw new Exception("sequnce error.");
                    }
                }
                // データは読み出せたけど、目的のデータが読み出せないので、タイムアウトとして処理します。
                ts = DateTime.Now - start;
                if (_com.ReadTimeout < ts.TotalMilliseconds)
                    throw new TimeoutException();
            }

            return result;
        }

        public void Write(Payload payload)
        {
            byte[] command = convert(payload);

            _com.Write(command, 0, command.Length);
        }

        private byte[] convert(Payload payload)
        {
            // payload length
            UInt16 payloadLength = (UInt16)(payload.ByteLength);
            // 2 = sizeof([Start of Sequence])
            // 2 = sizeof([Payload Length])
            // 1 = sizeof([CS])
            // 2 = sizeof([End of Sequence])
            int size = payloadLength + 7;

            // command buffer
            byte[] result = new byte[size];

            // set [Start of Sequence]
            result[0] = START_OF_SEQUENCE_1;
            result[1] = START_OF_SEQUENCE_2;

            // set [Payload Length] 
            result[2] = (byte)(MASK_LOBYTE & (payloadLength >> 8));
            result[3] = (byte)((MASK_LOBYTE & payloadLength));

            // payload
            payload.CopyTo(result, 4, payloadLength);

            // [CS]
            byte checkSum = 0x00;
            for (int index = 0; index < payloadLength + 1; ++index)
            {
                checkSum ^= result[index + 4];
            }
            result[size - 3] = checkSum;

            // [End of Sequence]
            result[size - 2] = END_OF_SEQUENCE_1;
            result[size - 1] = END_OF_SEQUENCE_2;

            return result;
        }

        private const int SECTOR_SIZE = 4096;
        private const int SECTOR_COUNT = 1; 

        private void ReadLogBuffer(byte[] buffer, int offset, int sectorsSize)
        {
            for (int readCount = 0; readCount < sectorsSize;)
            {
                readCount += _com.Read(buffer, offset + readCount, sectorsSize - readCount);
            }
        }

        private static byte[] CHECKSUM_MARKER = { 0x45, 0x4e, 0x44, 0x00, 0x43, 0x48, 0x45, 0x43, 0x4b, 0x53, 0x55, 0x4d, 0x3d };
        private static byte[] CHECKSUM_MARKER_FOOTER = { 0x0a, 0x0d };

        private byte ReadLogBufferCS()
        {
            byte d;
            for (int index = 0; index < CHECKSUM_MARKER.Length;)
            {
                d = (byte)_com.ReadByte();
                if (d == CHECKSUM_MARKER[index])
                {
                    ++index;
                }
                else
                {
                    index = 0;
                }
            }

            byte result = (byte)_com.ReadByte();

            for (int index = 0; index < CHECKSUM_MARKER_FOOTER.Length;)
            {
                d = (byte)_com.ReadByte();
                if (d == CHECKSUM_MARKER_FOOTER[index])
                {
                    ++index;
                }
                else
                {
                    index = 0;
                }
            }

            return result;
        }

        private enum RESULT
        {
            RESULT_ACK,
            RESULT_NACK
        };

        private RESULT waitResult(MessageID id)
        {
            Payload p = null;
            while (true)
            {
                p = Read();

                if (p.ID == MessageID.ACK)
                {
                    if (id == (MessageID)p.Body[0])
                    {
                        return RESULT.RESULT_ACK;
                    }
                }
                else if (p.ID == MessageID.NACK)
                {
                    if (id == (MessageID)p.Body[0])
                    {
                        return RESULT.RESULT_NACK;
                    }
                }
            }
        }

        private void sendRestart()
        {
            Payload p = new Payload(MessageID.System_Restart, new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            Write(p);
            if (RESULT.RESULT_ACK == this.waitResult(MessageID.System_Restart))
            {
                // リセットが成功したのでボーレートも戻す
                _com.BaudRate = 38400;
                System.Threading.Thread.Sleep(50);
            }
        }

        private void sendReadBuffer(int offsetSector, int sectorCount)
        {
            Payload p = new Payload(MessageID.Enable_data_read_from_the_log_buffer, new byte[] { 0x00, 0x00, 0x00, 0x02 });
            p.Body[0] = (byte)(0x00FF & (offsetSector >> 8));
            p.Body[1] = (byte)(0x00FF & (offsetSector >> 0));
            p.Body[2] = (byte)(0x00FF & (sectorCount >> 8));
            p.Body[3] = (byte)(0x00FF & (sectorCount >> 0));
            Write(p);
            this.waitResult(MessageID.Enable_data_read_from_the_log_buffer);
        }

        private void GetBufferStatus(out UInt16 totalSectors, out UInt16 freeSectors, out bool dataLogEnable)
        {
            Payload p = new Payload(MessageID.Request_Information_of_the_Log_Buffer_Status);
            Write(p);

            Payload result;
            if (RESULT.RESULT_ACK != this.waitResult(MessageID.Request_Information_of_the_Log_Buffer_Status))
            {
                throw new Exception("NACK!");
            }

            result = Read();
            if (result.ID != MessageID.Output_Status_of_the_Log_Buffer)
            {
                throw new Exception("Sequence error");
            }

            totalSectors = (UInt16)result.Body[7];
            totalSectors <<= 8;
            totalSectors |= (UInt16)result.Body[6];

            freeSectors = (UInt16)result.Body[5];
            freeSectors <<= 8;
            freeSectors |= (UInt16)result.Body[4];

            dataLogEnable = (0x01 == result.Body[32]);
        }

        private DataLogFixFull ReadLocation(BinaryReader br, DataLogFixFull current)
        {
            UInt16 pos1 = (byte)(0x00FF & br.ReadByte());
            pos1 <<= 8;
            pos1 |= (byte)(0x00FF & br.ReadByte());

            UInt16 velocity = pos1;
            velocity &= (UInt16)0x03ff;

            switch ((pos1 & 0xE000))
            {
                // empty
                case 0xE000:
                    return null;
                    break;

                // FIX FULL POI
                case 0x6000:
                // FIX FULL
                case 0x4000:
                    {
                        DataLogFixFull data = new DataLogFixFull();
                        data.type = ((pos1 & 0xE000) == 0x4000) ? DataLogFixFull.TYPE.FULL : DataLogFixFull.TYPE.FULL_POI;
                        data.V = velocity;

                        byte b = (byte)(0x00FF & br.ReadByte());
                        data.TOW = (byte)(0x000f & (b >> 4));
                        data.WN = (UInt16)(0x0003 & b);
                        data.WN <<= 8;
                        data.WN |= (UInt16)(0x00FF & br.ReadByte());
                        UInt32 un = (UInt32)(0x00FF & br.ReadByte());
                        un <<= 8;
                        un |= (UInt32)(0x00FF & br.ReadByte());
                        un <<= 4;
                        data.TOW |= un;

                        {
                            data.X = (Int32)(0x00FF & br.ReadByte());
                            data.X <<= 8;
                            data.X |= (Int32)(0x00FF & br.ReadByte());

                            un = (UInt32)(0x00FF & br.ReadByte());
                            un <<= 8;
                            un |= (UInt32)(0x00FF & br.ReadByte());

                            un <<= 16;
                            un &= 0xffff0000;
                            data.X |= (Int32)un;
                        }


                        {
                            data.Y = (Int32)(0x00FF & br.ReadByte());
                            data.Y <<= 8;
                            data.Y |= (Int32)(0x00FF & br.ReadByte());

                            un = (UInt32)(0x00FF & br.ReadByte());
                            un <<= 8;
                            un |= (UInt32)(0x00FF & br.ReadByte());

                            un <<= 16;
                            un &= 0xffff0000;
                            data.Y |= (Int32)un;

                        }

                        {
                            data.Z = (Int32)(0x00FF & br.ReadByte());
                            data.Z <<= 8;
                            data.Z |= (Int32)(0x00FF & br.ReadByte());

                            un = (UInt32)(0x00FF & br.ReadByte());
                            un <<= 8;
                            un |= (UInt32)(0x00FF & br.ReadByte());

                            un <<= 16;
                            un &= 0xffff0000;
                            data.Z |= (Int32)un;

                        }
                        return data;
                    }
                    break;

                // FIX COMPACT
                case 0x8000:
                    {
                        UInt16 diffTOW = (UInt16)(0x00FF & br.ReadByte());
                        diffTOW <<= 8;
                        diffTOW |= (UInt16)(0x00FF & br.ReadByte());

                        Int16 diffX = (Int16)(0x00FF & br.ReadByte());
                        diffX <<= 2;
                        UInt16 un = (UInt16)(0x00FF & br.ReadByte());
                        diffX = (Int16)(0x0003 & (un >> 6));

                        if (0 != (diffX & 0x0200))
                        {
                            UInt16 unWork = 0xfC00;
                            diffX |= (Int16)unWork;   // 1111 1100 0000 0000
                        }

                        Int16 diffY = (Int16)(un & 0x003f);
                        un = (UInt16)(0x00FF & br.ReadByte());
                        diffY |= (Int16)(0x03C0 & (un << 6));  // 11 1100 0000

                        if (0 != (diffY & 0x0200))
                        {
                            UInt16 unWork = 0xfC00;
                            diffY |= (Int16)unWork;   // 1111 1100 0000 0000
                        }


                        Int16 diffZ = (Int16)(0x0003 & un);
                        diffZ <<= 8;
                        diffZ |= (Int16)(0x00FF & br.ReadByte());

                        if (0 != (diffZ & 0x0200))
                        {
                            UInt16 unWork = 0xfC00;
                            diffZ |= (Int16)unWork;   // 1111 1100 0000 0000
                        }

                        if (null == current)
                        {
                            return null;
                        }

                        DataLogFixFull result = new DataLogFixFull();

                        result.type = DataLogFixFull.TYPE.COMPACT;
                        result.V = velocity;

                        result.WN = current.WN;
                        result.TOW = current.TOW + diffTOW;

                        result.X = current.X;
                        result.X += diffX;

                        result.Y = current.Y;
                        result.Y += diffY;

                        result.Z = current.Z;
                        result.Z += diffZ;

                        return result;
                    }
                    break;

                default:
                    throw new Exception("type error!");
            }
        }

        private void setBaudRate(BaudRate rate)
        {
            Payload p = new Payload(MessageID.Configure_Serial_Port, new byte[] { 0x00, (byte)rate, 0x02 });
            Write(p);

            if (RESULT.RESULT_ACK == this.waitResult(MessageID.Configure_Serial_Port))
            {
                // 成功したから、COM ポートのボーレートも変更する
                int[] ParaRate = { 4800, 9600, 19200, 38400, 57600, 115200, 230400 };
                _com.BaudRate = ParaRate[(int)rate];

                System.Threading.Thread.Sleep(50);
            }
        }

        private void recovery()
        {
            // 頻度の高そうな順にリセットを送ってみる
            int[] ParaRate = { 115200, 38400, 230400, 57600, 19200, 9600, 4800 };
            foreach (int baudRate in ParaRate)
            {
                try
                {
                    _com.BaudRate = baudRate;
                    System.Threading.Thread.Sleep(100);
                    sendRestart();

                    break;
                }
                catch (TimeoutException)
                {
                    // 処理なし
                    continue;
                }
            }
        }

        private void ECEF2LLA(double x, double y, double z, out double latitude, out double longitude, out double altitude)
        {
            const double PI = 3.1415926535898;
            // a = 6,378,137
            const double a = 6378137;
            // f = 1 / 298.257 223 563
            const double f = 1 / 298.257223563;
            // b = a - ( a * f)
            const double b = a - (a * f);
            // p =  √( x^2 + y^2)
            double p = Math.Sqrt((x * x) + (y * y));
            // e^2 = ( (a^2) - (b^2)) / (a^2)
            double e = ((a * a) - (b * b)) / (a * a);
            // e'^2 = ( (a^2) - (b^2)) / (b^2)
            double ed = ((a * a) - (b * b)) / (b * b);
            // θ = atan (z * a / p * b)
            double fi = Math.Atan2((z * a), (p * b));
            // φ = atan{z + e'^2 * b * (sin(θ)^3) / p - e^2 * a * (cos(θ)^3)}
            latitude = Math.Atan2(z + (ed * b * Math.Pow(Math.Sin(fi), 3)), p - (e * a * Math.Pow(Math.Cos(fi), 3)));
            // λ = atan{ y / x }
            longitude = Math.Atan2(y, x);
            // N = a / √( 1 - ( e * sin(φ)^2)
            double N = a / (Math.Sqrt(1 - (e * Math.Pow(Math.Sin(latitude), 2))));
            // h = ( p / cos(φ)) - N
            altitude = (p / Math.Cos(latitude)) - N;

            latitude = (latitude * 180) / PI;
            longitude = (longitude * 180) / PI;
        }

        private DateTime[] LeapSeconds = new DateTime[]
        {
            new DateTime(2006, 1, 1, 0, 0, 0),
            new DateTime(2009, 1, 1, 0, 0, 0),
            new DateTime(2012, 7, 1, 0, 0, 0),
            new DateTime(2015, 7, 1, 0, 0, 0)
        };

        private DateTime GPSTIME2diffUTC(long WN, long TOW)
        {
            // 1999年8月22日にroll overした分を加算
            long weekNumber = WN + 1024;

            // 1981/6/30 +1
            // 1982/6/30 +1
            // 1983/6/30 +1
            // 1985/6/30 +1
            // 1987/12/31 +1
            // 1989/12/31 +1
            // 1990/12/31 +1
            // 1992/6/30 +1
            // 1993/6/30 +1
            // 1994/6/30 +1
            // 1995/12/31 +1
            // 1997/6/30 +1
            // 1998/12/31 +1
            long leapSecond = -13;

            long SEC_OF_WEEK = 7 * 24 * 60 * 60;

            long now = (weekNumber * SEC_OF_WEEK) + TOW;

            DateTime result = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            result = result.AddSeconds(now);
            // 1999年8月22日までの分の閏秒加算（加算だけど減算）
            result = result.AddSeconds(leapSecond);

            // 今までの閏秒加算（加算だけど減算）
            foreach( DateTime dt in LeapSeconds)
            {
                if( result > dt)
                {
                    result = result.AddSeconds(-1);
                    continue;
                }
                break;
            }

            return result;
        }

        private bykIFv1.Point ECEF2LonLat(DataLogFixFull local)
        {
            bykIFv1.Point result = null;

            double x = (double)local.X;
            double y = (double)local.Y;
            double z = (double)local.Z;

            double lat = 0;
            double lon = 0;
            double alt = 0;

            ECEF2LLA(x, y, z, out lat, out lon, out alt);

            DateTime dt = GPSTIME2diffUTC(local.WN, local.TOW);

            decimal spd = (local.V * 1000);
            spd /= 3600;

            result = new bykIFv1.Point(dt, (decimal)lat, (decimal)lon, (decimal)alt, spd, (DataLogFixFull.TYPE.FULL_POI == local.type));

            return result;
        }

        private enum BaudRate
        {
            BaudRate_4800 = 0,
            BaudRate_9600 = 1,
            BaudRate_19200 = 2,
            BaudRate_38400 = 3,
            BaudRate_57600 = 4,
            BaudRate_115200 = 5,
            BaudRate_230400 = 6
        };

        #endregion

        /// <summary>
        /// 自動のポート選択
        /// COMポートの一覧から順次ポートを開いて、Version情報を取得してみる
        /// </summary>
        /// <returns>ポート名称</returns>
        public static string AutoSelectPort()
        {
            SkytraqController port = null;
            foreach (string portName in System.IO.Ports.SerialPort.GetPortNames())
            {
                try
                {
                    port = new SkytraqController(portName);

                    return portName;
                }
                catch
                {
                    continue;
                }
                finally
                {
                    if (null != port)
                    {
                        port.Dispose();
                        port = null;
                    }
                }
            }
            throw new InvalidOperationException("利用できるComPortがない");
        }

        /// <summary>
        /// ソフトバージョン
        /// コンストラクタでの初期化時に取得したものを返す。
        /// </summary>
        public SoftwareVersion SoftwareVersion
        {
            get
            {
                return _version;
            }
        }

        /// <summary>
        /// ソフトバージョン取得
        /// 取得のためにコマンドを再発行する
        /// </summary>
        /// <returns></returns>
        public SoftwareVersion GetSoftwareVersion()
        {
            Payload p = new Payload(MessageID.Query_Software_version, new byte[] { 0x01 });
            Write(p);

            Payload result;
            if (RESULT.RESULT_ACK != this.waitResult(MessageID.Query_Software_version))
            {
                throw new Exception("NACK!");
            }

            result = Read();
            if (result.ID != MessageID.Software_version)
            {
                throw new Exception("Sequence error");
            }

            SoftwareVersion resutl = new SoftwareVersion(result.Body);

            return resutl;

        }

        /// <summary>
        /// Logバッファの状況取得
        /// </summary>
        /// <returns></returns>
        public BufferStatus GetBufferStatus()
        {
            UInt16 totalSectors;
            UInt16 freeSectors;
            bool dataLogEnable;

            GetBufferStatus(out totalSectors, out freeSectors, out dataLogEnable);

            System.Diagnostics.Debug.Print("total={0}/free={1}/log enable={2}", totalSectors, freeSectors, dataLogEnable);

            return new BufferStatus(totalSectors, freeSectors, dataLogEnable);
        }

        /// <summary>
        /// Logデータ読み出し
        /// 読み出した後は、Lat/Lon/Altに変換して一覧に返す
        /// </summary>
        /// <param name="delgateProgress">進捗通知用のDelegate</param>
        /// <returns></returns>
        public List<bykIFv1.Point> ReadLatLonData()
        {
            // 処理が開始されていないことを最初に通知します。
            OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.UNSTART, 0, 0));

            try
            {
                List<bykIFv1.Point> items = null;

                UInt16 totalSectors;
                UInt16 freeSectors;
                bool dataLogEnable;
                // 初期化の開始を通知します。
                OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.INIT, 0, 2));
                // ボーレートの設定をする
                setBaudRate(BaudRate.BaudRate_115200);
                // 初期化の進捗を通知します。
                OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.INIT, 1, 2));

                // セクタ数を見る
                GetBufferStatus(out totalSectors, out freeSectors, out dataLogEnable);
                System.Diagnostics.Debug.Print("freeSectors/totalSectors = {0}/{1}", freeSectors, totalSectors);
                // 初期化の進捗を通知します。
                OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.INIT, 2, 2));

                // データが無効なら終わる
                //if (!dataLogEnable) return null;

                UInt16 sectors = totalSectors;
                sectors -= freeSectors;
                if (0 < sectors)
                {
                    // 読み出し開始を通知します
                    OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.READ, 0, sectors));

                    // データを読み込みます
                    byte[] readLog = new byte[sectors * SECTOR_SIZE];
                    int retryCount = 0;
                    int readSectors = 0;
                    for (int index = 0; index < sectors;)
                    {
                        readSectors = (SECTOR_COUNT <= (sectors - index)) ? SECTOR_COUNT : 1;

                        System.Diagnostics.Debug.Print("step-1");
                        // 読み出しを指示
                        sendReadBuffer(index, readSectors);

                        System.Diagnostics.Debug.Print("step-2");
                        // データを取得する
                        ReadLogBuffer(readLog, index * SECTOR_SIZE, readSectors * SECTOR_SIZE);

#if false
                        string s = string.Format(@"C:\Users\Tomo\Documents\GEOTagInjector\SkyTraqSerial\hoge_{0}.bin", index);
                        using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(File.OpenWrite(s)))
                        {
                            bw.Write(readLog, index * SECTOR_SIZE, readSectors * SECTOR_SIZE);
                        }
#endif
                        System.Diagnostics.Debug.Print("step-3");
                        // CS取得
                        byte resultCS = ReadLogBufferCS();

                        System.Diagnostics.Debug.Print("step-4");
                        byte calcCS = 0;
                        using (MemoryStream ms = new MemoryStream(readLog, index * SECTOR_SIZE, readSectors * SECTOR_SIZE))
                        {
                            using (System.IO.BinaryReader br = new System.IO.BinaryReader(ms))
                            {
                                while (true)
                                {
                                    try
                                    {
                                        calcCS ^= br.ReadByte();
                                    }
                                    catch (System.IO.EndOfStreamException)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        System.Diagnostics.Debug.Print("step-5");
                        if (calcCS != resultCS)
                        {
                            System.Diagnostics.Debug.Print("step-6-1");
                            if (3 <= retryCount)
                            {
                                throw new Exception("データ取得のリトライが失敗した");
                            }
                            ++retryCount;
                        }
                        else
                        {
                            System.Diagnostics.Debug.Print("step-6-2");
                            // 読み出し状況を通知します
                            OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.READ, index, sectors));

                            retryCount = 0;
                            // 次を読み込みます。
                            index += readSectors;
                        }
                        System.Diagnostics.Debug.Print("step-7");
                        System.Threading.Thread.Sleep(100);
                    }


                    // 読み出したデータを変換します
                    items = new List<bykIFv1.Point>();
                    using (BinaryReader br = new BinaryReader(new MemoryStream(readLog)))
                    {
                        // 変換開始を通知します
                        OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.CONVERT, 0, (int)br.BaseStream.Length));

                        DataLogFixFull local = null;
                        DataLogFixFull latest = null;
                        while (true)
                        {
                            try
                            {
                                // ECEFに変換する
                                local = ReadLocation(br, latest);
                                if (null != local)
                                {
                                    /*
                                    string s = @"C:\Users\Tomo\Documents\GEOTagInjector\SkyTraqSerial\XYZ_WN_TOW.txt";
                                    using (System.IO.TextWriter tw = new System.IO.StreamWriter(s, true))
                                    {
                                        tw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}", local.X, local.Y, local.Z, local.WN, local.TOW));
                                    }*/

                                    if(DataLogFixFull.TYPE.FULL == local.type || DataLogFixFull.TYPE.FULL_POI == local.type)
                                    {
                                        latest = local;
                                    }

                                    // longitude/latitudeに変換する
                                    items.Add(ECEF2LonLat(local));
                                }
                                // 変換状況を通知します
                                OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.CONVERT, (int)br.BaseStream.Position, (int)br.BaseStream.Length));

                            }
                            catch (System.IO.EndOfStreamException)
                            {
                                break;
                            }
                        }
                        // 変換終了を通知します
                        OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.CONVERT, (int)br.BaseStream.Length, (int)br.BaseStream.Length));
                    }
                }

                // リセット開始を通知します
                OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.RESTERT, 0, 1));
                // Restartして終了
                sendRestart();
                // リセット終了を通知します
                OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.RESTERT, 1, 1));

                // longitude/latitudeの配列を返す
                return items;

            }
            catch (TimeoutException)
            {
                // リセット開始を通知します
                OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.RESTERT, 0, 1));

                try
                {
                    // Restartして終了
                    sendRestart();
                }
                catch (TimeoutException)
                {
                    // 処理なし
                    recovery();
                }

                // リセット終了を通知します
                OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.RESTERT, 1, 1));
            }

            // 値は返せない
            return null;
        }

        public bool EraceLatLonData()
        {
            try
            {
                // ボーレートの設定をする
                setBaudRate(BaudRate.BaudRate_38400);

                Payload p = new Payload(MessageID.Clear_Data_Logging_Buffer);
                Write(p);

                if (RESULT.RESULT_ACK != this.waitResult(MessageID.Clear_Data_Logging_Buffer))
                {
                    throw new Exception("削除できない");
                }

                return true;
            }
            catch (TimeoutException)
            {
                try
                {
                    // Restartして終了
                    sendRestart();
                }
                catch (TimeoutException)
                {
                    // 処理なし
                    recovery();
                }
                return false;
            }
        }
    }
}
