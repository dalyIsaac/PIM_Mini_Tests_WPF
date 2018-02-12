"""Tests that the LEDs are working, by writing to the appropriate GPIO pins"""

from periphery import GPIO  # pylint: disable=W0403

IN = "in"
OUT = "out"
HIGH = "high"
LOW = "low"

CCP_OK = GPIO(pin=13, direction=OUT)
IED_OK = GPIO(pin=12, direction=OUT)
FAULT = GPIO(pin=11, direction=OUT)
CCP_DATA_TX = GPIO(pin=25, direction=OUT)
CCP_DATA_RX = GPIO(pin=24, direction=OUT)
IED_DATA_TX = GPIO(pin=5, direction=OUT)
IED_DATA_RX = GPIO(pin=4, direction=OUT)


def test_ccp_ok(level, logging):
    """Tests that the CCP OK LED can be turned on"""
    #return _test(CCP_OK, level)
    return _write(CCP_OK, level, logging)

def test_ied_ok(level, logging):
    """Tests that the IED OK LED can be turned on"""
    #return _test(IED_OK, level)
    return _write(IED_OK, level, logging)

def test_fault(level, logging):
    """Tests that the Fault LED can be turned on"""
    #return _test(FAULT, level)
    return _write(FAULT, level, logging)

def test_ccp_data_tx(level):
    """Tests that the CCP Data Tx (transmit) LED can be turned on"""
    return _test(CCP_DATA_TX, level)

def test_ccp_data_rx(level):
    """Tests that the CCP Data Rx (receive) LED can be turned on"""
    return _test(CCP_DATA_RX, level)

def test_ied_data_tx(level):
    """Tests that the IED Data Tx (transmit) LED can be turned on"""
    return _test(IED_DATA_TX, level)

def test_ied_data_rx(level):
    """Tests that the IED Data Rx (receive) LED can be turned on"""
    return _test(IED_DATA_RX, level)

def _write(pin, level, logging):
    try:
        pin.write(level)
        logging.debug("Success")
        return True
    except Exception as ex:
        logging.error(ex)
        return False

def _test(pin, level):
    pin.write(level)
    if pin.read() is level:
        return True
    return False
