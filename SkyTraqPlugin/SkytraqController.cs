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

        private const byte START_OF_SEQUENCE_1 = 0xa0;
        private const byte START_OF_SEQUENCE_2 = 0xa1;

        private const byte END_OF_SEQUENCE_1 = 0x0d;
        private const byte END_OF_SEQUENCE_2 = 0x0a;
        private const UInt16 MASK_LOBYTE = 0x00FF;

        private const int READ_TIMEOUT = (10 * 1000);

        public SkytraqController(string portName)
        {
            _com = new SerialPort(portName, 38400, Parity.None, 8, StopBits.One);

            _com.ReadTimeout = READ_TIMEOUT;
            _com.Open();
        }

        #region 内部処理
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
                if (READ_TIMEOUT < ts.TotalMilliseconds)
                    throw new TimeoutException();
            }

            // payload length
            UInt16 payloadLength = (UInt16)(0x00FF & _com.ReadByte());
            payloadLength <<= 8;
            payloadLength |= (UInt16)(0x00FF & _com.ReadByte());

            // read payload
            System.Threading.Thread.Sleep(10);
            byte[] rawPayload = new byte[payloadLength];
            for (int readCount = 0; readCount < payloadLength;)
            {
                readCount += _com.Read(rawPayload, readCount, payloadLength - readCount);
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
                }
                // データは読み出せたけど、目的のデータが読み出せないので、タイムアウトとして処理します。
                ts = DateTime.Now - start;
                if (READ_TIMEOUT < ts.TotalMilliseconds)
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

        private static int SECTOR_SIZE = 4096;

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

        private void RequestBufferStatus(out UInt16 totalSectors, out UInt16 freeSectors, out bool dataLogEnable)
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

            totalSectors = (UInt16)result.Body[8];
            totalSectors <<= 8;
            totalSectors |= (UInt16)result.Body[7];

            freeSectors = (UInt16)result.Body[6];
            freeSectors <<= 8;
            freeSectors |= (UInt16)result.Body[5];

            dataLogEnable = (0x01 == result.Body[33]);
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
                        data.V = velocity;

                        byte b = (byte)(0x00FF & br.ReadByte());
                        data.TOW = (byte)(0x000f & (b >> 4));
                        data.WN = (UInt16)(0x0030 & b);
                        data.WN <<= 4;
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

        private void ECEF2LLA( double x, double y, double z, out double latitude, out double longitude, out double altitude)
        {
            // a = 6,378,137
            double a = 6378137;
            // f = 1 / 298.257 223 563
            double f = 1 / 298.257223563;
            // b = a - ( a * f)
            double b = a - (a * f);
            // p =  √( x^2 + y^2)
            double p = Math.Pow((a * a) + (b * b), -1);
            // e = ( (a^2) - (b^2)) / (a^2)
            double e = ((a * a) - (b * b)) / (a * a);
            // e' = ( pow(a,2) - pow(b,2)) / pow(b,2)
            double ed = ((a * a) - (b * b)) / (b * b);
            // θ = atan (z * a / p * b)
            double fi = Math.Atan2((z * a), (p * b));
            // φ = atan{z + e' * b * (sin(θ)^3) / p - e * a * (cos(θ)^3)}
            latitude = Math.Atan2(z + (e * b * Math.Pow(Math.Sign(fi), 3)), p - (e * a * Math.Pow(Math.Cos(fi), 3)));
            // λ = atan{ x / y }
            longitude = Math.Atan2(x, y);
            // N = a / √( 1 - ( e * sin(φ)^2)
            double N = a / (Math.Pow( 1 - ( e * Math.Pow( Math.Sin(latitude),2)), -2));
            // h = ( p / cos(φ)) - N
            altitude = (p / Math.Cos(fi)) - N;
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

            DateTime result = new DateTime(1980, 1, 6, 0, 0, 0);
            result.AddSeconds(now);
            // 1999年8月22日までの分の閏秒加算（加算だけど減算）
            result.AddSeconds(leapSecond);

            // 今までの閏秒加算（加算だけど減算）
            foreach( DateTime dt in LeapSeconds)
            {
                if( result > dt)
                {
                    result.AddSeconds(-1);
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

            result = new bykIFv1.Point(dt, (decimal)lon, (decimal)lat, (decimal)alt, spd);

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

        public List<bykIFv1.Point> ReadLatLonData()
        {
            try
            {
                List<bykIFv1.Point> items = null;

                UInt16 totalSectors;
                UInt16 freeSectors;
                bool dataLogEnable;
                // ボーレートの設定をする
                setBaudRate(BaudRate.BaudRate_115200);

                // セクタ数を見る
                RequestBufferStatus(out totalSectors, out freeSectors, out dataLogEnable);
                System.Diagnostics.Debug.Print("freeSectors/totalSectors = {0}/{1}", freeSectors, totalSectors);

                // データが無効なら終わる
                //if (!dataLogEnable) return null;

                UInt16 sectors = totalSectors;
                sectors -= freeSectors;
                if (0 < sectors)
                {
                    byte[] readLog = new byte[sectors * SECTOR_SIZE];
                    int retryCount = 0;
                    int readSectors = 0;
                    for (int index = 0; index < sectors;)
                    {
                        readSectors = (2 <= (sectors - index)) ? 2 : 1;

                        // 読み出しを指示
                        sendReadBuffer(index, readSectors);

                        // データを取得する
                        ReadLogBuffer(readLog, index * SECTOR_SIZE, readSectors * SECTOR_SIZE);

                        string s = string.Format(@"C:\Users\Tomo\Documents\GEOTagInjector\SkyTraqSerial\hoge_{0}.bin", index);
                        using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(File.OpenWrite(s)))
                        {
                            bw.Write(readLog, index * SECTOR_SIZE, readSectors * SECTOR_SIZE);
                        }

                        // CS取得
                        byte resultCS = ReadLogBufferCS();

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

                        if (calcCS != resultCS)
                        {
                            if (3 <= retryCount)
                            {
                                throw new Exception("データ取得のリトライが失敗した");
                            }
                            ++retryCount;
                        }
                        else
                        {
                            retryCount = 0;
                            // 次を読み込みます。
                            index += readSectors;
                        }
                        System.Threading.Thread.Sleep(100);
                    }

                    items = new List<bykIFv1.Point>();
                    using (BinaryReader br = new BinaryReader(new MemoryStream(readLog)))
                    {
                        DataLogFixFull local = null;
                        while (true)
                        {
                            try
                            {
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

                    }
                }

                // Restartして終了
                sendRestart();

                // longitude/latitudeの配列を返す
                return items;

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
            }

            // 値は返せない
            return null;
        }

        public bool EraceLatLonData()
        {
            try
            {
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

        public void Dispose()
        {
            this.sendRestart();
            if (_com.IsOpen)
            {
                _com.Close();
                _com = null;
            }
        }
    }
}
