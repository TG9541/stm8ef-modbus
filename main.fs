\ Minimal MODBUS server with a single FC handler
\ Hint: the C0135 MODBUS server is C0135/board.fs
\
\ Features:
\ * implements FC06 "Write Single Register"
\ * prints debug infos to the console
\
\ Example output for 96008N1, Node=1, FC=6, Addr=5, Data=516
\
\ Write register: 5= 516
\  MODBUS rxbuf: 8
\   80   1  6  0  5  2  4 99 68  0  0  0  0  0  0  0  0  _______h________
\  MODBUS txbuf: 8
\   98   1  6  0  5  2  4 99 68  0  0  0  0  0  0  0  0  _______h________

\ check if the MODBUS protocol core is already present
\ hint: the development cycle will be faster if you PERSIST it
#require MBPROTO

\ Resetting the FC handler table can be helpful for development
#require WIPE
#require MBRESET
MBRESET   \ Reset the MODBUS Function Code table
#require :NVM
#require 'IDLE

NVM
#require MBDUMP

  \ --- FC06 handler "Write Single Register"
  :NVM  ( -- )
    \ write register address and value to the console
    ." Write register:" mbp1 . ." =" mbp2 . CR
    MBSWR  \ acknowledge FC06
  ;NVM ( xt ) 6 FC>XT !   \ register the FC handler

  : showfc ( -- )
    rxbuf C@ ." FC:" . CR
    1 MBEC  \ set error code
  ;

  : init ( -- )
    0 UARTISR                     \ init UART handler w/ default baud rate
    1 mbnode !                    \ set node address
    [ ' showfc ] LITERAL mbdef !  \ FC default action (optional feature)
    [ ' MBDUMP ] LITERAL mbact !  \ show buffers (debug demo)
    [ ' MBPROTO ] LITERAL 'IDLE ! \ run MB protocol handler as idle task
  ;

  ' init 'BOOT !
WIPE RAM
