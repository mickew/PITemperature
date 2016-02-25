/// <reference path="typings/Hubs.d.ts" />
/// <reference path="typings/knockout.d.ts" />
/// <reference path="typings/gauge.d.ts" />
function windowExternal(arg) {
    if (window.external.notify != undefined)
        window.external.notify(arg);
}
$(function () {
    var TempSensorViewModel = (function () {
        function TempSensorViewModel(sensor, name, temp, minValue, maxValue, ticksInterval, scaleColor) {
            this.MinValue = -30;
            this.MaxValue = 70;
            this.TicksInterval = 10;
            this.HighlightsDiv1 = 20;
            this.HighlightsDiv2 = 40;
            this.HighlightsDiv3 = 60;
            this.Sensor = sensor;
            this.Name = name;
            this.Temp = ko.observable(temp);
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.TicksInterval = ticksInterval;
            this.ScaleColor = scaleColor;
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
                var h1 = s.ScaleColor.FirstDivider / 100;
                var h2 = s.ScaleColor.SecondDivider / 100;
                var h3 = s.ScaleColor.ThirdDivider / 100;
                var fulScale = Math.abs(s.MinValue) + Math.abs(s.MaxValue);
                var tick = fulScale / s.TicksInterval;
                var localmajorTicks = new Array();
                var i;
                for (i = 0; i <= s.TicksInterval; i++) {
                    localmajorTicks.push(Math.round(s.MinValue + i * tick).toString());
                }
                s.Gauge = new Gauge({
                    renderTo: s.Sensor,
                    title: s.Name,
                    minValue: s.MinValue,
                    maxValue: s.MaxValue,
                    majorTicks: localmajorTicks,
                    units: "°C",
                    valueFormat: { int: 2, dec: 2 },
                    glow: true,
                    highlights: [{
                            from: s.MinValue,
                            to: s.MinValue + h1 * fulScale,
                            color: s.ScaleColor.FirstColor
                        }, {
                            from: s.MinValue + h1 * fulScale,
                            to: s.MinValue + h2 * fulScale,
                            color: s.ScaleColor.SecondColor
                        }, {
                            from: s.MinValue + h2 * fulScale,
                            to: s.MinValue + h3 * fulScale,
                            color: s.ScaleColor.ThirdColor
                        }, {
                            from: s.MinValue + h3 * fulScale,
                            to: s.MinValue + fulScale,
                            color: s.ScaleColor.LastColor
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
                    vm.sensors().forEach(function (s) { s.Gauge.setValue(s.MinValue); });
                    break;
                case 4:
                    vm.sensors().forEach(function (s) { s.Gauge.setValue(s.MaxValue); });
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
                return new TempSensorViewModel(item.Sensor, item.Name, item.Temp, item.MinValue, item.MaxValue, item.TicksInterval, item.ScaleColor);
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
