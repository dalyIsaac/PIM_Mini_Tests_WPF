"""Tests that GPIO pins can be written to and read from"""

from periphery import GPIO  # pylint: disable=W0403

IN = "in"
OUT = "out"
HIGH = "high"
LOW = "low"
PRESERVE = "preserve"


class UserInputs(object):
    """Tests that values can be written and read from a GPIO pin"""

    def __init__(self):
        self.pin = None
        if isinstance(self, UserInputOne):
            self.pin = 85
        elif isinstance(self, UserInputTwo):
            self.pin = 86
        elif isinstance(self, UserInputThree):
            self.pin = 90

    def test_high(self):
        """Tests that high values can be written and read from the GPIO pin"""
        return self._test(True)

    def test_low(self):
        """Tests that low values can be written and read from the GPIO pin"""
        return self._test(False)

    def _test(self, level):
        """level: bool"""
        try:
            gpio_in = GPIO(pin=self.pin, direction=IN)
            gpio_in.write(level)
            gpio_out = GPIO(pin=self.pin, direction=OUT)
            if gpio_out.read() is level:
                return str(True)
            return str(False)
        except:
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
