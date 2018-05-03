using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace NetModel.NetMgr.Tcp
{
    public class TcpHelper
    {
        NetworkStream stream;
        public void ConnectServer(string serverIp,int port)
        {
            if (string.IsNullOrEmpty(serverIp) || port <= 0 || port > 65535)
                return;

            TcpClient client = new TcpClient();
            try
            {
                client.Connect(IPAddress.Parse(serverIp), port);
                stream = client.GetStream();
            }
            catch (ArgumentNullException e)
            {

            }
            catch (FormatException e)
            {

            }
            catch (ObjectDisposedException e)
            {

            }
            catch (InvalidOperationException e)
            {

            }            
            catch (ArgumentOutOfRangeException e)
            {

            }            
            catch (SocketException e)
            {

            }
            
        }

        public void GetData()
        {

        }

    }
}
