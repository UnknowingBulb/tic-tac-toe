# Welcome to My tic-tac-toe!

Hi! This is tic-tac-toe. It's my second c# project (first one was my diploma many years ago). 
Here I'm learning how to work with HTML, css, reactJS, ASP.NET.

## TO-DO 
		✔ do some web front-end
		✔ update play field on other player move
		✔ database storage (sql, redis or smth else + Entity Framework?)
		✔ reduce db queries count (mostly for user), reorganize active user state storage
		✔ show errors on front-end
		✔ allow to user input on user creation (backend is mostly done, need to do frontend)
		✔ finish game with draw
		✔ show winner (or draw)
		- add functions of session close (if no 2nd player)
		- add display of active user sessions (and possibility to connect to them)
		- do cool front-end styles (css)
		✔ do controllers async (where it's possible). Maybe won't be done
		- disconnect from current sse-group on session change
		- ru and en locale
		- organize project files - partially done
		- fix formatting
		- remove start project files (mostly js) - partially done
		- remove code duplicates
		- do TODOs, fix comments
		- do Docker file for run in Docker
		✔ add credentials (in bottom or another page) 
		- do full credentials not only for localhost
		- tests (for practice)
			- core
			- integration
			- API
			- UI (Selenium, maybe in another project)
## How to run
From XOX directory run in PowerShell
```
dotnet build
dotnet run
```
(somewhere it needs to run npm install but i still didn't sort out)
App starts:
```
https://localhost:5001
http://localhost:5000
```
Also is possible to host on IIS/IIS Express.
