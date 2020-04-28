# stm8ef-modbus
[![Travis-CI](https://travis-ci.org/TG9541/stm8ef-modbus.svg)](https://travis-ci.org/TG9541/stm8ef-modbus)

This repository provides a lightweight MODBUS RTU implementation with [STM8 eForth](https://github.com/TG9541/stm8ef/wiki) for "wired" control nodes, e.g. for home automation. The main target is low-cost STM8S 8bit µCs like the STM8S003F3P6 with 8K Flash and 1K RAM.

The MODBUS I/O Node implementation for the low-cost [C0135 4-Relay RTU module][C0135] serves as a demonstrator, and in [GitHub Releases](https://github.com/TG9541/stm8ef-modbus/releases) you'll find a ready-to-use binary.

Using STM8 Forth for MODBUS has some advantages: the implementation is very compact and it gives applications access to many advanced architecture features like "I/O-locic execution in the background" or a CLI (command line interface).

The Forth compiler/interpreter is part of the binary you can literally change the code while your board is communicating with the MODBUS host!

The MODBUS RTU implementation covers basic FCs: it's a subset of [MODBUS V1.1b](http://www.modbus.org/docs/Modbus_Application_Protocol_V1_1b.pdf) common in simple I/O nodes. It's easy to write code for other FCs. It's also simple to turn the board in something like an independent controller for window blinds: the MODBUS host only commands "open" or "closed", not "up" and "down". Local control code can help to make home automation much more robust and reactive.

## Supported Boards

### C0135 4-Relay Board
The [C0135 board][C0135] is the default target.

[C0135]: https://github.com/TG9541/stm8ef/wiki/Board-C0135

![c0135-small](https://user-images.githubusercontent.com/5466977/52519844-fb3c6580-2c61-11e9-8f36-5a031338e6e5.png)

You can simply transfer the ready-made binary to your board with a cheap "ST-LINK V2" dongle, or run `make` to flash the STM8 eForth C0135 code.

Using a diode and a cheap USB-TTL dongle you can [get a console][TWOWIRE] (this means the MODBUS node *is* a computer, a bit like the console of a VIC20 in the old days ;-) ).

[TWOWIRE]: https://github.com/TG9541/stm8ef/wiki/STM8-eForth-Programming-Tools#using-a-serial-interface-for-2-wire-communication

### STM8S001J3RS485 Mini MODBUS Board

The STM8S001J3RS485 board is a tiny MODBUS node based on the STM8S001J3M3 "Low Density Value Line" STM8S µC in a SO8 package.

[![STM8S001J3RS485](https://raw.githubusercontent.com/TG9541/stm8s001rs485/master/doc/STM8S001J3_RS485_front.png)](https://github.com/TG9541/stm8s001rs485)

The code can be built and transferred to the devide by running `make -f forth.mk BOARD=STM8S001J3RS485 flash`. After flashing the `BUSCTRL` file in the board configuration folder should be transferred using e4thcom and a [2-wire connection][TWOWIRE] through PC5. After that, `STM8S001J3RS485/board.fs` can be transferred with `#include`.

### MINDEV STM8S103F3 Breakout Board
It's easy to build custom targets, e.g. using the $0.80 [MINDEV board](https://github.com/TG9541/stm8ef/wiki/Breakout-Boards#stm8s103f3p6-breakout-board), a cheap relay board, and an RS485 break-out board.

![MINDEV](https://camo.githubusercontent.com/82bd480f176951de9a469e134f543a6570f48597/68747470733a2f2f616530312e616c6963646e2e636f6d2f6b662f485442314e6642615056585858586263587058587136785846585858362f357063732d6c6f742d53544d3853313033463350362d73797374656d2d626f6172642d53544d38532d53544d382d646576656c6f706d656e742d626f6172642d6d696e696d756d2d636f72652d626f6172642e6a70675f323230783232302e6a7067)

When using PB5 for RS485 direction control (-> `BUSCTRL`) the C0135 code can be used (refer to the [C0135 STM8 eForth Wiki page][C0135]).

## Supported MODBUS Function Codes

`MBSERVER` contains MODBUS function plug-ins with the following function codes (FC):

FC | Description | Support
-|-|-
**1** | **Read Coils** | implemented
**2** | **Read Discrete Inputs** | implemented
**3** | **Read Holding Registers** | implemented 
**4** | **Read Input Registers** | implemented
**5** | **Write Single Coil** | implemented
**6** | **Write Single (Holding) Register** | implemented
**15** | **Write Multiple Coils** | implemented
16 | Write Multiple Registers | partial

A working example with Node-ID and Baud Rate stored in EEPROM is implemented in `C0135/board.fs`. An example that shows how to develop minimal servers with FC handlers from scratch using the Forth console is in `main.fs`.

## Installation

This project uses the STM8 eForth "Modular Build" feature: `make depend` fetches the STM8 eForth release defined in the `Makefile`.

On a Linux system common dependencies are e.g. GAWK, MAKE and Python. SDCC needs to be installed. It's also possible to use `tg9541/docker-sdcc` in a Docker container (refer to `.travis.yml` for details).

The [Getting Started](https://github.com/TG9541/stm8ef/wiki/Breakout-Boards#getting-started) section in the STM8 eForth Wiki provides an introduction to flashing STM8 eForth to a target µC.

Please refer to the [Installation Instructions](https://github.com/TG9541/stm8ef-modbus/wiki/HowTo#installation) in the STM8EF-MODBUS Wiki for build instructions.

## Console

The STM8S UART is used by [UARTISR](https://github.com/TG9541/stm8ef-modbus/blob/master/UARTISR) for MODBUS RTU communication. The Forth console communicates through a half-duplex simulated RS232 two-wire interface on the `PD1/SWIM` GPIO pin. For adding a standard USB-TTL converter only a diode is needed. Other CLI communication options are easy to implment, e.g. using simulated full-duplex RxD-TxD lines (e.g. using PA1 and PA2 after removing the C0135 8MHz crystal). It's also possible to use an STM8S High Density device with two UARTs, e.g. the STM8S207RBT6.

Please refer to the [STM8 eForth Wiki](https://github.com/TG9541/stm8ef/wiki/STM8S-Value-Line-Gadgets#other-target-boards) to learn more about half-duplex CLI communication options and preferred terminal programs.

## Architecture

The software architecture separates hardware abstraction and application in simple layers:

Layer|Source file|Description
-|-|-
5|`main.fs` or `{BOARD}/board.fs`|configuration and application layer
4|`MBSERVER`|MODBUS FC plug-ins (optional)
3|`MBPROTO`|MODBUS protocol layer
2|`UARTISR`|buffered UART communication
1|`BUSCTRL`|bus access (i.e. RS485 direction control)
0|STM8 eForth|lightweight interactive multi-tasking OS

The different concerns are separeted in the code and FC handlers can be changed through the CLI without restarting the application!

The code is organized in the following execution domains:
* interrupt service routines for buffered MODBUS communication
* fixed-rate background task for I/O logic (asynchronous to MODBUS)
* foreground "idle mode" MODBUS protocol handler
* foreground command line interface (CLI) through independent COM port provided by STM8 eForth
* handlers for MODBUS I/O: `mbpre` for input, `mbact` for output actions

Please refer to the [how-to in the wiki](https://github.com/TG9541/stm8ef-modbus/wiki/HowTo) and don't hesitate to open an [issue](https://github.com/TG9541/stm8ef-modbus/issues) if you have questions!
