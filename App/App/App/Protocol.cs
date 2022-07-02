using System;
using System.Collections.Generic;
using System.Text;

namespace App
{
    public class Protocol
    {
        private object _sync = new object();
        private const int BUFFER_MAX_LENGTH = 500;
        private List<byte> buffer;
        private List<byte> dataFrame;
        private int _header1;
        private int _header2;
        private int _tail1;
        private int _tail2;
        private int _datasize;
        public Protocol(int header1, int header2, int tail1, int tail2, int datasize)
        {
            _header1 = header1;
            _header2 = header2;
            _tail1 = tail1;
            _tail2 = tail2;

            _datasize = datasize;

            buffer = new List<byte>(BUFFER_MAX_LENGTH);
            dataFrame = new List<byte>();
        }

        private void Decode()
        {
            lock (_sync)
            {
                if (FindHeader())
                {
                    if (FindTail())
                    {
                        FormatedData();
                    }
                }
            }
        }
        private bool FindHeader()
        {
            var buffersize = buffer.Count;
            if (buffersize < 2) return false;

            for (int i = 0; i < buffersize; i++)
            {
                //if ((buffer.Count < _datasize)) return false;

                if (buffer[0] == _header1 && buffer[1] == _header2)
                {
                    buffer.RemoveAt(0);
                    buffer.RemoveAt(0);
                    return true;
                }
                else
                {
                    buffer.RemoveAt(0);
                }
            }
            return false;
        }
        private bool FindTail()
        {
            var buffersize = buffer.Count;
            if (buffersize < 2) return false;

            for (int i = 0; i < buffersize; i++)
            {
                //if (buffer.Count < _datasize)
                //{
                //    dataFrame.Clear();
                //    return false;
                //}

                if ((buffer[0] == _tail1 && buffer[1] == _tail2) && dataFrame.Count == _datasize)
                {
                    buffer.RemoveAt(0);
                    buffer.RemoveAt(0);

                    return true;
                }
                else
                {
                    dataFrame.Add(buffer[0]);
                    buffer.RemoveAt(0);
                }
            }
            dataFrame.Clear();
            return false;
        }
        private void FormatedData()
        {
            var frame = dataFrame.ToArray();

            byte[] temperature_array = new byte[2];
            Array.Copy(frame, 0, temperature_array, 0, 2);
            var temperature_raw = BitConverter.ToInt16(temperature_array, 0);
            var temperature = (double)temperature_raw / 100;
            
            byte[] ddp_ads_array = new byte[2];//Adc do Ads
            Array.Copy(frame, 2, ddp_ads_array, 0, 2);
            var ddp_ads_raw = BitConverter.ToInt16(ddp_ads_array, 0); 
            var ddp_ads = (double)ddp_ads_raw / 100;
            
            byte[] ddp_intern_array = new byte[2];//Adc do arduino
            Array.Copy(frame, 4, ddp_intern_array, 0, 2);
            var ddp_intern_raw = BitConverter.ToInt16(ddp_intern_array, 0);
            var ddp_intern = (double)ddp_intern_raw / 100;

            OnDataFromatedEvent?.Invoke(temperature, ddp_ads, ddp_intern);

            dataFrame.Clear();
            buffer.Clear();
        }

        public Action<double, double, double> OnDataFromatedEvent;

        public void Add(IEnumerable<byte> data)
        {
            buffer.AddRange(data);
            Decode();
        }
    }
}
