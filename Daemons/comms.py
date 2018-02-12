"""Tests the serial communications for the PIM_Mini"""

from serial import Serial, rs485, PARITY_ODD, STOPBITS_ONE, EIGHTBITS

CCP_TTL = Serial(baudrate=9600, parity=PARITY_ODD, stopbits=STOPBITS_ONE, bytesize=EIGHTBITS)
CCP_TTL.port = "/dev/ttymx2"

CCP_RS232 = Serial(baudrate=9600, parity=PARITY_ODD, stopbits=STOPBITS_ONE, bytesize=EIGHTBITS)
CCP_RS232.port = "/dev/ttymx2"

CCP_RS485 = Serial(baudrate=9600, parity=PARITY_ODD, stopbits=STOPBITS_ONE, bytesize=EIGHTBITS)
CCP_RS485.port = "/dev/ttymx2"
CCP_RS485.rs485_mode = rs485.RS485Settings()

IED_TTL = Serial(baudrate=9600, parity=PARITY_ODD, stopbits=STOPBITS_ONE, bytesize=EIGHTBITS)
IED_TTL.port = "/dev/ttymx4"

IED_RS232 = Serial(baudrate=9600, parity=PARITY_ODD, stopbits=STOPBITS_ONE, bytesize=EIGHTBITS)
IED_RS232.port = "/dev/ttymx4"

IED_RS485 = Serial(baudrate=9600, parity=PARITY_ODD, stopbits=STOPBITS_ONE, bytesize=EIGHTBITS)
IED_RS485.port = "/dev/ttymx4"

TEST_STRING = b"The quick brown fox jumps over the lazy dog 0123456789"


class SerialComms():
    """
    Tests the serial communications of the child class.  
    : DO NOT DIRECTLY USE THIS CLASS FOR TESTING.  ALWAYS IMPLEMENT A CHILD CLASS IN THE FORMAT: :
    `class ChildClass(SerialComms): pass`"""

    def __init__(self, logging):
        self.ttl = None
        self.rs232 = None
        self.rs485 = None
        self.configure()
        self.logging = logging

    def configure(self):
        """
        Configures the serial port
        """
        if isinstance(self, CCPComms):
            self.ttl = CCP_TTL
            self.rs232 = CCP_RS232
            self.rs485 = CCP_RS485
            self.logging("Configured as CCP")
        elif isinstance(self, IEDComms):
            self.ttl = IED_TTL
            self.rs232 = IED_RS232
            self.rs485 = IED_RS485
            self.logging("Configured as IED")
        else:
            raise Exception("Invalid child class")

    def test_ttl(self):
        """Tests that data can be written and read over TTL"""
        self.logging("TTL test")
        return self._test(self.ttl)

    def test_rs232(self):
        """Tests that data can be written and read over RS-232"""
        self.logging("RS-232 test")
        return self._test(self.rs232)

    def test_rs485(self):
        """Tests that data can be written and read over RS-485"""
        self.logging("RS-485 test")
        return self._test(self.rs485)

    def _test(self, comm):
        """Writes, reads, and ensures that the output is correct for the serial device"""
        try:
            self.logging.debug("Opening port")
            comm.open()
            self.logging.debug("Writing")
            comm.write(TEST_STRING)
            self.logging.debug("Reading")
            output = str(comm.read_all())
            if output == TEST_STRING:
                self.logging.debug("Data written is the same as data read")
                return True
            self.logging.debug("Data written is not the same as data read")
            return False
        except Exception as ex:
            self.logging.fatal(ex)
            return False


class CCPComms(SerialComms):
    """CCPComms class"""
    pass


class IEDComms(SerialComms):
    """IEDComms"""
    pass
