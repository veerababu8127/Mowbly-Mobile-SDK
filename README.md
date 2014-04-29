Mowbly
======

Mowbly - Open Enterprise Mobility Platform - Cross platform SDK

Mowbly mobile SDK is an open source, enterprise mobile app foundation layer. The primary objective of Mowbly is to keep focus on enterprise features alone. Mowbly APIs are exposed as standard Javascript APIs which are used with HTML and CSS to build cross platform apps. It has been built to fit into the larger goal of creating a single stack enterprise mobility platform (SDK, UI, Studio, Simulator, Management, Monitoring, Analytics, Backend & Remote Support). For more details see http://www.mowbly.com

Supported OS and status: 

1. iOS - released,
2. Android - released, 
3. Blackberry - pending release - 10th May 2014
4. Windows - pending release - 20th May 2014
5. Firefox - pending release - 15th June 2014


Getting Started
==========

[Mowbly Get Started Guide](http://www.mowbly.com/docs/#/open/getting-started/index.html)

Get the source by cloning or downloading the repository.

Mowbly cross platform SDK is divided as separate projects for each OS along side the core Javascript API library. Below is the folder structure of the repo

    mowbly
      |- android/
      |- ios/
      |- js/
      |- samples/
        |- Photo Journal/ - (Quick app building exercise with Mowbly)
          |- android/
          |- ios/
        |- Mowbly Magic/ - (Extensive project covering most features of Mowbly)
          |- android/
          |- ios/
      |- template/ - (Scaffold project setup for each OS)
        |- android/
        |- ios/

Additionally each folder has a readme for specific information related to it.

Features/API List
============
(for details see http://www.mowbly.com/docs/)

| Feature Group | API           |
| ------------- |-------------|
| Page Management   |               |
|                   | open          |
|                   | setResult     |
|                   | close         |
|                   | pageName      |
|                   | parentPage    |
|                   | onReady       |
|                   | onResume      |
|                   | onData        |
|                   | onResult      |
|                   | onPause       |
|                   | onClose       |
|                   | onForeground  |
|                   | onBackground  |
|                   | open          |
|                   | open          |
| Utility           |               |
|                   | alert         |
|                   | toast         |
|                   | confirm       |
|                   | getPref       |
|                   | putPref       |
|                   | savePref      |
|                   | removePref    |
|                   | removeAllPref |
|                   | showProgress  |
|                   | hideProgress  |
| File              |               |
|                   | file          |
|                   |    - create   |
|                   |    - exists   |
|                   |    - getPath  |
|                   |    - read     |
|                   |    - remove   |
|                   |    - write    |
|                   | directory     |
|                   |    - create   |
|                   |    - exists   |
|                   |    - getFiles |
|                   |    - getPath  |
|                   |    - remove   |
|                   | getFilePath   |
|                   | fileExists    |
|                   | readFile      |
|                   | writeFile     |
|                   | deleteFile    |
|                   | appendFile    |
|                   | unzipFile     |
| Database          |               |
|                   | openDatabase  |
| Network           |               |
|                   | getNetwork    |
|                   | networkConnected  |
|                   | isHostReachable   |
|                   | onNetworkConnect  |
|                   | onNetworkDisconnect   |
| Http              |               |
|                   | get           |
|                   | download      |
|                   | post          |
|                   | httpRequest   |
| Message           |               |
|                   | email         |
|                   | sms           |
|                   | sendBgSms     |
| Device            |               |
|                   | getDeviceId   |
|                   | getMemStat    |
|                   | isAndroid     |
|                   | isBlackBerry  |
|                   | isBlackBerry10    |
|                   | isIPad        |
|                   | isIOS         |
|                   | isIPhone      |
|                   | isWeb         |
|                   | isWindowsPhone    |
|                   | onAndroid     |
|                   | onBlackBerry  |
|                   | onBlackBerry10    |
|                   | onIPad        |
|                   | onIOS         |
|                   | onIPhone      |
|                   | onWeb         |
|                   | onWindowsPhone    |
| Camera            |               |
|                   | getCamConfig  |
|                   | camSetup      |
|                   | capturePic    |
|                   | choosePic     |
|                   | showAsGallery |
| Contacts          |               |
|                   | contact       |
|                   | findContact   |
|                   | pickContact   |
|                   | callContact   |
|                   | location      |
|                   | getLocation   |
| Logging           |               |
|                   | logDebug      |
|                   | logError      |
|                   | logFatal      |
|                   | logInfo       |
|                   | logWarn       |
| Misc              |               |
|                   | bridgeVersion |


Contribute
========

If you like to add anything new or update a component, feel free to fork the project and send us a pull request. We are glad for your support :)

Issue Tracking
-----------

File issues on the issue tracker repository - https://github.com/teammowbly/Mowbly-Enterprise-Mobility-Tracker/

License
=======

Apache License V2 - 2014 - CloudPact Software Technologies Pvt. Ltd.
