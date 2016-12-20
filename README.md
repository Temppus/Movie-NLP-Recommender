# ASP.NET-Web-Application-MongoDb-Migrations.
Starting project for ASP.NET Web Application  with AspNet.Identity.MongoDB and MongoMigrations with Microsoft Unity IoC.

# Notes
Change project name for your needs.

# Initial configurations
In Web.Config, change connectionString and database.
```xml
<add name="Mongo" connectionString="mongodb://localhost:27017"/>
<add key="DatabaseName" value="MovieRecommenderDb"/>
```

# Migrations
After first start, initial mongo migrations will insert admin user to user collection and assign him admin and user roles.
Check Migration1.cs + Migration2.cs.
You can remove these classes if u dont wanna to run them or mark these classes with [Experimental] attribute.

