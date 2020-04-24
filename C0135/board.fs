\ C0135 STM8 eForth MODBUS board code

( Hint for non-Forthers )
\ - this and the above are comments
\ - @ means "read" and ! means "write"
\ - : means "compile", [ switches to "interpret", ] back and ; "end compile"
\ - #require, \res, etc are e4thcom or codeload.py keywords

\ compile MODBUS server and protocol words
#require MBSERVER

\ We need the C0135 "read inputs" word
#require C0135/IN@

\ we're in RAM mode: load "scaffolding words"
#require :NVM
#require WIPE
#require LOCK
#require ULOCK
#require 'IDLE

\ define temporary constants
$4000  CONSTANT  EE_NODE
$4002  CONSTANT  EE_BAUD

\ now compile to Flash ROM
NVM
  \ headerless code Preparation Handler
  :NVM
     IN@ inputs !
  ;NVM ( xt-pre )  \ compile time: keep this eXecution Token on the stack

  \ headerless code Action Handler
  :NVM
     coils @ OUT!
  ;NVM ( xt-act )  \ and also this

  \ --- MODBUS server startup
  : init ( -- )
    \ register the xt (see above) as the MODBUS Action Handler
    ( xt-act ) LITERAL mbact !
    ( xt-pre ) LITERAL mbpre !

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

    \ initialize C0135 inputs
    IN@INIT

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
