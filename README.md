## TransactionLoaderService

* This app presents itself pretty standard ASP.NET Core MVC web app, derived from default mvc template.
* It has basic layered/onion/clean architecture style with dependency inversion - main business logic, domain objects and necessary abstractions reside in a Core project.
* Web UI and Api controller reside in a Web project, for easier Api test I've added Swagger.
* Storage layer abstracted behind ITransactionsRepository interface and under the hood EFCore with SqlServer adapter is being used, Core project is purposefully made storage-agnostic. 
* Unit tests were added during file readers development, so I could test my data converters.

### Specific implementation details
#### Transaction file loader
TransactionFileLoader operates on a stream, which decouples Core from specifics of a file source.
Inside a controller file format guess is made based on a content type, TransactionFileLoader tries to use this guess and pick appropriate ITransactionStreamReader - Csv or Xml.
For parsing ISO4217 codes I've used a NMoneys library.
Each reader has SetStream method to accept stream and CanRead property that tries to parse content and check if content if formatted as required, but doesn't attempt full parse.
If CanRead = true, then this reader is used to parse full content of a Stream. Since by condition we have to log parse/format errors to some log and also display this errors to the user,
I've made a simple ILogger wrapper StringListLogger, which saves error messages to later pass them back for user display. For simplicity I've used only console sink, but it could substituted by any log storage as necessary.

#### Api

I've decided to use a single endpoint which can accept all required filters simultaneously. Query is made directly with LINQ. 
Depending on amount of data in a table and specific query patterns different indexes could be used.
I've decided to create 2 non-clustered indexes both including dates as a 1st column and CurrencyCode/Status as a 2nd column, because usually in practice we want to constraint such queries by period.
(For a real Api there is would be a need for a result pagination as well)

### How to run
App requires dotnet 6 sdk and Sql Server. For testing purposes I've used developer edition of Sql Server with docker container and connection string in appsettings.json is set accordingly.
To replicate my db setup you'll need to run this command: `docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Passw0rd"  -p 1433:1433 --name sql1 --hostname sql1 -d mcr.microsoft.com/mssql/server:2022-latest`
EFCore migrations apply automatically when the app starts. If you need to clear db you can uncomment `db.Database.EnsureDeleted();` line in Program.cs
