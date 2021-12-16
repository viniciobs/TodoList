# TodoList

This project is a web API developed with ASP.Net Core 3.1, Entity Framework, SQL Server and [JWT](https://jwt.io/).

# Overview
The API allows creating users and set tasks to them.  
All requests, except user creation, needs authentication and must send a token within the authorization header. 

# How to start

* Clone

    Clone this repository, of course. ;D

* AppSetting.json

    Edit this file replacing the server, user and password within the "ConnectionStrings" part with your data.<br />
    Also replace <KEY> within the "Authentication" part with a secret key only your application must know.
    
* Database
    
    Now that the connection string is properly defined, open the command line tool, access the solution folder and run the database update
    
    > update-database   
           
***    
This is my very first project with .NET CORE and Entity Framework.  
Any suggestion? Feel free to contact me.
