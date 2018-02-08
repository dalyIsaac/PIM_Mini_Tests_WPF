"""Tests the serial communications for the PIM_Mini"""

from serial import Serial, rs485

CCP_TTL = Serial(baudrate=384000)
CCP_TTL.port = ""

CCP_RS232 = Serial(baudrate=384000)
CCP_RS232.port = ""

CCP_RS485 = Serial(baudrate=384000)
CCP_RS485.port = ""
CCP_RS485.rs485_mode = rs485.RS485Settings()

IED_TTL = Serial(baudrate=384000)
IED_TTL.port = ""

IED_RS232 = Serial(baudrate=384000)
IED_RS232.port = ""

IED_RS485 = Serial(baudrate=384000)
IED_RS485.port = ""

TEST_STRING = b"The quick brown fox jumps over the lazy dog 0123456789"


class SerialComms():
    """
    Tests the serial communications of the child class.  
    : DO NOT DIRECTLY USE THIS CLASS FOR TESTING.  ALWAYS IMPLEMENT A CHILD CLASS IN THE FORMAT: :
    `class ChildClass(SerialComms): pass`"""

    def __init__(self):
        self.ttl = None
        self.rs232 = None
        self.rs485 = None
        self.configure()

    def configure(self):
        """
        Configures the serial port
        """
        if isinstance(self, CCPComms):
            self.ttl = CCP_TTL
            self.rs232 = CCP_RS232
            self.rs485 = CCP_RS485
        elif isinstance(self, IEDComms):
            self.ttl = IED_TTL
            self.rs232 = IED_RS232
            self.rs485 = IED_RS485
        else:
            raise Exception("Invalid child class")

    def test_ttl(self):
        """Tests that data can be written and read over TTL"""
        self._test(self.ttl)

    def test_rs232(self):
        """Tests that data can be written and read over RS-232"""
        self._test(self.rs232)

    def test_rs485(self):
        """Tests that data can be written and read over RS-485"""
        self._test(self.rs485)

    def _test(self, comm):
        """Writes, reads, and ensures that the output is correct for the serial device"""
        try:
            comm.open()
            comm.write(TEST_STRING)
            output = str(comm.read_all())
            if output == TEST_STRING:
                return True
            return False
        except:
            return False


class CCPComms(SerialComms):
    """CCPComms class"""
    pass


class IEDComms(SerialComms):
    """IEDComms"""
    pass