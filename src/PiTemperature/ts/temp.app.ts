/// <reference path="typings/Hubs.d.ts" />
/// <reference path="typings/knockout.d.ts" />

$(function () {
    class TempSensorViewModel {
        public Sensor: string;
        public Name: string;
        Temp: KnockoutObservable<number>;
        constructor(sensor: string, name: string, temp: number) {
            this.Sensor = sensor;
            this.Name = name;
            this.Temp = ko.observable(temp);
        }
    }

    class SensorsListViewModel {
        public sensors: KnockoutObservableArray<TempSensorViewModel>;
        public connectionState: KnockoutObservable<number> = ko.observable(4);
        private hub = $.connection.tempHub;

        constructor() {
            $.connection.hub.stateChanged((state: SignalRStateChange) => this.connectionStateChanged(state));
            $.connection.hub.disconnected(() => this.disconnected());
            this.hub.client.broadcastTempSensors = this.broadcastTempSensors;
            this.hub.client.broadcastTemperature = this.broadcastTemperature;
            this.sensors = ko.observableArray([]);
        }

        init() {
            this.hub.server.getTempSensors();
        }

        connectionStateChanged(state: SignalRStateChange) {
            vm.connectionState(state.newState);
        }

        disconnected() {
            setTimeout(function () {
                $.connection.hub.start(function () { vm.init(); });
            }, 5000);               
        }

        broadcastTemperature(tempSensor: TempSensor) {
            var thesensor = ko.utils.arrayFirst(vm.sensors(), function (item) {
                return item.Sensor === tempSensor.Sensor;
            });
            thesensor.Temp(tempSensor.Temp);
        }

        broadcastTempSensors(list: Array<TempSensor>) {
            var mapedSensors = $.map(list, function (item) {
                return new TempSensorViewModel(item.Sensor, item.Name, item.Temp)
            });
            vm.sensors(mapedSensors);
        }
    }

    var vm = new SensorsListViewModel();
    ko.applyBindings(vm);
    $("#connectionStateContanier").removeClass("hidden");
    $.connection.hub.start(function () { vm.init(); });
});