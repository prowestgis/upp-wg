(function($, require, apiUrl, sdUrl) {
    var start_map, end_map, route_map, bridgeUtils;

    require({
        packages: [{
            "name": "scripts",
            "location": location.pathname.replace(/\/[^/]+$/, '') + "/js"
        }]
    }, [
        "esri/map",
        "esri/dijit/Search",
        "esri/tasks/locator",
        "esri/tasks/query",
        "esri/tasks/QueryTask",
        "esri/tasks/RouteTask",
        "esri/tasks/RouteParameters",
        "esri/tasks/FeatureSet",
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
    ], function (Map, Search, Locator, Query, QueryTask, RouteTask, RouteParameters, FeatureSet, webMercatorUtils, SimpleMarkerSymbol, SimpleLineSymbol, Graphic, GraphicsLayer, array, Deferred, DeferredList, domConstruct, dojoQuery, on, Bridges) {
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

        var routeResult = null;

        // Ask UPP for the geometry service
        var geometryService = null;
        $.get(sdUrl + "api/v1/hosts?type=geometry", function (data) {
            if (data && data.uri) {
                geometryService = new GeometryService(data.uri);
            }
        });

        function init_map(map_id, map_ref, center) {
			map_ref = map_ref || {};
            if (!map_ref.map) {
                map_ref.map = new Map(map_id, {
                    basemap: "topo",
                    center: center || [-94.8858, 47.4875],  // Bemidji 47.4875° N, 94.8858° W
                    zoom: 11
                });
				var searchLayer = new GraphicsLayer();
				map_ref.map.addLayer(searchLayer);
                var search = new Search({
                    map: map_ref.map,
					showInfoWindowOnSelect: false,
					graphicsLayer: searchLayer
                }, map_id + "-search");
                search.startup();
				search.on('select-result', function(evnt){
					map_ref.map.graphics.clear();
					map_ref.selectedLocation = { point: evnt.result.feature.geometry, address: evnt.result.name  };
				});
                on(map_ref.map, 'click', function (evt) {
                    map_ref.map.graphics.clear();
					searchLayer.clear();
                    map_ref.map.graphics.add(new Graphic(evt.mapPoint, symbol));
					map_ref.selectedLocation = { point: evt.mapPoint };
                });
            }

            return map_ref;
        };

        function find_address(map, el) {
			if(!map.selectedLocation){
				$(el).val('');
				$(el).data('location', null);
				return;
			}
            var point = map.selectedLocation.point;
			
			if(map.selectedLocation.address){
			    $(el).val(map.selectedLocation.address);
                $(el).data('location', point);
				check_route();
			} else {
				webPoint = webMercatorUtils.webMercatorToGeographic(point);

				locator.locationToAddress(webPoint, 100).then(function (result) {
					$(el).val(result.address.Match_addr);
					$(el).data('location', point);

					check_route();
				});
			}
        };

        // When the user submits the application, one of the first things to do
        // is use the designated geometry service to chop up the route among
        // the designated authorities
        function divide_route(route) {

            if (!route || !route.geometry) {
                alert("No route selected.");
                return;
            }
            var countyDef = getAuthorities("county.boundaries", "NAME", route);

            // Set the value of "Destination is within the applying County"
            // based on how many county authorities are returned.
            countyDef.then(function (result) {
                $("input[name='movementinfoDestinationWithinApplyingCounty'").prop('checked', result.length === 1);
            });
            var dl = new DeferredList([countyDef, getAuthorities("city.boundaries", "Name", route)]);
            dl.then(function (result) {
                $("#permit-authorities").val(result[0][1].concat(result[1][1]).toString());
            });

        };
        function getAuthorities(type, fieldName, route) {
            var def = new Deferred();
            // Ask UPP for the geometry service
            var url = sdUrl + "api/v1/hosts";
            $.get(url, {type: type}, function (data) {
                if (!data || data.length === 0) {
                    var message = 'No boundary service is configured for ' + type;
                    alert(message);
                    def.reject(message);
                }

                // Intersect the route geometry with the service layer and get a collection of
                // permit authorities
                var queryTask = new QueryTask(data[0].uri);
                var query = new Query();
                query.geometry = route.geometry;
                query.outFields = ["*"];
                query.returnGeometry = true;
                query.spatialRelationship = Query.SPATIAL_REL_INTERSECTS;

                queryTask.execute(query, function (result) {

                    def.resolve(array.map(result.features, function (county) {
                        return county.attributes[fieldName];
                    }));
                }, function (err) { console.log(err); def.reject(err); });
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
			return o;
		};
        // If both origin and destination have locations, route it
        function check_route(barriers) {
			//console.log(barriers);
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
						
						if(barriers){
							routeParams.polygonBarriers = new FeatureSet();
							array.forEach(barriers, function(barrier){
								routeParams.polygonBarriers.features.push(new Graphic(barrier, null, {}));
							});
						} else {
							bridgeUtils.clearBarriers();
						}
                        routeTask.solve(routeParams).then(function (result) {
                            routeResult = result.routeResults;

                            // Show the route on the route map
							route_map.graphics.clear();
                            var routeSymbol = new SimpleLineSymbol().setColor(new dojo.Color([0, 0, 255, 0.5])).setWidth(5);
                            array.forEach(result.routeResults, function (result) {
                                route_map.graphics.add(result.route.setSymbol(routeSymbol));
                                route_map.setExtent(result.route.geometry.getExtent().expand(1.5));
                                divide_route(result.route);
                            });
                            bridgeUtils.forRoute(result.routeResults[0], sdUrl).then(function (bridges) {
                                bridgeUtils.addToMap(bridges);
								//var form = $("#permit-form");
								bridgeUtils.addToForm(bridges);

							});
                            // Fill in the total miles traveled and the route description
                            var firstRoute = result.routeResults[0];
                            if (firstRoute) {
                                $("#movementinfo-route-description").val(firstRoute.route.attributes['Name']);
                                $("#movementinfo-total-route-length").val(firstRoute.route.attributes['Total_Miles']);
                            }
                        }, function(err){ alert(err.error.message);});
                    });
                });
            }
        };

        $('#startLocationModal').on('shown.bs.modal', function () {
            start_map = init_map('start-map', start_map, [-92.100487, 46.786671]);
        });

        $('#startLocationModal').on('hidden.bs.modal', function () {
            find_address(start_map, '#movementinfo-origin');
        });

        $('#endLocationModal').on('shown.bs.modal', function () {
            end_map = init_map('end-map', end_map, [-96.60645, 47.77423]);
        });

        $('#endLocationModal').on('hidden.bs.modal', function () {
            find_address(end_map, '#movementinfo-destination');
        });

        route_map = new Map('route-map', {
            basemap: "topo",
            center: [-94.8858, 47.4875],  // Bemidji 47.4875° N, 94.8858° W
            zoom: 10
        });
		
		bridgeUtils = new Bridges(route_map, $("#permit-form"));
		
		function submit_permit(authorityId){
			var def = new Deferred();
			$.get(serviceLocator, { type : "upp", scope: "permit.approval." + authorityId.replace(/\s+/g, '')}, function (data) { 
				var form = $("#permit-form");
				if(!data || data.length === 0){
					// The service directory did not return a service for the permit authority
					form.append('<input type="hidden" name="Authority" value="'+ authorityId +': Service Not Found." />');
					def.resolve("Service Not Found");
				} else {
                    // Submit the permit request to the permit authority.
					$.post(data[0].uri, serializeForm(form), function(permitData){
						
						form.append('<input type="hidden" name="Authority" value="'+ authorityId +': ' + permitData.status + ' - ' + permitData.timestamp + '." />');
						def.resolve("Service Found");
					});
				}
			});
			return def;
		}
		$("#request-permit").click(function (evt) {
			dojoQuery("input[name=Authority]").forEach(domConstruct.destroy);
			var auths = $("#permit-authorities").val().split(',');
			var form = $("#permit-form");
			
			var dl = new DeferredList(array.map(auths, function(auth){
				return submit_permit(auth);
			}));
			dl.then(function(result){
				form.submit();
			});
		}); 
		$("#recalc-route").click(function(evt){
			check_route(bridgeUtils.barriers);
		});
		$("#toggle-barriers").click(function (evt) {
		    bridgeUtils.toggleBarriers();
		});
		
		function UpdateSummaries() { 
			var h = parseInt($('#truckinfo-height').val(), 10);
			var w = parseInt($('#truckinfo-width').val(), 10);
			var l = parseInt($('#truckinfo-length').val(), 10);
			var of = parseInt($('#truckinfo-front-overhang').val(), 10) / 12;
			var or = parseInt($('#truckinfo-rear-overhang').val(), 10) / 12;
			var ol = parseInt($('#truckinfo-left-overhang').val(), 10) / 12;
			var ort = parseInt($('#truckinfo-right-overhang').val(), 10) / 12;

		   $('#truckinfo-dimension-summary').val(h + ' / ' + w + ' / ' + l);
		   $('#truckinfo-overall-dimension-description').val('Len: ' + (l + of + or) + ' Wid: ' + (w + ort + ol));
			check_route();
		}
		
		$('#truckinfo-height').change(UpdateSummaries);
		$('#truckinfo-width').change(UpdateSummaries);
		$('#truckinfo-length').change(UpdateSummaries);
		$('#truckinfo-front-overhang').change(UpdateSummaries);
		$('#truckinfo-rear-overhang').change(UpdateSummaries);
		$('#truckinfo-left-overhang').change(UpdateSummaries);
		$('#truckinfo-right-overhang').change(UpdateSummaries);
		$('#truckinfo-total-gross-weight').change(function(evt){check_route();});
    });

    // Ask the service locator to give us a UPP host that can provide company information.
    var serviceLocator = sdUrl + "api/v1/hosts"; 
    var select = $("#company-selector");

    $.get(serviceLocator, { type: "upp", scope: "information.company" }, function (data) {
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

    // Ask the service locator to give us a UPP host that can provide insurance information.
    var insurerSelect = $("#insurer-selector");

    $.get(serviceLocator, { type : "upp", scope: "information.insurance"}, function (data) {
        // Results are returned in priority order, so just take the first one.  Throw an error if no services are available
        if (data.length === 0) {
            alert('The service locator did not return any UPP services for insurance information');
            return;
        }

        // Got at least one provider, so use them to populate the company information drop-down
        var service = data[0];
        $.get(service.uri, function (companies) {
            $.each(companies, function (index, company) {
                var opt = $('<option>' + company.providerName + '</option>');
                opt.data("company", company);

                insurerSelect.append(opt);
            });
        });
    });

    // Set an event handler to populate the company form on select change
    insurerSelect.change(function (evt) {
        var opt = insurerSelect.find(":selected");
        var data = opt.data("company");
        
        if (data) {
            $('#insuranceinfo-insurance-provider').val(data.providerName);
            $('#insuranceinfo-agency-address').val(data.agencyAddress);
            $('#insuranceinfo-policy-number').val(data.policyNumber);
        }
    });

    // Ask the service locator to give us a UPP host that can provide vehicle information.
    var vehicleSelect = $("#vehicle-selector");

    $.get(serviceLocator, { type: "upp", scope: "information.vehicle" }, function (data) {
        // Results are returned in priority order, so just take the first one.  Throw an error if no services are available
        if (data.length === 0) {
            alert('The service locator did not return any UPP services for vehicle information');
            return;
        }

        // Got at least one provider, so use them to populate the company information drop-down
        var service = data[0];
        $.get(service.uri, function (vechiles) {
            $.each(vechiles, function (index, vehicle) {
                var opt = $('<option>' + vehicle.make + ' / ' + vehicle.model + ' (' + vehicle.usdotNumber + ')</option>');
                opt.data("vehicle", vehicle);

                vehicleSelect.append(opt);
            });
        });
    });

    // Set an event handler to populate the vehicle form on select change
    vehicleSelect.change(function (evt) {
        var opt = vehicleSelect.find(":selected");
        var data = opt.data("vehicle");
        
        if (data) {
            $('#vehicleinfo-year').val(data.year);
            $('#vehicleinfo-make').val(data.make);
            $('#vehicleinfo-model').val(data.model);
            $('#vehicleinfo-type').val(data.type);
            $('#vehicleinfo-license-number').val(data.license);
            $('#vehicleinfo-state').val(data.state);
            $('#vehicleinfo-truck-serial-number').val(data.serialNumber);
            $('#vehicleinfo-usdot-number').val(data.usdotNumber);
            $('#vehicleinfo-empty-weight').val(data.emptyWeight);
            $('#vehicleinfo-registered-weight').val(data.registedWeight);

            // Ask the service locator to give us a UPP host that can provide truck information.
            var truckSelect = $("#truck-selector");

            $.get(serviceLocator, { type: "upp", scope: "information.truck" }, function (data) {
                // Results are returned in priority order, so just take the first one.  Throw an error if no services are available
                if (data.length === 0) {
                    alert('The service locator did not return any UPP services for truck information');
                    return;
                }
                // Got at least one provider, so use them to populate the company information drop-down
                var service = data[0];
                $.get(service.uri, function (trucks) {
                    var index = $("#vehicle-selector").children('option:selected').index() -1;
                    if (trucks && trucks[index]) {
                        var truckData = trucks[index];
                        $('#truckinfo-total-gross-weight').val(truckData.grossWeight);
                        $('#truckinfo-empty-weight').val(truckData.emptyWeight);
                        $('#truckinfo-registered-weight').val(truckData.registedWeight);
                        $('#truckinfo-regulation-weight').val(truckData.regulationWeight);
                        $('#truckinfo-dimension-summary').val(truckData.height + ' / ' + truckData.width + ' / ' + truckData.length);
                        $('#truckinfo-overall-dimension-description').val('Len: ' + (truckData.length + truckData.frontOverhang + truckData.rearOverhang) + ' Wid: ' + (truckData.width + truckData.rightOverhang + truckData.leftOverhang));
                        $('#truckinfo-height').val(truckData.height);
                        $('#truckinfo-width').val(truckData.width);
                        $('#truckinfo-length').val(truckData.length);
                        $('#truckinfo-front-overhang').val(truckData.frontOverhang);
                        $('#truckinfo-rear-overhang').val(truckData.rearOverhang);
                        $('#truckinfo-left-overhang').val(truckData.leftOverhang);
                        $('#truckinfo-right-overhang').val(truckData.rightOverhang).change();  // Fire a change event to update the bridges
                        $('#truckinfo-diagram').val(truckData.diagram);
                    }

                });
            });
        }
    });


    // Ask the service locator to give us a UPP host that can provide insurance information.
    var trailerSelect = $("#trailer-selector");

    $.get(serviceLocator, { type: "upp", scope: "information.trailer" }, function (data) {
        // Results are returned in priority order, so just take the first one.  Throw an error if no services are available
        if (data.length === 0) {
            alert('The service locator did not return any UPP services for trailer information');
            return;
        }

        // Got at least one provider, so use them to populate the trailer information drop-down
        var service = data[0];
        $.get(service.uri, function (trailers) {
            $.each(trailers, function (index, trailer) {
                var opt = $('<option>' + trailer.description + '</option>');
                opt.data("trailer", trailer);

                trailerSelect.append(opt);
            });
        });
    });

    // Set an event handler to populate the company form on select change
    trailerSelect.change(function (evt) {
        var opt = trailerSelect.find(":selected");
        var data = opt.data("trailer");
        
        if (data) {
            $('#trailerinfo-description').val(data.description);
            $('#trailerinfo-make').val(data.make);
            $('#trailerinfo-model').val(data.model);
            $('#trailerinfo-trailer-type').val(data.type);
            $('#trailerinfo-serial-number').val(data.serialNumber);
            $('#trailerinfo-license-number').val(data.license);
            $('#trailerinfo-state').val(data.state);
            $('#trailerinfo-empty-weight').val(data.emptyWeight);
            $('#trailerinfo-registered-weight').val(data.registedWeight);
            $('#trailerinfo-regulation-weight').val(data.registedWeight);
        }
    });




})(
    jQuery,
    require,
    (typeof apiUrl === 'undefined') ? '/manager/' : apiUrl,
    (typeof sdUrl === 'undefined') ? '/sd/' : sdUrl
);
