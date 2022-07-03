\ Minimal MODBUS server application with a sole FC6 handler
\ Hint: C0135/board.fs uses MBSERVER as a full featured MODBUS server
\
\ Features:
\ * implements FC06 "Write Single Register"
\ * prints debug infos to the console
\
\ Example output for 96008N1, Node=1, FC=6, Addr=5, Data=516
\
\ Write register: 5= 516
\  MODBUS rtbuf: 8
\   80   1  6  0  5  2  4 99 68  0  0  0  0  0  0  0  0  _______h________
\  MODBUS rtbuf: 8
\   98   1  6  0  5  2  4 99 68  0  0  0  0  0  0  0  0  _______h________

\ check if the MODBUS protocol core is already present
\ hint: the development cycle will be much faster if you PERSIST it
#require MBPROTO

\ Resetting the FC handler table can be "helpful" for development
#require WIPE
#require MBRESET
MBRESET   \ Reset the MODBUS Function Code table
#require :NVM
#require 'IDLE

NVM
\ MODBUS buffer dump for demo (not needed in an application)
#require MBDUMP

  \ --- FC06 handler "Write Single Register"
  \ mbp1 (MODBUS parameter 1) provides the register address
  \ mbp2 (MODBUS parameter 2) provides the new register value
   :NVM  ( -- )
   \ write register address and value to the console
    \ (an application likely wouldn't print anything)
    ." Write register:" mbp1 . ." =" mbp2 . CR
    MBWR  \ protocol: acknowledge FC06
  ;NVM ( xt ) 6 FC>XT !   \ register the FC handler


  \ --- demo: show MODBUS function code
  : showfc ( -- )
    rtbuf 1+ C@ ." FC:" . CR
    1 MBEC  \ set error code
  ;

  \ --- set everything up
  : init ( -- )
    0 UARTISR                     \ init UART handler w/ default baud rate
    1 mbnode !                    \ set node address
    [ ' showfc ] LITERAL mbdef !  \ FC default action (optional feature)
    [ ' MBDUMP ] LITERAL mbpre !  \ demo: dump RXTX buffer after receive
    [ ' TXDUMP ] LITERAL mbact !  \       dump RXTX buffer before transmit
    [ ' MBPROTO ] LITERAL 'IDLE ! \ register MBPROTO as the idle task
    CR ." STM8EF-MODBUS minimal server FC6 'Write Single Register'"
    hi
  ;

  \ --- set init address to boot vector - run after reset
  ' init 'BOOT !
WIPE RAM
