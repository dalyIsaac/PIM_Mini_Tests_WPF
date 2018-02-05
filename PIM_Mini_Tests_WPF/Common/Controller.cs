using Renci.SshNet;
using Renci.SshNet.Common;
using Serilog;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.Common
{
    public static class Controller
    {
        public static bool IsDaemonStarted = false;
        internal readonly static int port = 1003;

        public static DaemonResponse StartDaemon()
        {
            Log.Information("Setting up SSH connection to start the daemon.");
            var connectionInfo = new ConnectionInfo(
                Properties.Settings.Default.targetAddress,
                Properties.Settings.Default.sshUsername,
                new PasswordAuthenticationMethod(
                    Properties.Settings.Default.sshUsername,
                    Properties.Settings.Default.sshPassword));

            using (var client = new SshClient(connectionInfo))
            {
                try
                {
                    Log.Information("SSH connecting.");
                    client.Connect();
                    Log.Information("SSH connected.");
                    var startCommand = client.CreateCommand("python /test/daemon.py");
                    startCommand.Execute();
                    Log.Information("Start command sent and executed.");
                }
                catch (ObjectDisposedException ex)
                {
                    Log.Fatal(ex.Message);
                    return DaemonResponse.ObjectDisposedException;
                }
                catch (SocketException ex)
                {
                    Log.Fatal(ex.Message);
                    return DaemonResponse.SocketException;
                }
                catch (SshConnectionException ex)
                {
                    Log.Fatal(ex.Message);
                    return DaemonResponse.SshConnectionException;
                }
                catch (ProxyException ex)
                {
                    Log.Fatal(ex.Message);
                    return DaemonResponse.ProxyException;
                }
                catch (InvalidOperationException ex)
                {
                    Log.Fatal(ex.Message);
                    return DaemonResponse.InvalidOperationException;
                }
            }

            var message = "ack";
            return Controller.SendTcpMessage(message);
        }

        public static DaemonResponse KillDaemon()
        {
            using (var client = new SimpleTcpClient().Connect(Properties.Settings.Default.targetAddress, Controller.port))
            {
                Log.Information("Sending kill signal to the daemon.");
                client.Delimiter = new byte();
                var message = "die";
                return Controller.SendTcpMessage(message);
            }
        }

        internal static DaemonResponse SendTcpMessage(string message)
        {
            Log.Information("Starting TCP client");
            using (var client = new TcpClient())
            {
                try
                {
                    Log.Information("Connecting TCP client to the daemon.");
                    client.Connect(Properties.Settings.Default.targetAddress, Controller.port);
                    Log.Information("Connected");
                    Stream dataStream = client.GetStream();

                    ASCIIEncoding ascii = new ASCIIEncoding();
                    byte[] messageOut = ascii.GetBytes(message);
                    Log.Information($"Transmitting {messageOut}");
                    dataStream.Write(messageOut, 0, messageOut.Length);
                    Log.Information("Data transmitted");

                    byte[] messageIn = new byte[100];
                    int length = dataStream.Read(messageIn, 0, 100);
                    string messageInStr = "";
                    foreach (var item in messageIn)
                    {
                        messageInStr += Convert.ToChar(item);
                    }

                    messageInStr = messageInStr.Trim();
                    if (messageInStr == message)
                    {
                        return DaemonResponse.Success;
                    }
                    else
                    {
                        return DaemonResponse.IncorrectResponse;
                    }
                }
                catch (ArgumentNullException ex)
                {
                    Log.Fatal(ex.Message);
                    return DaemonResponse.ArgumentNullException;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Log.Fatal(ex.Message);
                    return DaemonResponse.ArgumentOutOfRangeException;
                }
                catch (SocketException ex)
                {
                    Log.Fatal(ex.Message);
                    return DaemonResponse.SocketException;
                }
                catch (ObjectDisposedException ex)
                {
                    Log.Fatal(ex.Message);
                    return DaemonResponse.ObjectDisposedException;
                }
            }
        }

        public static DaemonResponse StartTest(string test)
        {
            using (var client = new SimpleTcpClient().Connect(Properties.Settings.Default.targetAddress, Controller.port))
            {
                client.Delimiter = new byte();
                try
                {
                    bool received = false;
                    int counter = 0;

                    // Tries multiple times to get the message through
                    while (!received && counter < 5)
                    {
                        var replyMsg = client.WriteLineAndGetReply(test, TimeSpan.FromSeconds(3));
                        client.Disconnect();
                        if (replyMsg.MessageString.Trim() != test)
                        {
                            counter += 1;
                        }
                        else
                        {
                            received = true;
                        }
                    }

                    if (counter >= 5 && received == false)
                    {
                        return DaemonResponse.DaemonReceiveFailure; // couldn't receive get the ack after 5 tries
                    }


                }
                catch (Exception)
                {
                    return DaemonResponse.TcpClientFailure;
                }
            }
        }
    }

    public enum DaemonResponse
    {
        Success,
        IncorrectResponse,
        InvalidOperationException,
        ObjectDisposedException,
        SocketException,
        SshConnectionException,
        ProxyException,
        ArgumentNullException,
        ArgumentOutOfRangeException,
        TcpClientFailure,
        DaemonReceiveFailure
    }
}
