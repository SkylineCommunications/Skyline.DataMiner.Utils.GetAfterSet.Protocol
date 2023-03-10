
# Get After Set Library

A library used to re-poll certain parameter. This is usefull when a device takes a while to handle a set. 

You can define how many times you want a parameter to be re-polled until it is the desired value. If after the defined retries the parameter is still not it's desired value it logs it.

This library requires a bit of setup to use.


## Table of Content

- [Installation](#Installation)
- [Setup](#Setup)
- [Usage/Examples](#UsageExamples)

## Installation

Add the nuget package to your QAction with the "Manage Nuget Packges..."

## Setup 

> **Note** You will need a Trigger for everything you want to re-poll so the QAction can use it. Most of the time this will already be there.

#### Step 1.
Add 2 parameters to the xml code. You can pick whatever id, name and description you want.
```xml
<Param id="99">
    <Name>AddToRetryBuffer</Name>
    <Description>Add to the Retry Buffer</Description>
    <Type>read</Type>
    <Interprete>
        <RawType>other</RawType>
        <Type>string</Type>
        <LengthType>next param</LengthType>
    </Interprete>
</Param>
<Param id="100">
    <Name>CheckRetryBuffer</Name>
    <Description>Check Request Buffer Dequeue</Description>
    <Type>dummy</Type>
</Param>
```

#### Step 2.
Add a new QAction and make sure the triggers are the id's of the 2 parameters you defines in Step 1.
```xml
<QAction id="100" name="Check Retry Buffer" encoding="csharp" triggers="99;100"></QAction>	
```

#### Step 3.
Add an action, that will trigger the dummy parameter from Step 1.
```xml
<Action id="100">
    <Name>Check Request Buffer</Name>
    <On id="100">parameter</On>
    <Type>run actions</Type>
</Action>
```

#### Step 4.
Add a group, that will trigger the Action defines in Step 3.
```xml
<Group id="100">
    <Name>Get After Set Check</Name>
    <Description>Get After Set Check</Description>
    <Type>poll action</Type>
    <Content>
        <Action>100</Action>
    </Content>
</Group>
```

#### Step 5.
Add a 1 second timer, that will trigger the group. If there is already a small timer you can use that one instead and just add the group created in Step 4.
```xml
<Timer id="3">
    <Name>1 sec timer</Name>
    <Time initial="true">1000</Time>
    <Interval>75</Interval>
    <Content>
        <Group>100</Group>
    </Content>
</Timer>
```

#### Step 6.
Open the QAction created in Step 2. Add the below code and fill in the parameter ids from Step 1.
```csharp
using System;

using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.GetAfterSet;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public class QAction
{
	private readonly RetryBuffer parameterBuffer = new RetryBuffer(/* The id of the add parameter from Step 1 */, /* The id of the dummy parameter of step 1. */);

	/// <summary>
	/// Check if there are requests queued.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public void Run(SLProtocol protocol)
	{
		try
		{
			parameterBuffer.Process(protocol);
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}
}

```

## Usage/Examples

#### With a standalone parameter.

- StandAloneParameter: The id of the parameter that needs to update after the set is done.
- TriggerId: The id of the trigger that will re-poll "StandAloneParameter".
- desiredValue: The value "StandAloneParameter" needs to be.

```csharp
using System;

using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.GetAfterSet;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
	/// <summary>
	/// Check if there are requests queued.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public void Run(SLProtocol protocol)
	{
		try
		{
			protocol.SetParameter(StandaloneParameter, "Set");

			var retryRequest = new GetAfterSetConfig(
					StandaloneParameter
					TriggerId,
					desiredValue);

			protocol.SetParameter(Parameter.addrequestbuffer, Convert.ToString(new RequestQueue(retryRequest, 5)));
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}
}

```

#### With a table cell.

- TablePid: The id of the table that holds the cell that needs to update after the set is done.
- RowKey: The primary key the row that holds the cell of that needs to update after the set is done.
- ColumnIdx: The column idx of the column that holds the cell of that needs to update after the set is done.
- TriggerId: The id of the trigger that will re-poll the table, row or cell.
- desiredValue: The value cell needs to be.

```csharp
using System;

using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.GetAfterSet;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
	/// <summary>
	/// Check if there are requests queued.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public void Run(SLProtocol protocol)
	{
		try
		{
			protocol.SetParameter(StandaloneParameter, "Set");

			var retryRequest = new GetAfterSetConfig(
					TablePid,
					RowKey,
					ColumnIdx,
					TriggerId,
					desiredValue);

			protocol.SetParameter(Parameter.addrequestbuffer, Convert.ToString(new RequestQueue(retryRequest, 5)));
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}
}

```

