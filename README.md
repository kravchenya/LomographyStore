# .NET Sample Reference Application

Sample .NET Core native Azure cloud reference application.

## Architecture overview

The application consists of APS.NET Core web-site which is hosted in Azure Web Sites. Back-end part (.NET Core Web API) of the web-site is hosted in Azure Web Sites as well. Textual product related data is stored in a Azure CosmosDB, where graphical product infromation is stored in container (blobs) in storage account. When an order in completed, information about it will be places first in a storage account's queue. Then an Azure Logic App dequeues items from the queue and also invokes Azure Function which creates an entry in storage account's table (oder history). Also Azure Logic App creates a file in file share of storage account.

![](Images/Architecture_overview.svg)

## ARM Template

![](Images/Container_Registry.svg)

### Getting Started with ARM template

In order to use ARM Template one would need to do the following

- Build source code
- Create docker images for 3 projects
  * LomographyStoreFuncs
  * LomographyStoreWeb
  * LomographyStoreApi
- Create a Resource Group with Container registry in Azure
- Deploy docker images to container registry 
- In ARM template provide username (variable "dockerregistry_username) and passwort (variable "dockerregistry_password") to access container registry , docker images names (variables "dockerimage_lomostoreapi", "dockerimage_lomostorefuncapp" and "dockerimage_lomostoreweb") and a login server link to container registry (variables "dockerregistry_domane" and "dockerregistry_url")
