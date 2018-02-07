using Renci.SshNet;
using Renci.SshNet.Common;
using Serilog;
using System;
using System.Collections.Generic;
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
            if (Controller.IsDaemonStarted == true)
            {
                Log.Logger.Warning("The daemon has already started.");
            }
            else
            {
                var response = Controller.SendSSHMessage("python /test/daemon.py start");   
                if (response != DaemonResponse.Success)
                {
                    return response;
                }
            }

            var message = "ack";
            return Controller.SendTcpMessage(message);
        }

        private static DaemonResponse SendSSHMessage(string message)
        {
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
                    var startCommand = client.CreateCommand(message);
                    startCommand.Execute();
                    Log.Information($"Command '{message}' sent and executed");
                    Controller.IsDaemonStarted = true;
                    return DaemonResponse.Success;
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
        }

        public static DaemonResponse KillDaemonTcp()
        {
            if (Controller.IsDaemonStarted == false)
            {
                Log.Logger.Warning("The daemon has already been killed");
                return DaemonResponse.AlreadyDead;
            }
            var message = "stop";
            return Controller.SendTcpMessage(message);
        }

        public static DaemonResponse KillDaemonSSH() => Controller.SendSSHMessage("python /test/daemon.py stop");
        

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
                    NetworkStream dataStream = client.GetStream();

                    ASCIIEncoding ascii = new ASCIIEncoding();
                    byte[] messageOut = ascii.GetBytes(message);
                    Log.Information($"Transmitting {messageOut}");
                    dataStream.Write(messageOut, 0, messageOut.Length);
                    Log.Information("Data transmitted");

                    DateTime now = DateTime.Now;
                    TimeSpan waitTime = new TimeSpan(hours: 0, minutes: 5, seconds: 0);

                    while (!dataStream.DataAvailable)
                    {
                        if (now + waitTime < DateTime.Now)
                        {
                            Log.Fatal("The daemon took too long to respond.");
                            client.Close();
                            return DaemonResponse.TimeOut;
                        }
                    }

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

        /// <summary>
        /// Starts a test via TCP, waits for a repeat of the command to be sent back, and waits for the result of the test.
        /// </summary>
        /// <param name="test">Name of the test</param>
        /// <returns>The result of the test, or any errors which occured during execution</returns>
        public static DaemonResponse ExecuteTest(string test)
        {
            Log.Information("Starting TCP client");
            using (var client = new TcpClient())
            {
                try
                {
                    Log.Information("Connecting TCP client to the daemon.");
                    client.Connect(Properties.Settings.Default.targetAddress, Controller.port);
                    Log.Information("Connected");
                    NetworkStream dataStream = client.GetStream();

                    ASCIIEncoding ascii = new ASCIIEncoding();
                    byte[] messageOut = ascii.GetBytes(test);
                    Log.Information($"Transmitting {messageOut}");
                    dataStream.Write(messageOut, 0, messageOut.Length);
                    Log.Information("Data transmitted");

                    DateTime now = DateTime.Now;
                    TimeSpan waitTime = new TimeSpan(hours: 0, minutes: 5, seconds: 0);

                    while (!dataStream.DataAvailable)
                    {
                        if (now + waitTime < DateTime.Now)
                        {
                            Log.Fatal("The daemon took too long to respond.");
                            client.Close();
                            return DaemonResponse.TimeOut;
                        }
                    }

                    byte[] messageIn = new byte[100];
                    int length = dataStream.Read(messageIn, 0, 100);
                    string messageInStr = "";
                    foreach (var item in messageIn)
                    {
                        messageInStr += Convert.ToChar(item);
                    }

                    messageInStr = messageInStr.Trim();
                    if (messageInStr != test)
                    {
                        return DaemonResponse.IncorrectResponse;
                    }

                    now = DateTime.Now;
                    waitTime = new TimeSpan(hours: 0, minutes: 5, seconds: 0);
                    while (!dataStream.DataAvailable)
                    {
                        if (now + waitTime < DateTime.Now)
                        {
                            Log.Fatal("The daemon took too long to respond.");
                            client.Close();
                            return DaemonResponse.TimeOut;
                        }
                    }

                    byte[] response = new byte[100];
                    int responseLength = dataStream.Read(messageIn, 0, 100);
                    string responseStr = "";
                    foreach (var item in messageIn)
                    {
                        responseStr += Convert.ToChar(item);
                    }

                    responseStr = responseStr.Trim();

                    switch (responseStr)
                    {
                        case "pin set":
                            Log.Information("The pin was successfully set.");
                            return DaemonResponse.Success;
                        case "pin fail":
                            Log.Fatal("The pin could not be set");
                            return DaemonResponse.PinSetFailed;
                        default:
                            Log.Fatal($"Unknown response: {responseStr}");
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
    }

    public enum DaemonResponse
    {
        Success,
        PinSetFailed,
        IncorrectResponse,
        InvalidOperationException,
        ObjectDisposedException,
        SocketException,
        SshConnectionException,
        ProxyException,
        ArgumentNullException,
        ArgumentOutOfRangeException,
        DaemonReceiveFailure,
        TimeOut,
        AlreadyDead
    }
}
