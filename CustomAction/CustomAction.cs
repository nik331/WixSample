////////////////////////////////////////////////////////////////////////////////////////////
// <copyright file=""SelfOperatingSignatureCheckMasterServiceCustomAction.cs"" company=""Video Gaming Technologies, Inc."">
// Copyright © 2018 Video Gaming Technologies, Inc.  All rights reserved.
// This document contains proprietary and copyrighted material and may not
// be reproduced or disclosed without the prior written permission of VGT.
// </copyright>
////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;

namespace CustomAction
{
    public class CustomActions
    {
        /// <summary>
        /// Name of Application Config Output file
        /// </summary>
        private const string ConfigFileName = "Master.exe.config";
        
        /// <summary>
        /// Name of Settings Output File
        /// </summary>
        private const string SettingsFileName = "appsettings.config";

        /// <summary>
        /// ConfigTemplate file text
        /// </summary>
        private static string ConfigTemplate { get; set; }

        /// <summary>
        /// SettingsTemplate File text
        /// </summary>
        private static string SettingsTemplate { get; set; }

        /// </summary>
        /// <param name="session">Session from Installer Engine</param>
        /// <returns>
        /// Returns ActionResult.Success always or install will fail.
        /// </returns>
        [CustomAction]
        public static ActionResult WriteAppConfiguration(Session session)
        {     
            session.Log("Begin WriteAppConfiguration");
            var assembly = typeof(CustomActions).Assembly;

            WriteAppConfig(session, assembly);
            WriteAppSettings(session, assembly);
            
            session.Log("End WriteAppConfiguration");

            return ActionResult.Success;
        }

        /// <summary>
        /// Write the Application config file
        /// </summary>
        /// <param name="session">installer session for log</param>
        /// <param name="assembly">assembly to use to get embedded resources</param>
        private static void WriteAppConfig(Session session, System.Reflection.Assembly assembly)
        {
            try
            {
                string resourceName = typeof(CustomActions).Namespace + ".appconfig-template.txt";
                var textStreamReader = new StreamReader(assembly.GetManifestResourceStream(resourceName));
                ConfigTemplate = textStreamReader.ReadToEnd();
                session.Log("Value of RABBITMQ_IP_PROP: {0}", session.CustomActionData["RABBITMQ_IP_PROP"]);
                session.Log("Value of MGT API_IP_PROP: {0}", session.CustomActionData["API_IP_PROP"]);
                session.Log("Value of MGT API_PORT_PROP: {0}", session.CustomActionData["API_PORT_PROP"]);

                File.WriteAllText(session.CustomActionData["CONFIG_DIR"] + ConfigFileName,
                    ConfigTemplate.Replace("^^RABBITMQ_IP^^", session.CustomActionData["RABBITMQ_IP_PROP"])
                        .Replace("^^API_IP^^", session.CustomActionData["API_IP_PROP"])
                        .Replace("^^API_PORT^^", session.CustomActionData["API_PORT_PROP"]));
            }
            catch (Exception ex)
            {
                session.Log("The WriteAppConfiguration custom action got an exception during execution: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Write the Application settings file
        /// </summary>
        /// <param name="session">installer session for log</param>
        /// <param name="assembly">assembly to use to get embedded resources</param>
        private static void WriteAppSettings(Session session, System.Reflection.Assembly assembly)
        {
            try
            {
                string resourceName = typeof(CustomActions).Namespace + ".appsettings-template.txt";
                var textStreamReader = new StreamReader(assembly.GetManifestResourceStream(resourceName));
                SettingsTemplate = textStreamReader.ReadToEnd();

                session.Log("Value of RABBITMQ_IP_PROP: {0}", session.CustomActionData["RABBITMQ_IP_PROP"]);
                session.Log("Value of MGT API_IP_PROP: {0}", session.CustomActionData["API_IP_PROP"]);
                session.Log("Value of MGT API_PORT_PROP: {0}", session.CustomActionData["API_PORT_PROP"]);

                File.WriteAllText(session.CustomActionData["CONFIG_DIR"] + SettingsFileName,
                        SettingsTemplate.Replace("^^RABBITMQ_IP^^", session.CustomActionData["RABBITMQ_IP_PROP"])
                        .Replace("^^BUILD_VER^^", session.CustomActionData["BUILD_VER_PROP"])
                        .Replace("^^BUILD_PATH^^", Directory.GetParent(session.CustomActionData["BUILD_PATH_PROP"]).FullName));
            }
            catch (Exception ex)
            {
                
                session.Log("The WriteAppConfiguration custom action got an exception during execution: {0}", ex.ToString());
            }
        }
        //attrib -r $(ProjectDir) BinariesFiles.wxs && "$(WixToolPath)heat" dir "$(ProjectDir)" -gg -sreg -srd -dr INSTALL_FOLDER -cg ProductComponents -sfrag -suid -var var.FileSourceDirectory template:componentgroup -out $(ProjectDir) BinariesFiles.wxs
        //attrib -r $(ProjectDir) SOSCClientFiles.wxs &amp;&amp; "$(WixToolPath)heat" dir "$(ArtifactsPath)Services\SOSC\Client" -gg -sreg -srd -dr SelfOperatingSignatureCheck_INSTALL_FOLDER -cg ProductComponents -sfrag -suid -var var.FileSourceDirectory template:componentgroup -out $(ProjectDir) SOSCClientFiles.wxs
    }
}
