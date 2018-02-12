"""
Daemon which allows the automation of the execution of tests.
"""

import sys
import os
import socket
import logging
from datetime import datetime, timedelta
import time
import atexit
from signal import SIGTERM
import user_inputs
import leds
import comms

class Daemon(object):
    """
    A generic daemon class.

    Usage: subclass the Daemon class and override the run() method
    """

    def __init__(self, pidfile, stdin='/dev/null', stdout='/dev/null', stderr='/dev/null'):
        self.stdin = stdin
        self.stdout = stdout
        self.stderr = stderr
        self.pidfile = pidfile
        # hackish way to get the local address - per https://stackoverflow.com/a/166589/5018082
        temp_sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        temp_sock.connect(("8.8.8.8", 80))
        self.server_address = temp_sock.getsockname()[0], 10000
        temp_sock.close()
        self.sock = None

    def daemonize(self):
        """
        do the UNIX double-fork magic, see Stevens' "Advanced 
        Programming in the UNIX Environment" for details (ISBN 0201563177)
        http://www.erlenstar.demon.co.uk/unix/faq_2.html#SEC16
        """
        try:
            pid = os.fork()  # pylint: disable=E1101
            if pid > 0:
                logging.error("Exiting first parent")
                sys.exit(0)
        except OSError, exception:
            message = "fork #1 failed: %d (%s)\n" % (exception.errno, exception.strerror)
            logging.error(message)
            sys.exit(1)

        # decouple from parent environment
        logging.info("Decoupling from parent environment")
        os.chdir("/")
        os.setsid()  # pylint: disable=E1101
        os.umask(0)

        # do second fork
        logging.info("Doing second fork")
        try:
            pid = os.fork()  # pylint: disable=E1101
            if pid > 0:
                # exit from second parent
                logging.error("Exiting from second parent")
                sys.exit(0)
        except OSError, exception:
            message = "fork #2 failed: %d (%s)\n" % (exception.errno, exception.strerror)
            logging.error(message)
            sys.exit(1)

        # redirect standard file descriptors
        logging.info("Redirecting standard file descriptors")
        sys.stdout.flush()
        sys.stderr.flush()
        sys_in_file = file(self.stdin, 'r')
        sys_out_file = file(self.stdout, 'a+')
        sys_err_file = file(self.stderr, 'a+', 0)
        os.dup2(sys_in_file.fileno(), sys.stdin.fileno())
        os.dup2(sys_out_file.fileno(), sys.stdout.fileno())
        os.dup2(sys_err_file.fileno(), sys.stderr.fileno())

        # write pidfile
        logging.info("Writing pidfile")
        atexit.register(self.delpid)
        pid = str(os.getpid())
        file(self.pidfile, 'w+').write("%s\n" % pid)

    def delpid(self):
        """
        Deletes PID
        """
        os.remove(self.pidfile)

    def start(self):
        """
        Start the daemon
        """
        # Check for a pidfile to see if the daemon already runs
        try:
            pid_file = file(self.pidfile, 'r')
            pid = int(pid_file.read().strip())
            pid_file.close()
        except IOError:
            pid = None

        if pid:
            message = "pidfile %s already exist. Daemon already running?\n" % self.pidfile
            logging.error(message)
            sys.exit(1)

        # Start the daemon
        logging.info("Starting daemonize.")
        self.daemonize()
        logging.info("Starting run")
        self.run()

    def stop(self):
        """
        Stop the daemon
        """
        # Get the pid from the pidfile
        try:
            logging.info("Retrieving the pid")
            pid_file = file(self.pidfile, 'r')
            pid = int(pid_file.read().strip())
            logging.info("Closing the pid file")
            pid_file.close()
            logging.info("Pid file closed")
        except IOError:
            logging.error("IOError")
            pid = None

        if not pid:
            message = "pidfile %s does not exist. Daemon not running?\n" % self.pidfile
            logging.error(message)
            return  # not an error in a restart

        # Try killing the daemon process
        try:
            while 1:
                os.kill(pid, SIGTERM)
                time.sleep(0.1)
        except OSError, err:
            err = str(err)
            if err.find("No such process") > 0:
                if os.path.exists(self.pidfile):
                    os.remove(self.pidfile)
                else:
                    print str(err)
            sys.exit(1)

    def restart(self):
        """
        Restart the daemon
        """
        self.stop()
        self.start()

    def test_runner(self):
        """Listens over TCP, executes tests, and returns the results to the controller"""
        command = None
        time_to_stop = datetime.now() + timedelta(minutes=2)
        logging.info("Starting countdown")
        while command != "stop" and time_to_stop > datetime.now():
            logging.info("Starting to listen")
            command = ""
            while command.strip() == "":
                message = "Received '" + command + "'"
                logging.info(message) 
                command = self.sock.recv(64) # TCP receives here
                if time_to_stop > datetime.now():
                    logging.info("Timeout")
                    self.stop()
            command = command.strip()
            output = "Received " + command
            logging.info(output)
            self.sock.sendall(command)
            message = "Ack: " + command
            logging.info(message)

            result = "error"
            commands = command.split("_")
            
            # UserInput
            if command[0] == "UserInput":
                user_input = None
                if commands[1] == "One":
                    user_input = user_inputs.UserInputOne()
                elif commands[1] == "Two":
                    user_input = user_input.UserInputTwo()
                elif commands[1] == "Three":
                    user_input = user_input.UserInputThree()


                if commands[2] == "high":
                    result = user_input.test_high()
                elif commands[2] == "low":
                    result = user_input.test_low()
            
            # LEDs
            elif commands[0] == "CCP_Ok":
                level = True if commands[1] == "on" else False
                result = leds.test_ccp_ok(level)
            elif commands[0] == "IED_Ok":
                level = True if commands[1] == "on" else False
                result = leds.test_ied_ok(level)
            elif commands[0] == "Fault":
                level = True if commands[1] == "on" else False
                result = leds.test_fault(level)
            elif commands[0] == "CCP_Data_Tx":
                level = True if commands[1] == "on" else False
                result = leds.test_ccp_data_tx(level)
            elif commands[0] == "CCP_Data_Rx":
                level = True if commands[1] == "on" else False
                result = leds.test_ccp_data_rx(level)
            elif commands[0] == "IED_Data_Tx":
                level = True if commands[1] == "on" else False
                result = leds.test_ied_data_tx(level)
            elif commands[0] == "IED_Data_Rx":
                level = True if commands[1] == "on" else False
                result = leds.test_ied_data_rx(level)

            # comms
            elif commands[0] == "CCP":
                com = comms.CCPComms()
                if commands[1] == "TTL":
                    result = com.test_ttl()
                elif commands[1] == "RS232":
                    result = com.test_rs232()
                elif commands[1] == "RS485":
                    result = com.test_rs485()
            elif commands[0] == "IED":
                com = comms.IED_TTL()
                if commands[1] == "TTL":
                    result = com.test_ttl()
                elif commands[1] == "RS232":
                    result = com.test_rs232()
                elif commands[1] == "RS485":
                    result = com.test_rs485()

            message = "Sending back" + result
            logging.info(message)
            self.sock.sendall(str(result))
            time_to_stop = datetime.now() + timedelta(minutes=2)
        self.stop()

    def run(self):
        """Starts listening over TCP, and starts the test runner"""
        try:
            logging.info("Starting socket")
            sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            logging.info("Binding socket")
            sock.bind(self.server_address)
            logging.info("Listening for an incoming connection")
            sock.listen(1)
            logging.info("Waiting for a connection")
            self.sock, client_address = sock.accept()
            output = "Connection from " + str(client_address)
            logging.info(output)

            logging.info("Receiving data")
            message = self.sock.recv(64) # should receive ack
            output = "Sending " + message + " back"
            logging.info(output)
            self.sock.sendall(message.strip())
            logging.info("Starting test runner")
            self.test_runner()
        except ValueError as ex:
            log = "Invalid number of arguments:" + str(ex)
            logging.error(log)
            sys.exit(2)
        except Exception as ex:
            log = "Unknown exception: " + str(ex)
            logging.error(log)
            sys.exit(2)


def _main():
    daemon = Daemon('/tests/pim_tests_daemon.pid')
    if len(sys.argv) == 2:
        if sys.argv[1] == 'stop':
            daemon.stop()
        # elif sys.argv[1] == 'restart':
        #     daemon.restart()
        else:
            logging.basicConfig(filename="/tests/pim_tests_daemon " + str(datetime.now()).replace(":", "-") + ".log",
                                filemode='w', format='%(levelname)s: %(asctime)s %(message)s', level=logging.DEBUG)
            daemon.start()
        sys.exit(0)
    else:
        print "usage: %s start|stop" % sys.argv[0]
        # print "usage: %s start|stop|restart" % sys.argv[0]
        sys.exit(2)


if __name__ == "__main__":
    _main()
