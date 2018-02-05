using Renci.SshNet;
using Renci.SshNet.Common;
using SimpleTCP;
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

        public static DaemonReponse StartDaemon()
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
                    client.Connect();
                    var startCommand = client.CreateCommand("python /test/daemon.py");
                    startCommand.Execute();
                }
                catch (ObjectDisposedException)
                {
                    return DaemonReponse.ObjectDisposedException;
                }
                catch (SocketException)
                {
                    return DaemonReponse.SocketException;
                }
                catch (SshConnectionException)
                {
                    return DaemonReponse.SshConnectionException;
                }
                catch (ProxyException)
                {
                    return DaemonReponse.ProxyException;
                }
                catch (InvalidOperationException)
                {
                    return DaemonReponse.InvalidOperationException;
                }
            }

            var message = "ack";
            return Controller.SendTcpMessage(message);
        }

        public static DaemonReponse KillDaemon()
        {
            using (var client = new SimpleTcpClient().Connect(Properties.Settings.Default.targetAddress, Controller.port))
            {
                client.Delimiter = new byte();
                var message = "die";
                return Controller.SendTcpMessage(message);
            }
        }

        internal static DaemonReponse SendTcpMessage(string message)
        {
            using (var client = new SimpleTcpClient().Connect(Properties.Settings.Default.targetAddress, Controller.port))
            {
                client.Delimiter = new byte();
                try
                {
                    var replyMsg = client.WriteLineAndGetReply(message, TimeSpan.FromSeconds(3));
                    client.Disconnect();
                    if (replyMsg.MessageString.Trim() == message)
                    {
                        return DaemonReponse.Success;
                    }
                }
                catch (Exception)
                {
                    return DaemonReponse.TcpClientFailure;
                }
                return DaemonReponse.Failure;
            }
        }

        public static DaemonReponse StartTest(string test)
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
                        return DaemonReponse.DaemonReceiveFailure; // couldn't receive get the ack after 5 tries
                    }


                }
                catch (Exception)
                {
                    return DaemonReponse.TcpClientFailure;
                }
            }
        }
    }

    public enum DaemonReponse
    {
        Success,
        Failure,
        InvalidOperationException,
        ObjectDisposedException,
        SocketException,
        SshConnectionException,
        ProxyException,
        ArgumentNullException,
        TcpClientFailure,
        DaemonReceiveFailure
    }
}
