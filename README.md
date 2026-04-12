<p align="center">
  <img src="src/client/public/favicon.svg" alt="Quibble Icon" />
</p>

# Quibble
Quibble! hosts interactive, real-time pub quizzes.

The first version was hacked together in record time with Blazor Server during Covid-19 to run pub quizzes online with friends.

The current version has an Angular frontend, and an ASP.NET Core API.
Real-time events (such as questions being revealed, and answers being marked) are synced to connected users via SignalR.
This is all behind YARP, a reverse-proxy gateway.

Data is persisted in PostgreSQL using EF Core, and cached in Redis.
Auth is delegated to Entra ID, and everything is self-hosted in containers.
