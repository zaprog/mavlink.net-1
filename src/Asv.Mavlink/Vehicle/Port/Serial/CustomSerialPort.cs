using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Asv.Mavlink.Port;

namespace Asv.Mavlink
{
    public class CustomSerialPort : PortBase
    {
        private readonly SerialPortConfig _config;
        private SerialPort _serial;

        public CustomSerialPort(SerialPortConfig config)
        {
            _config = config;
        }

        protected override Task InternalSend(byte[] data, int count, CancellationToken cancel)
        {
            return _serial?.BaseStream.WriteAsync(data, 0, count,cancel);
        }

        protected override void InternalStop()
        {
            _serial?.Dispose();
            if (_serial != null) _serial.DataReceived -= _serial_DataReceived;
        }

        protected override void InternalStart()
        {
            _serial = new SerialPort(_config.PortName,_config.BoundRate,_config.Parity,_config.DataBits,_config.StopBits);
            _serial.Open();
            _serial.DataReceived += _serial_DataReceived;
        }

        private void _serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var data = new byte[_serial.BytesToRead];
            _serial.Read(data, 0, data.Length);
            InternalOnData(data);
        }
    }
}