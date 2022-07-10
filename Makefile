STM8EF_VER=2.2.29
STM8EF_BIN=stm8ef-bin.tgz
STM8EF_URL=https://github.com/TG9541/stm8ef/releases/download/${STM8EF_VER}/${STM8EF_BIN}

E4THCOM=e4thcom
TERM_PORT=ttyUSB0
TERM_BAUD=9600
TERM_FLAGS="-p mcu:target:lib"

BOARDS = C0135 STM8S001J3RS485

# MODBOARD=C0135

ifeq ($(BOARD),)

# e4thcom style Forth code can't have a file suffix (but filenames are uppercase)
mmforth:=$(shell echo `ls|gawk '/^[A-Z0-9]*$$/ {print}'`)
forth=$(wildcard *fs) $(mmforth)

.PHONY: test clean

all: build

release: buildload zip tgz

zip:
	find out/ -name "*.ihx" -print | zip -r out/stm8ef-bin LICENSE.md docs/words.md inc/* mcu/* lib/* -@
	find out/ -name "simbreak.txt" -print | zip -r out/stm8ef-bin tools/* -@
	find out/ -name "target" -print | zip -r out/stm8ef-bin -@

tgz:
	( find . -maxdepth 1 -type f \( -iname "*" ! -iname ".*" \) -print0 | cut -z -c 3- ;\
		find out/ -path "*target/*" -print0 ;\
		find out/ -name "*.ihx" -type f -print0 ;\
		find out/ -name "simbreak.txt" -type f -print0 )\
		| tar -czvf out/stm8ef-bin.tgz $(BOARDS) inc mcu lib tools docs --null -T -
	( find out/ -name "forth.rst" -type f -print0 )\
		| tar -czvf out/stm8ef-rst.tgz --null -T -

build: depend
	for trgt in $(BOARDS); do \
		make BOARD=$$trgt; \
	done

buildload: depend
	for trgt in $(BOARDS); do \
		make MODBOARD=$$trgt simload ; \
	done

load: flash
	tools/codeload.py -b out/$(MODBOARD) -p /dev/$(TERM_PORT) serial $(MODBOARD)/board.fs

flash: target
	make BOARD=$(MODBOARD) flash

defaults:
	stm8flash -c stlinkv2 -p stm8s103f3 -s opt -w tools/stm8s103FactoryDefaults.bin

test: simload
	test/mbtest.sh $(MODBOARD)

simload: $(forth) target
	make BOARD=$(MODBOARD)
	tools/simload.sh $(MODBOARD)
	touch simload

target: binary
	rm -f target
	ln -s out/${MODBOARD}/target target

binary: depend
	make BOARD=$(MODBOARD)

depend:
	if [ ! -d "lib" ]; then \
		curl -# -L -O ${STM8EF_URL}; \
		tar -xz --exclude='out/*' -f ${STM8EF_BIN}; \
		mv LICENSE.md docs/stm8ef_LICENSE.md; \
		rm ${STM8EF_BIN}; \
	fi

# Usage:make term BOARD=<board dir> [TERM_PORT=ttyXXXX] [TERM_BAUD=nnnn] [TERM_FLAGS="--half-duplex --idm"]
term:
	$(E4THCOM) -t stm8ef -p .:lib $(TERM_FLAGS) -d $(TERM_PORT) -b B$(TERM_BAUD)

clean:
	rm -rf target docs lib mcu out inc tools
	rm -f forth.asm forth.h forth.mk main.c STM8S103.efr STM8S105.efr
	rm -f simload

else
# the STM8 eForth make is a dependency
include forth.mk
endif
