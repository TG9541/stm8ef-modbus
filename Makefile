STM8EF_BOARD=SWIMCOM
STM8EF_VER=2.2.23.pre4
STM8EF_BIN=stm8ef-bin.zip
STM8EF_URL=https://github.com/TG9541/stm8ef/releases/download/${STM8EF_VER}/${STM8EF_BIN}

E4THCOM=e4thcom-0.6.3
TERM_PORT=ttyUSB0
TERM_BAUD=9600
TERM_FLAGS=

# e4thcom style Forth code can't have a file suffix (but filenames are uppercase)
mmforth:=$(shell echo `ls|gawk '/^[A-Z0-9]*$$/ {print}'`)
forth=$(wildcard *fs) $(mmforth)

.PHONY: test clean

# Usage:make term BOARD=<board dir> [TERM_PORT=ttyXXXX] [TERM_BAUD=nnnn] [TERM_FLAGS="--half-duplex --idm"]
term:
	$(E4THCOM) -t stm8ef -p .:lib $(TERM_FLAGS) -d $(TERM_PORT) -b B$(TERM_BAUD)

all: flash

zip:
	zip -r ${STM8EF_BOARD}-forth ${STM8EF_BOARD}-forth.ihx

defaults:
	stm8flash -c stlinkv2 -p stm8s103f3 -s opt -w tools/stm8s103FactoryDefaults.bin

flash: depend
	stm8flash -c stlinkv2 -p stm8s103f3 -w out/$(STM8EF_BOARD)/$(STM8EF_BOARD).ihx

load: depend
	tools/codeload.py -b out/$(STM8EF_BOARD) -p /dev/$(TERM_PORT) serial main.fs

simload: $(forth) depend
	tools/simload.sh $(STM8EF_BOARD)
	touch simload

test: simload
	test/mbtest.sh $(STM8EF_BOARD)

target: depend
	rm -f target
	ln -s out/${STM8EF_BOARD}/target target

depend:
	if [ ! -d "out" ]; then \
		curl -# -L -O ${STM8EF_URL}; \
		unzip -q -o ${STM8EF_BIN} -x out/*; \
		unzip -q -o ${STM8EF_BIN} out/${STM8EF_BOARD}/*; \
		rm ${STM8EF_BIN}; \
	fi
	touch depend

clean:
	rm -rf target STM8S103.efr STM8S105.efr simload depend docs lib mcu out tools
