# Blog Engine in Blazor
This application has been developed utilizing the Blazor framework with a PostgreSQL database backend.

For the purpose of this demonstration, the application's functionality has been intentionally streamlined. Key features include a dedicated screen for category creation and a separate screen for managing blog entries.

The homepage presents a comprehensive list of all available blogs. A double-click action on any listed blog entry initiates its launch.

While designed to operate on a local Raspberry Pi environment, the application incorporates Auth0 for user authentication and authorization.

- **Authorized Users:** Possess the privilege to create new categories and manage blog entries.
- **Other Users:** Retain the ability to view all published blogs but are restricted from making any content modifications.

To execute the application, please ensure the environment variable 'ConnectionStrings_BlogDbConnection' is correctly configured to point to your specific PostgreSQL database instance. With this prerequisite met, the application should be ready for immediate use.