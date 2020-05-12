# STM8S001J3M3 based Micro MODBUS Node

The STM8S001J3M3 is a low-pin-count STM8S Low Density device in a SOIC-8 package. The chip is very similar to the STM8S903 and, like all other devices in this family, it has enough memory for supporting a Forth based MODBUS Server implementation (including the STM8 eForth system and memory to spare). The "half-duplex" UART feature allows implementing a normal RS485 interface with just two GPIOs (shared RX-TX and direction).

The [STM8S001RS485 PCB project](https://github.com/TG9541/stm8s001rs485) showcases a small and narrow STM8 eForth MODBUS node that 1/4" (6.4mm) wide fits inside the same tube as a standard 8mm proximity switch:

![STM8S001J3M3 MODBUS module](https://cdn.hackaday.io/images/3730111577482558707.png)

A simulated half-duplex serial interface Forth console is available through `PC5` and hence the MODBUS device can be controlled through the console while the MODBUS protocol is exchanging data. An optionaol DS1621S chip is connected to the I2C bus on `PB4` and `PB5` and the I2C bus is also on pin header. Two LEDs show RX/TX bus activity and RS485 direction:

![STM8S001J3M3 MODBUS schematics](https://raw.githubusercontent.com/TG9541/stm8s001rs485/master/doc/STM8S001J3_RS485_sch.png) 

The code can also be used on the STM8S003F3P6 or on other STM8S Low Density devices. 
