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
	"esri/symbols/SimpleFillSymbol",
    "esri/symbols/SimpleLineSymbol",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/tasks/query",
    "esri/tasks/QueryTask",
	"esri/geometry/geometryEngine",
	"esri/units"
	
], function (declare, lang, array, Deferred, domConstruct, dojoQuery,
        Color, InfoTemplate, Circle, Graphic, GraphicsLayer, SimpleFillSymbol, SimpleLineSymbol, SimpleMarkerSymbol, Query, QueryTask, GeometryEngine, Units) {

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
                            alert('No bridge service access is configured');
							def.reject('No bridge service access is configured');
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
						query.geometry = GeometryEngine.geodesicBuffer(route.directions.mergedGeometry, 10, 'feet', true);
						query.outFields = ["BRKEY","RoadWidth","VERT_CLEAR","RECORD_TYPE","FEATINT","FACILITY","LOCATION","ALTIRMETH", "LoadRating"];
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
			var messages = [];
			var valid = true;
			
			if (bridge.ALTIRMETH === "PED" && bridge.RECORD_TYPE === "ON" && bridge.VERT_CLEAR === 99) {
			    return { valid: true, messages: messages };
			}
			
			if(bridge.RoadWidth < load.width && bridge.RoadWidth > 0){
				messages.push("Load width " + load.width + " exceeds the road width " + bridge.RoadWidth);
				valid = false;
			}
			
			if(bridge.RECORD_TYPE === "UNDER" && bridge.VERT_CLEAR <= load.height ){
				messages.push("Load height " + load.height + " exceeds the clearance " + bridge.VERT_CLEAR);
				valid = false;
			} else {
				if(bridge.LoadRating <= load.weight / 2000) {
					messages.push("Load weight " + load.weight / 2000 + " exceeds the load rating " + bridge.LoadRating);
					valid = false;
				}
			}
			return { valid: valid, messages: messages};
        },
        addToMap: function(bridges) {
			if (!this.layer) {
				this.barrierLayer = new GraphicsLayer();
                this.barrierLayer.setInfoTemplate(new InfoTemplate("Barriers: ${BRKEY}", "${*}"));
				this.map.addLayer(this.barrierLayer);

				this.layer = new GraphicsLayer();
                this.layer.setInfoTemplate(new InfoTemplate("Bridge: ${BRKEY}", "${*}"));
                this.map.addLayer(this.layer);

                this.bridgeSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbol.STYLE_SQUARE, 15,
                                            new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID,
                                            new Color([0, 0, 0]), 1),
                                            new Color([0, 255, 0, 0.75]));
				this.invalidBridgeSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbol.STYLE_SQUARE, 15,
                                            new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([255, 0, 0]), 1),
                                            new Color([255, 255, 0, 0.75]));
				this.barrierSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbol.STYLE_SQUARE, 15,
										new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID,
										new Color([0,0,0]), 2),new Color([255,0,0,0.75]));
            }
			
			this.layer.clear();
			
            var self = this;
			
            array.forEach(bridges, function (bridge) {
				var load = {
					weight: Number(document.getElementById("truckinfo-total-gross-weight").value),
					width: Number(document.getElementById("truckinfo-width").value) + Number(document.getElementById("truckinfo-right-overhang").value)  / 12 + Number(document.getElementById("truckinfo-left-overhang").value) / 12,
					height: Number(document.getElementById("truckinfo-height").value)
				};
				//console.log(load);
				var status = self.validForLoad(bridge.attributes, load);
				if(status.valid){
					self.layer.add(new Graphic(bridge.geometry, self.bridgeSymbol, bridge.attributes));
				} else {
					self.barriers = self.barriers || [];
					var geo = new Circle(bridge.geometry, { "geodesic": true, "radius": 15, "radiusUnit":Units.FEET});
					var attribs = lang.clone(bridge.attributes);
					attribs.Reason = status.messages.toString();
					self.barriers.push(geo);
					
					self.barrierLayer.add(new Graphic(geo, self.barrierSymbol, attribs));
					self.layer.add(new Graphic(bridge.geometry, self.invalidBridgeSymbol, attribs));
				}
            });
        },
		toggleBarriers: function(){
			if(this.barrierLayer.visible){
				this.barrierLayer.hide();
			} else {
				this.barrierLayer.show();
			}
		},
		clearBarriers: function(){
			if(this.barrierLayer){
				this.barrierLayer.clear();
			}
			this.barriers = [];
		},
		clearBridges: function(){
			if(this.layer){
				this.layer.clear();
			}
		}
    });
});
