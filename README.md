#Apptus EPiServer Connector
The Apptus EPiServer Connector enables an EPiServer Commerce site to automatically export its product-, promotion- and configuration data to an Apptus eSales cluster.

##Apptus eSales
Apptus eSales is an automated online merchandising system. This means that it provides functionality for search, navigation, recommendations and personalization. Apptus eSales provides this functionality in an automated way, by basing its results on the actual behavior of the visitors to the web site.

[http://www.apptus.com/en/esales](http://www.apptus.com/en/esales)

##Apptus Behavioral Merchandising
Apptus Behavioral Merchandising brings state of the art search and recommendations to EPiServer Commerce stores, boosting customer satisfaction, conversion rates and order values. Powered by Apptus’ powerful self-learning software, it automatically optimizes and personalizes your offer, freeing merchandisers from the need to build and maintain multiple rules and settings.

[http://www.apptus.com/episerver](http://www.apptus.com/episerver)

#Documentation
[http://zone.apptus.com/display/EPI](http://zone.apptus.com/display/EPI)

#Building
In order to build the Apptus EPiServer Connector, please follow these steps:

Clone the repo:

    git clone https://github.com/Apptus/eSales.EPiServer.git

Copy the eSales .NET connector to (solution dir)\lib. The connector can be found in the eSales bundle.

Copy the following dependent EPiServer .dll files to (solution dir)\lib. These files can be found either in an EPiServer installation or in the [Nuget feed](http://nuget.episerver.com/):

    EPiServer.Data.dll
    EPiServer.dll
    EPiServer.Framework.dll
    EPiServer.Shell.dll
    EPiServer.UI.dll
    Mediachase.Commerce.dll
    Mediachase.Commerce.Website.dll
    Mediachase.ConsoleManager.dll
    Mediachase.MetaDataPlus.dll
    Mediachase.Search.dll
    Mediachase.Search.Extensions.dll
    Mediachase.WebConsoleLib.dll

Open `ESales.EPiServer.sln` in Visual Studio and build the solution.