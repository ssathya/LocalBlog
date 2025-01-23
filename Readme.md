# Blog Engine in Blazor
This application has been developed utilizing the Blazor framework with a PostgreSQL database backend.

For the purpose of this demonstration, the application's functionality has been intentionally streamlined. Key features include a dedicated screen for category creation and a separate screen for managing blog entries.

The homepage presents a comprehensive list of all available blogs. A double-click action on any listed blog entry initiates its launch.

While designed to operate on a local Raspberry Pi environment, the application incorporates Auth0 for user authentication and authorization.

- **Authorized Users:** Possess the privilege to create new categories and manage blog entries.
- **Other Users:** Retain the ability to view all published blogs but are restricted from making any content modifications.

To execute the application, one must set the following variables in appsettings.json or environment variables.

### appsettings.json
```
"ConnectionStrings": {
  "BlogDbConnection": "Connection String to PostgreSQL"
},
"Gemini": {
  "ApiKey": "Gemini's API Key"
}
```
### Environment variables 
```
export ASPNETCORE_URLS="https://0.0.0.0:7208;http://0.0.0.0:5071"
export ConnectionStrings__BlogDbConnection="Connection String"
export Gemini__ApiKey="Gemini's API Key"
```

With this prerequisite met, the application should be ready for immediate use.