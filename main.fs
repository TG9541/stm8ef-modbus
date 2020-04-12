#require MBSERVER

  $4000  CONSTANT  EE_NODE
  $4002  CONSTANT  EE_BAUD

NVM
  \ --- MODBUS server startup

  : init ( -- )
    EE_NODE @ DUP 0 256 WITHIN NOT IF
      DROP 1  \ out of range - use default
    THEN
    ( n ) mbnode !

    EE_BAUD @ ( #BR ) UARTISR

    MBSERVER
  ;

  ' init    'BOOT !
RAM
