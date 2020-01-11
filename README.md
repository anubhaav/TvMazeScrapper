# TvMazeScrapper

## Steps to run the project:
* Run `dotnet restore`
* Run `dotnet run`

I have included SqLite database with scrapped data of 5 pages from TvMaze Scrapper. If you want to start fresh then:

* Delete the showcast.db
* Run `EntityFrameworkCore\Update-Database`
* Then `dotnet run`

(You can also use IISExpress which comes with Visual Studio.)

## Urls

* BasicUrl: {http|https}://localhost:{port}
* All are `GET` Requests

* To start Scrapping: BasicUrl/api/Scrap/StartScrap/{PageNumber}
  * If user do not send PageNumber then it will automatically determine Page number.
  * See this: https://www.tvmaze.com/api#show-index
  * Reponse will be Status 200 or 404. If 200 Status Code then Scrapping will start.
  * The scrapping will continue in background.
  * It uses Throttling therefore one will have to wait for more than 2 minutes to get data in Database if it is clean, as there is a rate limit.
  * See: https://www.tvmaze.com/api#rate-limiting
* To Get a Single Show with an ID: BasicUrl/api/Show/GetShowCast/{Id}/{PageNumber}/{PerPage}
  * Id, PageNumber, PerPage is not required
  * By Default: Id = 1, PageNumber = 1, PerPage = 1
  * PageNumber, PerPage is not necessary at all as it only sends single Show with Cast.
* To Get List of Shows with Range of Show Id: BasicUrl/api/Show/GetShowCastList/{FromId}/{ToId}/{PageNumber}/{PerPage}
  * If FromId, ToId are not given then by default it is 1, and 250 respectively. 
  * If PageNumber, and PerPage is not given then by default it is 1 and 10 respectively.

### Also want to state that Cast response can contain duplicate Casts and Birthdays can be null, so already handled that.

## Images (Used PostMan, but Pretty JSON was not used as it was overflowing from Screen)
* Starting Scrapping 
![Starting Scrapping](https://docs.google.com/uc?id=1fA9jmrrvPnNGG5acsyIstLC9izWj93oZ)

* Get a Single Show
![Get a Single Show](https://docs.google.com/uc?id=10UyWljKaSGVCNDnvu5gfVPpXf1RcetNo)

* Get List of Shows with Range
![Get List of Shows with Range](https://docs.google.com/uc?id=1A478sWU4Uw7WE8Dq1C2k1W3YG9O74r-L)
