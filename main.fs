#require MBSERVER

  $4000  CONSTANT  EE_NODE
  $4002  CONSTANT  EE_BAUD

#require :NVM
#require WIPE

NVM
  \ output handler
  :NVM
     coils @ out!
  ;NVM ( xt )

  \ --- MODBUS server startup
  : init ( -- )
    ( xt ) LITERAL mbact !
    0 coils !

    EE_BAUD @ ( #BR ) UARTISR
    EE_NODE @ DUP 0 256 WITHIN NOT IF
      DROP 1  \ out of range - use default
    THEN
    ( n ) mbnode !

    MBSERVER
  ;

  ' init 'BOOT !
WIPE RAM
