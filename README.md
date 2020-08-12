# .NET Sample Reference Application

A sample .NET Core native Azure cloud reference application which demonstrates abilities of such Azure services as Azure Web Sites, Azure Logic App, Azure Function, Azure Storage, Azure Cosmos DB, Azure Resource Manager (PaaS and SaaS) and Azure Container Registry.

## Architecture overview

The application consists of APS.NET Core web-site which is hosted in Azure Web Sites. Back-end part (.NET Core Web API) of the web-site is hosted in Azure Web Sites as well. Textual product related data is stored in a Azure CosmosDB, where graphical product information is stored in container (blobs) in Azure Storage. When an order is completed, information about it will be placed first in a Storage's queue. Then an Azure Logic App dequeues items from the queue and also invokes Azure Function which creates an entry in a Storage table (order history). Also Azure Logic App creates a file in Storage's file.

![](Images/Architecture_overview.svg)

## ARM Template

![](Images/Container_Registry.svg)

### Getting Started with ARM template

In order to use ARM Template one would need to do the following

- Create docker images for 3 projects
  * LomographyStoreFuncs
  * LomographyStoreWeb
  * LomographyStoreApi
- Create a Resource Group with Container Registry in Azure
- Deploy docker images to Azure Container Registry
- In ARM template provide username (variable "dockerregistry_username) and passwort (variable "dockerregistry_password") to access container registry, docker images names (variables "dockerimage_lomostoreapi", "dockerimage_lomostorefuncapp" and "dockerimage_lomostoreweb") and a login server link (variables "dockerregistry_domane" and "dockerregistry_url")
- After deploying main template, you would need to deploy logic app template

### Notes

This project was created bases on a [course](https://www.linkedin.com/learning/building-a-web-application-on-microsoft-azure) by  Matt Milner on LinkedIn Learning.
In addition to the course I refactored the code, wrote unit tests, introduced docker containers and added their support in ARM templates


