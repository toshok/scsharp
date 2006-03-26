
SUBDIRS = Starcraft src

all clean:
	@for dir in $(SUBDIRS); do \
		echo "Making $@ in $$dir"; \
		$(MAKE) -C $$dir $@; \
	done
