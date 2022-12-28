/*
****************************************************************************
*  Copyright (c) 2022,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

dd/mm/2022	1.0.0.1		XXX, Skyline	Initial version
****************************************************************************
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net.Messages;
using Skyline.DataMiner.Net.Profiles;

/// <summary>
/// DataMiner Script Class.
/// </summary>
public class Script
{
	/// <summary>
	/// The Script entry point.
	/// </summary>
	/// <param name="engine">Link with SLAutomation process.</param>
	public void Run(Engine engine)
	{
		try
		{
			var name = engine.GetScriptParam("Activity Name").Value;
			var createScriptMessage = new SaveAutomationScriptMessage
			{
				IsUpdate = false,
				Definition = new GetScriptInfoResponseMessage
				{
					CheckSets = false,
					Options = ScriptOptions.AllowUndef,
					Name = name,
					Description = "Automatically generated automation script for PA Task: " + name,
					Type = AutomationScriptType.Automation,
					Dummies = new[]
					{
						new AutomationProtocolInfo
						{
							Description = "FunctionDVE",
							ProtocolId = 1,
							ProtocolName = "Skyline Process Automation.PA Script Task",
							ProtocolVersion = "Production",
						},
					},
					Parameters = new[]
					{
						new AutomationParameterInfo
						{
							Description = "Info",
							ParameterId = 1,
						},
						new AutomationParameterInfo
						{
							Description = "ProcessInfo",
							ParameterId = 2,
						},
						new AutomationParameterInfo
						{
							Description = "ProfileInstance",
							ParameterId = 3,
						},
					},
					Exes = new[]
					{
						new AutomationExeInfo
						{
							Id = 2,
							CSharpDebugMode = false,
							CSharpDllRefs = @"C:\Skyline DataMiner\Files\Newtonsoft.Json.dll;C:\Skyline DataMiner\Files\SLSRMLibrary.dll;C:\Skyline DataMiner\ProtocolScripts\ProcessAutomation.dll",
							PreCompile = false,
							Type = AutomationExeType.CSharpCode,
							ValueOffset = 0,
							Value = defaultcsharp,
						},
					},
					Memories = new AutomationMemoryInfo[0],
				},
			};

			engine.SendSLNetMessage(createScriptMessage);

			var scripTaskProfile = GetPaScriptTaskProfile(engine);

			var profile = new ProfileDefinition
			{
				Name = name,
				Scripts = new[] { new ScriptEntry { Name = "PA Script", Script = name } },
			};
			profile.BasedOnIDs.Add(scripTaskProfile.ID);

			var newProfileMessage = new SetProfileDefinitionMessage(profile);
			var profileDefResponse = engine.SendSLNetMessage(newProfileMessage)[0] as ProfileManagerResponseMessage;

			var profileDefId = profileDefResponse.UpdatedProfileDefinitions.First().ID;

			var profileInstance = new ProfileInstance
			{
				Name = name,
				AppliesToID = profileDefId,
			};

			var newProfileInstanceMessage = new SetProfileInstanceMessage(profileInstance);
			engine.SendSLNetMessage(newProfileInstanceMessage);
		}
		catch (Exception ex)
		{
			engine.Log("Error creating activity: " + ex);
		}
	}

	private static ProfileDefinition GetPaScriptTaskProfile(Engine engine)
	{
		var oRequestProfiles = new GetProfileDefinitionMessage();
		var oAvailableProfiles = engine.SendSLNetMessage(oRequestProfiles)[0] as ProfileManagerResponseMessage;

		var aoBaseProfiles = oAvailableProfiles.ManagerObjects.Select(x => x.Item2 as ProfileDefinition).Where(x => x != null && x.Name.Equals("PA Script Task"));
		if (!aoBaseProfiles.Any())
		{
			engine.Log("Missing PA Script Task profile.");
			engine.ExitFail("Missing PA Script Task profile.");
		}

		var scripTaskProfile = aoBaseProfiles.First();
		return scripTaskProfile;
	}

	#region defaultcsharp
	private const string defaultcsharp = @"
/*
****************************************************************************
*  Copyright (c) 2022,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

dd/mm/2022	1.0.0.1		XXX, Skyline	Initial version
****************************************************************************
*/

using System;
using System.Diagnostics;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.DataMinerSolutions.ProcessAutomation.Helpers.Logging;
using Skyline.DataMiner.DataMinerSolutions.ProcessAutomation.Manager;

/// <summary>
/// DataMiner Script Class.
/// </summary>
public class Script
{
	/// <summary>
	/// The Script entry point.
	/// </summary>
	/// <param name=""engine"">Link with SLAutomation process.</param>
	public void Run(Engine engine)
	{
		var helper = new PaProfileLoadDomHelper(engine);
		try
		{
			
		}
		catch (Exception ex)
		{
			engine.Log(""Error: "" + ex);
		}
	}
}
";
	#endregion
}