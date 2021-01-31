\ STM8 eForth MODBUS board code for the STM8S001J3RS485 board

( Hint for non-Forthers )
\ - this and the above are comments
\ - @ means "read" and ! means "write"
\ - : means "compile", [ switches to "interpret", ] back and ; "end compile"
\ - #require, \res, etc are e4thcom or codeload.py keywords

\ pre-load BUSCTRL so that a later #require won't load the C0135 default code
#include STM8S001J3RS485/BUSCTRL

\ compile MODBUS server and protocol words
#require MBSERVER

\ no inputs here but this would be a good place to start (or use I2C)
\ #include C0135/IN@

\ we're in RAM mode: load "scaffolding words"
#require :NVM
#require WIPE
#require LOCK
#require ULOCK
#require 'IDLE
#require .OK

\ define temporary constants
$4000  CONSTANT  EE_NODE
$4002  CONSTANT  EE_BAUD

\ now compile to Flash ROM
NVM

  \ set MODBUS RTU default node ID (1) and 9600 baud RTU
  : default ( -- )
    ULOCK
    1 EE_NODE !   \ 1 as Node-ID (holding register 0)
    0 EE_BAUD !   \ default rate (holding register 1)
    LOCK
  ;

  \ headerless code Preparation Handler
  :NVM
     \ no inputs here but this is a good place to read a sensor value from I2C
     \ IN@ inputs !
  ;NVM ( xt-pre )  \ compile time: keep this eXecution Token on the stack

  \ headerless code Action Handler
  :NVM
     \ no outputs here but this is the right place to write to I2C
     \ coils @ OUT!
  ;NVM ( xt-act )  \ and also this execution token

  \ --- MODBUS server startup
  : init ( -- )
    \ register the xt (see above) as the MODBUS Action Handler
    ( xt-act ) LITERAL mbact !
    ( xt-pre ) LITERAL mbpre !

    \ Holding C0135 key "S2" while start-up resets Node-ID and baud rate
    \ BKEY IF
    default
    \ THEN

    \ no inputs here but this is the right place to init the I2C peripheral
    \ IN@INIT

    \ initialize MODBUS "coils" and outputs
    0 coils !  ( no outputs here \ 0 OUT! )

    \ set MODBUS node ID
    EE_NODE @ DUP 0 256 WITHIN NOT IF
      DROP 1  \ out of range - use 1 as default node ID
    THEN ( n ) mbnode !

    \ start interrupt handler
    EE_BAUD @ ( #BR ) UARTISR

    \ register protocol handler
    [ ' MBPROTO ( xt ) ] LITERAL 'IDLE !

    CR ." STM8EF-MODBUS STM8S001J3RS485" .OK
  ;

  \ register initialization
  ' init 'BOOT !
WIPE RAM
