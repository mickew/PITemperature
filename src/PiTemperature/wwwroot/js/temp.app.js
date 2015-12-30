/// <reference path="typings/Hubs.d.ts" />
/// <reference path="typings/knockout.d.ts" />
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
        SensorsListViewModel.prototype.connectionStateChanged = function (state) {
            vm.connectionState(state.newState);
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
        };
        SensorsListViewModel.prototype.broadcastTempSensors = function (list) {
            var mapedSensors = $.map(list, function (item) {
                return new TempSensorViewModel(item.Sensor, item.Name, item.Temp);
            });
            vm.sensors(mapedSensors);
        };
        return SensorsListViewModel;
    })();
    var vm = new SensorsListViewModel();
    ko.applyBindings(vm);
    $("#connectionStateContanier").removeClass("hidden");
    $.connection.hub.start(function () { vm.init(); });
});
