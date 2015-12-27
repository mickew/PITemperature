// Get signalr.d.ts.ts from https://github.com/borisyankov/DefinitelyTyped (or delete the reference)
/// <reference path="signalr.d.ts" />
/// <reference path="jquery.d.ts" />

////////////////////
// available hubs //
////////////////////
//#region available hubs

interface SignalR {
    tempHub: TempHub;
}
//#endregion available hubs

///////////////////////
// Service Contracts //
///////////////////////
//#region service contracts

//#region InfoDisplayHub hub

interface TempHub {
    server: TempHubServer;
    client: TempHubClient;
}

interface TempHubServer {

    //* 
    //  * Sends a "send" message to the ChatHub hub.
    //  * Contract Documentation: ---
    //  * @param name {string} 
    //  * @param message {string} 
    //  * @return {JQueryPromise of void}
      
    //send(name: string, message: string): JQueryPromise<void>
    getTempSensors(): JQueryPromise<void>
}

interface TempHubClient {

    ///**
    //  * Set this function with a "function(name : string, message : string){}" to receive the "addNewMessageToPage" message from the ChatHub hub.
    //  * Contract Documentation: ---
    //  * @param name {string} 
    //  * @param message {string} 
    //  * @return {void}
    //  */
    //addNewMessageToPage: (name: string, message: string) => void;
    broadcastTemperature: (tempSensor: TempSensor) => void;
    broadcastTempSensors: (list: Array<TempSensor>) => void;
}

//#endregion InfoDisplayHub hub

//#endregion service contracts



////////////////////
// Data Contracts //
////////////////////
//#region data contracts

interface TempSensor {
    Sensor: string;
    Name: string;
    Temp: number;
}

//#endregion data contracts