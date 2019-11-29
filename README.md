# zipService
A windows service that zips files individually from a given source folder to a given target folder 

v 1.00 : First release

### Configuration :
in App.config if you use the source or in \bin\Release\zipService.exe.config is you use the compiled zipService.exe
set the timer speed : default is loop every 15 seconds,the source folder where you will put the files to compress and the destination folder (targetFolder) where the sipped files will go.

Take notice that the source files are destroyed in the process
(I don't assume any guaranty for that)

```
  <appSettings>
    <add key="loopSpeedInSeconds" value="15" />
    <add key="sourceFolder" value ="c:\testzip\"/>
    <add key="targetFolder" value="c:\testzip\done\"/>
  </appSettings>
```
To install the service, search for cmd, right click to execute as administrator and type :
```
sc.exe create ZipService binPath= "C:\...yourpath..\bin\Release\\zipService.exe"
```


