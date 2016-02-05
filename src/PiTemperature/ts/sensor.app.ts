/// <reference path="typings/jquery.d.ts" />
/// <reference path="typings/knockout.d.ts" />

$(function () {
    interface TempSensorBase {
        Sensor: string;
        Name: string;
    }

    class SensorViewModel {
        public Sensor: string;
        public Name: KnockoutObservable<string>;
        constructor(sensor: string, name: string) {
            this.Sensor = sensor;
            this.Name = ko.observable(name);
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
        }

        getAll() {
            $.ajax({
                context: this,
                type: "get",
                url: "/api/Sensor",
                success: function (list: Array<TempSensorBase>) {
                    var mapedSensors = $.map(list, function (item) {
                        return new SensorViewModel(item.Sensor, item.Name)
                    });
                    vm.items(mapedSensors);
                }
            });
        }

        getRefresh() {
            $.ajax({
                context: this,
                type: "get",
                url: "/api/Sensor/Refresh",
                success: function (list: Array<TempSensorBase>) {
                    var mapedSensors = $.map(list, function (item) {
                        return new SensorViewModel(item.Sensor, item.Name)
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
                data: ko.toJSON(item),
                success: function (d) {
                    //alert("Ok");
                }
            });
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
                }
            });
        }

     }



    var vm = new SensorListViewModel();
    ko.applyBindings(vm);
});