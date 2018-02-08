# PIM_Mini_Tests_WPF

## Building

In order to build the `PIM_Mini_Tests_WPF` project, the [Aardvark Software API](https://www.totalphase.com/products/aardvark-software-api/) is required.
`PIM_Mini_Tests_WPF` has only been **tested only with  Aardvark Software API v5.30 (Windows 64-bit). There is no guarantee that it will work with 32-bit SKUs, or other versions (+v5.30 or -v5.30).**

The file structure from root should look like the following:

```
aardvark/
  bin/
    Debug/ # if PIM_Mini_Tests_WPF is built in the Debug configuration
      aardvark.dll
      aardvark.pdb
      aardvark_net.dll
    Release/ # if PIM_Mini_Tests_WPF is built in the Release configuration
      aardvark.dll
      aardvark.pdb
      aardvark_net.dll
  obj/
    Debug/
      ...
      aardvark.dll
      ...
    Release/
      ...
      aardvark.dll
      ...
  Properties/
    AssemblyInfo.cs
  aardvark.cs
  aardvark.csproj
Daemons/
  ...
PIM_Mini_Tests_WPF/
  ...
```

### Tested configuration

`PIM_Mini_Tests_WPF` has been tested on the following configuration:

- Windows 10 Pro Version 1709 (OS Build 16299.214)
- Intel x86-64
- PIM Mini (made by Production Software Limited)
  - Python 2.7
    - `pyserial` version ___
    - `python-periphery` version ___
- [Total Phase Aardvark I2C/SPI Host Adapter](https://www.totalphase.com/products/aardvark-i2cspi/)
  - [Total Phase USB Drivers - Windows](https://www.totalphase.com/products/usb-drivers-windows/)

Code which targets the PIM Mini is preloaded onto the device during the production of the device.

## Licenses

### [`python-periphery`](https://github.com/vsergeev/python-periphery)

The `python-periphery` library is licensed under the [M.I.T. license](https://github.com/vsergeev/python-periphery/blob/master/LICENSE).

Copyright (c) 2015-2016 vsergeev / Ivan (Vanya) A. Sergeev

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.

### [`pySerial`](https://github.com/pyserial/pyserial)

The [`pySerial`](https://github.com/pyserial/pyserial) library is licensed under the [BSD 3-clause "New" or "Revised" License](https://github.com/pyserial/pyserial/blob/master/LICENSE.txt).

Copyright (c) 2001-2016 Chris Liechti <cliechti@gmx.net>
All Rights Reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

  * Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.

  * Redistributions in binary form must reproduce the above
    copyright notice, this list of conditions and the following
    disclaimer in the documentation and/or other materials provided
    with the distribution.

  * Neither the name of the copyright holder nor the names of its
    contributors may be used to endorse or promote products derived
    from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

---------------------------------------------------------------------------
Note:
Individual files contain the following tag instead of the full license text.

    SPDX-License-Identifier:    BSD-3-Clause

This enables machine processing of license information based on the SPDX
License Identifiers that are here available: [http://spdx.org/licenses/](http://spdx.org/licenses/)
