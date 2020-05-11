# STM8S001J3M3 based Micro MODBUS Node

The STM8S001J3M3 is a low-pin-count µC in a SOIC-8 package. The chip is very similar to an STM8S903 and it has, like all other STM8S Low Density devices sufficient memory for supporting a Forth based MODBUS Server implementation. The "half-duplex" feature of the STM8S Low Density UART makes it possible to implement a normal RS485 interface with two GPIOs (shared RX-TX and direction controll).

The [STM8S001 RS485 project](https://github.com/TG9541/stm8s001rs485) prototypes a very small STM8 eForth MODBUS node that, 1/4" (6.4mm) wide, fits in the same tube as a 8mm proximity switch:

![STM8S001J3M3 MODBUS module](https://cdn.hackaday.io/images/3730111577482558707.png)

Since `PD1/SWIM` is on the same pin as `PD5/RX-TX` a half-duplex a Forth console is available through `PC5`. The I2C bus on `PB4` and `PB5` is connected to an optional DS1621S temperature sensor and also accessible on pin headers. Two LEDs show direction and RX/TX bus activity:

![STM8S001J3M3 MODBUS schematics](https://raw.githubusercontent.com/TG9541/stm8s001rs485/master/doc/STM8S001J3_RS485_sch.png)
