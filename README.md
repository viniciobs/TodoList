# TodoList

This project is a web API developed with ASP.Net Core 3.1, Entity Framework, SQL Server and [JWT](https://jwt.io/).

# Overview
The API allows creating users and set tasks to them.  
All requests, except user creation, needs authentication and must send a token within the authorization header. 

# How to start

* Clone    
Clone this repository, of course. ;D

* Settings    
Add the below content to *usersecrets.json* file on API project replacing with your data.
```json
{
  "ConnectionStrings": {
    "ToDoListDB": "..."
  },
  "MessageBroker": {
    "HostName": "localhost",
    "Exchange": "todolist-historyAction",
    "RoutingKey": "todolist-historyAction",
    "QueueName": "todolist-historyAction"
  },
  "Authentication": {
    "Secret": "..."
  }
}
```
    
* Messaging    
This project use RabbitMQ and already has its (direct) exchanges and queues configured as durable on admin panel.
    
* Database    
Now that the connection string is properly defined, open the command line tool, access the solution folder and run the database update
    
> update-database   
           
***    
This is my very first project with .NET CORE and Entity Framework.  
Any suggestion? Feel free to contact me.
