/// <reference path="typings/Hubs.d.ts" />
/// <reference path="typings/knockout.d.ts" />
/// <reference path="typings/gauge.d.ts" />

interface External {
    notify: Function;
}

function windowExternal(arg: string) {
    if (window.external.notify != undefined)
        window.external.notify(arg);
}

$(function () {
    class TempSensorViewModel {
        public Sensor: string;
        public Name: string;
        Temp: KnockoutObservable<number>;
        Gauge: Gauge;
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

        drawGauges() {
            this.sensors().forEach(s => {
                s.Gauge = new Gauge(
                    {
                        renderTo: s.Sensor,
                        title: s.Name,
                        minValue: -30,
                        maxValue: 70,
                        majorTicks: ['-30', '-20', '-10', '0', '10', '20', '30', '40', '50', '60', '70'],
                        units: "°C",
                        valueFormat: { int: 2, dec: 2 },
                        glow: true,
                        highlights: [{
                            from: -30,
                            to: -10,
                            color: 'SkyBlue'
                            }, {
                                from: -10,
                                to: 10,
                                color: 'Khaki'
                            }, {
                                from: 10,
                                to: 30,
                                color: 'PaleGreen'
                            }, {
                                from: 30,
                                to: 70,
                                color: 'LightSalmon'
                            }],
                        animation: {
                            delay: 10,
                            duration: 300,
                            fn: 'bounce'
                        }
                    });
                s.Gauge.setValue(s.Temp());
                s.Gauge.draw();
            });
        }

        connectionStateChanged(state: SignalRStateChange) {
            vm.connectionState(state.newState);
            switch (state.newState) {
                case 0:
                    vm.sensors().forEach(s => { s.Gauge.setValue(-30) });
                    break;
                case 4:
                    vm.sensors().forEach(s => { s.Gauge.setValue(70) });
                    break;
                default:
                    break;
            }
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
            thesensor.Gauge.setValue(tempSensor.Temp);
        }

        broadcastTempSensors(list: Array<TempSensor>) {
            var mapedSensors = $.map(list, function (item) {
                return new TempSensorViewModel(item.Sensor, item.Name, item.Temp)
            });
            vm.sensors(mapedSensors);
            vm.drawGauges();
        }
    }

    var vm = new SensorsListViewModel();
    ko.applyBindings(vm);
    $("#connectionStateContanier").removeClass("hidden");
    $.connection.hub.start(function () { vm.init(); });
});