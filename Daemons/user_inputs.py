"""Tests that GPIO pins can be written to and read from"""

from periphery import GPIO  # pylint: disable=W0403

IN = "in"
OUT = "out"
HIGH = "high"
LOW = "low"
PRESERVE = "preserve"


USER_INPUT_1 = GPIO(pin=85, direction=OUT)
USER_INPUT_2 = GPIO(pin=86, direction=OUT)
USER_INPUT_3 = GPIO(pin=90, direction=OUT)


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
        return self._test(True)

    def test_low(self):
        """Tests that low values can be written and read from the GPIO pin"""
        return self._test(False)

    def _test(self, level):
        self.gpio.write(level)
        if self.gpio.read() is level:
            return str(True)
        return str(False)


class UserInputOne(UserInputs):
    """UserInput 1 class"""
    pass


class UserInputTwo(UserInputs):
    """UserInput 2 class"""
    pass


class UserInputThree(UserInputs):
    """UserInput 3 class"""
    pass
