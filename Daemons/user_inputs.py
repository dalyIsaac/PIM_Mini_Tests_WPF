"""Tests that GPIO pins can be written to and read from"""

from periphery import GPIO # pylint: disable=W0403

IN = "in"
OUT = "out"
HIGH = "high"
LOW = "low"
PRESERVE = "preserve"


USER_INPUT_1 = GPIO(pin=0, direction=PRESERVE)
USER_INPUT_2 = GPIO(pin=0, direction=PRESERVE)
USER_INPUT_3 = GPIO(pin=0, direction=PRESERVE)


class UserInputs(object):
    """Tests that values can be written and read from a GPIO pin"""
    def __init__(self):
        self.gpio = None
        if isinstance(self, UserInputOne):
            self.gpio = USER_INPUT_1
        elif isinstance(self, UserInputTwo):
            self.gpio = USER_INPUT_2
        elif isinstance(self, UserInputThree):
            self.gpio = USER_INPUT_3

    def test_high(self):
        """Tests that high values can be written and read from the GPIO pin"""
        self.gpio.write(True)
        if self.gpio.read() is True:
            return True
        return False

    def test_low(self):
        """Tests that low values can be written and read from the GPIO pin"""
        self.gpio.write(False)
        if self.gpio.read() is False:
            return True
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
