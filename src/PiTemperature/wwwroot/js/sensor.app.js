/// <reference path="typings/jquery.d.ts" />
/// <reference path="typings/knockout.d.ts" />
$(function () {
    var ScaleColorClass = (function () {
        function ScaleColorClass() {
        }
        return ScaleColorClass;
    })();
    var TempsensorClass = (function () {
        function TempsensorClass() {
            this.ScaleColor = new ScaleColorClass();
        }
        return TempsensorClass;
    })();
    var SensorViewModel = (function () {
        function SensorViewModel(sensor, name, minValue, maxValue, ticksInterval, scaleColor) {
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
        return SensorViewModel;
    })();
    var SensorListViewModel = (function () {
        function SensorListViewModel() {
            this.items = ko.observableArray([]);
            this.editItem = ko.observable(null);
            this.itemCount = ko.computed({
                owner: this,
                read: function () {
                    if (typeof this.items === "undefined")
                        return "";
                    switch (this.items().length) {
                        case 0:
                            return "There no temp sensors atached";
                        default:
                            return "There are " + this.items().length + " temp sensor(s) atached";
                    }
                }
            });
            this.getAll();
            //$('#editItem').on('shown.bs.modal', function (e) {
            //    var xx: any = $('#FirstColorPicker');
            //    xx.colorpicker();
            //})       
        }
        SensorListViewModel.prototype.getAll = function () {
            $.ajax({
                context: this,
                type: "get",
                url: "/api/Sensor",
                success: function (list) {
                    var mapedSensors = $.map(list, function (item) {
                        return new SensorViewModel(item.Sensor, item.Name, item.MinValue, item.MaxValue, item.TicksInterval, item.ScaleColor);
                    });
                    vm.items(mapedSensors);
                }
            });
        };
        SensorListViewModel.prototype.getItem = function (item) {
            $.ajax({
                context: this,
                dataType: "json",
                url: "/api/Sensor/" + item.Sensor,
                data: null,
                success: function (data) {
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
        };
        SensorListViewModel.prototype.saveItem = function (item) {
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
        };
        SensorListViewModel.prototype.convertItem = function (item) {
            var ret = new TempsensorClass();
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
        };
        SensorListViewModel.prototype.deleteItem = function (item) {
            $.ajax({
                context: this,
                type: 'DELETE',
                contentType: "application/json",
                dataType: "json",
                url: '/api/Sensor/' + item.Sensor,
                data: ko.toJSON(item),
                success: function (data) {
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
        };
        return SensorListViewModel;
    })();
    var vm = new SensorListViewModel();
    ko.applyBindings(vm);
    $('#editItem').on('shown.bs.modal', function (e) {
        var xx = $('#FirstColorPicker');
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
    });
});
