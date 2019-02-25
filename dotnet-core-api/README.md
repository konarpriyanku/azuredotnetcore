---
services: app-service\web,app-service
platforms: dotnet
author: priyanku
---
ef migrations
dotnet ef migrations add CreateTaskManagerDB
dotnet ef database update