(function($, require, apiUrl, sdUrl) {
    var start_map, end_map, route_map;

    require([
        "esri/map",
        "esri/tasks/locator",
        "esri/tasks/RouteTask",
        "esri/tasks/RouteParameters",
        "esri/tasks/FeatureSet",
        "esri/geometry/webMercatorUtils",
        "esri/symbols/SimpleMarkerSymbol",
        "esri/symbols/SimpleLineSymbol",
        "esri/graphic",
        "dojo/_base/array",
        "dojo/on",
        "dojo/domReady!"
    ], function (Map, Locator, RouteTask, RouteParameters, FeatureSet, webMercatorUtils, SimpleMarkerSymbol, SimpleLineSymbol, Graphic, array, on) {
        var symbol = new SimpleMarkerSymbol({
            "color": [255, 255, 255, 64],
            "size": 12,
            "angle": 0,
            "xoffset": 0,
            "yoffset": 0,
            "type": "esriSMS",
            "style": "esriSMSCircle",
            "outline": {
                "color": [0, 0, 0, 255],
                "width": 1,
                "type": "esriSLS",
                "style": "esriSLSSolid"
            }
        });

        var locator = new Locator("https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer");

        function init_map(map_id, map_ref) {
            if (!map_ref) {
                map_ref = new Map(map_id, {
                    basemap: "topo",
                    center: [-122.45, 37.75],
                    zoom: 13
                });

                on(map_ref, 'click', function (evt) {
                    map_ref.graphics.clear();
                    map_ref.graphics.add(new Graphic(evt.mapPoint, symbol));
                });
            }

            return map_ref;
        };

        function find_address(map, el) {
            var point = map.graphics.graphics[0].geometry;
            webPoint = webMercatorUtils.webMercatorToGeographic(point);

            locator.locationToAddress(webPoint, 100).then(function (result) {
                $(el).val(result.address.Match_addr);
                $(el).data('location', point);

                check_route();
            });
        };

        // When the user submits the application, one of the first things to do
        // is use the designated geometry service to chop up the route among
        // the designated authorities
        function divide_route(route) {

            // Ask UPP for the geometry service
            var url = apiUrl + "api/hauler";
            $.get(url, function (data) {
                if (!data.boundaryServiceUrl) {
                    alert('No boundary service is configured');
                    return;
                }

                // Intersect the route geometry with the service layer and get a collection of
                // permit authorities

            });
        };

        // If both origin and destination have locations, route it
        function check_route() {
            var origin_pt = $('#movementinfo-origin').data('location');
            var destination_pt = $('#movementinfo-destination').data('location');

            if (origin_pt && destination_pt) {

                // Ask UPP what service we should use for routing.  The UPP services API is responsible for acquiring any
                // OAuth tokens that are needed for the service
                var serviceLocator = sdUrl + "api/v1/hosts?type=route";
                $.get(serviceLocator, function (records) {
                    // Results are returned in priority order, so just take the first one.  Throw an error if no services are available
                    if (records.length === 0) {
                        alert('The service locator did not return any services for routing');
                        return;
                    }

                    var record = records[0];

                    // Now ask for credentials
                    var url = sdUrl + "api/v1/hosts/" + record.name + "/access";
                    $.get(url, function (service) {
                        if (!service.url) {
                            alert('No route service access is configured');
                            return;
                        }

                        if (service.url && service.isSecured && !service.token) {
                            alert('Unable to aquire token to access secured routing service');
                            return;
                        }

                        var routeTask = new RouteTask(service.url + "?token=" + service.token);
                        var routeParams = new RouteParameters();
                        routeParams.stops = new FeatureSet();
                        routeParams.stops.features.push(new Graphic(origin_pt, null, {}));
                        routeParams.stops.features.push(new Graphic(destination_pt, null, {}));
                        routeTask.solve(routeParams).then(function (result) {
                            // Show the route on the route map
                            var routeSymbol = new SimpleLineSymbol().setColor(new dojo.Color([0, 0, 255, 0.5])).setWidth(5);
                            array.forEach(result.routeResults, function (result) {
                                route_map.graphics.add(result.route.setSymbol(routeSymbol));
                                route_map.setExtent(result.route.geometry.getExtent().expand(1.5));
                            });
                        });
                    });
                });
            }
        };

        $('#startLocationModal').on('shown.bs.modal', function () {
            start_map = init_map('start-map', start_map);
        });

        $('#startLocationModal').on('hidden.bs.modal', function () {
            find_address(start_map, '#movementinfo-origin');
        });

        $('#endLocationModal').on('shown.bs.modal', function () {
            end_map = init_map('end-map', end_map);
        });

        $('#endLocationModal').on('hidden.bs.modal', function () {
            find_address(end_map, '#movementinfo-destination');
        });

        route_map = new Map('route-map', {
            basemap: "topo",
            center: [-122.45, 37.75],
            zoom: 12
        });
    });

    // Ask the service locator to give us a UPP host that can provide company information.
    var serviceLocator = sdUrl + "api/v1/hosts?type=upp&scope=information.company";
    var select = $("#company-selector");

    $.get(serviceLocator, function (data) {
        // Results are returned in priority order, so just take the first one.  Throw an error if no services are available
        if (data.length === 0) {
            alert('The service locator did not return any UPP services for company information');
            return;
        }

        // Got at least one provider, so use them to populate the company information drop-down
        var service = data[0];
        $.get(service.uri, function (companies) {
            $.each(companies, function (index, company) {
                var opt = $('<option>' + company.companyName + '</option>');
                opt.data("company", company);

                select.append(opt);
            });
        });
    });

    // Set an event handler to populate the company form on select change
    select.change(function (evt) {
        var opt = select.find(":selected");
        var data = opt.data("company");
        if (data) {
            $('#companyinfo-name').val(data.companyName);
            $('#companyinfo-email').val(data.email);
            $('#companyinfo-contact').val(data.contact);
            $('#companyinfo-phone').val(data.phone);
            $('#companyinfo-fax').val(data.fax);
            $('#companyinfo-cell').val(data.cell);
            $('#companyinfo-bill-to').val(data.billTo);
            $('#companyinfo-billing-address').val(data.billingAddress);
        }
    });
})(
    jQuery,
    require,
    (typeof apiUrl === 'undefined') ? '/manager/' : apiUrl,
    (typeof sdUrl === 'undefined') ? '/sd/' : sdUrl
);
