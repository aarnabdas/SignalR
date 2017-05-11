# Crank

We have modified crank to create direct hub connections and also subscribe to some channel. And it can be configured to connect to multiple urls, proxy and/or channels. Essentially simplest command to invoke the tool may be:

```
crank /Config:"<path-to>\connections.config"
```

And the `connections.config` file may be:

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="connections.conf" type="Microsoft.AspNet.SignalR.Core.ConnectionConfiguration.ConnectionConfigurationSection, Microsoft.AspNet.SignalR.Core"/>
  </configSections>
  
  <connections.conf>
		<connections>
			<connection name="_message_hub_channel_name_30_" url="http://url/" proxy="hub-name" channel="channel_name" connections="30"></connection>
		</connections>
  </connections.conf>
</configuration>
```