﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Example.Bootstrapping.Logging;

namespace Example.Bootstrapping
{
    public class LoggingOrchestrator
    {
        public void InitializeLogging<TLog>(string mainThreadName, string banner) where TLog : ILog, new()
        {
            // Set the main thread's name to make it clear in the logs.
            if (Thread.CurrentThread.Name != mainThreadName)
                Thread.CurrentThread.Name = mainThreadName;
            
            // Sets my logger to the console, which goes to the debug output.
            Log.InitializeWith<TLog>();

            this.Log().Debug("Logging initialized.");

            // Show a banner to easily pick out where new instances start
            // in the log file. Plus it just looks cool.
            banner.Split(new[] {Environment.NewLine}, StringSplitOptions.None)
                .ForEach(x => this.Log().Info(x));
        }

        public void LogUsefulInformation(IEnvironmentFacade environment, AppSettings appSettings)
        {
            this.Log().Debug("Gathering system information...");
            var assemblyLocation = environment.GetAssemblyLocation();
            var productName = environment.GetProductName();
            var assemblyVersion = environment.GetAssemblyVersion();
            var fileVersion = environment.GetAssemblyFileVersion();
            var productVersion = environment.GetProductVersion();
            var principalName = environment.GetPrincipalName();
            var culture = environment.GetCurrentCulture();
            var hostName = environment.GetHostName();
            var ipAddress = environment.GetCurrentIpV4Address();
            var instanceName = environment.GetServiceInstanceName().IfNullOrEmpty("[Running in console mode]");
            var windowsVersion = environment.GetWindowsVersionName();

            var keyValues = new Dictionary<string, string>
            {
                ["Assembly location"] = assemblyLocation,
                ["Assembly version"] = assemblyVersion,
                ["File version"] = fileVersion,
                ["Product version"] = productVersion,
                ["Instance name"] = instanceName,
                ["Running as"] = $"{principalName} ({culture})",
                ["Network host"] = $"{hostName} ({ipAddress})",
                ["Windows version"] = windowsVersion,
            };

            var appSettingProperties = appSettings.GetType().GetProperties();

            if (appSettingProperties.Any())
            {
                keyValues.Add("Configuration", "=====================");

                appSettingProperties
                    .ForEach(x => keyValues.Add(x.Name, x.GetValue(appSettings)?.ToString() ?? "[NULL]"));
            }

            var longestKey = keyValues.Keys.Max(x => x.Length);

            this.Log().Info("");
            foreach (var keyValue in keyValues)
            {
                this.Log().Info($"{keyValue.Key.PadLeft(longestKey)}: {keyValue.Value}");
            }
            this.Log().Info("");
            this.Log().Info($"Starting {productName} v{productVersion}");
            this.Log().Info("");
        }
    }
}