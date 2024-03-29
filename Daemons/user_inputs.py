"""Tests that GPIO pins can be written to and read from"""

from periphery import GPIO
import logging

IN = "in"
OUT = "out"
HIGH = "high"
LOW = "low"
PRESERVE = "preserve"


class UserInputs(object):
    """Tests that values can be written and read from a GPIO pin"""

    def __init__(self, logging):
        self.gpio = None
        if isinstance(self, UserInputOne):
            self.gpio = GPIO(pin=85, direction=OUT)
        elif isinstance(self, UserInputTwo):
            self.gpio = GPIO(pin=86, direction=OUT)
        elif isinstance(self, UserInputThree):
            self.gpio = GPIO(pin=90, direction=OUT)
        self.logging = logging

    def test_high(self):
        """Tests that high values can be written and read from the GPIO pin"""
        return self._test(True)

    def test_low(self):
        """Tests that low values can be written and read from the GPIO pin"""
        return self._test(False)

    def _test(self, level):
        try:
            self.gpio.write(level)
            logging.debug("Value written")
            val = self.gpio.read()
            if val is level:
                self.logging.debug("Value read is the one same as the one written")
                return True
            else:
                message = "Value read is different to the one written: " + str(val) + " != " + str(level)
                self.logging.fatal()
                return False
        except Exception as ex:
            self.logging.fatal(ex)
            return False


class UserInputOne(UserInputs):
    """UserInput 1 class"""
    pass


class UserInputTwo(UserInputs):
    """UserInput 2 class"""
    pass


class UserInputThree(UserInputs):
    """UserInput 3 class"""
    pass
