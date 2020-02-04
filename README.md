# azure-train-group2



1. execute `script/create.ps1` and `script/create_part2.ps1` to create azure resources

2. configure stream analytics
  
- add `rg2iothub` as input (alias: `iothub`)
  
   - add table `sensorvalid`  as output (alias: `valid`; partion key: `deviceid`)
   
   - add table `sensorerror` as output (alias: `invalid` partion key: `deviceid`)
   
   - add data lake `sensordata`as output (alias `lake`)
   
   - create a user-defined function
   
     ```
     function isvalid(temperature, windSpeed, windDirection, humidity)
     {
         return (temperature <= 50 && temperature >= -50 &&
         windSpeed >= 0 && windSpeed <= 70 &&
         windDirection >= 0 && windSpeed <= 359 &&
         humidity >= 0 && humidity <= 100);
     }
     ```
     
   - add the following query
   
     ```
     WITH enriched AS (
         SELECT *, udf.isvalid(temperature, windSpeed, windDirection, humidity) FROM [iothub]
     )
     
     SELECT
         *
     INTO
         [invalid]
     FROM
         [enriched]
     WHERE
         isvalid = 0
     
     SELECT
         *
     INTO
         [valid]
     FROM
         [enriched]
     WHERE
         isvalid = 1
     
     SELECT
         *
     INTO
         [lake]
     FROM
         [iothub]
     ```
   
     
   
3. Start the simulator

   - If running via vs.code, you must open the respective folder

   - The simulator accepts the following environment variables to configure it
     - `DEVICE_ID` : the device id of the simulated device (must be registered beforehand)
     - `PUSH_INVALID_DATA` (true/false): if enabled, the simulator (also) pushes invalid data to the iot hub
