(function($, require, apiUrl, sdUrl) {
    var start_map, end_map, route_map, bridgeUtils;

    require({
        packages: [{
            "name": "scripts",
            "location": location.pathname.replace(/\/[^/]+$/, '') + "/../../../../../js"
        }]
    }, [
        "esri/map",
        "scripts/override/dijit/Directions",
        "esri/dijit/Search",
        "esri/tasks/locator",
        "esri/tasks/query",
        "esri/tasks/QueryTask",
        "esri/tasks/RouteTask",
        "esri/tasks/RouteParameters",
        "esri/tasks/FeatureSet",
		"esri/geometry/Point",
		"esri/geometry/Polygon",
        "esri/geometry/webMercatorUtils",
        "esri/symbols/SimpleMarkerSymbol",
        "esri/symbols/SimpleLineSymbol",
        "esri/graphic",
		"esri/layers/GraphicsLayer",
        "dojo/_base/array",
		"dojo/_base/lang",
		"dojo/_base/Deferred",
		"dojo/DeferredList",
		"dojo/dom-construct", 
		"dojo/query",
        "dojo/on",
		"scripts/bridges",
        "dojo/domReady!"
    ], function (Map, Directions, Search, Locator, Query, QueryTask, RouteTask, RouteParameters, FeatureSet, Point, Polygon, webMercatorUtils, SimpleMarkerSymbol, SimpleLineSymbol, Graphic, GraphicsLayer, array, lang, Deferred, DeferredList, domConstruct, dojoQuery, on, Bridges) {
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
        $.get(sdUrl + "api/v1/services?type=geometry", function (data) {
            if (data && data.uri) {
                geometryService = new GeometryService(data.uri);
            }
        });

        // When the user submits the application, one of the first things to do
        // is use the designated geometry service to chop up the route among
        // the designated authorities
        function divide_route(route) {

            if (!route ) {
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
            var url = sdUrl + "api/v1/services";
            $.get(url, {type: type}, function (data) {
                if (!data || data.length === 0) {
                    var message = 'No boundary service is configured for ' + type;
                    alert(message);
                    def.reject(message);
                }
console.log(data);
                // Intersect the route geometry with the service layer and get a collection of
                // permit authorities
                var queryTask = new QueryTask(data.data[0].attributes.uri);
                var query = new Query();
                query.geometry = route;
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
				var directionsData = directions.directions;
			    var routeGeometry = directionsData.mergedGeometry;
                var routeStops = directions.stops;
                var routeBarriers = bridgeUtils.barriers;

                // Update the permit application with the route information
                var data = {
                    geometry: routeGeometry,
                    stops: routeStops,
                    barriers: routeBarriers
                };
				document.getElementById("saved-route").value = JSON.stringify(data);
				
				
/*			dojoQuery("input[name=Authority]").forEach(domConstruct.destroy);
			var auths = $("#permit-authorities").val().split(',');
			var form = $("#permit-form");
			
			var dl = new DeferredList(array.map(auths, function(auth){
				return submit_permit(auth);
			}));
			dl.then(function(result){
				form.submit();
			}); */
		}); 
        function add_route_data(record) {

            var def = $.Deferred();

            $('#submitModalMessage').text('Adding route data...');
            console.log("add_route_data", arguments);

            // Check that a route has been creates
            var directionsData = directions.directions;
			console.log(directions);
            if (!directionsData) {
                def.reject('No route has been created');
                return def.promise();
            }

            try {
                // Get the current direction geometry, the stops and barriers
                var routeGeometry = directionsData.mergedGeometry;
                var routeStops = directions.stops;
                var routeBarriers = bridgeUtils.barriers;
console.log(directionsData);
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

			check_directions_route();
		}

		$('#truckinfo-height').change(UpdateSummaries);
		$('#truckinfo-width').change(UpdateSummaries);
		$('#truckinfo-length').change(UpdateSummaries);
		$('#truckinfo-front-overhang').change(UpdateSummaries);
		$('#truckinfo-rear-overhang').change(UpdateSummaries);
		$('#truckinfo-left-overhang').change(UpdateSummaries);
		$('#truckinfo-right-overhang').change(UpdateSummaries);
		$('#truckinfo-total-gross-weight').change(function (evt) { check_directions_route(); });

        // Ask UPP what service we should use for routing.  The UPP services API is responsible for acquiring any
        // OAuth tokens that are needed for the service
        var serviceLocator = sdUrl + "api/v1/services";
		var directions;
        $.get(serviceLocator, { type: "route" }, function (records) {
            // Results are returned in priority order, so just take the first one.  Throw an error if no services are available
            if (records.length === 0) {
                alert('The service locator did not return any services for routing');
                return;
            }
console.log(records);
            var record = records.data[0];

            // Now ask for credentials
            var url = sdUrl + "api/v1/services/" + record.attributes.name + "/access";
            $.get(url, function (service) {
                if (!service.url) {
                    alert('No route service access is configured');
                    return;
                }

                if (service.url && service.isSecured && !service.token) {
                    alert('Unable to aquire token to access secured routing service');
                    return;
                }

                directions = new Directions({
                    map: route_map,
                    routeTaskUrl: service.url + "?token=" + service.token,
                    showClearButton: true,
					showTrafficOption: false,
					showPrintPage: false
                }, "route-directions");
                
				var savedRoute = JSON.parse($("#saved-route").val());
				directions.startup();
				directions.on('load', function(loadEvt){
					bridgeUtils.loadBarriers(savedRoute.barriers);
					directions.routeParams.polygonBarriers = new FeatureSet();
					array.forEach(savedRoute.barriers, function(barrier){
						directions.routeParams.polygonBarriers.features.push(new Graphic(new Polygon(barrier.geometry), null, {}));
					});
					array.forEach(savedRoute.stops, function(stop){
						directions.addStop(new Point(stop.feature.geometry));
					});

				});
				
				
				directions.on('directions-finish',function(rr){
					
					if(rr.result.routeResults){
						divide_route(rr.result.routeResults[0].directions.mergedGeometry);
						bridgeUtils.forRoute(rr.result.routeResults[0], sdUrl).then(function (bridges) {
							
							bridgeUtils.addToMap(bridges);
							bridgeUtils.addToForm(bridges);
							directions.routeParams.polygonBarriers = new FeatureSet();
							array.forEach(bridgeUtils.barriers, function(barrier){
								directions.routeParams.polygonBarriers.features.push(new Graphic(barrier.geometry, null, {}));
							});

						});
						// Fill in the total miles traveled and the route description
                        var firstRoute = rr.result.routeResults[0];
                        if (firstRoute) {
                            $("#movementinfo-route-description").val(firstRoute.directions.routeName);
                            $("#movementinfo-total-route-length").val(firstRoute.directions.summary.totalLength);
							
							var stops = firstRoute.stops;
							$("#movementinfo-origin").val(stops[0].attributes["Name"]);
							$("#movementinfo-destination").val(stops[stops.length - 1].attributes["Name"]);
                        }
					}
				});
				directions.on('directions-clear',function(cr){
					bridgeUtils.clearBarriers();
					bridgeUtils.clearBridges();
				});
				directions.setTravelMode('Trucking Time');
            });
        });
    });

    // Ask the service locator to give us a UPP host that can provide company information.
    var serviceLocator = sdUrl + "api/v1/services"; 
    var select = $("#company-selector");

})(
    jQuery,
    require,
    (typeof apiUrl === 'undefined') ? '/manager/' : apiUrl,
    (typeof sdUrl === 'undefined') ? '/sd/' : sdUrl
);
