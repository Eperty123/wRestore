# What is it?
A simple GUI to easily restore your iDevice by parsing the appropriate commands to futurerestore, allowing you to not mess up or miss any crucial commands! (Windows only!)

# Requirements
* Requires Visual Studio 2017 to compile.
* .NET 4.0 framework.
* Futurerestore (very essential, won't run without it!)

# Pre-requisit for restoring
### You must have the following files:
- Baseband (.bbfw)
- Buildmanifest (.plist)
- Shsh blob (.shsh2/.shsh)
- Sep (.im4p)

Find your **device's info** here:
### For iPhone
https://www.theiphonewiki.com/wiki/Firmware/iPhone
### For iPad
https://www.theiphonewiki.com/wiki/Firmware/iPad
### For iPod Touch
https://www.theiphonewiki.com/wiki/Firmware/iPod_touch

*Also make sure to have internet access as futurerestore will likely verify the restore progress with Apple's restore server during the process.*

# What else?
You must always extract the latest revision of the baseband, buildmanifest and sep, as those are only gonna work when signed (except buildmanifest) and will likely brick your device forcing you to lose your jailbreak if not doing so.

# Latest futurerestore
wRestore uses the latest available futurerestore to ensure full compatibility with latest iOS version.
So far s0uthwest has compiled an iOS 12 compatible version:
https://github.com/s0uthwest/futurerestore/releases
Thanks bro!
