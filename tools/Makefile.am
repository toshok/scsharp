MCSFLAGS= -debug
MCS= gmcs

all: extract.exe lsmpq.exe

%.exe: %.cs
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $< -r:../SCSharp/SCSharp.dll

lsmpq.exe: lsmpq.cs
	$(MCS) $(MCSFLAGS) -target:exe -out:$@ $< -r:../SCSharp/SCSharp.dll -resource:list.txt,list.txt

clean:
	rm -f *.exe *.mdb *.dll

EXTRA_DIST= extract.cs lsmpq.cs