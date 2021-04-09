# OptiMetro
This application is made to solve the problem of minimum visited points between two given points using Dijkstra algorithm.
Each point can be configured to be transited by any type of transport or by an specific type of transport.
Trough the configuration file it can receive the layout of the points, connections between them and the type of each station.
An example of the configuration is included in the solution.

Executed from command line it can receive 4 parameters:

-s  Name of the starting point. Default "A".

-e  Name of the ending point. Default "F".

-c  Transport type. Default any type.

-p  Path of the configuration file. Default "StationMatrix.csv".

Example:
OptiMetro.exe -s A -e F -c Green -p StationMatrix.csv.
