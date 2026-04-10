# Simple Web App

A simple ASP.NET Core MVC web application.

## Prerequisites

- .NET 8.0 SDK or later

## Running the Application

```bash
dotnet restore
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`.

## Features

- Home page with welcome message
- About page with application information
- Clean, responsive design
- MVC architecture pattern


~/.microsoft/usersecrets/<UserSecretsId>/secrets.json
  dotnet user-secrets set "HumanConfiguration:px_app_id" "..."
  dotnet user-secrets set "HumanConfiguration:px_cookie_secret" "..."
  dotnet user-secrets set "HumanConfiguration:px_auth_token" "..."
why not gopass
