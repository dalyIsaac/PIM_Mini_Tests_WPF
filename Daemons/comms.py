"""Tests the serial communications for the PIM_Mini"""

from multiprocessing.pool import ThreadPool
from serial import Serial, rs485, PARITY_ODD, STOPBITS_ONE, EIGHTBITS


TEST_STRING = b"The quick brown fox jumps over the lazy dog 0123456789"


class SerialComms(object):
    """
    Tests the serial communications of the child class.  
    : DO NOT DIRECTLY USE THIS CLASS FOR TESTING.  ALWAYS IMPLEMENT A CHILD CLASS IN THE FORMAT: :
    `class ChildClass(SerialComms): pass`"""

    def __init__(self, logging):
        self.writer = Serial(baudrate=9600, parity=PARITY_ODD,
                             stopbits=STOPBITS_ONE, bytesize=EIGHTBITS)
        self.reader = Serial(baudrate=9600, parity=PARITY_ODD,
                             stopbits=STOPBITS_ONE, bytesize=EIGHTBITS)
        logging.debug("Initialized")
        self.logging = logging
        self.configure()

    def configure(self):
        """
        Configures the serial port
        """
        self.logging.debug("Configuring")

        if isinstance(self, CCPComms):
            self.logging.debug("Entered CCP config")
            self.writer.port = "/dev/ttymxc2"
            self.reader.port = "/dev/ttymxc2"
            self.logging.debug("Ports configured as CCP")
        elif isinstance(self, IEDComms):
            self.logging.debug("Entered IED config")
            self.writer.port = "/dev/ttymxc4"
            self.reader.port = "/dev/ttymxc4"
            self.logging.debug("Ports configured as IED")
        else:
            raise Exception("Invalid child class")

    def test_rs232_rude_write(self):
        """Tests that data can be written and read over RS-232 with no RTS/CTS handshaking"""
        self.logging.debug("RS-232 test write no RTS/CTS handshaking")
        self.writer.rts = False
        return self._test_write()

    def test_rs232_polite_write(self):
        """Tests that data can be written and read over RS-232 with RTS/CTS handshaking"""
        self.logging.debug("RS-232 test write with RTS/CTS handshaking")
        self.writer.rts = True
        return self._test_write()

    def test_rs232_rude_receive(self):
        """Tests that data can be written and read over RS-232 with no RTS/CTS handshaking"""
        self.logging.debug("RS-232 test receive with no RTS/CTS handshaking")
        self.writer.rts = False
        return self._test_read()

    def test_rs232_polite_receive(self):
        """Tests that data can be written and read over RS-232 with RTS/CTS handshaking"""
        self.logging.debug("RS-232 test receive with no RTS/CTS handshaking")
        self.writer.rts = True
        return self._test_read()

    def test_rs485(self):
        """Tests that data can be written and read over RS-485"""
        self.logging.debug("RS-485 test")
        self.writer.rs485_mode = rs485.RS485Settings()

    def _test_write(self):
        """Tests that data can be written, and that the data read is equal"""
        try:
            if (self.writer.rts is True):
                self.logging.debug("Checking CTS")
                if (self.writer.cts is False):
                    self.logging.fatal("CTS is False")
                    return
                self.logging.debug("CTS is true")

            message = "Writing: " + TEST_STRING
            self.logging.debug(message)
            self.writer.open()
            self.writer.write(TEST_STRING)
            self.writer.close()

            self.logging.debug("Reading")
            self.reader.open()
            output = str(self.reader.read(len(TEST_STRING)))
            self.reader.close()
            message = "Read: " + output
            self.logging.debug(message)

            if output == TEST_STRING:
                self.logging.debug("Data written is the same as data read")
                return True
            self.logging.debug("Data written is not the same as data read")
            return False
        except Exception as ex:
            self.logging.fatal(ex)
            try:
                self.writer.close()
                self.reader.close()
            except:
                pass
            return False

    def _test_read(self):
        """Opens a thread which waits until all the data is read, which the main thread writes, then checks
        that the data is equal"""
        try:
            self.logging.debug("Starting reader in another thread")
            pool = ThreadPool(processes=1)
            async_result = pool.apply_async(self._async_reader)

            if (self.writer.rts is True):
                self.logging.debug("Checking CTS")
                if (self.writer.cts is False):
                    self.logging.fatal("CTS is False")
                    return
                self.logging.debug("CTS is true")

            message = "Writing: " + TEST_STRING
            self.writer.open()
            self.writer(TEST_STRING)
            self.writer.close()
            return_val = str(async_result.get())
            message = "Value received:  " + return_val
            self.logging.debug(message)
            if return_val == TEST_STRING:
                self.logging.debug("Data written is the same as data read")
                return True
            self.logging.debug("Data written is not the same as data read")
            return False
        except Exception as ex:
            self.logging.fatal(ex)
            try:
                self.writer.close()
                self.reader.close()
            except:
                pass
            return False

    def _async_reader(self):
        """Reads data from self.reader in another thread"""
        self.reader.open()
        result = self.reader.read(len(TEST_STRING))
        self.reader.close()
        return result


class CCPComms(SerialComms):
    """CCPComms class"""
    pass


class IEDComms(SerialComms):
    """IEDComms"""
    pass
