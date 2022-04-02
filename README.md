# Mergen

Mergen is a project that facilitates the analysis of traffic by logging incoming web traffics to ASP.NET applications broadcasting with Microsoft IIS (Internet Information Services) service.

Logging can be done with three different rules specified in the configuration file:
  1- Logs all incoming web traffic
  2- Logs inbound web traffic over a specific Source IP address
  3- Logs inbound web traffic to the specified URL extension

## Setup:

- "Microsoft.MergenModules.dll" and "mergen.xml" files should be copied to the bin folder in the project directory.
- Mergen module should be added to the web.config file in the project directory as in the example below.

      <system.webServer>
        <modules>
          <add name="MergenLogger" type="Microsoft.MergenModules.MergenLogger, Microsoft.MergenModules" />
        </modules>
      </system.webServer>
      
- In case of error, the activities of the w3wp application can be analyzed with the "Process Monitor (Procmon)" application.

## Logging settings:

- Logging of all traffic:

In order to log all incoming web traffic to the project, 'type="All"' value should be added to the mergen.xml file as in the example below.
With the method value, the desired HTTP method to be logged can be selected. (Default: All)
If the response value is true, it also logs the body part of the traffic responses. (Default: true)

For example:

    <?xml version="1.0" encoding="UTF-8" ?>
    <mergen>
      <listener>
        <url type="All" response="true" method="All"></url>
      </listener>
     </mergen>  

- Logging of web traffic from a specific Source IP address:

In order to log web traffic coming from a specific source IP address, the 'type="IP"' value should be added to the "mergen.xml" file as in the example below.
With the method value, the desired HTTP method to be logged can be selected. (Default: All)
If the response value is true, it also logs the body part of the traffic responses. (Default: true)

For example:

    <?xml version="1.0" encoding="UTF-8" ?>
    <mergen>
      <listener>
        <url type="IP" response="false" method="POST">192.168.14.1</url>
      </listener>
     </mergen>  

- Logging of web traffic from a specific URL extension:

In order to log web traffic coming from a specific URL extension, the 'type="IP"' value should be added to the "mergen.xml" file as in the example below.
With the method value, the desired HTTP method to be logged can be selected. (Default: All)
If the response value is true, it also logs the body part of the traffic responses. (Default: true)

For example:

    <?xml version="1.0" encoding="UTF-8" ?>
    <mergen>
      <listener>
        <url type="Path" response="true" method="GET">404.aspx</url>
      </listener>
     </mergen> 
 

## Suspicious traffic logs:

If the project is running under Localsystem authority, you can access suspicious web traffic from the Microsoft-IIS-MergenLog event log in the Windows Event Log directory. If the project is running with a different authority, you can access suspicious traffic with the 8875 event id in the Application event log in the Windows Event Log directory.

If an error is received, you can access the details of the error using the Error ID 8875 in the Application event log located in the Windows Event Log directory.

Demo [here](https://youtu.be/pTvdWe7tT5U)
