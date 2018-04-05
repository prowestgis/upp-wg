define([
	"dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
	"dojo/_base/Deferred",
    "dojo/dom-construct",
	"dojo/query",

    "esri/Color",
    "esri/InfoTemplate",
	"esri/geometry/Circle",
    "esri/graphic",
    "esri/layers/GraphicsLayer",
    "esri/symbols/SimpleLineSymbol",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/tasks/query",
    "esri/tasks/QueryTask",
	"esri/geometry/geometryEngine",
	"esri/units"
	
], function (declare, lang, array, Deferred, domConstruct, dojoQuery,
        Color, InfoTemplate, Circle, Graphic, GraphicsLayer, SimpleLineSymbol, SimpleMarkerSymbol, Query, QueryTask, GeometryEngine, Units) {

    return declare(null,{
        layer: null,
		map: null,
		form: null,
		
		constructor: function(map, form){
			this.map = map;
			this.form = form;
		},
        forRoute: function (route, sdUrl) {
			
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
		
        addToForm: function ( bridges) {
			var self = this;
            dojoQuery("input[name=Bridges]").forEach(domConstruct.destroy);
			array.forEach(bridges,function(bridge){
			    self.form.append('<input type="hidden" name="Bridges" value="'+  encodeURIComponent(JSON.stringify(bridge.attributes)) +'" />');
			});
        },
        validForLoad: function (bridge, load) {
			
            return (bridge.ALTIRLOAD / 0.90718474) > load.weight;
        },
        addToMap(bridges) {
			if (!this.layer) {
                this.layer = new GraphicsLayer();
                this.layer.setInfoTemplate(new InfoTemplate("Bridge", "${*}"));
                this.map.addLayer(this.layer);
                this.bridgeSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbol.STYLE_SQUARE, 12,
                                            new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID,
                                            new Color([0, 0, 0]), 1),
                                            new Color([0, 255, 0, 0.75]));
				this.invalidBridgeSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbol.STYLE_SQUARE, 12,
                                            new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0]), 1),
                                            new Color([255, 255, 0, 0.75]));
            }
			
			this.layer.clear();
			
            var self = this;
			
            array.forEach(bridges, function (bridge) {
				var load = {
					weight: document.getElementById("truckinfo-total-gross-weight").value
				};
				if(self.validForLoad(bridge.attributes, load)){
					self.layer.add(new Graphic(bridge.geometry, self.bridgeSymbol, bridge.attributes));
				} else {
					self.barriers = self.barriers || [];
					self.barriers.push(new Circle(bridge.geometry, { "geodesic": true, "radius": 10, "radiusUnit":Units.FEET}));
					self.layer.add(new Graphic(bridge.geometry, self.invalidBridgeSymbol, bridge.attributes));
				}
            });
        }
    });
});
