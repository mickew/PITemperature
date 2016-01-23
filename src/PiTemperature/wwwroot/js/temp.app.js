/// <reference path="typings/Hubs.d.ts" />
/// <reference path="typings/knockout.d.ts" />
/// <reference path="typings/gauge.d.ts" />
function windowExternal(arg) {
    if (window.external.notify != undefined)
        window.external.notify(arg);
}
$(function () {
    var TempSensorViewModel = (function () {
        function TempSensorViewModel(sensor, name, temp) {
            this.Sensor = sensor;
            this.Name = name;
            this.Temp = ko.observable(temp);
        }
        return TempSensorViewModel;
    })();
    var SensorsListViewModel = (function () {
        function SensorsListViewModel() {
            var _this = this;
            this.connectionState = ko.observable(4);
            this.hub = $.connection.tempHub;
            $.connection.hub.stateChanged(function (state) { return _this.connectionStateChanged(state); });
            $.connection.hub.disconnected(function () { return _this.disconnected(); });
            this.hub.client.broadcastTempSensors = this.broadcastTempSensors;
            this.hub.client.broadcastTemperature = this.broadcastTemperature;
            this.sensors = ko.observableArray([]);
        }
        SensorsListViewModel.prototype.init = function () {
            this.hub.server.getTempSensors();
        };
        SensorsListViewModel.prototype.drawGauges = function () {
            this.sensors().forEach(function (s) {
                s.Gauge = new Gauge({
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
        };
        SensorsListViewModel.prototype.connectionStateChanged = function (state) {
            vm.connectionState(state.newState);
            switch (state.newState) {
                case 0:
                    vm.sensors().forEach(function (s) { s.Gauge.setValue(-30); });
                    break;
                case 4:
                    vm.sensors().forEach(function (s) { s.Gauge.setValue(70); });
                    break;
                default:
                    break;
            }
        };
        SensorsListViewModel.prototype.disconnected = function () {
            setTimeout(function () {
                $.connection.hub.start(function () { vm.init(); });
            }, 5000);
        };
        SensorsListViewModel.prototype.broadcastTemperature = function (tempSensor) {
            var thesensor = ko.utils.arrayFirst(vm.sensors(), function (item) {
                return item.Sensor === tempSensor.Sensor;
            });
            thesensor.Temp(tempSensor.Temp);
            thesensor.Gauge.setValue(tempSensor.Temp);
        };
        SensorsListViewModel.prototype.broadcastTempSensors = function (list) {
            var mapedSensors = $.map(list, function (item) {
                return new TempSensorViewModel(item.Sensor, item.Name, item.Temp);
            });
            vm.sensors(mapedSensors);
            vm.drawGauges();
        };
        return SensorsListViewModel;
    })();
    var vm = new SensorsListViewModel();
    ko.applyBindings(vm);
    $("#connectionStateContanier").removeClass("hidden");
    $.connection.hub.start(function () { vm.init(); });
});
