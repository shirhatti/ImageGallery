To use a storage account instead of the Azure Storage Emulator, add this to
`Web.Debug.Config` or `Web.Release.Config`

```xml
<?xml version="1.0"?>
<configuration xmlns:xdt="http://schemas.microsoft.com/XML-Document-Transform">
    <appSettings>
        <add key="AzureStorageConnectionString" value="RealConnectionString" xdt:Transform="Replace" xdt:Locator="Match(key)" />
    </appSettings>
</configuration>
```