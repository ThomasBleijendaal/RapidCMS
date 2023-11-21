# RapidCMS

#### RapidCMS

[![#](https://img.shields.io/nuget/v/RapidCMS.UI?style=flat-square)](https://www.nuget.org/packages/RapidCMS.UI)

RapidCMS is a Blazor framework that allows you to build a CMS purely from code. It provides a comprehensive set of
editors and controls, next to allowing you to add your own razor components for custom editors, buttons, labels, complete 
sections and pages, and dashboard sections. By running as an element within your ASP.NET Core application, you have full control
over the DI container, repositories, authentication, authorization and additional features like Api controllers, Mvc controllers,
and Razor pages. 

You can choose to provide your own repositories for data access by deriving from `BaseRepository` which do not have a strong preference
for a type of database, so hooking up an EF Core database context is just as easy as hooking up a Azure Table Storage client, or MongoDb database.

It is also possible to employ the Model Maker plugin for RapidCMS, which is a C# source generator that allows you to design and generate 
a RapidCMS setup right inside RapidCMS. Model Maker currently generates an EF Core application, complete with DbContext, entity configuration,
validation and repositories. Just provide a connection string and create migrations after using Model Maker, and you are up and running.

You either run RapidCMS as a Blazor Server-side application, or as a Blazor WebAssembly application directly in the browser. Using the
Api companion you can run your repositories on a separate web server, or an Azure Function app, or choose to run the repositories directly
in the browser.

## Demo

A demo of the WebAssembly variant of the CMS (running version 4.3.x) can be found here: 
[https://rapidcms.z6.web.core.windows.net/](https://rapidcms.z6.web.core.windows.net/). This uses a repository that saves its data to the 
local storage of the browser.

## How to setup RapidCMS

Since RapidCMS can be used in various ways, please pick a deployment mode. And remember, switching deployment modes with RapidCMS is easy.

- [Server-side RapidCMS](SETUP_SERVERSIDE.md) - requires an ASP.NET Core web server.
- [Client-side RapidCMS](SETUP_CLIENTSIDE.md) - can be statically hosted.
- [Companion API for client-side RapidCMS](SETUP_COMPANION.md) - requires an ASP.NET Core web server or Azure Function App.

### Authentication

- [Add authentication to Server-side](AUTHserver.md)
- [Add authentication to Client-side + Companion API](AUTHclient.md)

## Quick starts

- [RapidCMS](QUICKSTART.md)
- [RapidCMS Model Maker](MMQUICKSTART.md)
