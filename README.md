# AddressAPI

This AddressAPI can be used to interact with a database containing addresses.

## Used NuGet packages:
- [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/5.0.9)
    - [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/5.0.9)
    - [Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.SqlServer/5.0.9)
    - [Microsoft.EntityFrameworkCore.Tools](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/5.0.9)

These packages were used to establish a connection to the Sqlite database.

- [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore/6.1.5)
    - [Swashbuckle.AspNetCore.Annotations](https://www.nuget.org/packages/Swashbuckle.AspNetCore.Annotations/6.1.5)

These packages were used to document the API in SwaggerUI.

- [Microsoft.VisualStudio.Web.CodeGeneration.Design](https://www.nuget.org/packages/Microsoft.VisualStudio.Web.CodeGeneration.Design/5.0.2)

This package was used to help generate a controller for the API.

## Usage

After starting the program, a webpage should be opened, this is SwaggerUI. This page is a guide to all possible requests to the API and has been documented to help make this as easy as possible.

### Functionality

- Create, Read, Update and Delete addresses

    The API has endpoints to interact with the database to save, edit, delete and view addresses.

## Information
This API was commisioned by [Social Brothers](https://www.socialbrothers.nl) and coded by [Gerson Mak](https://www.linkedin.com/in/gerson-mak).