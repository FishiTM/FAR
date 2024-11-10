using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Net.Sockets;
using SocketIOClient;

namespace FAR
{
    public class WebApp
    {
        public static string receivedData = null;
        public static string receivedConfig = null;
        public static void socketIoManager()
        {
            var client = new SocketIOClient.SocketIO("http://localhost:3004/");
            
            client.On("data", response =>
            {
                receivedData = response.ToString();
            });

            client.On("config", response =>
            {
                receivedConfig = response.ToString();
            });

            client.ConnectAsync();
            //// Instantiate the socket.io connection
            //var socket = IO.Socket("http://127.0.0.1:3004");
            //// Upon a connection event, update our status
            //socket.On(Socket.EVENT_CONNECT, () =>
            //{
            //    receivedData = "Connected!";
            //});
            //// Upon temperature data, update our temperature status
            //socket.On("config", (data) =>
            //{
            //    var selectedConfig = new { config = "" };
            //    var tempValue = JsonConvert.DeserializeAnonymousType((string)data, selectedConfig);
            //    receivedData = JsonConvert.SerializeObject(data);

            //});
            //socket.On("data", (data) =>
            //{
            //    var selectedData = new { X = 0, Y = 0, Z = 0 };
            //    var tempValue = JsonConvert.DeserializeAnonymousType((string)data, selectedData);
            //    receivedData = "test";//JsonConvert.SerializeObject(data);
            //});
        }
    }
}