define([
    "dojo/_base/lang",
    "dojo/_base/array",
	"dojo/_base/Deferred",
    "esri/tasks/query",
    "esri/tasks/QueryTask",
	"esri/geometry/geometryEngine"
	
], function (lang, array, Deferred, Query, QueryTask, GeometryEngine) {

    return {
		
        forRoute: function (route, sdUrl) {
			console.log(route);
            var def = new Deferred();
            // Ask UPP for the bridge service
            var url = sdUrl + "api/v1/hosts";
            $.get(url, {type: "bridges"}, function (data) {
                if (!data || data.length === 0) {
                    var message = 'No service is configured for bridges.';
                    alert(message);
                    def.reject(message);
                }
                    var record = data[0];

                    // Now ask for credentials
                    var url = sdUrl + "api/v1/hosts/" + record.name + "/access";
                    $.get(url, function (service) {
                        if (!service.url) {
                            alert('No route service access is configured');
							def.reject('No route service access is configured');
                            return;
                        }

                        if (service.url && service.isSecured && !service.token) {
                            alert('Unable to aquire token to access secured service');
							def.reject('Unable to aquire token to access secured service');
                            return;
                        }
						console.log(GeometryEngine.geodesicBuffer(route.route.geometry, 10, 'feet', true));
						// Intersect the route geometry with the service layer and get a collection of
						// permit authorities
						var queryTask = new QueryTask(service.url + "/0?token=" + service.token);
						var query = new Query();
						query.geometry = GeometryEngine.geodesicBuffer(route.route.geometry, 10, 'feet', true);
						query.outFields = ["BRIDGE_ID","FEATINT","FACILITY","ALTIRMETH","ALTIRLOAD"];
						query.returnGeometry = true;
						query.spatialRelationship = Query.SPATIAL_REL_INTERSECTS;

						queryTask.execute(query, function (result) {
							def.resolve(result.features);
						}, function (err) { console.log(err); def.reject(err); });
					});
            });
            return def.promise;
        },
		
		addToForm: function(form, bridges){
			array.forEach(bridges,function(bridge){
			    form.append('<input type="hidden" name="Bridges" value="'+  encodeURIComponent(JSON.stringify(bridge.attributes)) +'" />');
			});
		}
    }
});