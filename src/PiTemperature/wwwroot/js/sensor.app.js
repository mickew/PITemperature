/// <reference path="typings/jquery.d.ts" />
/// <reference path="typings/knockout.d.ts" />
$(function () {
    var SensorViewModel = (function () {
        function SensorViewModel(sensor, name) {
            this.Sensor = sensor;
            this.Name = ko.observable(name);
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
        }
        SensorListViewModel.prototype.getAll = function () {
            $.ajax({
                context: this,
                type: "get",
                url: "/api/Sensor",
                success: function (list) {
                    var mapedSensors = $.map(list, function (item) {
                        return new SensorViewModel(item.Sensor, item.Name);
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
                data: ko.toJSON(item),
                success: function (d) {
                    //alert("Ok");
                }
            });
        };
        return SensorListViewModel;
    })();
    var vm = new SensorListViewModel();
    ko.applyBindings(vm);
});
