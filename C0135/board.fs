\ C0135 STM8 eForth MODBUS board code

\ compile MODBUS server and protocol words
#require MBSERVER

\ we're in RAM mode: load "scaffolding words"
#require :NVM
#require WIPE
#require LOCK
#require ULOCK
#require 'IDLE

\ temporary symbols
$4000  CONSTANT  EE_NODE
$4002  CONSTANT  EE_BAUD

NVM
\ from here on compile to Flash ROM
#require OUT!

  \ headerless code Action Handler
  :NVM
     coils @ OUT!
  ;NVM ( xt-act )  \ compile time: keep this eXecution Token on the stack

  \ --- MODBUS server startup
  : init ( -- )
    \ register the xt (see above) as the MODBUS Action Handler
    ( xt-act ) LITERAL mbact !

    \ Holding C0135 key "S2" while start-up resets Node-ID and baud rate
    BKEY IF
      ULOCK
      1 EE_NODE !   \ 1 as Node-ID (holding register 0)
      0 EE_BAUD !   \ default rate (holding register 1)
      LOCK
      \ defaults written - blink C0135 LED "D5" until key "S2" is released
      BEGIN
        TIM 16 AND OUT! \ yes, this is a kludge ;-)
        BKEY 0=
      UNTIL
    THEN

    \ initialize MODBUS "coils" and outputs
    0 coils !  0 OUT!

    EE_NODE @ DUP 0 256 WITHIN NOT IF
      DROP 1  \ out of range - use default
    THEN
    ( n ) mbnode !

    EE_BAUD @ ( #BR ) UARTISR

    [ ' MBPROTO ( xt ) ] LITERAL 'IDLE !
  ;

  \ register initialization
  ' init 'BOOT !
WIPE RAM
