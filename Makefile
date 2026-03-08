
# SECOND:=$(call mod,$(shell date +%s),86400)

VERSION:=6600.4.$(shell date +%y%j).$(shell echo $$(($(shell date +%s) % 86400)))

clean-nupkg:
	rm -rfv $(shell find -type f | grep nupkg$ | grep -v symbols.nupkg | grep Release)

restore:
	dotnet restore --configfile Nuget.config

release: build
	find -type f | grep nupkg$ | grep -v symbols.nupkg | grep Release | while read line ; do \
		echo "Publising $$line"; \
		dotnet nuget push -k ${NUGET_PUBLISH_KEY} -s ${NUGET_PUBLISH_URL} $$line; \
	done

build: restore clean-nupkg
	echo "Release Version:${VERSION}"
	$(MAKE) clean-nupkg
	dotnet build -c Release -p:Version=${VERSION}

.PHONY: restore release build clean-nupkg
