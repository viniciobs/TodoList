# Overview
This project is intended for study purposes.

The API allows creating users and assigning tasks to them.  
All requests - except user creation and authentication - required authentication and must include a Bearer token within the `Authorization` header. 


# Configuration

### 1. SQL Server container
The `docker-compose.yml` file uses the `SA_PASSWORD` environment variable, which must be set or replaced with a real value.

It is strongly recommended to use a `.env` file instead of hardcoding the password.

At the root of the project, create a `.env` file and define the SQL Server `sa` password:
```
SA_PASSWORD=your-strong-password
```

### 2. RabbitMQ container
The `docker-compose.yml` file uses configuration files to set up RabbitMQ.   
Open the `definitions.json` located at `./Infra/MessageBroker/` and you'll notice it uses a `password_hash` value.
To generate a hash for your password, run the following Docker command (replace _your-secret-password_ with a real password) 
```
docker run --rm rabbitmq:3.13-management rabbitmqctl hash_password your-secret-password
```
This command returns a hash value, which must be copied into the `password_hash` field in `definitions.json`.

### 3. appsettings.json
The `appsettings.json` in `./Api/ToDoList` needs few changes in order to make the application to work.

#### ConnectionStrings
This is configured to use integrated security, so it must be changed to use `User Id=sa;Password=...` instead. Add the same `SA_PASSWORD` you used for creating the SQL Server container in step 1.

#### Authentication
Added a safety to the token by using a key, so update the `Authentication.Secret` adding a 32 lenght string.

#### MessageBroker
The `Password` must be the same used to set the hash_password in the step 2. This is not the hash, it is the plain password.
