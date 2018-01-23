using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Modbus.Device;

namespace ModbusTcpTester
{
    class Program
    {
        public const int WriteReadTimeout = 500;

        public static void Main(string[] args)
        {
            //Working();
            NotWorking();
        }

        public static void NotWorking()
        {
            var ip = "192.168.100.13";
            var port = 8081;

            var client = new TcpClient();
            var modbusIpMaster = ModbusIpMaster.CreateIp(client);

            if (!client.ConnectAsync(ip, port).Wait(1000))
            {
                throw new Exception("Connection error");
            }

            Console.WriteLine("Connected");

            client.ReceiveTimeout = WriteReadTimeout;
            client.SendTimeout = WriteReadTimeout;
            //modbusIpMaster.Transport.RetryOnOldResponseThreshold = 10;

            new Task(() =>
            {
                //modbusIpMaster.WriteSingleRegister(40069, 0);
                while (true)
                {
                    try
                    {
                        var index = /*Demand - 40083*/ /*battery state - 40311*/ /* privod z panelov - 40100 */40274;
                        var numberOfPoints = 1;

                        //client.GetStream().Write(new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x01, 0x03, 0x9C, 0x84, 0x00, 0x01 }, 0, 12);
                        //var buffer2 = new byte[256];
                        //client.Client.Receive(buffer2);
                        //client.GetStream().Read(buffer2, 0, 255);

                        //Thread.Sleep(5000);
                        //try
                        //{
                        var buffer = modbusIpMaster.ReadHoldingRegisters((ushort) index, (byte) numberOfPoints);
                        //var buffer2 = modbusIpMaster.ReadHoldingRegisters((ushort)40083, (byte)numberOfPoints);
                        //    var task = modbusIpMaster.ReadHoldingRegistersAsync((ushort)index, (byte)numberOfPoints);

                        //    task.Wait();
                        //}
                        //catch (Exception e)
                        //{
                        //    Console.WriteLine(e);
                        //    Console.WriteLine();
                        //    Console.WriteLine();
                        //}
                        //client.GetStream().Write(new byte[] { 0x04, 0x00, 0x00, 0x00, 0x00, 0x10, 0x01, 0x03, (byte)156, (byte)185, 0x00, 0x01 }, 0, 12);
                        //var buffer2 = new byte[256];
                        //client.Client.Receive(buffer2);
                        //client.GetStream().Read(buffer2, 0, 255);
                        var intBuffer = buffer.Select(b => (short) b);
                        Console.WriteLine(string.Join(",", intBuffer));
                        //Console.WriteLine(buffer2[0] - buffer[0]);
                        Console.WriteLine();
                    }
                    catch (IOException ioException)
                    {
                        Console.WriteLine(ioException);
                        Console.WriteLine();
                        Console.WriteLine();

                        if (ioException.Message.Contains("transaction ID"))
                        {
                            Console.WriteLine("-----------Reconnecting-----------");
                            client.Close();
                            client = new TcpClient();
                            modbusIpMaster = ModbusIpMaster.CreateIp(client);
                            if (!client.ConnectAsync(ip, port).Wait(1000))
                            {
                                throw new Exception("Connection error");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine();
                        Console.WriteLine();
                    }

                    Thread.Sleep(100);
                }
            }).Start();

            while (true) { }
            ;
        }

        public static void Working()
        {
            var ip = "192.168.100.13";
            var port = 8081;

            var client = new TcpClient();
            var modbusIpMaster = ModbusIpMaster.CreateIp(client);

            if (!client.ConnectAsync(ip, port).Wait(1000))
            {
                throw new Exception("Connection error");
            }

            Console.WriteLine("Connected");

            client.ReceiveTimeout = WriteReadTimeout;
            client.SendTimeout = WriteReadTimeout;
            client.NoDelay = true;
            client.GetStream().ReadTimeout = WriteReadTimeout;
            client.GetStream().WriteTimeout = WriteReadTimeout;
            modbusIpMaster.Transport.ReadTimeout = WriteReadTimeout;
            modbusIpMaster.Transport.WriteTimeout = WriteReadTimeout;
            modbusIpMaster.Transport.Retries = 2;


            while (true)
            {
                try
                {
                    client.GetStream().Write(new byte[] { 0x04, 0x00, 0x00, 0x00, 0x00, 0x10, 0x01, 0x03, (byte)156, (byte)185, 0x00, 0x01 }, 0, 12);

                    //Thread.Sleep(1000);

                    var buffer = new byte[256];
                    //client.Client.Receive(buffer);
                    client.GetStream().Read(buffer, 0, 255);
                    Console.WriteLine(string.Join(",", buffer));
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine();
                    Console.WriteLine();
                }

                //Thread.Sleep(5000);
            }
        }
    }
}
