#require MBSERVER

RAM

  2 CONSTANT BAUD9600

NVM
  \ --- MODBUS server startup

  : init ( -- )
    BAUD9600 UARTISR
    1 mbnode !
    MBSERVER
  ;

  ' init    'BOOT !
RAM
