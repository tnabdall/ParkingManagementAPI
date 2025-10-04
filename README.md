# ParkingManagementAPI
Handles allocating and deallocating vehicle parking spaces

# Running the Project
Install the .NET 8 SDK <br>
Run `dotnet run --project src/ParkingManagement.API/ParkingManagement.API.csproj --launch-profile http` <br>
It will run the app at http://localhost:5260. Go to http://localhost:5260/swagger/index.html for the swagger interface.

# Questions to Ask / Things to Consider
How should this application respond to a parking request in general? A parking application should probably handle errors gracefully with allowing the user to proceed as long as no safety boundaries are broken. For example, if a vehicle type is something like tiny, should the application allow the vehicle through and should it default to small/large? <br>
How should the application respond to a parking release in general? You should always allow a car to exit the parking lot but in case there's an exception, what would be appropriate for the charge? <br>
What happens if a vehicle is already park and tries to park again? <br>
Should parking spaces be configurable to allow a range of vehicle types? <br>
How extensible should this app be to additional parking lots? Do we need to store parking lot info other than ID? <br>
Do we need to store history of parking requests and releases? <br>
Do we need user friendly error messages or error codes? <br>
Do we need to plan for concurrency? This app may be used at multiple terminals in the same parking lot.

# Assumptions
Basic error handling was implemented with try/catch. Usually, you would implement specific error codes and user friendly messages. For example, we haven't covered the case where a spot is requested but there are 0 in total available. <br>
The app should be extensible to multiple parking lots. An id was added but hidden in the application logic for the purposes of this demo and matching endpoints. <br>
History does not need to be stored, although I am not taking steps to delete vehicle allocations at the moment. <br>
Concurrency doesn't need to be considered. This could be DB dependent and there are C# classes that have concurrency friendly counterparts for this sort of implementation. <br>
Charges take into account fractional minutes but round to 2 decimal places. <br>
Parking spaces allow any type of vehicle.