(function ($, require, apiUrl, sdUrl) {

    // Package-scoped variables
    var start_map, end_map, route_map, bridgeUtils;

	// Get the token from the cookie and add it to the headers.
	var UPP_TOKEN = document.cookie.match(new RegExp('(^| )uppToken=([^;]+)'))[2];

	$.ajaxSetup({
		xhrFields: {
		   withCredentials: true
		},
		crossDomain: true
	});

    // Generic error handler for deferreds
	function generic_error(err) {
	    if (typeof err !== 'string') {
	        err = JSON.stringify(err);
	    }
        console.log('generic_error', err);
        alert(err + '');
    }

    // static utility functions
    function get_service(filter) {
        var def = $.Deferred();
        var serviceLocator = sdUrl + "api/v1/services";

        $.get(serviceLocator, filter)
            .then(
                function (result) {
                    // Results are returned in priority order, so just take the first one.  Throw an error if no services are available
                    var records = result.data;
                    if (records.length === 0) {
                        def.reject('The locator did not return any matching services: ' + JSON.stringify(filter));
                    }
                    def.resolve(records[0]);
                },
                function (err) {
                    def.reject(err);
                }
            );

        return def.promise();
    }
    function get_services(filter) {
        var def = $.Deferred();
        var serviceLocator = sdUrl + "api/v1/services";

        $.get(serviceLocator, filter)
            .then(
                function (result) {
                    // Results are returned in priority order, so just take the first one.  Throw an error if no services are available
                    var records = result.data;
                    if (records.length === 0) {
                        def.reject('The locator did not return any matching services: ' + JSON.stringify(filter));
                    }
                    def.resolve(records);
                },
                function (err) {
                    def.reject(err);
                }
            );

        return def.promise();
    }
    function query_service(record) {
        // Default authorization header
        var headers = { 'Authorization': UPP_TOKEN };

        if (record.attributes.oAuthId === "rtvision" || record.attributes.tokenId === "rtvision") {
            // Extract the claims from the UPP_TOKEN
            var token_body = JSON.parse(atob(UPP_TOKEN.split(".")[1]));

            // Look for the RTVision: token in the tokens claim
            var prefix = 'RTVision:';

            var tokens = token_body.tokens.split(' ');
            $.each(tokens, function (idx, _) {
                if (_.startsWith(prefix)) {
                    headers['Authorization'] = _.substring(prefix.length);
                    return false;
                }
            });
        }

        return $.ajax({
            url: record.attributes.uri,
            type: 'GET',
            headers: headers
        });
    }

    function get_token(record) {
        var def = $.Deferred();

        // Check to see if the service is secured
        var attributes = (record && record.attributes) || {};
        if (attributes.oAuthId || attributes.tokenId) {
            var url = sdUrl + "api/v1/services/" + attributes.name + "/access";
            $.get(url, function (service) {
                if (!service.url) {
                    def.reject('No secure url is configured');
                    return;
                }

                if (service.url && service.isSecured && !service.token) {
                    def.reject('Unable to aquire token to access secured routing service');
                    return;
                }

                attributes.token = service.token;
                attributes.isSecured = service.isSecured;

                def.resolve(record);
            });
        }
        else {
            def.resolve(record);
        }

        return def.promise();
    }

    require({
        packages: [{
            "name": "scripts",
            "location": location.pathname.replace(/\/[^/]+$/, '') + "/js"
        }]
    }, [
        "esri/map",
        "esri/config",
        "scripts/override/dijit/Directions",
        "esri/dijit/Search",
        "esri/tasks/locator",
        "esri/tasks/query",
        "esri/tasks/QueryTask",
        "esri/tasks/RouteTask",
        "esri/tasks/RouteParameters",
        "esri/tasks/FeatureSet",
        "esri/tasks/GeometryService",
        "esri/geometry/webMercatorUtils",
        "esri/symbols/SimpleMarkerSymbol",
        "esri/symbols/SimpleLineSymbol",
        "esri/graphic",
		"esri/layers/GraphicsLayer",
        "dojo/_base/array",
		"dojo/_base/Deferred",
		"dojo/DeferredList",
		"dojo/dom-construct", 
		"dojo/query",
        "dojo/on",
		"scripts/bridges",
        "dojo/domReady!"
    ], function (Map, esriConfig, Directions, Search, Locator, Query, QueryTask, RouteTask, RouteParameters, FeatureSet, GeometryService, webMercatorUtils, SimpleMarkerSymbol, SimpleLineSymbol, Graphic, GraphicsLayer, array, Deferred, DeferredList, domConstruct, dojoQuery, on, Bridges) {

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

        // CORS-servers
        esriConfig.defaults.io.corsEnabledServers.push('tasks.arcgisonline.com');

        var locator = new Locator("https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer");

        var routeResult = null;

        // Ask UPP for the geometry service
        var geometryService = null;
        get_service({ type: 'geometry' })
            .then(function (data) {
                if (data && data.attributes) {
                    geometryService = new GeometryService(data.attributes.uri);
                }
            }, generic_error);

        // When the user submits the application, one of the first things to do
        // is use the designated geometry service to chop up the route among
        // the designated authorities
        function divide_route(route) {

            if (!route ) {
                alert("No route selected.");
                return;
            }

            get_authorities(route).then(function (result) {
                // Set the value of "Destination is within the applying County"
                // based on how many county authorities are returned.
                $("input[name='movementInfo.destinationWithinApplyingCounty'").prop('checked', result.length === 1);

                // Create a comma-separated list                
                var authorityList = array.map(result, function (_) { return _.name; });
                authorityList.sort();
                $("#permit-authorities").val(authorityList.join(", "));
            });
        };

        function get_authorities(route) {

            var def = new Deferred();

            var dl = new DeferredList([
                get_county_authorities(route),
                get_city_authorities(route)
            ]);

            dl.then(function (result) {
                var all = result[0][1].concat(result[1][1]);
                def.resolve(all);
            }, def.reject);

            return def.promise;
        }

        function get_county_authorities(route) {
            return _get_authorities(route, 'NAME', 'county.boundaries');
        }

        function get_city_authorities(route) {
            return _get_authorities(route, 'Name', 'city.boundaries');
        }

        function _get_authorities(route, fieldName, serviceType) {

            var def = new Deferred();

            // Get the county boundaries
            get_service({ type: serviceType })
                .then(function (service) {

                    // Intersect the route geometry with the service layer and get a collection of
                    // permit authorities
                    var queryTask = new QueryTask(service.attributes.uri);
                    var query = new Query();
                    query.geometry = route;
                    query.outFields = ["*"];
                    query.returnGeometry = true;
                    query.spatialRelationship = Query.SPATIAL_REL_INTERSECTS;

                    queryTask.execute(query,
                        function (result) {
                            def.resolve(array.map(result.features, function (feature) {
                                return {
                                    name: (feature.attributes[fieldName]+ (serviceType === 'city.boundaries' ? '_ci_mn' : '_co_mn')).toLowerCase(),
                                    geometry: feature.geometry
                                };
                            }));
                        }, function (err) {
                            console.log(err); def.reject(err);
                        }
                    );
                });

            return def.promise;
        };

        // Serialize the data from the permit so it can be sent to the Permit Authorities.
        function serializeForm(form){
            var o = {};
            var a = form.serializeArray();
            $.each(a, function() {
                if (o[this.name]) {
                    if (!o[this.name].push) {
                        o[this.name] = [o[this.name]];
                    }
                    o[this.name].push(this.value || '');
                } else {
                    o[this.name] = this.value || '';
                }
            });

            // Add in the barriers and stops from the map
            // o.route = JSON.stringify({ "stops": directions.stops, "barriers": bridgeUtils.barriers });

            // Add in any extra datasets that have been queried, e.g. bridges, restrictions, etc.
            return o;
        };

        function check_directions_route(){
            directions.routeParams.polygonBarriers = new FeatureSet();
            bridgeUtils.clearBarriers();
            $('.esriDirectionsButton.esriStopsGetDirections').click();			
        };
		
        route_map = new Map('route-map', {
            basemap: "topo",
            center: [-94.8858, 47.4875],  // Bemidji 47.4875° N, 94.8858° W
            zoom: 10
        });
		
        bridgeUtils = new Bridges(route_map, $("#permit-form"));
		
        function submit_permit(authority, record){
            get_services({ authority: authority })
            .then(function (services) { 
                if(services ){
                    // Submit the permit request to the permit authorities.
                    array.forEach(services, function (data) {
                        $.post(data.attributes.uri + "api/v1/issue", record.data)
                    });
                }
            });
            
        }

        function create_permit() {
            return $.post("api/permits", {});
        }

        function _create_permit() {
            console.log("create_permit", arguments);
            $('#submitModalMessage').text('Creating new permit application bundle...');

            var def = $.Deferred();
            setTimeout(function () {
                $('#submitModalProgress').css('width', '20%');
                def.resolve({ permit: {} });
            }, 1000);
            return def.promise();
        }

        function _patch_permit(permit, section, payload) {

            // Serialize the data payload
            var form = JSON.stringify(payload);

            // POST the JSON data to the permit endpoint
            var url = permit.data.links.self + "/patch?section=" + section;            
            return $.post(url, form);
        }

        function add_form_data(record) {
            console.log("add_form_data", arguments);
            $('#submitModalMessage').text('Adding form data...');

            var def = $.Deferred();

            try
            {
                var data = serializeForm($("#permit-form"));
                _patch_permit(record, 'form-data', data).then(function () {
                    def.resolve(record, data);
                    $('#submitModalProgress').css('width', '50%');
                }, def.reject);
            }
            catch (e)
            {
                def.reject(e);
            }

            return def.promise();
        }

        function add_route_data(record) {

            var def = $.Deferred();

            $('#submitModalMessage').text('Adding route data...');
            console.log("add_route_data", arguments);

            // Check that a route has been creates
            var directionsData = directions.directions;
			
            if (!directionsData) {
                def.reject('No route has been created');
                return def.promise();
            }

            try {
                // Get the current direction geometry, the stops and barriers
                var routeGeometry = directionsData.mergedGeometry;
                var routeStops = directions.stops;
                var routeBarriers = bridgeUtils.barriers;

                // Update the permit application with the route information
                var data = {
                    geometry: routeGeometry,
                    stops: routeStops,
                    barriers: routeBarriers
                };

                _patch_permit(record, 'route', data).then(function () {
                    def.resolve(record, data);
                    $('#submitModalProgress').css('width', '66%');
                }, def.reject);
            }
            catch (e) {
                def.reject(e);
            }

            return def.promise();
        }

        function add_authority_data(record) {

            var def = $.Deferred();

            $('#submitModalMessage').text('Identifying permit authorities...');
            console.log("add_authority_data", arguments);

            // Check that a route has been creates
            var directionsData = directions.directions;
            if (!directionsData) {
                def.reject('No route has been created');
                return def.promise();
            }

            // Get the current direction geometry
            var routeGeometry = directionsData.mergedGeometry;

            // Find all of the authorities that intersect this route
            get_authorities(routeGeometry).then(function (authorities) {

                console.log(authorities);

                // Update the permit with the list of authorities (names-as-keys only)
                var authority_set = {};
                array.forEach(authorities, function(authority) {
                    authority_set[authority.name] = {};
                });

                // Get the geometry for each authoity and add those geometries to the authority's 
                // specific recrod.
                var authorityGeometries = array.map(authorities, function (authority) {
                    return authority.geometry;
                });

                // Perform an intersection and post to ?authority=<value>
                geometryService.intersect(authorityGeometries, routeGeometry).then(function (geometries) {
                    for (var i = 0; i < geometries.length; i++) {
                        authority_set[authorities[i].name].route = geometries[i];
                    }

                    _patch_permit(record, 'authorities', authority_set).then(function () {
                        def.resolve(record)
                    }, def.reject);
                }, def.reject);
            }, def.reject);

            // Identify the authorities along the route
            return def.promise();
        }

        function add_bridge_data(record) {
            console.log("add_bridge_data", arguments);
            $('#submitModalMessage').text('Adding bridge data...');

            var def = $.Deferred();
            setTimeout(function () {
                $('#submitModalProgress').css('width', '100%');
                def.resolve(record);
            }, 1000);
            return def.promise();
        }

        function permit_success(record) {
			console.log("permit_success", record);
            // We are done, set the progress to 100%
            $('#submitModalMessage').text('Application Complete');
			submit_permit("upp.permit", record);
        }		

        function permit_failure(err) {
            // Somthing happened
            $('#submitModalProgress').addClass('progress-bar-danger').css('width', '100%');
            $('#submitModalMessage').html("<span class='text-danger'>" + err + "</span>");
        }

        function get_route() {
        }

        $('#submitModal').on('show.bs.modal', function (e) {

            // Set the progress to zero
            $('#submitModalProgress').removeClass('progress-bar-danger').css('width', '0%');
            $('#submitModalMessage').text('');
        });

        // When the user opens up the dialog, start processing
        $('#submitModal').on('shown.bs.modal', function (e) {

            // Try to create the new permit bundle here (this server is the origin of the request)
            create_permit()
                .then(add_form_data)
                .then(add_route_data)
                .then(add_authority_data)
                .then(add_bridge_data)
                .then(permit_success, permit_failure)
            ;
        });

        // When the user closes the dialog
        $('#submitModal').on('hide.bs.modal', function (e) {
            console.log('close');
        });

        $("#request-permit").click(function (evt) {

            /*
			dojoQuery("input[name=Authority]").forEach(domConstruct.destroy);
			var auths = $("#permit-authorities").val().split(',');
			var form = $("#permit-form");
			
			create_permit();
			return;

			var dl = new DeferredList(array.map(auths, function (auth) {
			    return submit_permit(auth);
			}));
			dl.then(function(result){
				form.submit();
			});
            */
        }); 

        $("#toggle-barriers").click(function (evt) {
            bridgeUtils.toggleBarriers();
        });
		
        function UpdateSummaries() { 
            var h = parseInt($('#truckInfo\\.height').val(), 10);
            var w = parseInt($('#truckInfo\\.width').val(), 10);
            var l = parseInt($('#truckInfo\\.length').val(), 10);
            var of = parseInt($('#truckInfo\\.frontOverhang').val(), 10) / 12;
            var or = parseInt($('#truckInfo\\.rearOverhang').val(), 10) / 12;
            var ol = parseInt($('#truckInfo\\.leftOverhang').val(), 10) / 12;
            var ort = parseInt($('#truckInfo\\.rightOverhang').val(), 10) / 12;

            $('#truckInfo\\.dimensionSummary').val(h + ' / ' + w + ' / ' + l);
            $('#truckInfo\\.dimensionDescription').val('Len: ' + (l + of + or) + ' Wid: ' + (w + ort + ol));

            check_directions_route();
        }

        $('#truckInfo\\.height').change(UpdateSummaries);
        $('#truckInfo\\.width').change(UpdateSummaries);
        $('#truckInfo\\.length').change(UpdateSummaries);
        //$('#truckinfo-front-overhang').change(UpdateSummaries);
        //$('#truckinfo-rear-overhang').change(UpdateSummaries);
        $('#truckInfo\\.leftOverhang').change(UpdateSummaries);
        $('#truckinfo\\.rightOverhang').change(UpdateSummaries);
        $('#truckInfo\\.grossWeight').change(function (evt) { check_directions_route(); });

        // Initialize the direction widget and routing services
        var directions;

        get_service({ type: 'route' })
        .then(get_token)
        .then(function (service) {
            directions = new Directions({
                map: route_map,
                routeTaskUrl: service.attributes.uri + "?token=" + service.attributes.token,
                showClearButton: true,
                showTrafficOption: false,
                showPrintPage: false
            }, "route-directions");

            directions.startup();

            directions.on('directions-finish', function (rr) {

                if (rr.result.routeResults) {
                    divide_route(rr.result.routeResults[0].directions.mergedGeometry);
                    bridgeUtils.forRoute(rr.result.routeResults[0], sdUrl).then(function (bridges) {

                        bridgeUtils.addToMap(bridges);
                        bridgeUtils.addToForm(bridges);
                        directions.routeParams.polygonBarriers = new FeatureSet();
                        array.forEach(bridgeUtils.barriers, function (barrier) {
                            directions.routeParams.polygonBarriers.features.push(new Graphic(barrier, null, {}));
                        });

                    });

                    // Fill in the total miles traveled and the route description
                    var firstRoute = rr.result.routeResults[0];
                    if (firstRoute) {
                        $("#movementInfo\\.routeDescription").val(firstRoute.directions.routeName);
                        $("#movementInfo\\.routeLength").val(firstRoute.directions.summary.totalLength);

                        var stops = firstRoute.stops;
                        $("#movementInfo\\.origin").val(stops[0].attributes["Name"]);
                        $("#movementInfo\\.destination").val(stops[stops.length - 1].attributes["Name"]);
                    }
                }
            });

            directions.on('directions-clear', function (cr) {
                bridgeUtils.clearBarriers();
                bridgeUtils.clearBridges();
            });

            directions.setTravelMode('Trucking Time');
        }, generic_error);
    });

    //#region Initialization Code

    // Ask the service locator to give us a UPP host that can provide company information.    
    get_service({ type: "upp.information.company" })
        .then(query_service)
        .then(function (companies) {
            var select = $("#company-selector");

            $.each(companies.data, function (index, company) {
                var opt = $('<option>' + company.attributes.companyName + '</option>');
                opt.data("company", company);
                select.append(opt);
            });

            // Set an event handler to populate the company form on select change
            select.change(function (evt) {
                update_company_info(select.find(":selected").data("company"));
            });
        }, generic_error);    

    // Ask the service locator to give us a UPP host that can provide insurance information.
    get_service({ type: "upp.information.insurance" })
        .then(query_service)
        .then(function (companies) {
            var select = $("#insurer-selector");

            $.each(companies.data, function (index, company) {
                var opt = $('<option>' + company.attributes.providerName + '</option>');
                opt.data("company", company);

                select.append(opt);
            });

            // Set an event handler to populate the insurance form on select change
            select.change(function (evt) {
                update_insurance_info(select.find(":selected").data("company"));
            });
        }, generic_error);


    // Ask the service locator to give us a UPP host that can provide insurance information.
    get_service({ type: "upp.information.vehicle" })
        .then(query_service)
        .then(function (vehicles) {
            var select = $("#vehicle-selector");

            $.each(vehicles.data, function (index, vehicle) {
                var attrs = vehicle.attributes;
                var opt = $('<option>' + attrs.make + ' / ' + attrs.model + ' (' + attrs.usdotNumber + ')</option>');
                opt.data("vehicle", vehicle);

                select.append(opt);
            });

            // Set an event handler to populate the vehicle form on select change
            select.change(function (evt) {
                var opt = select.find(":selected");
                var data = opt.data("vehicle");

                if (data) {
                    update_vehicle_info(data);

                    // Ask the service locator to give us a UPP host that can provide truck information.
                    get_service({ type: "upp.information.truck" })
                        .then(query_service)
                        .then(function (trucks) {
                            var index = $("#vehicle-selector").children('option:selected').index() - 1;
                            if (trucks.data && trucks.data[index]) {
                                update_truck_info(trucks.data[index]);
                            }
                        });
                }
            });
        }, generic_error);

    // Ask the service locator to give us a UPP host that can provide insurance information.
    get_service({ type: "upp.information.trailer" })
        .then(query_service)
        .then(function (trailers) {
            var select = $("#trailer-selector");
            $.each(trailers.data, function (index, trailer) {
                var opt = $('<option>' + trailer.attributes.description + '</option>');
                opt.data("trailer", trailer);

                select.append(opt);
            });

            // Set an event handler to populate the company form on select change
            select.change(function (evt) {
                update_trailer_info(select.find(":selected").data("trailer"));
            });
        }, generic_error);

    //#endregion Initialization Code

    // #region Form Update Helpers

    function update_company_info(data) {
        if (data && data.attributes) {
            data = data.attributes;
            $('#companyInfo\\.name').val(data.companyName);
            $('#companyInfo\\.email').val(data.email);
            $('#companyInfo\\.contact').val(data.contact);
            $('#companyInfo\\.phone').val(data.phone);
            $('#companyInfo\\.fax').val(data.fax);
            $('#companyInfo\\.address').val(data.billingAddress);
        }
    }

    function update_insurance_info(data) {
        if (data && data.attributes) {
            data = data.attributes;
            $('#insuranceInfo\\.provider').val(data.providerName);
            $('#insuranceInfo\\.agencyAddress').val(data.agencyAddress);
            $('#insuranceInfo\\.policyNumber').val(data.policyNumber);
        }
    }

    function update_vehicle_info(data) {
        if (data && data.attributes) {
            data = data.attributes;
            $('#vehicleInfo\\.make').val(data.make);
            $('#vehicleInfo\\.type').val(data.type);
            $('#vehicleInfo\\.license').val(data.license);
            $('#vehicleInfo\\.state').val(data.state);
            $('#vehicleInfo\\.serialNumber').val(data.serialNumber);
            $('#vehicleInfo\\.USDOTNumber').val(data.usdotNumber);
            $('#vehicleInfo\\.emptyWeight').val(data.emptyWeight);
        }
    }

    function update_truck_info(data) {
        if (data && data.attributes) {
            data = data.attributes;
            $('#truckInfo\\.grossWeight').val(data.grossWeight);
            $('#truckInfo\\.dimensionSummary').val(data.height + ' / ' + data.width + ' / ' + data.length);
            $('#truckInfo\\.dimensionDescription').val('Len: ' + (data.length + data.frontOverhang + data.rearOverhang) + ' Wid: ' + (data.width + data.rightOverhang + data.leftOverhang));
            $('#truckInfo\\.height').val(data.height);
            $('#truckInfo\\.width').val(data.width);
            $('#truckInfo\\.length').val(data.length);
            $('#truckInfo\\.frontOverhang').val(data.frontOverhang);
            $('#truckInfo\\.rearOverhang').val(data.rearOverhang);
            $('#truckInfo\\.leftOverhang').val(data.leftOverhang);
            $('#truckInfo\\.rightOverhang').val(data.rightOverhang).change();  // Fire a change event to update the bridges
            $('#truckInfo\\.diagram').val(data.diagram);
        }
    }

    function update_trailer_info(data) {
        if (data && data.attributes) {
            data = data.attributes;
            $('#trailerInfo\\.description').val(data.description);
            $('#trailerInfo\\.make').val(data.make);

            $('#trailerInfo\\.type').val(data.type);
            $('#trailerInfo\\.serialNumber').val(data.serialNumber);
            $('#trailerInfo\\.licenseNumber').val(data.license);
            $('#trailerInfo\\.state').val(data.state);
            $('#trailerInfo\\.emptyWeight').val(data.emptyWeight);
        }
    }

    // #endregion
})(
    jQuery,
    require,
    (typeof apiUrl === 'undefined') ? '/manager/' : apiUrl,
    (typeof sdUrl === 'undefined') ? '/sd/' : sdUrl
);
