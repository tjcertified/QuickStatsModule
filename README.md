QuickStatsModule [![Build status](https://ci.appveyor.com/api/projects/status/ijwnm38fdokgnid6?svg=true) ![AppVeyor tests branch](https://img.shields.io/appveyor/tests/tjcertified/quickstatsmodule/master.svg)](https://ci.appveyor.com/project/tjcertified/quickstatsmodule)
================

This is an HttpModule that displays a few quick stats about response times and sizes for an ASP.NET application.

# Included Stats
This module has two sets of stats:

### Current Stats
This module will collect:
- the size of the current response 
- the time spent on the current request
- the time specifically spent inside the HttpHandler for the current request

### Aggregate Stats
This module will also collect:
- how many responses have been completely processed by the ASP.NET pipeline and this module
- the average response size
- the largest response size so far
- the smallest response size so far

All of these stats will be displayed in a section at the bottom of each page that goes through the pipeline.

# Installation
To install this module, download QSMInstaller.exe and QuickStatsModule.dll into the same folder on your IIS server. In an elevated (Administrator) command prompt, navigate to this folder, and run QSMInstaller.exe, which will:
- install the .dll into the GAC
- add the module to the ApplicationHost.config