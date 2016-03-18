using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;

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
        private const int READ_TIMEOUT_INTERNAL = (200);

        private const int EPHEMERIS_BLOCK_SIZE = 0x56;
        private const int EPHEMERIS_WRITE_SIZE = 8192;

        private const int RETRY_COUNT = 3;
        private const int RETRY_TIME = 100;


        public event ReadProgressEventHandler OnRead;
        public event SetEphemerisProgressHandler OnSetEphemeris;

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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="portName">ポート名称</param>
        /// <param name="timeout">タイムアウト値</param>
        private SkytraqController(string portName, int timeout)
        {
            _com = new SerialPort(portName, 38400, Parity.None, 8, StopBits.One);

            _com.ReadTimeout = timeout;
            _com.Open();

            // 同期しつつバージョンを取得してみる
            SyncBaudRate(out _version);
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

        private RESULT waitSendResult(Payload sendPayload)
        {
            for (int index = 1; ; ++index)
            {
                try
                {
                    Write(sendPayload);

                    Payload readPayload = null;
                    while (true)
                    {
                        readPayload = Read();

                        if (readPayload.ID == MessageID.ACK)
                        {
                            if (sendPayload.ID == (MessageID)readPayload.Body[0])
                            {
                                return RESULT.RESULT_ACK;
                            }
                        }
                        else if (readPayload.ID == MessageID.NACK)
                        {
                            if (sendPayload.ID == (MessageID)readPayload.Body[0])
                            {
                                return RESULT.RESULT_NACK;
                            }
                        }
                    }
                }
                catch (TimeoutException)
                {
                    if (index >= RETRY_COUNT)
                        throw;

                    System.Threading.Thread.Sleep(RETRY_TIME);
                }
            }
        }

        private void sendRestart()
        {
            Payload p = new Payload(MessageID.System_Restart, new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            if (RESULT.RESULT_ACK == this.waitSendResult(p))
            {
                // リセットが成功したのでボーレートも戻す
                _com.BaudRate = 38400;
                System.Threading.Thread.Sleep(50);
            }
        }

        private RESULT sendReadBuffer(int offsetSector, int sectorCount)
        {
            Payload p = new Payload(MessageID.Enable_data_read_from_the_log_buffer, new byte[] { 0x00, 0x00, 0x00, 0x02 });
            p.Body[0] = (byte)(0x00FF & (offsetSector >> 8));
            p.Body[1] = (byte)(0x00FF & (offsetSector >> 0));
            p.Body[2] = (byte)(0x00FF & (sectorCount >> 8));
            p.Body[3] = (byte)(0x00FF & (sectorCount >> 0));

            return waitSendResult(p);
        }

        private void GetBufferStatus(out UInt16 totalSectors, out UInt16 freeSectors, out bool dataLogEnable)
        {
            UInt32 dummy1;
            UInt32 dummy2;
            UInt32 dummy3;

            GetBufferStatus(out  totalSectors, out  freeSectors, out dummy1, out dummy2, out dummy3, out dataLogEnable);
        }

        private void GetBufferStatus(out UInt16 totalSectors, out UInt16 freeSectors, out UInt32 time, out UInt32 distance, out UInt32 speed, out bool dataLogEnable)
        {
            Payload p = new Payload(MessageID.Request_Information_of_the_Log_Buffer_Status);
            if (RESULT.RESULT_ACK != this.waitSendResult(p))
            {
                throw new Exception("NACK!");
            }

            Payload result;
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

            time = (UInt32)result.Body[15];
            time <<= 8;
            time |= (UInt32)result.Body[14];
            time <<= 8;
            time |= (UInt32)result.Body[14];
            time <<= 8;
            time |= (UInt32)result.Body[12];

            distance = (UInt32)result.Body[23];
            distance <<= 8;
            distance |= (UInt32)result.Body[22];
            distance <<= 8;
            distance |= (UInt32)result.Body[21];
            distance <<= 8;
            distance |= (UInt32)result.Body[20];

            speed = (UInt32)result.Body[31];
            speed <<= 8;
            speed |= (UInt32)result.Body[30];
            speed <<= 8;
            speed |= (UInt32)result.Body[29];
            speed <<= 8;
            speed |= (UInt32)result.Body[28];

            dataLogEnable = (0x01 == result.Body[32]);
        }

        private DataLogFixFull ReadLocation(BinaryReader br, DataLogFixFull current)
        {
            UInt16 pos1 = (UInt16)(0x00FF & br.ReadByte());
            pos1 <<= 8;
            pos1 |= (UInt16)(0x00FF & br.ReadByte());

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

                        UInt16[] unWork = new UInt16[8];
                        for (int index = 0; index < unWork.Length; ++index)
                        {
                            unWork[index] = (UInt16)(0x00FF & br.ReadByte());
                            unWork[index] <<= 8;
                            unWork[index] |= (UInt16)(0x00FF & br.ReadByte());
                        }

                        data.WN = (UInt16)( 0x03ff & unWork[0]);
                        data.TOW = unWork[1];
                        data.TOW <<= 4;
                        data.TOW |= (UInt32)( 0x000f & (unWork[0] >> 12));

                        data.X = unWork[3];
                        data.X <<= 16;
                        data.X |= unWork[2];

                        data.Y = unWork[5];
                        data.Y <<= 16;
                        data.Y |= unWork[4];

                        data.Z = unWork[7];
                        data.Z <<= 16;
                        data.Z |= unWork[6];

                        return data;
                    }
                    break;

                // FIX COMPACT
                case 0x8000:
                    {
                        UInt16[] unWork = new UInt16[3];
                        for (int index = 0; index < unWork.Length; ++index)
                        {
                            unWork[index] = (UInt16)(0x00FF & br.ReadByte());
                            unWork[index] <<= 8;
                            unWork[index] |= (UInt16)(0x00FF & br.ReadByte());
                        }
                        UInt16 diffTOW = unWork[0];
                        Int16 diffX = (Int16)(0x03ff & (unWork[1] >> 6));
                        Int16 diffY = (Int16)(0x03C0 & (unWork[2] >> 6));
                        diffY |= (Int16)(0x003f & unWork[1]);
                        Int16 diffZ = (Int16)(0x03ff & unWork[2]);

                        const Int16 flag = 511;
                        if (flag < diffX)
                        {
                            diffX = (Int16)(flag - diffX);
                        }
                        if (flag < diffY)
                        {
                            diffY = (Int16)(flag - diffY);
                        }
                        if (flag < diffZ)
                        {
                            diffZ = (Int16)(flag - diffZ);
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
            if (RESULT.RESULT_ACK == this.waitSendResult(p))
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


        private void WriteString(string s)
        {
            _com.Write(s);
        }

        private string ReadString()
        {
            string result = string.Empty;
            char ch;
            int n;

            while (true)
            {
                n = _com.ReadChar();
                if (-1 == n) break;

                ch = (char)n;
                if (0x00 == ch) break;

                result += ch;
            }
            return result;
        }


        private void ReadSectors(byte[] readLog, UInt16 sectors)
        {
            int retryCount = 0;
            int readSectors = 0;
            for (int index = 0; index < sectors;)
            {
                readSectors = (SECTOR_COUNT <= (sectors - index)) ? SECTOR_COUNT : 1;

                System.Diagnostics.Debug.Print("step-1");
                // 読み出しを指示
                if (RESULT.RESULT_ACK != sendReadBuffer(index, readSectors))
                {
                    throw new Exception("読み出しコマンドでエラーが返ってきた");
                }

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
        }

        private void SectorsData2LatLon(byte[] readLog, List<bykIFv1.Point> items)
        {
            using (BinaryReader br = new BinaryReader(new MemoryStream(readLog)))
            {
                // 変換開始を通知します
                OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.CONVERT, 0, (int)br.BaseStream.Length));

                DataLogFixFull local = null;
                while (true)
                {
                    try
                    {
                        // 変換状況を通知します
                        OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.CONVERT, (int)br.BaseStream.Position, (int)br.BaseStream.Length));

                        // ECEFに変換する
                        local = ReadLocation(br, local);
                        if (null != local)
                        {
                            // longitude/latitudeに変換する
                            items.Add(ECEF2LonLat(local));
                        }
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
                    port = new SkytraqController(portName, READ_TIMEOUT_INTERNAL);

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
            if (RESULT.RESULT_ACK != this.waitSendResult(p))
            {
                throw new Exception("NACK!");
            }

            Payload result;
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

            UInt32 time;
            UInt32 distance;
            UInt32 speed;

            GetBufferStatus(out totalSectors, out freeSectors, out time, out distance, out speed, out dataLogEnable);

            System.Diagnostics.Debug.Print("total={0}/free={1}/time={2}/distance={3}/speed={4}/log enable={5}", totalSectors, freeSectors, time, distance, speed, dataLogEnable);

            return new BufferStatus(totalSectors, freeSectors, time, distance, speed, dataLogEnable);
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

            List<bykIFv1.Point> items = new List<bykIFv1.Point>();
            try
            {
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
                if (!dataLogEnable) return null;

                UInt16 sectors = totalSectors;
                sectors -= freeSectors;
                if (0 < sectors)
                {
                    // 読み出し開始を通知します
                    OnRead(new ReadProgressEvent(ReadProgressEvent.READ_PHASE.READ, 0, sectors));

                    // データを読み込みます
                    byte[] readLog = new byte[sectors * SECTOR_SIZE];
                    ReadSectors(readLog, sectors);

                    // 読み出したデータを変換します
                    SectorsData2LatLon(readLog, items);
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
            return items;
        }

        /// <summary>
        /// データの消去を行います
        /// </summary>
        /// <returns></returns>
        public bool EraceLatLonData()
        {
            try
            {
                // ボーレートの設定をする
                setBaudRate(BaudRate.BaudRate_38400);

                Payload p = new Payload(MessageID.Clear_Data_Logging_Buffer);
                if (RESULT.RESULT_ACK != this.waitSendResult(p))
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

        /// <summary>
        /// A-GPS(Ephemeris)更新
        /// </summary>
        /// <param name="ephemeris">更新データ</param>
        /// <returns></returns>
        public bool SetEphemeris(byte[] ephemeris)
        {
            try
            {
                if (0 != (ephemeris.Length % EPHEMERIS_BLOCK_SIZE))
                {
                    throw new Exception("Ehemerisのサイズがおかしいと思う。");
                }

                int max = ephemeris.Length;
                int value = 0;

                OnSetEphemeris(new SetEphemerisProgressEvent(value, max));


                // チェックサムを計算します。
                byte checkSuma = 0;
                byte checkSumb = 0;
                for (int index = 0; index < 0x10000; ++index)
                {
                    checkSumb += ephemeris[index];
                }
                checkSuma = checkSumb;
                for (int index = 0x10000; index < ephemeris.Length; ++index)
                {
                    checkSuma += ephemeris[index];
                }

                // Ephemeris書き込み開始を送信
                Payload p = new Payload(MessageID.Set_AGPS);
                if (RESULT.RESULT_ACK != this.waitSendResult(p))
                {
                    throw new Exception("設定できない");
                }
                
                // データサイズ、チェックサム送信
                string s = string.Format("BINSIZE = {0} Checksum = {1} Checksumb = {2} \0", ephemeris.Length, checkSuma, checkSumb);
                System.Diagnostics.Debug.Print(s);
                WriteString(s);

                // 結果受信
                string result = ReadString();
                System.Diagnostics.Debug.Print(result);
                if ("OK" != result) return false;

                byte[] blockdata = new byte[EPHEMERIS_WRITE_SIZE];
                int writeSize = 0;
                for( int index = 0; index < ephemeris.Length; index+= EPHEMERIS_WRITE_SIZE)
                {
                    // データを書き込みます
                    writeSize = ((ephemeris.Length - index) >= EPHEMERIS_WRITE_SIZE) ? EPHEMERIS_WRITE_SIZE : (ephemeris.Length - index);
                    System.Buffer.BlockCopy(ephemeris, index, blockdata, 0, writeSize);
                    _com.Write(blockdata, 0, writeSize);

                    // 結果を取得します。
                    result = ReadString();
                    System.Diagnostics.Debug.Print(result);
                    if ("OK" != result) return false;

                    value += writeSize;
                    OnSetEphemeris(new SetEphemerisProgressEvent(value, max));
                }

                // 結果を取得します。
                result = ReadString();
                System.Diagnostics.Debug.Print(result);
                if ("END" != result) return false;

                System.Threading.Thread.Sleep(500);

                // Ephemeris書き込み開始を送信
                p = new Payload(MessageID.Enable_AGPS, new byte[] { 0x01 });
                if (RESULT.RESULT_ACK != this.waitSendResult(p))
                {
                    throw new Exception("A-GPSを有効にできない");
                }

                OnSetEphemeris(new SetEphemerisProgressEvent(max, max));

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

        /// <summary>
        /// 設定情報の更新
        /// </summary>
        /// <param name="bs">設定情報</param>
        public void SetLogConfigure(BufferStatus bs)
        {
            Payload p = new Payload(MessageID.Configuration_Data_Logging_Criteria, new byte[] {
                0xff, 0xff, 0xff, 0xff, // max time
                0x00, 0x00, 0x00, 0x00, // min time
                0xff, 0xff, 0xff, 0xff, // max distance
                0x00, 0x00, 0x00, 0x00, // min distance
                0xff, 0xff, 0xff, 0xff, // max speed
                0x00, 0x00, 0x00, 0x00, // min speed
                0x01,   // datalog enale
                0x00    // reserved
            });

            // canwayの動きを参考にして、両方が0ならTimeを5とする
            if (0 == bs.Distance && 0 == bs.Time)
                bs.Time = 5;

            p.Body[4] = (byte)(0x000000ff & (bs.Time >> 24));
            p.Body[5] = (byte)(0x000000ff & (bs.Time >> 16));
            p.Body[6] = (byte)(0x000000ff & (bs.Time >> 8));
            p.Body[7] = (byte)(0x000000ff & (bs.Time >> 0));

            p.Body[12] = (byte)(0x000000ff & (bs.Distance >> 24));
            p.Body[13] = (byte)(0x000000ff & (bs.Distance >> 16));
            p.Body[14] = (byte)(0x000000ff & (bs.Distance >> 8));
            p.Body[15] = (byte)(0x000000ff & (bs.Distance >> 0));

            p.Body[20] = (byte)(0x000000ff & (bs.Speed >> 24));
            p.Body[21] = (byte)(0x000000ff & (bs.Speed >> 16));
            p.Body[22] = (byte)(0x000000ff & (bs.Speed >> 8));
            p.Body[23] = (byte)(0x000000ff & (bs.Speed >> 0));

            if (RESULT.RESULT_ACK != this.waitSendResult(p))
            {
                throw new Exception("設定の書き込みに失敗");
            }
        }
    }
}
