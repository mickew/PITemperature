/// <reference path="typings/jquery.d.ts" />
/// <reference path="typings/knockout.d.ts" />

$(function () {
    interface ScaleColor {
        FirstColor: string;
        FirstDivider: number;
        SecondColor: string;
        SecondDivider: number;
        ThirdColor: string;
        ThirdDivider: number;
        LastColor: string;
    }
    interface TempSensorBase {
        Sensor: string;
        Name: string;
        MinValue: number;
        MaxValue: number;
        TicksInterval: number;
        ScaleColor: ScaleColor;
    }

    class ScaleColorClass implements ScaleColor {
        FirstColor: string;
        FirstDivider: number;
        SecondColor: string;
        SecondDivider: number;
        ThirdColor: string;
        ThirdDivider: number;
        LastColor: string;
    }
    class TempsensorClass implements TempSensorBase {
        Sensor: string;
        Name: string;
        MinValue: number;
        MaxValue: number;
        TicksInterval: number;
        ScaleColor: ScaleColor;
        constructor() {
            this.ScaleColor = new ScaleColorClass();
        }
    }

    class SensorViewModel {
        public Sensor: string;
        public Name: KnockoutObservable<string>;
        public MinValue: KnockoutObservable<number>;
        public MaxValue: KnockoutObservable<number>;
        public TicksInterval: KnockoutObservable<number>;

        public FirstColor: KnockoutObservable<string>;
        public FirstDivider: KnockoutObservable<number>;
        public SecondColor: KnockoutObservable<string>;
        public SecondDivider: KnockoutObservable<number>;
        public ThirdColor: KnockoutObservable<string>;
        public ThirdDivider: KnockoutObservable<number>;
        public LastColor: KnockoutObservable<string>;
        constructor(sensor: string, name: string, minValue: number, maxValue: number, ticksInterval: number, scaleColor: ScaleColor) {
            this.Sensor = sensor;
            this.Name = ko.observable(name);
            this.MinValue = ko.observable(minValue);
            this.MaxValue = ko.observable(maxValue);
            this.TicksInterval = ko.observable(ticksInterval);
            this.FirstColor = ko.observable(scaleColor.FirstColor);
            this.FirstDivider = ko.observable(scaleColor.FirstDivider);
            this.SecondColor = ko.observable(scaleColor.SecondColor);
            this.SecondDivider = ko.observable(scaleColor.SecondDivider);
            this.ThirdColor = ko.observable(scaleColor.ThirdColor);
            this.ThirdDivider = ko.observable(scaleColor.ThirdDivider);
            this.LastColor = ko.observable(scaleColor.LastColor);
        }
    }

    class SensorListViewModel {
        public items: KnockoutObservableArray<SensorViewModel> = ko.observableArray([]);
        editItem: KnockoutObservable<SensorViewModel> = ko.observable(null);
        itemCount: KnockoutComputed<string> = ko.computed(
            {
                owner: this,
                read: function () {
                    if (typeof this.items === "undefined") return "";
                    switch (this.items().length) {
                        case 0:
                            return "There no temp sensors atached";
                        default:
                            return "There are " + this.items().length + " temp sensor(s) atached";
                    }
                }
            });

        constructor() {
            this.getAll();
            //$('#editItem').on('shown.bs.modal', function (e) {
            //    var xx: any = $('#FirstColorPicker');
            //    xx.colorpicker();
            //})       
        }

        getAll() {
            $.ajax({
                context: this,
                type: "get",
                url: "/api/Sensor",
                success: function (list: Array<TempSensorBase>) {
                    var mapedSensors = $.map(list, function (item) {
                        return new SensorViewModel(item.Sensor, item.Name, item.MinValue, item.MaxValue, item.TicksInterval, item.ScaleColor)
                    });
                    vm.items(mapedSensors);
                }
            });
        }

        getItem(item: SensorViewModel) {
            $.ajax({
                context: this,
                dataType: "json",
                url: "/api/Sensor/" + item.Sensor,
                data: null,
                success: function (data: TempSensorBase) {
                    item.Name(data.Name);
                    item.MinValue(data.MinValue);
                    item.MaxValue(data.MaxValue);
                    item.TicksInterval(data.TicksInterval);
                    item.FirstColor(data.ScaleColor.FirstColor);
                    item.FirstDivider(data.ScaleColor.FirstDivider);
                    item.SecondColor(data.ScaleColor.SecondColor);
                    item.SecondDivider(data.ScaleColor.SecondDivider);
                    item.ThirdColor(data.ScaleColor.ThirdColor);
                    item.ThirdDivider(data.ScaleColor.ThirdDivider);
                    item.LastColor(data.ScaleColor.LastColor);
                }
            });
        }

        saveItem(item: SensorViewModel) {
            $.ajax({
                context: this,
                type: 'PUT',
                contentType: "application/json",
                dataType: "json",
                url: '/api/Sensor/' + item.Sensor,
                data: this.convertItem(item),
                success: function (d) {
                    //alert("Ok");
                }
            });
        }

        convertItem(item: SensorViewModel) {
            var ret: TempsensorClass = new TempsensorClass();
            ret.Sensor = item.Sensor;
            ret.Name = item.Name();
            ret.MinValue = item.MinValue();
            ret.MaxValue = item.MaxValue();
            ret.TicksInterval = item.TicksInterval();
            ret.ScaleColor.FirstColor = item.FirstColor();
            ret.ScaleColor.FirstDivider = item.FirstDivider();
            ret.ScaleColor.SecondColor = item.SecondColor();
            ret.ScaleColor.SecondDivider = item.SecondDivider();
            ret.ScaleColor.ThirdColor = item.ThirdColor();
            ret.ScaleColor.ThirdDivider = item.ThirdDivider();
            ret.ScaleColor.LastColor = item.LastColor();
            return ko.toJSON(ret);
        }

        deleteItem(item: SensorViewModel) {
            $.ajax({
                context: this,
                type: 'DELETE',
                contentType: "application/json",
                dataType: "json",
                url: '/api/Sensor/' + item.Sensor,
                data: ko.toJSON(item),
                success: function (data: TempSensorBase) {
                    item.Name(data.Name);
                    item.MinValue(data.MinValue);
                    item.MaxValue(data.MaxValue);
                    item.TicksInterval(data.TicksInterval);
                    item.FirstColor(data.ScaleColor.FirstColor);
                    item.FirstDivider(data.ScaleColor.FirstDivider);
                    item.SecondColor(data.ScaleColor.SecondColor);
                    item.SecondDivider(data.ScaleColor.SecondDivider);
                    item.ThirdColor(data.ScaleColor.ThirdColor);
                    item.ThirdDivider(data.ScaleColor.ThirdDivider);
                    item.LastColor(data.ScaleColor.LastColor);
                }
            });
        }

     }

    var vm = new SensorListViewModel();
    ko.applyBindings(vm);
    $('#editItem').on('shown.bs.modal', function (e) {
        var xx: any = $('#FirstColorPicker');
        xx.colorpicker().on('changeColor', function (event) {
            $('#FirstColorId').change();
        });
        xx = $('#SecondColorPicker');
        xx.colorpicker().on('changeColor', function (event) {
            $('#SecondColorId').change();
        });
        xx = $('#ThirdColorPicker');
        xx.colorpicker().on('changeColor', function (event) {
            $('#ThirdColorId').change();
        });
        xx = $('#LastColorPicker');
        xx.colorpicker().on('changeColor', function (event) {
            $('#LastColorId').change();
        });
    })       
});