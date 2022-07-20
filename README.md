# Zeiss Machine Stream Solution Introduction

This solution will host a service to receive notifications from remote machines. These notification data will be stored in memory and each machine will keep the latest 100 items at most.
There are two APIs provided in this solution and can be invoked by the front end. Details as follows:

## http://<host>/api/MachineStreamStatus?[status=idled|running|finished|errored|repaired]
This API will return all machines' latest status info with a JSON format response. If the optional status parameter is included in the URL, only those machines with the specified status will be listed.
If the status parameter is provided and its value is not in one of the pre-defined five values, a BAD-REQUEST(code=400) response will be returned.

## http://<host>/api/MachineStreamStatus/{machine_id}?[count=NN]
This API will return one specified machine's latest status details with a JSON format response. The optional count parameter will determine how many items will be included in the response. If it is not specified, all data will be returned.
If the machine id in the URL can not be found, a NOT-FOUND(code=404) response will be returned.