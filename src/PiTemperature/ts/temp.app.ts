/// <reference path="typings/Hubs.d.ts" />
/// <reference path="typings/knockout.d.ts" />
/// <reference path="typings/gauge.d.ts" />

interface External {
    notify: Function;
}

function windowExternal(arg: string) {
    //if (window.external.notify != undefined)
        window.external.notify(arg);
}

$(function () {
    class TempSensorViewModel {
        public Sensor: string;
        public Name: string;
        public MinValue: number = -30;
        public MaxValue: number = 70;
        public TicksInterval: number = 10;
        public HighlightsDiv1: number = 20;
        public HighlightsDiv2: number = 40;
        public HighlightsDiv3: number = 60;
        public ScaleColor: ScaleColor;
        Temp: KnockoutObservable<number>;
        Gauge: Gauge;
        constructor(sensor: string, name: string, temp: number, minValue: number, maxValue: number, ticksInterval: number, scaleColor: ScaleColor) {
            this.Sensor = sensor;
            this.Name = name;
            this.Temp = ko.observable(temp);
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.TicksInterval = ticksInterval;
            this.ScaleColor = scaleColor;
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
                var h1: number = s.ScaleColor.FirstDivider / 100;
                var h2: number = s.ScaleColor.SecondDivider / 100;
                var h3: number = s.ScaleColor.ThirdDivider / 100;
                var fulScale: number = Math.abs(s.MinValue) + Math.abs(s.MaxValue);
                var tick: number = fulScale / s.TicksInterval;
                var localmajorTicks: Array<string> = new Array();

                var i: number;
                for (i = 0; i <= s.TicksInterval; i++) {
                    localmajorTicks.push(Math.round(s.MinValue + i * tick).toString());
                }

                s.Gauge = new Gauge(
                    {
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
        }

        connectionStateChanged(state: SignalRStateChange) {
            vm.connectionState(state.newState);
            switch (state.newState) {
                case 0:
                    vm.sensors().forEach(s => { s.Gauge.setValue(s.MinValue) });
                    break;
                case 4:
                    vm.sensors().forEach(s => { s.Gauge.setValue(s.MaxValue) });
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

        broadcastTemperature(tempSensor: TempAndSensor) {
            var thesensor = ko.utils.arrayFirst(vm.sensors(), function (item) {
                return item.Sensor === tempSensor.Sensor;
            });
            thesensor.Temp(tempSensor.Temp);
            thesensor.Gauge.setValue(tempSensor.Temp);
        }

        broadcastTempSensors(list: Array<TempSensor>) {
            var mapedSensors = $.map(list, function (item) {
                return new TempSensorViewModel(item.Sensor, item.Name, item.Temp, item.MinValue, item.MaxValue, item.TicksInterval, item.ScaleColor)
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